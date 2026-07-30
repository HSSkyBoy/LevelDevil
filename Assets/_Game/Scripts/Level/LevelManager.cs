using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private static LevelManager ins;
    public static LevelManager Ins => ins;

    public List<Map> mapList;
    [Header("Level Catalog (Preferred)")]
    public LevelCatalog levelCatalog;
    public Map mapScr;
    public int curMap;
    public MapSO mapSO;
    public Image imgBackGround;
    public bool endAnim;
    public bool endShakingScence;

    private List<Map> curMaplList = new List<Map>();
    

    private void Awake()
    {
        LevelManager.ins = this;
        OnInit();
    }

    public void OnInit()
    {
        curMap = PlayerPrefs.GetInt("CurrentMap", 0);
        if (mapSO != null)
        {
            mapSO.EnsureEntriesForCatalog(levelCatalog);
        }
        mapSO.LoadWinStates();
    }

    public int GetMapCount()
    {
        if (levelCatalog != null && levelCatalog.EntryCount > 0)
        {
            return levelCatalog.EntryCount;
        }

        return mapList != null ? mapList.Count : 0;
    }

    public void ResetWinStates()
    {
        // Reset trạng thái chiến thắng cho tất cả các màn trong mapSO
        for (int i = 0; i < mapSO.mapList.Count; i++)
        {
            mapSO.mapList[i].isWon = false;
        }

        Debug.Log("Reset all win states");
    }

    public void LoadMapByID(int id)
    {
        if (mapScr != null)
        {
            DespawnMap();
        }

        Map mapPrefab = GetMapPrefab(id);
        if (mapPrefab == null)
        {
            Debug.LogError("Unable to load Map with legacy index " + id + ".");
            return;
        }

        mapScr = SimplePool.Spawn<Map>(mapPrefab);
        mapScr.ResetState();
        curMaplList.Add(mapScr);
    }

    private Map GetMapPrefab(int id)
    {
        LevelEntry catalogEntry;
        if (levelCatalog != null && levelCatalog.TryGetByLegacyMapIndex(id, out catalogEntry))
        {
            return catalogEntry.MapPrefab;
        }

        if (mapList == null || id < 0 || id >= mapList.Count)
        {
            return null;
        }

        foreach (Map map in mapList)
        {
            if (map != null && map.id == id)
            {
                return mapList[id];
            }
        }

        return null;
    }

    public void DespawnMap()
    {
        if (mapScr != null)
        {
            foreach (Map map in curMaplList)
            {
                mapScr.ResetState();
                SimplePool.Despawn(map);
            }
            curMaplList.Clear();
            mapScr = null;
        }
    }

    public void DestroyWhenEsc()
    {
        if (mapScr != null)
        {
            DespawnMap();
        }
    }

    public void WaitForPlayerInputToRestart()
    {
        StartCoroutine(WaitForInput());
    }

    private IEnumerator WaitForInput()
    {
        Debug.Log("Chờ nhấn phím...");

        yield return new WaitForSeconds(1f);  // Chờ 1 giây trước khi bắt đầu kiểm tra phím

        while (true)
        {
            if (Input.anyKeyDown)
            {
                Debug.Log("Phím được nhấn, load level...");
                mapScr.LoadLevel(); // Gọi load level
                yield break;
            }

            yield return null;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
