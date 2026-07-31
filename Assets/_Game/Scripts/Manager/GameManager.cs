using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    //[SerializeField] UserData userData;
    //[SerializeField] CSVData csv;
    //private static GameState gameState = GameState.MainMenu;

    // Start is called before the first frame update
    public bool isActive;

    protected void Awake()
    {
        DOTween.SetTweensCapacity(500, 50);
        //base.Awake();
        Input.multiTouchEnabled = true;
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // Android owns the surface size.  Forcing a lower fullscreen resolution here
        // made the render target, the world canvas, and overlay UI disagree about the
        // display dimensions on high-density and ultra-wide devices.  It also caused
        // a surface reconfiguration immediately after startup.  Keep the native
        // surface and let the canvases/camera adapt to its actual aspect ratio.

        //csv.OnInit();
        //userData?.OnInitData();

        //ChangeState(GameState.MainMenu);
        if (isActive)
        {
            UIManager.Ins.OpenUI<StartScene>();
        }
    }

    //public static void ChangeState(GameState state)
    //{
    //    gameState = state;
    //}

    //public static bool IsState(GameState state)
    //{
    //    return gameState == state;
    //}

}

