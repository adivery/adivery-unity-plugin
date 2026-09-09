---
name: release-plugin
description: >-
  Build Adivery.unitypackage (the actual customer-facing Unity plugin
  package — just Assets/Adivery's C# source, no AAR, no Gradle templates)
  and publish it as the plugin's own numbered vX.Y.Z GitHub release. Use
  when the user asks to "release the plugin", "build Adivery.unitypackage",
  "cut a new plugin release", or similar. This is NOT the sample-app flow
  (see release-sample) — it never touches the sample's APK, and every run
  creates a brand-new numbered release (never a rolling/mutable tag), so
  it always drafts first and asks before publishing. Flow: preflight
  checks → batchmode export → verify package contents → publish as a
  draft release → human confirms → publish.
---

# Release the plugin package

This skill builds `Adivery.unitypackage` — the package customers actually
import (`Assets/Adivery/*.cs` only) — and publishes it as the plugin's own
numbered release on GitHub. It does **not** touch the sample app
(`source/plugin`'s buildable APK) — that's `release-sample`, a separate,
independent release stream under its own `sample-latest` tag.

Repo: `adivery/adivery-unity-plugin` (origin: `https://github.com/adivery/adivery-unity-plugin.git`)
Unity project used only to *export* the package (not built/compiled for a
platform): `source/plugin`
Exported path scope: `Assets/Adivery` — confirmed against the actual
contents of the real, currently-published `v4.8.5` release (a
`.unitypackage` is a gzipped tarball of `<guid>/{asset,asset.meta,pathname}`
entries — extract and read `pathname` to verify). It contains exactly
`Assets/Adivery/` and its files, nothing else — no `.aar`, no
`Assets/Plugins/Android/*`. Don't add other paths to the export without
re-verifying against a real release first; the version of this in
`build.gradle`'s `exportPackage` task additionally lists
`Assets/Plugins/Android/adivery-sdk.aar`, which has never existed
anywhere in this repo — don't copy that path into this skill.

## Hard rules

- **Never kill a running Unity Editor.** If the project is already open
  (locked), STOP and ask the user to close it themselves — they may have
  unsaved changes open. Do not `kill`/`pkill` Unity on their behalf even if
  it looks idle.
- **Every run is a new numbered release — always draft first, always ask
  before publishing.** Unlike `release-sample`'s rolling `sample-latest`
  tag, `vX.Y.Z` here is a real, permanent, customer-facing release. There
  is no "just update the asset, no confirmation needed" path — every
  single run needs the human gate before draft → published.
- **Never silently overwrite an already-published numbered release.** If
  `vX.Y.Z` already exists and is *not* a draft, STOP — see STOP conditions.
- If any STOP condition below is hit, halt and report — do not work around it.

## STOP conditions (halt and report before doing anything)

- The project is already open in a running Unity Editor (a live lock on
  `source/plugin/Temp/UnityLockfile`) — ask the user to close it.
- No Unity Editor install matching `source/plugin/ProjectSettings/ProjectVersion.txt` found under `~/Unity/Hub/Editor/`.
- `source/plugin/Assets/Plugins/Android/launcherTemplate.gradle` doesn't
  contain a `com.adivery:sdk:X.Y.Z` line to extract the version from.
- The release tag `vX.Y.Z` (derived from that version) already exists **and
  is not a draft** — this version has already been published. Don't
  overwrite it; report it and ask the user whether the version genuinely
  needs bumping first, or whether they intend to re-publish under the same
  number (unusual — confirm explicitly before doing that).
- The batchmode export exits non-zero, or no `.unitypackage` ends up at the
  expected path, or it's empty.
- The exported package's contents don't match `Assets/Adivery`'s current
  file list when you extract and check it (see Phase 1 verification) —
  something's wrong with the export, don't publish a broken package.
- `gh auth status` fails.
- `git status --short` shows uncommitted changes unrelated to this skill's
  own build artifacts — surface them and ask before continuing (never
  commit on the user's behalf here; this skill doesn't need to).

---

## Phase 0 — Preflight

```bash
cd /path/to/adivery-unity-plugin   # repo root

# 1. Version this build targets, and the tag it maps to.
VERSION=$(grep -oP "com\.adivery:sdk:\K[0-9.]+" source/plugin/Assets/Plugins/Android/launcherTemplate.gradle)
TAG="v$VERSION"
echo "Building Adivery.unitypackage for SDK $VERSION -> release tag $TAG"

# 2. Does this tag already exist? Decides whether we can proceed at all.
if gh release view "$TAG" --repo adivery/adivery-unity-plugin --json isDraft -q .isDraft 2>/dev/null | grep -q "^false$"; then
  echo "STOP: $TAG already exists and is published — this version was already released"
elif gh release view "$TAG" --repo adivery/adivery-unity-plugin >/dev/null 2>&1; then
  echo "$TAG exists as a draft — will update it"
else
  echo "No release $TAG yet — will create it as a draft"
fi

# 3. Resolve the matching Unity Editor
UNITY_VERSION=$(awk '{print $2}' source/plugin/ProjectSettings/ProjectVersion.txt | head -1)
UNITY_EXE="$HOME/Unity/Hub/Editor/$UNITY_VERSION/Editor/Unity"
[ -x "$UNITY_EXE" ] && echo "Unity: $UNITY_EXE" || echo "STOP: Unity $UNITY_VERSION not installed at $UNITY_EXE"

# 4. Project must not already be open in an Editor
if [ -e source/plugin/Temp/UnityLockfile ] && fuser source/plugin/Temp/UnityLockfile >/dev/null 2>&1; then
  echo "STOP: source/plugin is open in a running Unity Editor — ask the user to close it first"
fi

# 5. GitHub auth
gh auth status

# 6. Working tree
git status --short
```

Report the SDK version, the tag, whether it already exists (and as what),
and the Unity path, before proceeding.

## Phase 1 — Build and verify the package

```bash
WORKDIR=$(mktemp -d)
PACKAGE="$WORKDIR/Adivery.unitypackage"
LOG="$WORKDIR/export.log"

"$UNITY_EXE" -batchmode -nographics \
  -projectPath source/plugin \
  -logFile "$LOG" \
  -exportPackage Assets/Adivery "$PACKAGE" \
  -quit
echo "exit: $?"   # STOP if non-zero — tail "$LOG" and report the failure

ls -la "$PACKAGE"   # STOP if missing/zero bytes

# Verify the package actually contains what it should, not a silent
# partial/empty export. A .unitypackage is a gzipped tar of
# <guid>/{asset,asset.meta,pathname} entries.
mkdir "$WORKDIR/verify"
tar -xzf "$PACKAGE" -C "$WORKDIR/verify"
PACKAGED_PATHS=$(for d in "$WORKDIR"/verify/*/; do
  [ -f "$d/pathname" ] && cat "$d/pathname"
done | sort)
EXPECTED_PATHS=$(find source/plugin/Assets/Adivery -type f -name "*.cs" | sed 's#source/plugin/#Assets/#' | sort)
echo "$PACKAGED_PATHS"
diff <(echo "$PACKAGED_PATHS" | grep '\.cs$') <(echo "$EXPECTED_PATHS") \
  && echo "Package contents match Assets/Adivery — OK" \
  || echo "STOP: packaged .cs files don't match Assets/Adivery's current contents"
```

## Phase 2 — Publish to GitHub

Asset name: `Adivery.unitypackage` (matches the existing convention —
`build.gradle`'s `exportPath`, and the docs' download link
`.../releases/latest/download/Adivery.unitypackage`).

**Always create as a draft first** — there is no non-interactive path here,
every tag is new:

```bash
gh release create "$TAG" \
  --repo adivery/adivery-unity-plugin \
  --title "$TAG" \
  --notes "Adivery Unity plugin package for SDK version $VERSION." \
  --draft \
  "$PACKAGE#Adivery.unitypackage"
```

(If `$TAG` already existed as a draft per Phase 0, use
`gh release upload "$TAG" "$PACKAGE#Adivery.unitypackage" --repo adivery/adivery-unity-plugin --clobber`
to replace its asset instead of creating a new release.)

Show the user the draft URL and the placeholder release notes, and ask
them to:
1. Confirm or rewrite the release notes (a real changelog, not just the
   placeholder — this is customer-facing).
2. Confirm publishing.

Only on confirmation:

```bash
gh release edit "$TAG" --repo adivery/adivery-unity-plugin --draft=false
```

---

## Final report

Summarize: the SDK version, the release URL, the asset name
(`Adivery.unitypackage`), and confirmation the package's contents were
verified against `Assets/Adivery`. Note the local `$WORKDIR` path and that
it's safe to delete afterward.
