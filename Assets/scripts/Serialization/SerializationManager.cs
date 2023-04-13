using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// This is the class which actually does the saving and loading
/// To load:
///     SaveData.current = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/--SAVENAME--.SAVE")
/// </summary>

public class SerializationManager : MonoBehaviour{
    public static bool Save(string saveName, object saveData) {
        BinaryFormatter formatter = GetBinaryFormatter();

        if(!Directory.Exists(Application.persistentDataPath + "/saves")) Directory.CreateDirectory(Application.persistentDataPath + "/saves"); //create save folder

        string path = Application.persistentDataPath + "/saves" + saveName + ".SAVE"; //create path to the save file

        FileStream file = File.Create(path); //create the save file
        formatter.Serialize(file, saveData); //serialize the data into the save file
        file.Close();

        return true;
    }

    public static object Load(string path) {
        if (!File.Exists(path)) {
            return null; //nothing to load, no save file
        }

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);

        //in case of wrong path
        try {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch {
            Debug.LogErrorFormat("Failed to load file at {0}", path);
            file.Close();
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter() {
        return new BinaryFormatter();
    }
}
