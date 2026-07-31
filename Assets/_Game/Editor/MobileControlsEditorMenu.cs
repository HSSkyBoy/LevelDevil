using UnityEditor;

public static class MobileControlsEditorMenu
{
    private const string EditorSimulationKey = "LevelDevil.ShowMobileControlsInEditor";
    private const string MenuPath = "Tools/LevelDevil/Show Mobile Controls In Editor";

    [MenuItem(MenuPath)]
    private static void Toggle()
    {
        bool nextValue = !EditorPrefs.GetBool(EditorSimulationKey, false);
        EditorPrefs.SetBool(EditorSimulationKey, nextValue);
        Menu.SetChecked(MenuPath, nextValue);
        MobileControlsBootstrap.RefreshVisibility();
    }

    [MenuItem(MenuPath, true)]
    private static bool Validate()
    {
        Menu.SetChecked(MenuPath, EditorPrefs.GetBool(EditorSimulationKey, false));
        return true;
    }
}
