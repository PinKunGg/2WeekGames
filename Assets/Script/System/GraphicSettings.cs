using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class GraphicSettings : MonoBehaviour
{
    public TMP_Dropdown QualitySelection, ResolutionSelection;
    public SaveClassGraphic sc;
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
        js.SaveJson(Application.persistentDataPath + "/Setting","Graphic", data);
    }
    void Load(){
        string loadData = js.LoadJson(Application.persistentDataPath + "/Setting","Graphic");
        if (string.IsNullOrEmpty(loadData)){
            LoadGraphicSetting();
            Debug.LogError("Fail to load Sound");
            return;
        }

        JsonUtility.FromJsonOverwrite(loadData, sc);

        Debug.LogWarningFormat("Load Sound complete");

        LoadGraphicSetting();
    }

    int[] currecntResolution = new int[2];
    bool isFullScreen;
    public void LoadGraphicSetting(){
        QualitySettings.SetQualityLevel(sc.QualityIndex);
        QualitySelection.value = sc.QualityIndex;
        ResolutionSelection.value = sc.ResolutionIndex;

        switch(sc.ResolutionIndex){
            case 0:
            currecntResolution[0] = 1920;
            currecntResolution[1] = 1080;
            break;
            case 1:
            currecntResolution[0] = 1600;
            currecntResolution[1] = 900;
            break;
            case 2:
            currecntResolution[0] = 1366;
            currecntResolution[1] = 768;
            break;
            case 3:
            currecntResolution[0] = 1280;
            currecntResolution[1] = 720;
            break;

            default:
            currecntResolution[0] = 1920;
            currecntResolution[1] = 1080;
            break;
        }

        Screen.SetResolution(currecntResolution[0],currecntResolution[1],isFullScreen);
    }

    public void OnClickSaveSetting(){
        Save();
    }
    public void OnClickLoadSetting(){
        Load();
    }

    public void OnQualityChange(){
        sc.QualityIndex = QualitySelection.value;
        LoadGraphicSetting();
    }
    public void OnResolutionChange(){
        sc.ResolutionIndex = ResolutionSelection.value;
        LoadGraphicSetting();
    }
}

[System.Serializable]
public class SaveClassGraphic
{
    public int QualityIndex = 1;
    public int ResolutionIndex = 0;
}
