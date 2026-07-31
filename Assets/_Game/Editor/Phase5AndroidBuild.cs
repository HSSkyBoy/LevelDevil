using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class Phase5AndroidBuild
{
    private const string ApkRelativePath = "Builds/Android/LevelDevil-Phase5-Development.apk";

    [MenuItem("Tools/LevelDevil/Android/Build Phase 5 Development APK")]
    public static void BuildDevelopmentApk()
    {
        ConfigureAndroidPlayer();

        string[] scenes = Array.ConvertAll(
            Array.FindAll(
                EditorBuildSettings.scenes,
                scene => scene.enabled && !string.IsNullOrEmpty(scene.path)),
            scene => scene.path);

        if (scenes.Length == 0)
        {
            throw new BuildFailedException("No enabled scenes are configured for the Android build.");
        }

        string outputPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), ApkRelativePath));
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.Development | BuildOptions.AllowDebugging
        };

        Phase5GradleCompatibility.EnableCompileSdk35Workaround();
        BuildReport report;
        try
        {
            report = BuildPipeline.BuildPlayer(options);
        }
        finally
        {
            Phase5GradleCompatibility.DisableCompileSdk35Workaround();
        }
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new BuildFailedException(
                "Phase 5 Android build failed: " + report.summary.result +
                " (" + report.summary.totalErrors + " errors)");
        }

        Debug.Log(
            "Phase 5 Development APK built: " + outputPath +
            " (" + report.summary.totalSize + " bytes)");
    }

    [MenuItem("Tools/LevelDevil/Android/Apply Phase 5 Player Settings")]
    public static void ConfigureAndroidPlayer()
    {
        ConfigureExternalAndroidTools();

        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "top.nkbe.leveldevil");
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SplashScreen.show = false;
        PlayerSettings.SplashScreen.showUnityLogo = false;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)24;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)36;

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;

        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        AssetDatabase.SaveAssets();
    }

    private static void ConfigureExternalAndroidTools()
    {
        string jdkRoot = Environment.GetEnvironmentVariable("LEVELDEVIL_JDK_ROOT");
        string sdkRoot = Environment.GetEnvironmentVariable("ANDROID_HOME");
        string ndkRoot = Environment.GetEnvironmentVariable("LEVELDEVIL_NDK_ROOT");

        if (!string.IsNullOrEmpty(jdkRoot))
        {
            EditorPrefs.SetBool("JdkUseEmbedded", false);
            EditorPrefs.SetString("JdkPath", jdkRoot);
        }

        if (!string.IsNullOrEmpty(sdkRoot))
        {
            EditorPrefs.SetBool("SdkUseEmbedded", false);
            EditorPrefs.SetString("AndroidSdkRoot", sdkRoot);
        }

        if (!string.IsNullOrEmpty(ndkRoot))
        {
            EditorPrefs.SetBool("NdkUseEmbedded", false);
            EditorPrefs.SetBool("NdkDisableSettingValidation", true);
            EditorPrefs.SetString("NdkPath", ndkRoot);
            EditorPrefs.SetString("AndroidNdkPath", ndkRoot);
            EditorPrefs.SetString("AndroidNDKPath", ndkRoot);
            EditorPrefs.SetString("AndroidNdkRootR23B", ndkRoot);
            EditorPrefs.SetString("AndroidNdkRootR23b", ndkRoot);
            EditorPrefs.SetString("AndroidNDKRootR23B", ndkRoot);
            EditorPrefs.SetString("AndroidNDKRootR23b", ndkRoot);
            EditorPrefs.SetString("AndroidNdkRoot", ndkRoot);
            EditorPrefs.SetString("AndroidNDKRoot", ndkRoot);
        }

        Debug.Log($"Configured External Android Tools - JDK: '{jdkRoot}', SDK: '{sdkRoot}', NDK: '{ndkRoot}'");
    }
}
