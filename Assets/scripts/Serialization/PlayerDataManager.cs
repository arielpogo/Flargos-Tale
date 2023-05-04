using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveManager : PersistentSingleton<SaveManager>{
    /// <summary>
    /// Class to hold player data
    /// </summary>
    [System.Serializable]
    public class PlayerStats {
        public string playerName;
        public int strength;
        public int stealth;
    }

    public void Save() { 
    }
    //Singleton.Instance
    public string[] SaveFiles {
        get {
            string[] saves;
            if (!Directory.Exists(Application.persistentDataPath + "/saves/")) Directory.CreateDirectory(Application.persistentDataPath + "/saves/");

            saves = Directory.GetFiles(Application.persistentDataPath + "/saves/", "*.SAVE");
            return saves;
        }
    }
}
