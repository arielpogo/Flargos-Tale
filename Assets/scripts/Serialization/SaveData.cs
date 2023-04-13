using System.Collections;
using UnityEngine;

/// <summary>
/// Class for SaveData object, is a singleton
/// Therefore, anything that needs to be saved will directly access this class's current object
/// ex. SaveData.current.playerStats.playerName = "Timmy";
/// </summary>

[System.Serializable]
public class SaveData {
    private static SaveData _current;
    //singleton behavior
    public static SaveData current {
        get {
            if(_current == null) {
                _current = new SaveData();
            }
            return _current;
        }
    }

    public PlayerStats playerStats;


}