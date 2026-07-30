using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class LevelCatalogPlayModeTests
{
    [UnityTest]
    public IEnumerator ExistingCatalogLoadsEveryConfiguredLevel()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        yield return null;

        GameObject managerObject = GameObject.Find("LevelManager");
        Assert.IsNotNull(managerObject, "SampleScene must contain LevelManager.");

        Behaviour levelManager = managerObject.GetComponent("LevelManager") as Behaviour;
        Assert.IsNotNull(levelManager, "LevelManager GameObject must have a LevelManager component.");

        levelManager.enabled = true;
        yield return null;

        System.Type managerType = levelManager.GetType();
        MethodInfo getMapCount = managerType.GetMethod("GetMapCount");
        MethodInfo loadMapById = managerType.GetMethod("LoadMapByID");
        MethodInfo despawnMap = managerType.GetMethod("DespawnMap");
        FieldInfo activeMapField = managerType.GetField("mapScr");

        Assert.IsNotNull(getMapCount);
        Assert.IsNotNull(loadMapById);
        Assert.IsNotNull(despawnMap);
        Assert.IsNotNull(activeMapField);

        int mapCount = (int)getMapCount.Invoke(levelManager, null);
        Assert.AreEqual(18, mapCount, "The existing catalog must expose all 18 Maps.");

        HashSet<Object> uniqueLevelPrefabs = new HashSet<Object>();
        for (int mapIndex = 0; mapIndex < mapCount; mapIndex++)
        {
            loadMapById.Invoke(levelManager, new object[] { mapIndex });

            Component activeMap = activeMapField.GetValue(levelManager) as Component;
            Assert.IsNotNull(activeMap, "Map " + mapIndex + " did not load.");

            System.Type mapType = activeMap.GetType();
            FieldInfo levelListField = mapType.GetField("levelList");
            FieldInfo curLevelField = mapType.GetField("CurLevel");
            FieldInfo activeLevelField = mapType.GetField("level");
            MethodInfo loadLevel = mapType.GetMethod("LoadLevel");

            Assert.IsNotNull(levelListField);
            Assert.IsNotNull(curLevelField);
            Assert.IsNotNull(activeLevelField);
            Assert.IsNotNull(loadLevel);

            IList levelList = levelListField.GetValue(activeMap) as IList;
            Assert.IsNotNull(levelList, "Map " + mapIndex + " has no levelList.");
            Assert.Greater(levelList.Count, 0, "Map " + mapIndex + " has an empty levelList.");

            for (int levelIndex = 0; levelIndex < levelList.Count; levelIndex++)
            {
                Object levelPrefab = levelList[levelIndex] as Object;
                Assert.IsNotNull(levelPrefab, "Map " + mapIndex + " contains a null Level prefab at index " + levelIndex + ".");
                uniqueLevelPrefabs.Add(levelPrefab);

                curLevelField.SetValue(activeMap, levelIndex);
                loadLevel.Invoke(activeMap, null);

                Assert.IsNotNull(activeLevelField.GetValue(activeMap), "Map " + mapIndex + " failed to load Level " + levelIndex + ".");
            }

            despawnMap.Invoke(levelManager, null);
            yield return null;
            Assert.IsNull(activeMapField.GetValue(levelManager), "Map " + mapIndex + " did not despawn.");
        }

        Assert.AreEqual(81, uniqueLevelPrefabs.Count, "The 18 Map definitions should retain the 81 original Level Prefabs.");
    }
}
