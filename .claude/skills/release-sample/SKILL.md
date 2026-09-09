---
name: release-sample
description: >-
  Build the Adivery Unity plugin's sample app (source/plugin, MainScene) for
  Android and publish it as its own GitHub release asset. Use when the user
  asks to "release the sample", "build and release the sample app",
  "publish the sample app on GitHub", or similar. This is NOT the plugin
  release flow — it never touches Adivery.unitypackage, the AAR, or
  pub-equivalent publishing, and it never attaches to the plugin's own
  vX.Y.Z release for that SDK version; the sample gets its own dedicated
  release. Flow: preflight checks → batchmode build → publish the APK
  (create a DRAFT release first if one doesn't exist yet, prompting before
  publishing; otherwise just update the asset on the existing sample
  release).
---

# Release the sample app

This skill builds `source/plugin`'s sample scene (`MainScene`, driven by
`Assets/Scripts/AdsController.cs`) for Android and publishes the APK as a
GitHub release of its own. It does **not** build or publish
`Adivery.unitypackage` — that's a separate, manual flow (`./gradlew
exportPackage` + the existing tag-per-SDK-version releases) — and it does
**not** attach anything to that release either; the sample's release is
independent, under its own tag namespace, so the two never collide or need
to be kept in sync.

Repo: `adivery/adivery-unity-plugin` (origin: `https://github.com/adivery/adivery-unity-plugin.git`)
Unity project: `source/plugin` (scene: `Assets/Scenes/MainScene.unity`)
Sample app id: `com.adivery.unitysample`
Build script: `source/plugin/Assets/Editor/BuildScripts/AdiverySampleBuild.cs`
(`-executeMethod AdiverySampleBuild.BuildAndroid`, batchmode-only, excluded
from player builds and from the exported unitypackage)

## Hard rules

- **Never kill a running Unity Editor.** If the project is already open
  (locked), STOP and ask the user to close it themselves — they may have
  unsaved changes open. Do not `kill`/`pkill` Unity on their behalf even if
  it looks idle.
- **Human gate before a new release goes from draft → published.** Only
  applies the first time (see Phase 2) — once the sample's release exists,
  later runs just update its asset in place, no repeat confirmation needed.
- If any STOP condition below is hit, halt and report — do not work around it.

## STOP conditions (halt and report before doing anything)

- The project is already open in a running Unity Editor (a live lock on
  `source/plugin/Temp/UnityLockfile`) — ask the user to close it.
- No Unity Editor install matching `source/plugin/ProjectSettings/ProjectVersion.txt` found under `~/Unity/Hub/Editor/`.
- `source/plugin/Assets/Plugins/Android/launcherTemplate.gradle` doesn't
  contain `unityStreamingAssets.tokenize` — i.e. it's regressed to the old
  `noCompress` format that breaks the Android build (this bit us before;
  see git history on that file). Don't try to fix it as part of this
  skill — report it and stop.
- The batchmode build exits non-zero, or no APK ends up at the expected path.
- `gh auth status` fails.
- `git status --short` shows uncommitted changes unrelated to this skill's
  own build artifacts — surface them and ask before continuing (never
  commit on the user's behalf here; this skill doesn't need to).

---

## Phase 0 — Preflight

```bash
cd /path/to/adivery-unity-plugin   # repo root

# 1. Version, for the release notes/title — the Adivery SDK version the
#    sample currently targets. Not used for the tag, deliberately: the
#    sample release is independent of the plugin's own vX.Y.Z releases.
VERSION=$(grep -oP "com\.adivery:sdk:\K[0-9.]+" source/plugin/Assets/Plugins/Android/launcherTemplate.gradle)
TAG="sample-latest"
echo "Sample currently targets Adivery SDK $VERSION -> release tag $TAG"

# 2. Regression guard on the Gradle template (see STOP conditions)
grep -q "unityStreamingAssets.tokenize" source/plugin/Assets/Plugins/Android/launcherTemplate.gradle \
  && echo "launcherTemplate.gradle OK" \
  || echo "STOP: launcherTemplate.gradle has regressed to the old noCompress format"

# 3. Resolve the matching Unity Editor
UNITY_VERSION=$(awk '{print $2}' source/plugin/ProjectSettings/ProjectVersion.txt | head -1)
UNITY_EXE="$HOME/Unity/Hub/Editor/$UNITY_VERSION/Editor/Unity"
[ -x "$UNITY_EXE" ] && echo "Unity: $UNITY_EXE" || echo "STOP: Unity $UNITY_VERSION not installed at $UNITY_EXE"

# 4. Project must not already be open in an Editor
if [ -e source/plugin/Temp/UnityLockfile ] && fuser source/plugin/Temp/UnityLockfile >/dev/null 2>&1; then
  echo "STOP: source/plugin is open in a running Unity Editor — ask the user to close it first"
fi

# 5. Android SDK
ANDROID_HOME="${ANDROID_HOME:-$(grep sdk.dir local.properties 2>/dev/null | cut -d= -f2-)}"
[ -d "$ANDROID_HOME" ] && echo "ANDROID_HOME: $ANDROID_HOME" || echo "STOP: no usable Android SDK found"

# 6. GitHub auth
gh auth status

# 7. Working tree
git status --short

# 8. Does the sample's own release already exist? Decides Phase 2's path.
gh release view "$TAG" --repo adivery/adivery-unity-plugin >/dev/null 2>&1 \
  && echo "Release $TAG exists — will just update its asset" \
  || echo "No release $TAG yet — will create it as a draft"
```

Report the SDK version, the tag, Unity/Android SDK paths, and whether
`$TAG` already exists, before proceeding.

## Phase 1 — Build the sample APK

```bash
export ANDROID_HOME ANDROID_SDK_ROOT="$ANDROID_HOME"
WORKDIR=$(mktemp -d)
APK="$WORKDIR/AdiverySample.apk"
LOG="$WORKDIR/build.log"

ADIVERY_SAMPLE_BUILD_OUTPUT="$APK" "$UNITY_EXE" -batchmode -nographics \
  -projectPath source/plugin \
  -logFile "$LOG" \
  -executeMethod AdiverySampleBuild.BuildAndroid \
  -quit
echo "exit: $?"   # STOP if non-zero — tail "$LOG" and report the failure

ls -la "$APK"      # STOP if missing/zero bytes
```

## Phase 2 — Publish to GitHub

Asset name: `AdiverySample.apk` — kept stable so the download link never
changes shape across updates.

**If `$TAG` already exists** (the common case after the first run): just
replace its asset, no confirmation needed — this is the expected,
repeatable "latest sample build" update:

```bash
gh release upload "$TAG" "$APK#AdiverySample.apk" \
  --repo adivery/adivery-unity-plugin --clobber
gh release edit "$TAG" --repo adivery/adivery-unity-plugin \
  --notes "Sample app build. Currently targets Adivery SDK $VERSION."
gh release view "$TAG" --repo adivery/adivery-unity-plugin --json url -q .url
```

**If `$TAG` doesn't exist yet:** create it as a **draft** first (human gate
applies):

```bash
gh release create "$TAG" \
  --repo adivery/adivery-unity-plugin \
  --title "Sample app" \
  --notes "Sample app build. Currently targets Adivery SDK $VERSION." \
  --draft \
  "$APK#AdiverySample.apk"
```

Show the user the draft URL and ask whether to publish it. Only on
confirmation:

```bash
gh release edit "$TAG" --repo adivery/adivery-unity-plugin --draft=false
```

---

## Final report

Summarize: the SDK version the build targets, the release URL, and the
asset name (`AdiverySample.apk`). Note the local `$WORKDIR` path if the
user wants the APK itself, and that it's safe to delete afterward.
