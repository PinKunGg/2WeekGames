using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    SoundManager sm;
    SoundSetting sound;

    private void Start() {
        sm = SoundManager.soundman;
        sound = SoundSetting.sound;

        foreach(var item in Resources.FindObjectsOfTypeAll<Button>())
        {
            item.onClick.AddListener(PlaySFX);
        }
    }

    public int SoundIndex = 0;
    float Time = 0.2f;
    public void PlaySFX()
    {
        if(!sm){return;}
        sm.PlaySFX(SoundIndex,Vector3.zero,Time,sound.sc.SFXvalue);
    }
}
