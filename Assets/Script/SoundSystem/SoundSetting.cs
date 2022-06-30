using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    public static SoundSetting sound;
    private void Awake() {
        sound = this;
        try{bgmSource = GameObject.Find("Sound").GetComponent<AudioSource>();}catch{}
    }

    [SerializeField] Slider bgm_bar;
    [SerializeField] Slider sfx_bar;
    AudioSource bgmSource;

    public SaveClassSound sc;

    JsonSaveSystem js;
    private void OnEnable() {
        js = JsonSaveSystem.js;

        try{
            SaveManager.saveEvent.AddListener(Save);
            SaveManager.loadEvent.AddListener(Load);
        }catch{}
    }

    void Save(){
        string data = JsonUtility.ToJson(sc, true);
        js.SaveJson(Application.persistentDataPath + "/Setting","Sound", data);
    }
    void Load(){
        string loadData = js.LoadJson(Application.persistentDataPath + "/Setting","Sound");
        if (string.IsNullOrEmpty(loadData)){
            Debug.LogError("Fail to load Sound");
            LoadSoundSetting();
            return;
        }

        JsonUtility.FromJsonOverwrite(loadData, sc);

        Debug.LogWarningFormat("Load Sound complete");

        LoadSoundSetting();
    }

    public void OnClickSaveSetting(){
        Save();
    }
    public void OnClickLoadSetting(){
        Load();
    }

    public void LoadSoundSetting(){
        try{
            bgm_bar.value = sc.BGMvalue;
            sfx_bar.value = sc.SFXvalue;
        }catch{}

        try{bgmSource.volume = sc.BGMvalue / bgm_bar.maxValue;}catch{}
    }

    public void OnBgmChange(){
        sc.BGMvalue = bgm_bar.value;
        try{bgmSource.volume = sc.BGMvalue / bgm_bar.maxValue;}catch{}
    }

    public void OnSfxChange(){
        sc.SFXvalue = sfx_bar.value;
    }

    public float GetSfxVolume(){
        return sc.SFXvalue / sfx_bar.maxValue;
    }
}

[System.Serializable]
public class SaveClassSound
{
    public float BGMvalue = 1f;
    public float SFXvalue = 1f;
}