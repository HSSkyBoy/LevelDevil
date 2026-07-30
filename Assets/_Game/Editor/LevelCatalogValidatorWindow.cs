using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelCatalogValidatorWindow : EditorWindow
{
    private readonly List<ValidationMessage> messages = new List<ValidationMessage>();
    private LevelCatalog catalog;
    private Vector2 scrollPosition;

    [MenuItem("Tools/LevelDevil/Level Catalog Validator")]
    public static void Open()
    {
        GetWindow<LevelCatalogValidatorWindow>("Level Catalog Validator");
    }

    private void OnEnable()
    {
        if (catalog == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:LevelCatalog");
            if (guids.Length > 0)
            {
                catalog = AssetDatabase.LoadAssetAtPath<LevelCatalog>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Level Catalog Validation", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This window is read-only. It reports catalog and legacy-data issues without changing Prefabs, Scenes, or assets.", MessageType.Info);

        EditorGUI.BeginChangeCheck();
        catalog = (LevelCatalog)EditorGUILayout.ObjectField("Catalog", catalog, typeof(LevelCatalog), false);
        if (EditorGUI.EndChangeCheck())
        {
            messages.Clear();
        }

        using (new EditorGUI.DisabledScope(catalog == null))
        {
            if (GUILayout.Button("Validate Catalog"))
            {
                ValidateCatalog();
            }
        }

        EditorGUILayout.Space();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        if (messages.Count == 0)
        {
            EditorGUILayout.HelpBox("Choose a catalog and click Validate Catalog.", MessageType.None);
        }
        else
        {
            for (int i = 0; i < messages.Count; i++)
            {
                ValidationMessage message = messages[i];
                EditorGUILayout.HelpBox(message.Text, message.Type);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void ValidateCatalog()
    {
        messages.Clear();

        if (catalog == null)
        {
            AddError("No LevelCatalog asset is assigned.");
            return;
        }

        if (catalog.EntryCount == 0)
        {
            AddError("The catalog contains no entries.");
            return;
        }

        HashSet<string> ids = new HashSet<string>();
        HashSet<int> legacyIndices = new HashSet<int>();
        Dictionary<Level, string> levelOwners = new Dictionary<Level, string>();
        int highestLegacyIndex = -1;

        for (int i = 0; i < catalog.Entries.Count; i++)
        {
            LevelEntry entry = catalog.Entries[i];
            string label = "Entry " + i;
            if (entry == null)
            {
                AddError(label + " is null.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(entry.Id))
            {
                AddError(label + " has an empty stable ID.");
            }
            else if (!ids.Add(entry.Id))
            {
                AddError(label + " duplicates stable ID '" + entry.Id + "'.");
            }

            if (entry.LegacyMapIndex < 0)
            {
                AddError(label + " has a negative legacy map index.");
            }
            else if (!legacyIndices.Add(entry.LegacyMapIndex))
            {
                AddError(label + " duplicates legacy map index " + entry.LegacyMapIndex + ".");
            }
            else
            {
                highestLegacyIndex = Mathf.Max(highestLegacyIndex, entry.LegacyMapIndex);
            }

            ValidateMap(entry, label, levelOwners);
        }

        ValidateLegacyMapSO(highestLegacyIndex);

        if (!HasErrors())
        {
            messages.Insert(0, new ValidationMessage(MessageType.Info, "Validation completed with no errors."));
        }
    }

    private void ValidateMap(LevelEntry entry, string label, Dictionary<Level, string> levelOwners)
    {
        Map map = entry.MapPrefab;
        if (map == null)
        {
            AddError(label + " has no Map Prefab reference.");
            return;
        }

        if (map.id != entry.LegacyMapIndex)
        {
            AddWarning(label + " references " + map.name + " (Map.id=" + map.id + "), which differs from legacy index " + entry.LegacyMapIndex + ".");
        }

        if (map.levelList == null || map.levelList.Count == 0)
        {
            AddError(label + " references " + map.name + ", which has an empty levelList.");
            return;
        }

        for (int i = 0; i < map.levelList.Count; i++)
        {
            Level level = map.levelList[i];
            if (level == null)
            {
                AddError(label + " has a null Level reference at levelList[" + i + "].");
                continue;
            }

            string previousOwner;
            if (levelOwners.TryGetValue(level, out previousOwner))
            {
                AddWarning(label + " levelList[" + i + "] (" + level.name + ") is also used by " + previousOwner + ".");
            }
            else
            {
                levelOwners.Add(level, label + " levelList[" + i + "]");
            }

            if (level.GetComponentsInChildren<Gate>(true).Length == 0)
            {
                AddWarning(label + " levelList[" + i + "] (" + level.name + ") has no Gate component.");
            }

            if (level.GetComponentsInChildren<PlayerCtrl>(true).Length == 0)
            {
                AddWarning(label + " levelList[" + i + "] (" + level.name + ") has no PlayerCtrl component.");
            }
        }
    }

    private void ValidateLegacyMapSO(int highestLegacyIndex)
    {
        string[] guids = AssetDatabase.FindAssets("t:MapSO");
        if (guids.Length == 0)
        {
            AddWarning("No legacy MapSO asset was found. Catalog loading will still work, but legacy progress compatibility cannot be checked.");
            return;
        }

        MapSO mapSO = AssetDatabase.LoadAssetAtPath<MapSO>(AssetDatabase.GUIDToAssetPath(guids[0]));
        if (mapSO.mapList.Count <= highestLegacyIndex)
        {
            AddWarning("MapSO has " + mapSO.mapList.Count + " entries, while the catalog requires index " + highestLegacyIndex + ". LevelManager will add missing legacy progress entries at runtime.");
        }
    }

    private bool HasErrors()
    {
        for (int i = 0; i < messages.Count; i++)
        {
            if (messages[i].Type == MessageType.Error)
            {
                return true;
            }
        }

        return false;
    }

    private void AddError(string text)
    {
        messages.Add(new ValidationMessage(MessageType.Error, text));
    }

    private void AddWarning(string text)
    {
        messages.Add(new ValidationMessage(MessageType.Warning, text));
    }

    private struct ValidationMessage
    {
        public readonly MessageType Type;
        public readonly string Text;

        public ValidationMessage(MessageType type, string text)
        {
            Type = type;
            Text = text;
        }
    }
}
