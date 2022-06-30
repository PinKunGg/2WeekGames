using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBG : MonoBehaviour
{
    AudioSource audioSource;
    SoundManager soundManager;
    SoundSetting sound;
    public AudioClip[] AllBossBGSound;
    public AudioClip MainMenuBGSounds;
    // Start is called before the first frame update
    void Start()
    {
        sound = SoundSetting.sound;
        audioSource = GetComponent<AudioSource>(); ;
        soundManager = GetComponent<SoundManager>();
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            soundManager.PlaySFX(0, Camera.main.gameObject.transform.position, 5, sound.GetSfxVolume());
        }
    }

    public void OnclickToStart() 
    {
        Debug.Log("???");
        audioSource.Play();
    }

    public void ChangeBGSound(int bossNumber) 
    {
        audioSource.clip = AllBossBGSound[bossNumber];
        audioSource.Play();
    }

    public void ChangeToMainMenuBGSound() 
    {
        audioSource.clip = MainMenuBGSounds;
        audioSource.Play();
    }
}
