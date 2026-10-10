#!/usr/bin/env python3
"""Compare the plugin's C# reflection bridge against the Unity bridge in an Adivery SDK AAR.

The C# layer (Assets/Adivery/*.cs) reaches Java only by name - CallStatic("x"), Call("x"),
new AndroidJavaObject("cls", ...), and AndroidJavaProxy methods named after a Java interface's
methods - so the C# compiler can't catch a rename, removal or new callback. This does.

  bridge.py api   <sdk.aar>              print the bridge API (com.adivery.sdk.plugins.unity), sorted
  bridge.py check <sdk.aar> <cs_dir>     check every C# reference against that AAR; exit 1 on errors

Diff two `api` outputs to see what a new SDK version changed in the bridge.
Run with `python3 -I`; needs `javap` (any JDK) on PATH.
"""
import os
import re
import subprocess
import sys
import tempfile
import zipfile

PKG = "com/adivery/sdk/plugins/unity/"
PKG_DOT = PKG.replace("/", ".")
# R8 gives synthetic helpers one/two-letter names; they're not bridge API.
OBFUSCATED = re.compile(r"^[a-z]{1,2}$")
JAVA_TO_CS = {
    "java.lang.String": "string", "boolean": "bool", "int": "int", "long": "long",
    "float": "float", "double": "double", "byte[]": "byte[]",
}


def load_api(aar):
    """{class simple name: {"interface": bool, "methods": {name: set(param tuples)}, "ctors": set(arities)}}"""
    with tempfile.TemporaryDirectory() as tmp:
        with zipfile.ZipFile(aar) as z:
            jar = os.path.join(tmp, "classes.jar")
            with open(jar, "wb") as f:
                f.write(z.read("classes.jar"))
        with zipfile.ZipFile(jar) as j:
            names = [n[:-6].replace("/", ".") for n in j.namelist()
                     if n.startswith(PKG) and n.endswith(".class") and "$" not in n]
        if not names:
            sys.exit("no %s classes in %s" % (PKG_DOT, aar))
        out = subprocess.run(["javap", "-public", "-cp", jar] + sorted(names),
                             check=True, capture_output=True, text=True).stdout

    api, cur = {}, None
    for line in out.splitlines():
        line = line.strip()
        m = re.match(r"public (?:abstract )?(?:final )?(class|interface) %s(\w+)" % re.escape(PKG_DOT), line)
        if m:
            cur = api.setdefault(m.group(2), {"interface": m.group(1) == "interface", "methods": {}, "ctors": set()})
            continue
        if cur is None or "(" not in line:
            continue  # fields, braces, "Compiled from"
        m = re.match(r"public (?:[\w.$\[\]<>]+ )*?([\w.$\[\]]+)\((.*)\);$", line)
        if not m:
            continue
        name, params = m.group(1), [p.strip() for p in m.group(2).split(",") if p.strip()]
        if name.startswith(PKG_DOT):
            cur["ctors"].add(len(params))
        elif not OBFUSCATED.match(name):
            cur["methods"].setdefault(name, set()).add(tuple(params))
    return api


def cmd_api(aar):
    api = load_api(aar)
    for cls in sorted(api):
        info = api[cls]
        print("%s %s" % ("interface" if info["interface"] else "class", cls))
        for n in sorted(info["ctors"]):
            print("  <init>(%d args)" % n)
        for name in sorted(info["methods"]):
            for params in sorted(info["methods"][name]):
                print("  %s(%s)" % (name, ", ".join(params)))


def split_args(src, start):
    """Top-level comma-separated args of the call whose '(' is at src[start]."""
    depth, args, cur = 0, [], ""
    for ch in src[start + 1:]:
        if ch in "([{":
            depth += 1
        elif ch in ")]}":
            if depth == 0:
                break
            depth -= 1
        if ch == "," and depth == 0:
            args.append(cur.strip())
            cur = ""
        else:
            cur += ch
    if cur.strip():
        args.append(cur.strip())
    return args


def cmd_check(aar, cs_dir):
    api = load_api(aar)
    files = {}
    for root, _, fs in os.walk(cs_dir):
        for f in fs:
            if f.endswith(".cs"):
                p = os.path.join(root, f)
                files[p] = open(p, encoding="utf-8-sig").read()

    errors, warnings, checked = [], [], 0

    def receivers(src):
        """receiver expression -> Java class simple name, as assigned/returned in this source."""
        found = {}
        for m in re.finditer(r'(\w+)\s*=\s*new AndroidJavaObject\("%s(\w+)"' % re.escape(PKG_DOT), src):
            found[m.group(1)] = m.group(2)
        for m in re.finditer(r'(\w+)\(\)\s*\{\s*return new AndroidJavaClass\("%s(\w+)"\)' % re.escape(PKG_DOT), src):
            found[m.group(1) + "()"] = m.group(2)
        return found

    # A file's own fields win (BannerAd and NativeAd both call theirs `adObject`); otherwise fall
    # back to one declared elsewhere (Adivery.cs calls AdiveryListener's adiveryListenerObject).
    global_cls_of = {}
    for src in files.values():
        global_cls_of.update(receivers(src))

    for path, src in sorted(files.items()):
        rel = os.path.relpath(path, cs_dir)
        cls_of = dict(global_cls_of, **receivers(src))
        line_of = lambda pos: src.count("\n", 0, pos) + 1

        # Constructors.
        for m in re.finditer(r'new AndroidJavaObject\("%s(\w+)"' % re.escape(PKG_DOT), src):
            cls = m.group(1)
            nargs = len(split_args(src, src.index("(", m.start()))) - 1
            checked += 1
            if cls not in api:
                errors.append("%s:%d: class %s is gone" % (rel, line_of(m.start()), cls))
            elif nargs not in api[cls]["ctors"]:
                errors.append("%s:%d: new %s(%d args) - AAR has %s" % (
                    rel, line_of(m.start()), cls, nargs, sorted(api[cls]["ctors"])))

        # Call / CallStatic on a known receiver.
        for m in re.finditer(r'([\w.]+(?:\(\))?)\??\.Call(Static)?(?:<[^>]+>)?\("(\w+)"', src):
            receiver = m.group(1).split(".")[-1]
            cls = cls_of.get(receiver)
            if cls is None:
                continue  # not an Adivery bridge object (e.g. the Unity activity)
            name = m.group(3)
            nargs = len(split_args(src, src.index("(", m.end() - len(name) - 3))) - 1
            where = "%s:%d: %s.%s" % (rel, line_of(m.start()), cls, name)
            checked += 1
            if cls not in api:
                errors.append(where + " - class %s is gone" % cls)
            elif name not in api[cls]["methods"]:
                errors.append(where + " - no such method in the AAR")
            elif nargs not in {len(p) for p in api[cls]["methods"][name]}:
                errors.append(where + " called with %d args - AAR has %s" % (
                    nargs, sorted(api[cls]["methods"][name])))

        # AndroidJavaProxy implementations: every interface method needs a C# method.
        m = re.search(r'base\("%s(\w+)"\)' % re.escape(PKG_DOT), src)
        if m and re.search(r":\s*AndroidJavaProxy", src):
            iface = m.group(1)
            if iface not in api:
                errors.append("%s: proxied interface %s is gone" % (rel, iface))
                continue
            cs_methods = {}
            for mm in re.finditer(r"public (?:virtual |override )?void (\w+)\(([^)]*)\)", src):
                types = tuple(re.sub(r"\s+\w+$", "", p.strip()) for p in mm.group(2).split(",") if p.strip())
                cs_methods.setdefault(mm.group(1), set()).add(types)
            for name, overloads in sorted(api[iface]["methods"].items()):
                for params in overloads:
                    want = tuple(JAVA_TO_CS.get(p, "AndroidJavaObject") for p in params)
                    checked += 1
                    if want not in cs_methods.get(name, set()):
                        errors.append("%s: %s.%s(%s) has no matching C# method %s(%s)" % (
                            rel, iface, name, ", ".join(params), name, ", ".join(want)))
            for name in sorted(set(cs_methods) - set(api[iface]["methods"])):
                if name[0].islower():  # Java-style names only; C# helpers are PascalCase
                    warnings.append("%s: %s() isn't in this AAR's %s (harmless; renamed/removed, or newer than this SDK)" % (
                        rel, name, iface))

    for w in warnings:
        print("WARN  " + w)
    for e in errors:
        print("ERROR " + e)
    print("%d reference(s) checked: %d error(s), %d warning(s) against %s" % (
        checked, len(errors), len(warnings), os.path.basename(aar)))
    return 1 if errors else 0


if __name__ == "__main__":
    if len(sys.argv) == 3 and sys.argv[1] == "api":
        cmd_api(sys.argv[2])
    elif len(sys.argv) == 4 and sys.argv[1] == "check":
        sys.exit(cmd_check(sys.argv[2], sys.argv[3]))
    else:
        sys.exit(__doc__)
