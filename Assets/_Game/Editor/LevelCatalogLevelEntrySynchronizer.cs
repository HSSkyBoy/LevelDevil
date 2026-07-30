using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class LevelCatalogLevelEntrySynchronizer
{
    [MenuItem("Tools/LevelDevil/Synchronize Catalog Level Entries")]
    public static void SynchronizeCatalogLevelEntries()
    {
        LevelCatalog catalog = Selection.activeObject as LevelCatalog;
        if (catalog == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:LevelCatalog");
            if (guids.Length == 0)
            {
                Debug.LogError("No LevelCatalog asset was found to synchronize.");
                return;
            }

            catalog = AssetDatabase.LoadAssetAtPath<LevelCatalog>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        SerializedObject serializedCatalog = new SerializedObject(catalog);
        SerializedProperty levels = serializedCatalog.FindProperty("levelEntries");
        levels.arraySize = 0;

        HashSet<Level> seenLevels = new HashSet<Level>();
        int writeIndex = 0;
        for (int mapIndex = 0; mapIndex < catalog.Entries.Count; mapIndex++)
        {
            LevelEntry mapEntry = catalog.Entries[mapIndex];
            if (mapEntry == null || mapEntry.MapPrefab == null || mapEntry.MapPrefab.levelList == null)
            {
                continue;
            }

            for (int levelIndex = 0; levelIndex < mapEntry.MapPrefab.levelList.Count; levelIndex++)
            {
                Level level = mapEntry.MapPrefab.levelList[levelIndex];
                if (level == null || !seenLevels.Add(level))
                {
                    continue;
                }

                levels.InsertArrayElementAtIndex(writeIndex);
                SerializedProperty entry = levels.GetArrayElementAtIndex(writeIndex);
                entry.FindPropertyRelative("id").stringValue = "level-map-" + (mapEntry.LegacyMapIndex + 1).ToString("D3") + "-" + (levelIndex + 1).ToString("D3");
                entry.FindPropertyRelative("displayName").stringValue = mapEntry.DisplayName + " / Level " + (levelIndex + 1);
                entry.FindPropertyRelative("difficulty").enumValueIndex = (int)LevelDifficulty.Unspecified;
                entry.FindPropertyRelative("prefab").objectReferenceValue = level;
                writeIndex++;
            }
        }

        serializedCatalog.ApplyModifiedProperties();
        EditorUtility.SetDirty(catalog);
        AssetDatabase.SaveAssets();
        Debug.Log("Synchronized " + writeIndex + " unique Level entries into " + AssetDatabase.GetAssetPath(catalog) + ".");
    }
}
