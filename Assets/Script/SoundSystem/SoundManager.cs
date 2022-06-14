using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundman;
    public bool isPlayBGM = true;

    [Header("Container")]
    public AudioClip[] Bgm;
    public AudioClip[] Sfx;

    [Space]
    public AudioSource BGMSource;
    bool isCanPlaySFX = true;

    private void Awake(){
        soundman = this;
    }
    private void Start(){
        if(isPlayBGM){
            PlayBGM(0);
        }
    }
    public void PlayBGM(int value){
        BGMSource.clip = Bgm[value];
        BGMSource.Play();
    }
    public void PlaySFX(int value, Vector3 pos, float DestroyTime, float volume)
    {
        if(isCanPlaySFX){
            isCanPlaySFX = false;
            GameObject sfx = ObjectPooler.pool.GetFromPool("PlaySFX");
            sfx.transform.position = pos;
            sfx.transform.rotation = Quaternion.identity;

            sfx.GetComponent<AudioSource>().clip = Sfx[value];
            sfx.GetComponent<AudioSource>().volume = volume;
            sfx.GetComponent<AudioSource>().Play();

            StartCoroutine(DestroyPlaySFX(sfx,DestroyTime));
            Debug.Log("playSFX = " + value);
            Invoke("DelaySFX",0.001f);
        }
    }
    IEnumerator DestroyPlaySFX(GameObject obj,float time){
        yield return new WaitForSeconds(time);
        obj?.SetActive(false);
    }

    void DelaySFX(){
        isCanPlaySFX = true;
    }
}
