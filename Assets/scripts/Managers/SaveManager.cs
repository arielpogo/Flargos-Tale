using System.Collections.Generic;
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

    public Item[] GameItem = new Item[] {
        new("test item", false, true),
        new("Tyler's Hat", false, true)
    };

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

        //this might be problematic in the future, not sure
        PlayerData.status = 0;
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

    //****************************//
    //                            //
    //   PLAYER DATA INTERFACE    //
    //                            //
    //****************************//

    /// <summary>
    /// Please don't use negative parameters.
    /// </summary>
    /// <param name="dollars"></param>
    /// <param name="cents"></param>
    public void AddMoney(int dollars, int cents) {
        PlayerData.dollars += dollars;
        PlayerData.cents += cents;
        if(PlayerData.cents >= 100) {
            PlayerData.dollars += PlayerData.cents / 100;
            PlayerData.cents %= 100;
        }
    }

    /// <summary>
    /// Please don't use negative parameters.
    /// </summary>
    /// <param name="dollars"></param>
    /// <param name="cents"></param>
    /// <returns>Whether the transaction can go through (i.e. no negative money)</returns>
    public bool RemoveMoney(int dollars, int cents) {
        int tempDollars = PlayerData.dollars;
        int tempCents = PlayerData.cents;
        tempDollars -= dollars;
        tempCents -= cents;
        if(tempCents < 0) {
            int dollarsDown = tempCents / -100 + 1;
            tempCents += dollarsDown * 100;
            tempDollars -= dollarsDown;
        }
        if (tempDollars < 0) return false;
        else {
            PlayerData.dollars = tempDollars;
            PlayerData.cents = tempCents;
            return true;
        }
    }
}

//****************************//
//                            //
//      RELATED CLASSES       //
//                            //
//****************************//

//todo: may split into multiple files

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
    public int dollars = 1; //make my life easier, might change later
    public int cents = 10;
    public int debt = 0;
    public int TylerValue = 0;
    public string sceneName = "mainMenu";
    public Vector2 savedPos;
    public List<Item> inventory = new List<Item>();
}

public class Item {
    public string Name { get; private set; } = "DEFAULT_ITEM";
    public bool Stackable { get; private set; } = true;
    public bool Key { get; private set; } = false;  //key item aka deleteable?

    public Item(string name, bool stackable, bool key) {
        Name = name;
        Stackable = stackable;
        Key = key;
    }
}