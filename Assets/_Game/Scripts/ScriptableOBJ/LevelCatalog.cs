using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelCatalog", menuName = "LevelDevil/Level Catalog", order = 0)]
public class LevelCatalog : ScriptableObject
{
    [SerializeField] private List<LevelEntry> entries = new List<LevelEntry>();
    [SerializeField] private List<LevelDefinition> levelEntries = new List<LevelDefinition>();

    public IReadOnlyList<LevelEntry> Entries => entries;
    public int EntryCount => entries.Count;
    public IReadOnlyList<LevelDefinition> LevelEntries => levelEntries;
    public int LevelEntryCount => levelEntries.Count;

    public bool TryGetByLegacyMapIndex(int legacyMapIndex, out LevelEntry entry)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            LevelEntry candidate = entries[i];
            if (candidate != null && candidate.LegacyMapIndex == legacyMapIndex)
            {
                entry = candidate;
                return true;
            }
        }

        entry = null;
        return false;
    }

    public bool TryGetById(string id, out LevelEntry entry)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            LevelEntry candidate = entries[i];
            if (candidate != null && candidate.Id == id)
            {
                entry = candidate;
                return true;
            }
        }

        entry = null;
        return false;
    }

    public bool TryGetLevelById(string id, out LevelDefinition entry)
    {
        for (int i = 0; i < levelEntries.Count; i++)
        {
            LevelDefinition candidate = levelEntries[i];
            if (candidate != null && candidate.Id == id)
            {
                entry = candidate;
                return true;
            }
        }

        entry = null;
        return false;
    }
}

[Serializable]
public class LevelEntry
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private int legacyMapIndex = -1;
    [SerializeField] private ELevel legacyLevel = ELevel.None;
    [SerializeField] private Map mapPrefab;

    public string Id => id;
    public string DisplayName => displayName;
    public int LegacyMapIndex => legacyMapIndex;
    public ELevel LegacyLevel => legacyLevel;
    public Map MapPrefab => mapPrefab;
}

public enum LevelDifficulty
{
    Unspecified = 0,
    Easy = 1,
    Normal = 2,
    Hard = 3,
    Expert = 4
}

[Serializable]
public class LevelDefinition
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private LevelDifficulty difficulty = LevelDifficulty.Unspecified;
    [SerializeField] private Level prefab;

    public string Id => id;
    public string DisplayName => displayName;
    public LevelDifficulty Difficulty => difficulty;
    public Level Prefab => prefab;
}
