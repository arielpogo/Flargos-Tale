using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles saving & loading savefiles.
/// </summary>
public class SaveManager : PersistentSingleton<SaveManager> {
    public const int maxSaveFiles = 3;

    public static PlayerStats PlayerData = new();

    public void Start() {
        GameEvents.Instance.OnDecideSave += Save;
    }
    public void OnDestroy() {
        if (GameEvents.Instance != null) GameEvents.Instance.OnDecideSave -= Save;
    }

    /// <summary>
    /// Gets all save files in the save folder.
    /// </summary>
    public string[] SaveFiles {
        get {
            string[] saves;
            if (!Directory.Exists(Application.persistentDataPath + "/saves/")) Directory.CreateDirectory(Application.persistentDataPath + "/saves/");

            saves = Directory.GetFiles(Application.persistentDataPath + "/saves/", "*.SAVE");
            return saves;
        }
    }

    public void Save() {
        string saveFolderPath = Application.persistentDataPath + "/saves/";
        BinaryFormatter formatter = new();

        //this might be problematic in the future, not sure
        PlayerData.status = 0;

        if (!Directory.Exists(saveFolderPath)) Directory.CreateDirectory(saveFolderPath); //create save folder

        FileStream file = File.Create(saveFolderPath + PlayerData.saveNum + ".SAVE"); //create the save file
        formatter.Serialize(file, PlayerData); //serialize the data into the save file
        file.Close();
    }

    public void Load(int saveNum) {
        string saveFilePath = Application.persistentDataPath + "/saves/" + saveNum + ".SAVE";

        if (!File.Exists(saveFilePath)) {
            PlayerData.status = 1;
            return;
        }

        BinaryFormatter formatter = new();

        FileStream file = File.Open(saveFilePath, FileMode.Open);

        //in case of wrong path or other
        try {
            PlayerData = formatter.Deserialize(file) as PlayerStats;
            file.Close();
        }
        catch {
            Debug.LogErrorFormat("Failed to load file at {0}", saveFilePath);
            PlayerData.status = 2;
            file.Close();
        }
    }

    public void Erase(int saveNum) {
        string saveFilePath = Application.persistentDataPath + "/saves/" + saveNum + ".SAVE";

        if (!File.Exists(saveFilePath)) return;

        try {
            System.IO.File.Delete(saveFilePath);
        }
        catch {
            Debug.LogErrorFormat("Failed to erase file at {0}", saveFilePath);
        }
    }
}
/// <summary>
/// Object which stores player data.
/// </summary>
[System.Serializable]
public class PlayerStats {
    /// <summary>
    /// 0 = valid |
    /// 1 = file not found |
    /// 2 = file couldnt be read
    /// </summary>
    public int status = 0; 
    public System.DateTime date;
    public int saveNum = 0;
    public int strength = 1;
    public int stealth = 1;
    public string sceneName = "mainMenu";
}

