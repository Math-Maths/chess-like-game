using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ChessGame.Managers
{
    public class DataManager : MonoBehaviour
    {
        public static string SavePath => Path.Combine(Application.persistentDataPath, "saveData.json");

        public void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log("Data saved");
        }

        public SaveData Load()
        {
            if (!File.Exists(SavePath))
            {
                return null;
            }

            string json = File.ReadAllText(SavePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data;
        }

        public void DeleteSaveData()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public string playerName;
        public int currentLevel;

        public List<string> unlockedPiecesID;
        public List<TeamPiecesData> team;
    }
}