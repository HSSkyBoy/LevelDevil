using System.Collections.Generic;
using UnityEngine;

public class SelectLevelUI : UICanvas
{
    [SerializeField] private GameObject[] listGOBJ;
    [SerializeField] private int gateNum;

    private void Start()
    {
        LoadGate();
    }

    public void LoadGate()
    {
        int availableGateCount = Mathf.Min(listGOBJ.Length, LevelManager.Ins.GetMapCount());
        gateNum = Mathf.Clamp(LevelManager.Ins.curMap + 1, 0, availableGateCount);
        Debug.Log(gateNum);

        for (int i = 0; i < gateNum; i++)
        {
            listGOBJ[i].gameObject.SetActive(true);
        }

        if (gateNum == listGOBJ.Length - 1)
        {
            listGOBJ[listGOBJ.Length-1].gameObject.SetActive(true);
        }
    }

    public void ResetAllGate()
    {
        for (int i = 1; i < listGOBJ.Length; i++)
        {
            listGOBJ[i].gameObject.SetActive(false);
        }
    }
}
