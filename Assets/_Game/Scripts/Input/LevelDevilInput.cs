using UnityEngine;

/// <summary>
/// Single input adapter for gameplay and navigation.
/// Keyboard input remains available in the Editor; mobile controls feed the same state.
/// </summary>
public static class LevelDevilInput
{
    private static bool leftHeld;
    private static bool rightHeld;
    private static bool jumpHeld;
    private static int jumpPressedFrame = -1;
    private static int jumpReleasedFrame = -1;
    private static int pointerPressedFrame = -1;

    public static int Move
    {
        get
        {
            if (leftHeld || rightHeld)
            {
                return (rightHeld ? 1 : 0) - (leftHeld ? 1 : 0);
            }

            bool keyboardLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
            bool keyboardRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
            return (keyboardRight ? 1 : 0) - (keyboardLeft ? 1 : 0);
        }
    }

    public static bool JumpPressed =>
        jumpPressedFrame == Time.frameCount || Input.GetKeyDown(KeyCode.Space);

    public static bool JumpReleased =>
        jumpReleasedFrame == Time.frameCount || Input.GetKeyUp(KeyCode.Space);

    public static bool JumpHeld =>
        jumpHeld || Input.GetKey(KeyCode.Space);

    public static bool RestartPressed
    {
        get
        {
            if (pointerPressedFrame == Time.frameCount || Input.anyKeyDown)
            {
                return true;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    return true;
                }
            }
#endif
            return false;
        }
    }

    public static bool BackPressed => Input.GetKeyDown(KeyCode.Escape);

    public static void SetLeftHeld(bool held)
    {
        leftHeld = held;
    }

    public static void SetRightHeld(bool held)
    {
        rightHeld = held;
    }

    public static void SetJumpHeld(bool held)
    {
        if (jumpHeld == held)
        {
            return;
        }

        jumpHeld = held;
        if (held)
        {
            jumpPressedFrame = Time.frameCount;
            NotifyPointerPressed();
        }
        else
        {
            jumpReleasedFrame = Time.frameCount;
        }
    }

    public static void NotifyPointerPressed()
    {
        pointerPressedFrame = Time.frameCount;
    }

    public static void ClearTouchState()
    {
        leftHeld = false;
        rightHeld = false;
        jumpHeld = false;
        jumpPressedFrame = -1;
        jumpReleasedFrame = -1;
        pointerPressedFrame = -1;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        ClearTouchState();
    }
}
