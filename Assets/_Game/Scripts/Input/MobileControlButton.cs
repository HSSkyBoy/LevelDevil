using UnityEngine;
using UnityEngine.EventSystems;

public enum MobileControlAction
{
    Left,
    Right,
    Jump
}

public sealed class MobileControlButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private MobileControlAction action;

    public MobileControlAction Action => action;

    public void Initialize(MobileControlAction controlAction)
    {
        action = controlAction;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        LevelDevilInput.NotifyPointerPressed();
        LevelDevilInput.SetMobileControl(eventData.pointerId, action);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        LevelDevilInput.ReleaseMobileControl(eventData.pointerId);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (action == MobileControlAction.Jump)
        {
            return;
        }

        MobileControlButton target = eventData.pointerCurrentRaycast.gameObject == null
            ? null
            : eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<MobileControlButton>();

        if (target != null && target.action != MobileControlAction.Jump)
        {
            LevelDevilInput.SetMobileControl(eventData.pointerId, target.action);
        }
        else
        {
            LevelDevilInput.ReleaseMobileControl(eventData.pointerId);
        }
    }

    private void OnDisable()
    {
        LevelDevilInput.ReleaseMobileControls(action);
    }
}
