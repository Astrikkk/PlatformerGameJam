using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;


public static class SaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "simple_save.json");

    public static void SaveGame(SaveData saveData)
    {
        try
        {
            string jsonData = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SavePath, jsonData);
            Debug.Log("Game saved to: " + SavePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save failed: " + e.Message);
        }
    }

    public static SaveData LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found. Creating new one.");
            return new SaveData();
        }

        try
        {
            string jsonData = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<SaveData>(jsonData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Load failed: " + e.Message);
            return new SaveData();
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted.");
        }
    }
}