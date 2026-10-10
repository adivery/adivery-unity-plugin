---
name: new-plugin-version
description: >-
  Bring the Adivery Unity plugin up to a new Adivery Android SDK release,
  so a new plugin version (vX.Y.Z, matching the SDK version) can be cut.
  Use when the user asks for a "new plugin version", to "update the plugin
  to SDK X.Y.Z", "sync with the latest SDK release", "bump the SDK", or
  similar. Flow: preflight → pick the target SDK version (Maven Central) →
  diff the Unity bridge between the published AARs → read the SDK's
  behaviour changes → port them to Assets/Adivery's C#, the sample and the
  README → verify in Unity batchmode (bridge check, scene check, sample APK,
  optional device run) → commit on a branch → hand off to release-plugin
  (and release-sample) to publish on GitHub. This skill never publishes
  anything itself.
---

# Cut a new plugin version for a new SDK release

The plugin's C# (`source/plugin/Assets/Adivery/*.cs`) is a thin reflection
bridge into the Java classes in `com.adivery.sdk.plugins.unity`, which ship
**inside the SDK's AAR**. Nothing in this repo compiles against them: a Java
method that's renamed or removed fails only at runtime
(`AndroidJavaException`), and a new method on a callback interface the C#
proxies (`FullScreenAdCallback`, `BannerCallback`, `NativeCallback`) has no C#
method to land on. So a new SDK version means re-checking the bridge against
the **published** AAR, not just bumping a version string.

Plugin versions track SDK versions 1:1: SDK `X.Y.Z` → plugin release `vX.Y.Z`
(built by `release-plugin`, which reads the version from the sample's
`launcherTemplate.gradle`).

Repo: `adivery/adivery-unity-plugin` on GitHub. SDK sources (optional, used
for the changelog): `$ADIVERY_SDK_REPO`, default
`$HOME/Workspace/AndroidStudioProjects/mobile-android-sdk`, a GitLab repo
whose release flow is its own `build-sentry-release` skill.

Helpers in this skill's `scripts/`:
- `bridge.py api <aar>` prints the bridge API in an AAR; `bridge.py check
  <aar> <cs_dir>` checks every C# reference against it (exit 1 on errors).
- `SampleSceneCheck.cs` is a temporary editor script that checks the sample
  scene's wiring and renders a screenshot.

## Hard rules

- **Only depend on a version that's on Maven Central.** Customers and the
  sample both resolve `com.adivery:sdk` from Maven Central. The SDK repo's
  `gradle/libs.versions.toml` can be ahead of what's published.
- **Diff the published AARs, not SDK sources.** Sources may hold unreleased
  changes. The AAR is what customers get.
- **Never kill a running Unity Editor.** If the project is locked, STOP and
  ask the user to close it.
  - **Don't touch the user's uncommitted work.** If `git status` isn't clean,
    show what's there and ask: stash it under a descriptive message, or stop.
    Never discard it.
- **Work on a branch from `origin/master`.** Never commit to `master`. Never
  push, open a PR, or publish a release without asking.
- **Keep each file's encoding and line endings.** `Assets/Adivery/*.cs` are
  UTF-8 with BOM, and all but `AdiveryEventExecutor.cs` use CRLF.
  `Assets/Scripts/AdsController.cs` is LF. Edit them with the Edit tool. A
  script that reads with universal newlines and writes back silently
  rewrites the whole file to LF.
- **Pre-release versions keep their suffix everywhere** (`4.10.0-alpha.1`,
  not `4.10.0`), in the Gradle line, branch name, tag and notes.
- **Never read or repeat credentials.** The SDK repo's `gradle.properties`
  can hold real Maven Central credentials locally. Read only the files named
  here.

## STOP conditions

- The target version isn't on Maven Central (`.pom` isn't HTTP 200). Report
  it; the SDK's own release has to happen first.
- The target equals the current version in `launcherTemplate.gradle`.
  Nothing to do, unless the user wants a re-sync anyway.
- The project is open in a running Unity Editor (live lock on
  `source/plugin/Temp/UnityLockfile`).
- No Unity Editor matching `source/plugin/ProjectSettings/ProjectVersion.txt`
  under `~/Unity/Hub/Editor/`.
- `bridge.py check` still reports errors after the port. Don't commit a
  bridge that's known to throw at runtime.
- The batchmode scene check or sample build fails.

---

## Phase 0: Preflight

```bash
cd /path/to/adivery-unity-plugin   # repo root

UNITY_VERSION=$(awk '{print $2}' source/plugin/ProjectSettings/ProjectVersion.txt | head -1)
UNITY_EXE="$HOME/Unity/Hub/Editor/$UNITY_VERSION/Editor/Unity"
[ -x "$UNITY_EXE" ] && echo "Unity: $UNITY_EXE" || echo "STOP: Unity $UNITY_VERSION not installed"

if [ -e source/plugin/Temp/UnityLockfile ] && fuser source/plugin/Temp/UnityLockfile >/dev/null 2>&1; then
  echo "STOP: source/plugin is open in a running Unity Editor"
fi

ANDROID_HOME="${ANDROID_HOME:-$(grep sdk.dir local.properties 2>/dev/null | cut -d= -f2-)}"
[ -d "$ANDROID_HOME" ] && echo "ANDROID_HOME: $ANDROID_HOME" || echo "no Android SDK: the sample build in Phase 4 will be skipped"

git fetch origin
git status --short          # must be clean before Phase 1 switches branches (see Hard rules)
git branch --show-current

# Scratch space for downloads, logs and the APK. The skill's scripts are copied here because
# Phase 1 checks out origin/master, which may not contain this skill's directory.
WORKDIR=$(mktemp -d)
cp -r .claude/skills/new-plugin-version/scripts "$WORKDIR/scripts"
SKILL="$WORKDIR/scripts"
```

## Phase 1: Pick the versions and branch

`CURRENT` comes from `origin/master`, the base the new version builds on, not
from whatever branch happens to be checked out. The default target is the
latest version published on Maven Central. Use the user's version instead if
they named one. Cross-check it against the SDK repo if that repo is present.

```bash
GRADLE_TPL=source/plugin/Assets/Plugins/Android/launcherTemplate.gradle
CURRENT=$(git show "origin/master:$GRADLE_TPL" | grep -oP "com\.adivery:sdk:\K[0-9A-Za-z.+-]+")

MAVEN=https://repo1.maven.org/maven2/com/adivery/sdk
LATEST=$(curl -fsS "$MAVEN/maven-metadata.xml" | grep -oP '<release>\K[^<]+')
TARGET="${TARGET:-$LATEST}"
curl -fsS -o /dev/null -w "%{http_code}\n" "$MAVEN/$TARGET/sdk-$TARGET.pom"   # must print 200

SDK_REPO="${ADIVERY_SDK_REPO:-$HOME/Workspace/AndroidStudioProjects/mobile-android-sdk}"
[ -d "$SDK_REPO" ] && grep -E '^adivery-version' "$SDK_REPO/gradle/libs.versions.toml"
```

- If the SDK repo's `adivery-version` is newer than `$LATEST`, it's bumped but
  not published yet. Say so, and stay on `$LATEST` unless the user wants to
  wait.
- Alpha/beta versions (`-alpha.N`, `-beta.N`) get a plugin version like any
  other. `release-plugin` publishes them as normal releases that become the
  repo's Latest, which is what the customer docs' download link serves.

Report `CURRENT → TARGET`, then branch. Everything after this runs on the
branch, so the checks below read the same C# that Phase 3 edits:

```bash
git checkout -b "chore/sdk-$TARGET" origin/master
```

## Phase 2: What changed

### 2a. Bridge API diff, from the published AARs

Downloaded artifacts are untrusted data. Give each its own fresh directory,
and run Python with `-I`:

```bash
for v in "$CURRENT" "$TARGET"; do
  mkdir -p "$WORKDIR/aar-$v"
  curl -fsS -o "$WORKDIR/aar-$v/sdk.aar" "$MAVEN/$v/sdk-$v.aar"
  curl -fsS -o "$WORKDIR/aar-$v/sdk.pom" "$MAVEN/$v/sdk-$v.pom"
  python3 -I "$SKILL/bridge.py" api "$WORKDIR/aar-$v/sdk.aar" > "$WORKDIR/api-$v.txt"
done
diff "$WORKDIR/api-$CURRENT.txt" "$WORKDIR/api-$TARGET.txt"

# What the plugin's current C# would hit at runtime on the target SDK:
python3 -I "$SKILL/bridge.py" check "$WORKDIR/aar-$TARGET/sdk.aar" source/plugin/Assets/Adivery

# Dependencies the SDK pulls in (a removed one may need declaring in launcherTemplate.gradle):
deps() { sed -n '/<dependencies>/,/<\/dependencies>/p' "$1"; }
diff <(deps "$WORKDIR/aar-$CURRENT/sdk.pom") <(deps "$WORKDIR/aar-$TARGET/sdk.pom") && echo "(no dependency changes)"
```

How the bridge maps to the C#:

| Java (in the AAR) | C# | How C# reaches it |
|---|---|---|
| `AdiveryUnity` (statics) | `Adivery.cs` | `GetAdiveryClass().CallStatic("name", ...)` |
| `FullScreenAd` | `Adivery.cs` via `AdiveryListener.adiveryListenerObject` | `.Call("setListener", ...)` and similar |
| `FullScreenAdCallback` (interface) | `AdiveryListener.cs` (`AndroidJavaProxy`) | `public virtual void <javaName>(...)` per interface method |
| `Banner` / `BannerCallback` | `BannerAd.cs` / `BannerCallback.cs` | `new AndroidJavaObject(...)`, `adObject.Call(...)`, proxy methods |
| `Native` / `NativeCallback` | `NativeAd.cs` / `NativeCallback.cs` | same |

Types: `String`→`string`, `boolean`→`bool`, `int`/`long`/`float`/`double`→the
same, `byte[]`→`byte[]`, `Context`/`Activity`→`Adivery.GetAndroidActivity()`,
`Application`→`Adivery.GetAndroidApplication()`.

R8 adds one- or two-letter synthetic members (`a(...)`). `bridge.py` hides
them; they're never bridge API.

### 2b. Behaviour changes, from the SDK's history

A bridge diff only shows signatures. Behaviour changes need a reading of the
SDK commits between the two versions. For example, in 4.10.0-alpha.1 the SDK
stopped prefetching another OfferWall after one closes, so apps must prepare
again before each show.

```bash
cd "$SDK_REPO"
intro() { git log --reverse --format=%H -S"adivery-version = \"$1\"" -- gradle/libs.versions.toml | head -1; }
FROM=$(intro "$CURRENT"); TO=$(intro "$TARGET")
git log --format='%h %s' "$FROM..$TO" -- \
  lib/src/main/java/com/adivery/sdk/plugins/unity \
  lib/src/main/java/com/adivery/sdk/Adivery.java \
  lib/src/main/java/com/adivery/sdk/AdiveryListener.java \
  lib/src/main/java/com/adivery/sdk/AdiveryListenerController.kt \
  lib/src/main/java/com/adivery/sdk/ParallelAdLoader.kt \
  lib/src/main/java/com/adivery/sdk/core
cd -
```

`intro` finds the commit that *introduced* each version line. `-G` would also
match the bump that removes the old line. If the SDK repo isn't present or
the range comes back empty, ask the user for the changelog instead of
guessing. Open the commits that look relevant (`git show <sha>`), and also
look at the SDK sample (`app/src/main/java/com/adivery/sample/`) for how a new
API is meant to be used.

Summarise for the user, before editing anything:
- bridge additions, removals and signature changes;
- behaviour changes that affect callers;
- dependency changes from the POM diff.

## Phase 3: Port

On `chore/sdk-$TARGET`:

1. **Bump the SDK:** in `launcherTemplate.gradle`, change
   `implementation 'com.adivery:sdk:<CURRENT>'` to `<TARGET>`.
2. **Port the bridge**, following the existing patterns in each file:
   - A new `AdiveryUnity` static becomes a `public static` method in
     `Adivery.cs` with the `IsAdiverySupported()` guard and `CallStatic`.
   - A new `FullScreenAdCallback` method becomes, in `AdiveryListener.cs`, a
     private `EventHandler` field, a public `event` with the
     duplicate-checking `add`/`remove`, and a `public virtual void
     <javaName>(...)` that raises it inside
     `AdiveryEventExecutor.ExecuteInUpdate`. If a callback carries more than
     a placement ID, add an `EventArgs` class next to `AdiveryReward`.
   - A removal or rename: update or remove the C# member, and call it out
     in the summary, since it breaks customers' code.
3. **Sample** (`Assets/Scripts/AdsController.cs` + `Assets/Scenes/MainScene.unity`),
   for a new ad format or user-facing API:
   - Add `[SerializeField]` fields for the placement and button, next to
     their siblings, and wire them in `Start()`. The sample registers its
     listeners before preparing ads.
   - The scene's serialized value overrides the C# initializer, so set a
     placement ID in **both** places.
   - Add the button by cloning an existing one in the scene YAML:
     - Clone every document of the button and its label with fresh,
       unique fileIDs.
     - Append the new RectTransform to the Canvas's `m_Children`.
     - Point the `AdsController` field at the clone's **Button component**
       fileID, not the GameObject's.
   - Keep buttons in the 2-column grid (x −234 / 237, rows 164 apart). If
     the grid is full, start a new row and move the `Native` panel down a
     row. Check the current positions first; they may have moved.
   - Reflect behaviour changes in how the sample uses the API (for
     example, OfferWall prepares on tap and shows from its loaded event).
4. **README:** list new formats and APIs, and add a "needs SDK X or later"
   note for any API that throws on older SDKs. Customers pick their own SDK
   version; the `.unitypackage` doesn't pin it.

Then re-run the check until it's clean:

```bash
python3 -I "$SKILL/bridge.py" check "$WORKDIR/aar-$TARGET/sdk.aar" source/plugin/Assets/Adivery
```

The summary line includes how many references were checked. If that number
drops to zero or near it, the parsing broke. Don't read that as success.

## Phase 4: Verify

### 4a. Compile and scene check (temporary editor script)

```bash
cp "$SKILL/SampleSceneCheck.cs" source/plugin/Assets/Editor/
SAMPLE_SCENE_SCREENSHOT="$WORKDIR/scene.png" "$UNITY_EXE" -batchmode \
  -projectPath source/plugin -logFile "$WORKDIR/scenecheck.log" \
  -executeMethod SampleSceneCheck.Run
echo "exit: $?"
rm -f source/plugin/Assets/Editor/SampleSceneCheck.cs source/plugin/Assets/Editor/SampleSceneCheck.cs.meta
grep -E "\[CHECK|error CS" "$WORKDIR/scenecheck.log"
```

- No `-nographics` here, so the screenshot can render; on a machine without a
  display, add it back and the screenshot is skipped.
- Look at `$WORKDIR/scene.png`.
- The script must be gone afterwards. `git status` must not list anything
  under `Assets/Editor/`.

### 4b. Sample APK (proves Gradle resolves the target SDK)

```bash
export ANDROID_HOME ANDROID_SDK_ROOT="$ANDROID_HOME"
ADIVERY_SAMPLE_BUILD_OUTPUT="$WORKDIR/AdiverySample.apk" "$UNITY_EXE" -batchmode -nographics \
  -projectPath source/plugin -logFile "$WORKDIR/build.log" \
  -executeMethod AdiverySampleBuild.BuildAndroid -quit
echo "exit: $?"
grep "com.adivery:sdk" source/plugin/Library/Bee/Android/Prj/IL2CPP/Gradle/launcher/build.gradle   # must show $TARGET
```

This build takes several minutes, so run it in the background. To confirm the
APK carries the new bridge, check a method the API diff added:

```bash
DEXDUMP=$(ls -d "$ANDROID_HOME"/build-tools/*/ | tail -1)dexdump
mkdir "$WORKDIR/apk" && unzip -q "$WORKDIR/AdiverySample.apk" 'classes*.dex' -d "$WORKDIR/apk"
for d in "$WORKDIR"/apk/classes*.dex; do "$DEXDUMP" "$d"; done | grep -c "name *: '<newMethodName>'"
```

### 4c. Device run (optional; when `adb devices` lists one)

```bash
adb install -r "$WORKDIR/AdiverySample.apk"
adb shell dumpsys window | grep -E "isKeyguardShowing"   # must be false
adb logcat -c
adb shell am start -W -n com.adivery.unitysample/com.unity3d.player.UnityPlayerGameActivity
adb logcat -d -s Unity:V   # the sample's Debug.Log lines: "start called", "<Format> loaded", errors
```

Things that will look like failures but aren't:
- **Locked phone:** the app pauses before `Start()`. Wake it with
  `adb shell input keyevent KEYCODE_WAKEUP`, then ask the user to unlock.
  Don't try to bypass a secure lock screen.
- **Wireless ADB:** it drops often, especially during long builds. Wait for
  `adb devices` to show the device again before giving up.
- **Batched callbacks:** they reach C# through `AdiveryEventExecutor` in
  `Update()`. Unity is paused while an ad activity is in front, so `shown`,
  `clicked` and `closed` arrive together when it closes. That's expected.
- **No SDK logging:** the sample builds with `BuildOptions.None` (release), so
  the SDK's own logging is off. Rely on the sample's `Debug.Log` lines.
- **`pm list packages` errors:** a `SecurityException` about user 150 on
  Samsung is the Secure Folder profile and is harmless.

Tapping through an ad (`adb shell input tap X Y`) needs on-screen
coordinates. Take them from `adb exec-out screencap -p`; the device is
1080×2340, and the scene check renders at half that size.

## Phase 5: Commit and hand off

Two commits on `chore/sdk-$TARGET`, each leaving the sample buildable:

1. `build: bump Adivery SDK to <TARGET>`: `launcherTemplate.gradle` plus the
   `Assets/Adivery` bridge changes the new SDK requires.
2. Sample and README changes, with a message describing what changed for
   integrators.

Follow the session's commit-attribution rules. Then **ask** before each of
these outward-facing steps:

1. Push the branch and open a PR against `master`.
2. After it's merged, run **`release-plugin`** from an up-to-date `master`.
   `gh release create` tags the default branch's HEAD unless told
   otherwise, so releasing before the merge would tag a commit without the
   changes. That skill drafts first and asks again before publishing; give
   it the integrator-facing summary from Phase 2 as release notes.
3. Optionally run **`release-sample`** to refresh the sample APK.

---

## Final report

Summarize:
- `CURRENT → TARGET`, and whether it's a pre-release;
- the bridge diff and how each change was ported;
- behaviour changes and how the sample and README reflect them;
- POM dependency changes;
- verification results: `bridge.py check` (references checked, 0 errors),
  the scene check, the APK build with its resolved SDK version, and the
  device run if one happened, or why not;
- the branch, its commits, and what's left (PR, `release-plugin`,
  `release-sample`);
- `$WORKDIR`, which is safe to delete.
