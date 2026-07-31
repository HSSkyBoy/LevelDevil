using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class MobileControlsBootstrap : MonoBehaviour
{
    private const string EditorSimulationKey = "LevelDevil.ShowMobileControlsInEditor";
    private static MobileControlsBootstrap instance;

    private Canvas controlsCanvas;
    private GraphicRaycaster raycaster;
    private bool lastVisibility;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Create()
    {
        if (instance != null)
        {
            return;
        }

        GameObject root = new GameObject("MobileControls");
        instance = root.AddComponent<MobileControlsBootstrap>();
        DontDestroyOnLoad(root);
        instance.BuildCanvas();
    }

    public static void RefreshVisibility()
    {
        if (instance != null)
        {
            instance.ApplyVisibility(true);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        ApplyVisibility(false);
#endif
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            LevelDevilInput.ClearTouchState();
        }
    }

    private void BuildCanvas()
    {
        controlsCanvas = gameObject.AddComponent<Canvas>();
        controlsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        controlsCanvas.sortingOrder = 10;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        raycaster = gameObject.AddComponent<GraphicRaycaster>();

        GameObject safeArea = new GameObject("SafeArea", typeof(RectTransform), typeof(SafeAreaPanel));
        safeArea.transform.SetParent(transform, false);
        Stretch(safeArea.GetComponent<RectTransform>());

        CreateButton(safeArea.transform, "Left", "<", MobileControlAction.Left,
            new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(48f, 48f));
        CreateButton(safeArea.transform, "Right", ">", MobileControlAction.Right,
            new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(236f, 48f));
        CreateButton(safeArea.transform, "Jump", "JUMP", MobileControlAction.Jump,
            new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-48f, 48f));

        ApplyVisibility(true);
    }

    private static void CreateButton(
        Transform parent,
        string name,
        string label,
        MobileControlAction action,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(MobileControlButton));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(160f, 160f);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.06f, 0.06f, 0.06f, 0.58f);
        image.raycastTarget = true;
        buttonObject.GetComponent<MobileControlButton>().Initialize(action);

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
        labelObject.transform.SetParent(buttonObject.transform, false);
        Stretch(labelObject.GetComponent<RectTransform>());

        Text text = labelObject.GetComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = action == MobileControlAction.Jump ? 34 : 64;
        text.fontStyle = FontStyle.Bold;
        text.color = new Color(1f, 1f, 1f, 0.9f);
        text.raycastTarget = false;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void ApplyVisibility(bool force)
    {
        bool shouldShow = ShouldShow();
        if (!force && shouldShow == lastVisibility)
        {
            return;
        }

        controlsCanvas.enabled = shouldShow;
        raycaster.enabled = shouldShow;
        if (!shouldShow)
        {
            LevelDevilInput.ClearTouchState();
        }

        lastVisibility = shouldShow;
    }

    private static bool ShouldShow()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return true;
#elif UNITY_EDITOR
        return UnityEditor.EditorPrefs.GetBool(EditorSimulationKey, false);
#else
        return false;
#endif
    }
}
