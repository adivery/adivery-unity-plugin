# Adivery Unity Plugin

A Unity plugin that wraps the [Adivery](https://adivery.com) Android SDK
(mediated ad network: rewarded, interstitial, banner, and native ads) for
use from C# in a Unity project. Android only — every call is a no-op on
other platforms.

Repo layout:

```
Assets/Adivery/            → the plugin itself (this is what gets exported/installed)
Assets/Plugins/Android/    → Gradle template used when building the sample (the SDK itself is a Maven dependency, not a bundled AAR)
source/plugin/             → a full Unity project used to build/export the package and host the sample scene
```

## Requirements

- Unity **6000.0.75f1** (see `source/plugin/ProjectSettings/ProjectVersion.txt`) with Android Build Support installed
- Android SDK/NDK (Unity's bundled Android module works; `ANDROID_HOME`/`ANDROID_SDK_ROOT` must point at one) — needed to build the sample APK; not needed to export the plugin package itself
- JDK, matching whatever Unity's own internal Android/Gradle build needs — only relevant when building the sample APK, not for exporting the plugin package

## Installing the plugin in your own project

The distributable is a `.unitypackage` containing just `Assets/Adivery`
(confirmed against the real, currently-published release — no bundled AAR;
the native SDK is pulled in as a Maven dependency by whoever imports the
package, per the [Unity integration docs](https://adivery.com/unity)).
Build it with the `release-plugin` Claude Code skill in this repo
(`.claude/skills/release-plugin/`), or manually:

```bash
UNITY_EXE=/path/to/Unity "$UNITY_EXE" -batchmode -nographics \
  -projectPath source/plugin \
  -exportPackage Assets/Adivery Adivery.unitypackage \
  -quit
```

This produces `Adivery.unitypackage` at the repo root. Import it into your
Unity project via `Assets > Import Package > Custom Package`.

Basic usage once imported:

```csharp
using AdiveryUnity;

Adivery.Configure("YOUR_APP_ID");

var listener = new AdiveryListener();
listener.OnInterstitialAdLoaded += (s, placementId) => Adivery.Show(placementId);
Adivery.AddListener(listener);
Adivery.PrepareInterstitialAd("YOUR_PLACEMENT_ID");
```

See `Assets/Adivery/Adivery.cs`, `AdiveryListener.cs`, `BannerAd.cs`, and
`NativeAd.cs` for the full API (interstitial/rewarded via `Adivery` +
`AdiveryListener` events, banners via `BannerAd`, native ads via `NativeAd`).

Note: `Adivery.AddListener`/`RemoveListener`/`AddPlacementListener`/
`RemovePlacementListener` are `internal`, so your calling code must live in
the same assembly as the plugin — i.e. don't put it under a custom
`.asmdef` unless that `.asmdef` also references `Assets/Adivery`'s (the
plugin ships with no `.asmdef` of its own, so this only matters if you add
one yourself).

## Running the sample app

`source/plugin` is a self-contained Unity project. Its `MainScene`
(`Assets/Scenes/MainScene.unity`, driven by `Assets/Scripts/AdsController.cs`)
exercises every ad type against a demo app ID/placements.

Open `source/plugin` directly in Unity Hub/Editor, then `File > Build
Settings > Android > Build And Run` (with a device connected) or just press
Play in the Editor to test in-Editor UI wiring (ads themselves only work on
device, since the SDK is Android-only).

To build headlessly instead, use the batchmode build script this repo
already ships, `Assets/Editor/BuildScripts/AdiverySampleBuild.cs`
(excluded from player builds and from the exported plugin package):

```bash
UNITY_EXE=/path/to/Unity ANDROID_HOME=/path/to/sdk \
  ADIVERY_SAMPLE_BUILD_OUTPUT=/path/to/AdiverySample.apk \
  "$UNITY_EXE" -batchmode -nographics \
    -projectPath source/plugin \
    -executeMethod AdiverySampleBuild.BuildAndroid \
    -quit
```

(`ADIVERY_SAMPLE_BUILD_OUTPUT` is optional — it defaults to
`build/AdiverySample.apk` under `source/plugin/`.)

Or use the `release-sample` Claude Code skill in this repo
(`.claude/skills/release-sample/`) to build *and* publish it as a GitHub
release in one step.

Then install/run on a connected device:

```bash
adb install -r path/to/AdiverySample.apk
adb shell monkey -p com.adivery.unitysample -c android.intent.category.LAUNCHER 1
```

## License

Apache License 2.0 — see [LICENSE](LICENSE).
