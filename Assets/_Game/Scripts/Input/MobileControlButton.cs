using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MobileControlAction
{
    Left,
    Right,
    Jump
}

public sealed class MobileControlButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private readonly HashSet<int> activePointers = new HashSet<int>();
    private MobileControlAction action;

    public void Initialize(MobileControlAction controlAction)
    {
        action = controlAction;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (activePointers.Add(eventData.pointerId))
        {
            LevelDevilInput.NotifyPointerPressed();
            ApplyHeld(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ReleasePointer(eventData.pointerId);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ReleasePointer(eventData.pointerId);
    }

    private void OnDisable()
    {
        activePointers.Clear();
        ApplyHeld(false);
    }

    private void ReleasePointer(int pointerId)
    {
        if (!activePointers.Remove(pointerId))
        {
            return;
        }

        if (activePointers.Count == 0)
        {
            ApplyHeld(false);
        }
    }

    private void ApplyHeld(bool held)
    {
        switch (action)
        {
            case MobileControlAction.Left:
                LevelDevilInput.SetLeftHeld(held);
                break;
            case MobileControlAction.Right:
                LevelDevilInput.SetRightHeld(held);
                break;
            case MobileControlAction.Jump:
                LevelDevilInput.SetJumpHeld(held);
                break;
        }
    }
}
