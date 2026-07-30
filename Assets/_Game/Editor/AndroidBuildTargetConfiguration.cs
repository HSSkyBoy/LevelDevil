using UnityEditor;

public static class AndroidBuildTargetConfiguration
{
    [MenuItem("Tools/LevelDevil/Android/Activate Android Build Target")]
    public static void ActivateAndroidBuildTarget()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            return;
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
    }

    [MenuItem("Tools/LevelDevil/Android/Activate Android Build Target", true)]
    private static bool CanActivateAndroidBuildTarget()
    {
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }
}
