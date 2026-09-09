# Adivery Unity Plugin

A Unity plugin that wraps the [Adivery](https://adivery.com) Android SDK
(mediated ad network: rewarded, interstitial, banner, and native ads) for
use from C# in a Unity project. Android only — every call is a no-op on
other platforms.

Repo layout:

```
Assets/Adivery/            → the plugin itself (this is what gets exported/installed)
Assets/Plugins/Android/    → adivery-sdk.aar + Gradle template used when building the sample
source/plugin/             → a full Unity project used to build/export the package and host the sample scene
build.gradle                → `exportPackage` task: builds Adivery.unitypackage from source/plugin/Assets/Adivery
```

## Requirements

- Unity **6000.0.75f1** (see `source/plugin/ProjectSettings/ProjectVersion.txt`) with Android Build Support installed
- Android SDK/NDK (Unity's bundled Android module works; `ANDROID_HOME`/`ANDROID_SDK_ROOT` must point at one)
- JDK 21 (only needed for the Gradle `exportPackage` task, not for building the sample from the Unity Editor)

## Installing the plugin in your own project

The distributable is a `.unitypackage` containing `Assets/Adivery` and the
bundled `adivery-sdk.aar`. Build it with:

```bash
UNITY_EXE=/path/to/Unity ./gradlew exportPackage
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

To build headlessly instead, add an Editor script under
`Assets/Editor/` calling `BuildPipeline.BuildPlayer` for `BuildTarget.Android`
with `MainScene` in the scene list, then run it via:

```bash
UNITY_EXE=/path/to/Unity ANDROID_HOME=/path/to/sdk \
  "$UNITY_EXE" -batchmode -nographics \
    -projectPath source/plugin \
    -executeMethod YourBuildScript.BuildAndroid \
    -quit
```

Then install/run on a connected device:

```bash
adb install -r path/to/AdiverySample.apk
adb shell monkey -p com.adivery.unitysample -c android.intent.category.LAUNCHER 1
```

## License

Apache License 2.0 — see [LICENSE](LICENSE).
