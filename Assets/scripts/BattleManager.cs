using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : PersistentSingleton<BattleManager>{
    [SerializeField] private GameObject _character;
    private string SceneNameToReturnTo = string.Empty;

    public static Battle[] MasterBattleList = {
        new Battle(new Enemy("Tyler",3,"battlesprites/tyler"), ps: false),
        new Battle(new Enemy("Tyler",3,"battlesprites/tyler"), new Enemy("Tyler",3,"battlesprites/tyler"), new Enemy("Tyler",3,"battlesprites/tyler")),
        new Battle(new Enemy("Max",100,"battlesprites/max")),
        new Battle(new Enemy("Don",100,"battlesprites/don"))
    };
    
    public void StartBattle(int ID) {
        Battle battle = MasterBattleList[ID];
        SceneNameToReturnTo = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("battle");
        if(battle.playSong) SoundManager.Instance.PlaySong(Resources.Load(battle.song) as AudioClip);
    }

    public void EndBattle() {
        GameManager.Instance.ChangeGameState(GameState.overworld);
        SceneManager.LoadScene(SceneNameToReturnTo);
        SceneNameToReturnTo = string.Empty;
    }
}

public class Enemy {
    public Enemy(string n, int h, string s) {
        name = n;
        health = h;
        sprite = s;
    }

    string name;
    int health;
    string sprite;
}



public class Battle {
    public Battle(Enemy e1, bool ps = true, string s = "music/lab") {
        enemies.Add(e1);
        playSong = ps;
        song = s;
    }
    public Battle(Enemy e1, Enemy e2, bool ps = true, string s = "music/lab") {
        enemies.Add(e1);
        enemies.Add(e2);
        playSong = ps;
        song = s;
    }
    public Battle(Enemy e1, Enemy e2, Enemy e3, bool ps = true, string s = "music/lab") {
        enemies.Add(e1);
        enemies.Add(e2);
        enemies.Add(e3);
        playSong = ps;
        song = s;
    }

    public List<Enemy> enemies = new();
    public string song;
    public bool playSong = true;
}

