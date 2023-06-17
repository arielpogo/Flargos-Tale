using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour {
    public string Name;
    public Sprite DefaultSprite;
    public int Health;
    public AudioClip BattleTheme;
    
    virtual public void Fight() {

    }
    virtual public void Act() {
        
    }

    virtual public void Item() {

    }

    virtual public void Flee() {
        GameEvents.Instance.EndBattle.Invoke();
    }
}
