using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int rows;
    public int columns;
    public int score;
    public int combo;

    public List<int> cardIds;
    public List<bool> matched;
    public List<bool> revealed;
}

public static class SaveManager
{
    private const string SAVE_KEY = "MEMORY_GAME_SAVE";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        Debug.Log("Game saved: " + json);
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    public static SaveData Load()
    {
        if (!HasSave())
        {
            Debug.LogWarning("No save found!");
            return null;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        var data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }

    public static void Delete()
    {
        if (HasSave())
            PlayerPrefs.DeleteKey(SAVE_KEY);
    }
}