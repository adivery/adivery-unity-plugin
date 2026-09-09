using UnityEditor;
using UnityEngine;

// Dev tooling for building the sample app (MainScene) in batchmode, used by
// the `release-sample` skill. Lives under Assets/Editor so it's excluded
// from player builds and NOT part of the exported Adivery.unitypackage
// (the `release-plugin` skill only exports Assets/Adivery).
public static class AdiverySampleBuild
{
    private const string MainScene = "Assets/Scenes/MainScene.unity";

    public static void BuildAndroid()
    {
        Build(BuildTarget.Android, "ADIVERY_SAMPLE_BUILD_OUTPUT", "build/AdiverySample.apk");
    }

    public static void BuildLinux64()
    {
        Build(BuildTarget.StandaloneLinux64, "ADIVERY_SAMPLE_BUILD_OUTPUT", "build/AdiverySample.x86_64");
    }

    private static void Build(BuildTarget target, string outputEnvVar, string defaultOutput)
    {
        string outputPath = System.Environment.GetEnvironmentVariable(outputEnvVar);
        if (string.IsNullOrEmpty(outputPath))
        {
            outputPath = defaultOutput;
        }

        var options = new BuildPlayerOptions
        {
            scenes = new[] { MainScene },
            locationPathName = outputPath,
            target = target,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);
        Debug.Log("Build result: " + report.summary.result);
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            EditorApplication.Exit(1);
        }
    }
}
