using System.Collections.Generic;
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
    private static readonly Dictionary<int, MobileControlAction> mobileControls =
        new Dictionary<int, MobileControlAction>();

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

    /// <summary>
    /// Associates a finger with its current on-screen control. A finger can move
    /// between the left and right buttons without being lifted.
    /// </summary>
    public static void SetMobileControl(int pointerId, MobileControlAction action)
    {
        mobileControls[pointerId] = action;
        ApplyMobileControlState();
    }

    public static void ReleaseMobileControl(int pointerId)
    {
        if (mobileControls.Remove(pointerId))
        {
            ApplyMobileControlState();
        }
    }

    public static void ReleaseMobileControls(MobileControlAction action)
    {
        List<int> releasedPointers = null;
        foreach (KeyValuePair<int, MobileControlAction> control in mobileControls)
        {
            if (control.Value == action)
            {
                if (releasedPointers == null)
                {
                    releasedPointers = new List<int>();
                }

                releasedPointers.Add(control.Key);
            }
        }

        if (releasedPointers == null)
        {
            return;
        }

        foreach (int pointerId in releasedPointers)
        {
            mobileControls.Remove(pointerId);
        }

        ApplyMobileControlState();
    }

    private static void ApplyMobileControlState()
    {
        bool wasJumpHeld = jumpHeld;
        leftHeld = false;
        rightHeld = false;
        jumpHeld = false;

        foreach (MobileControlAction action in mobileControls.Values)
        {
            switch (action)
            {
                case MobileControlAction.Left:
                    leftHeld = true;
                    break;
                case MobileControlAction.Right:
                    rightHeld = true;
                    break;
                case MobileControlAction.Jump:
                    jumpHeld = true;
                    break;
            }
        }

        if (jumpHeld != wasJumpHeld)
        {
            if (jumpHeld)
            {
                jumpPressedFrame = Time.frameCount;
                NotifyPointerPressed();
            }
            else
            {
                jumpReleasedFrame = Time.frameCount;
            }
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
        mobileControls.Clear();
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
