using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : PersistentSingleton<SoundManager> { 



    public void PlaySound() {
        AudioSource _audiosource;
        _audiosource = gameObject.AddComponent<AudioSource>();


    }

    public enum Sound {

    }

}
