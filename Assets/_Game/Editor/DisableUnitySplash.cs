using UnityEditor;

/// <summary>
/// Keeps the Unity-branded startup splash disabled for Android builds.
/// Unity can recreate the setting when switching platforms, so this is both
/// an explicit menu command and an automatic editor-load guard.
/// </summary>
[InitializeOnLoad]
public static class DisableUnitySplash
{
    static DisableUnitySplash()
    {
        Apply();
    }

    [MenuItem("Tools/LevelDevil/Android/Disable Unity Startup Splash")]
    public static void Apply()
    {
        PlayerSettings.SplashScreen.show = false;
        PlayerSettings.SplashScreen.showUnityLogo = false;
        AssetDatabase.SaveAssets();
    }
}
