using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorCanvas : UICanvas
{
    public RectTransform cursorImage;

#if UNITY_ANDROID && !UNITY_EDITOR
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public override void Open()
    {
        gameObject.SetActive(false);
    }
#else
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        Vector2 cursorPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cursorImage.parent.GetComponent<RectTransform>(),
            Input.mousePosition,
            null, // Sử dụng null nếu Canvas Render Mode là Screen Space - Overlay
            out cursorPos
        );
        cursorImage.localPosition = cursorPos;
    }
#endif
}
