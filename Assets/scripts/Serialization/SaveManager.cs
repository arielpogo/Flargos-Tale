using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveManager : PersistentSingleton<SaveManager>{
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
