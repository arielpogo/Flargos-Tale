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

    //****************************//
    //                            //
    //      AWAKE/ONDESTROY       //
    //                            //
    //****************************//

    public new void Awake() {
        base.Awake();
        GameEvents.Instance.OnDecideSave += Save;
        GameEvents.Instance.LoadSave += Load;
    }
    public void OnDestroy() {
        if (GameEvents.Instance != null) {
            GameEvents.Instance.OnDecideSave -= Save;
            GameEvents.Instance.LoadSave -= Load;
        }
    }

    //****************************//
    //                            //
    //       SAVING/LOADING       //
    //                            //
    //****************************//

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

        PlayerData.status = 0; //this might be problematic in the future, not sure
        PlayerData.sceneName = SceneManager.GetActiveScene().name;
        PlayerData.date = System.DateTime.Now;
        PlayerData.savedPos = GameManager.Instance.Player.transform.position;

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

        //in case the file is invalid
        try {
            PlayerData = formatter.Deserialize(file) as PlayerStats;
            file.Close();
        }
        catch {
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