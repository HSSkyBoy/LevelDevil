using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class LevelQuickTestLauncher
{
    private const string PreviewLevelPathKey = "LevelDevil.PreviewLevelPath";
    private const string PreviousScenePathKey = "LevelDevil.PreviewPreviousScenePath";
    private const string RuntimeScenePath = "Assets/Scenes/SampleScene.unity";

    static LevelQuickTestLauncher()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    [MenuItem("Tools/LevelDevil/Play Selected Level %#p", true)]
    private static bool ValidatePlaySelectedLevel()
    {
        return GetSelectedLevel() != null && !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    [MenuItem("Tools/LevelDevil/Play Selected Level %#p")]
    private static void PlaySelectedLevel()
    {
        Level level = GetSelectedLevel();
        if (level == null)
        {
            Debug.LogError("Select a Level Prefab before starting a LevelDevil preview.");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        SessionState.SetString(PreviewLevelPathKey, AssetDatabase.GetAssetPath(level));
        SessionState.SetString(PreviousScenePathKey, UnityEngine.SceneManagement.SceneManager.GetActiveScene().path);
        EditorSceneManager.OpenScene(RuntimeScenePath);
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            StartPreview();
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            RestorePreviousScene();
        }
    }

    private static void StartPreview()
    {
        string levelPath = SessionState.GetString(PreviewLevelPathKey, string.Empty);
        if (string.IsNullOrEmpty(levelPath))
        {
            return;
        }

        Level level = AssetDatabase.LoadAssetAtPath<Level>(levelPath);
        LevelManager manager = Resources.FindObjectsOfTypeAll<LevelManager>().Length > 0
            ? Resources.FindObjectsOfTypeAll<LevelManager>()[0]
            : null;
        if (level == null || manager == null)
        {
            Debug.LogError("LevelDevil preview could not find the selected Level or LevelManager.");
            return;
        }

        if (!manager.gameObject.activeSelf)
        {
            manager.gameObject.SetActive(true);
        }
        else if (!manager.enabled)
        {
            manager.enabled = true;
        }

        manager.LoadPreviewLevel(level);
        Debug.Log("LevelDevil preview started: " + level.name);
    }

    private static void RestorePreviousScene()
    {
        string previousScenePath = SessionState.GetString(PreviousScenePathKey, string.Empty);
        SessionState.EraseString(PreviewLevelPathKey);
        SessionState.EraseString(PreviousScenePathKey);

        if (!string.IsNullOrEmpty(previousScenePath) && previousScenePath != RuntimeScenePath)
        {
            EditorSceneManager.OpenScene(previousScenePath);
        }
    }

    private static Level GetSelectedLevel()
    {
        Level level = Selection.activeObject as Level;
        if (level != null)
        {
            return level;
        }

        GameObject selectedPrefab = Selection.activeObject as GameObject;
        return selectedPrefab != null ? selectedPrefab.GetComponent<Level>() : null;
    }
}
