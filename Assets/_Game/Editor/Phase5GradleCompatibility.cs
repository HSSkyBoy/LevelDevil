using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

public sealed class Phase5GradleCompatibility : IPostGenerateGradleAndroidProject
{
    private const string EnableKey = "LevelDevil.Phase5CompileSdk35";

    public static void EnableCompileSdk35Workaround()
    {
        SessionState.SetBool(EnableKey, true);
    }

    public static void DisableCompileSdk35Workaround()
    {
        SessionState.SetBool(EnableKey, false);
    }

    public int callbackOrder => 0;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        string projectRoot = Directory.GetParent(path)?.FullName;
        if (!SessionState.GetBool(EnableKey, false) || !Directory.Exists(projectRoot))
        {
            return;
        }

        foreach (string gradlePath in Directory.GetFiles(projectRoot, "*.gradle", SearchOption.AllDirectories))
        {
            string contents = File.ReadAllText(gradlePath);
            string patched = Regex.Replace(contents, @"compileSdkVersion\s+\d+", "compileSdkVersion 35");
            patched = Regex.Replace(patched, @"compileSdk\s+\d+", "compileSdk 35");
            patched = Regex.Replace(patched, @"minSdkVersion\s+\d+", "minSdkVersion 24");
            if (patched != contents)
            {
                File.WriteAllText(gradlePath, patched);
            }
        }

        string gradlePropertiesPath = Path.Combine(projectRoot, "gradle.properties");
        string aapt2Path = Path.GetFullPath("Library/Phase5Tools/android-sdk/build-tools/35.0.0/aapt2.exe")
            .Replace('\\', '/');
        string properties = File.Exists(gradlePropertiesPath)
            ? File.ReadAllText(gradlePropertiesPath)
            : string.Empty;
        const string propertyPrefix = "android.aapt2FromMavenOverride=";
        string[] lines = properties.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);
        bool replaced = false;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith(propertyPrefix, System.StringComparison.Ordinal))
            {
                lines[i] = propertyPrefix + aapt2Path;
                replaced = true;
            }
        }
        properties = string.Join(System.Environment.NewLine, lines);
        if (!replaced)
        {
            properties += System.Environment.NewLine + propertyPrefix + aapt2Path + System.Environment.NewLine;
        }
        File.WriteAllText(gradlePropertiesPath, properties);
        Debug.Log("Phase 5 Gradle AAPT2 override: " + aapt2Path);
    }
}
