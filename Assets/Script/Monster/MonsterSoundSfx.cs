using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSoundSfx : MonoBehaviour
{
    SoundManager soundManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BossPlaySoundSfx(int value) 
    {
        if (soundManager == null) { soundManager = SoundManager.soundman; }
        soundManager.PlaySFX(value,this.transform.position,2,0.7f);
    }
}
