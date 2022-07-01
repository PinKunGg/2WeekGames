using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingMenu : MonoBehaviour
{
    public GameObject SettingMenu_ui;

    private void Awake() {
        Application.targetFrameRate = 60;
    }

    private void Start() {
        OnClick_ToggleSettingMenu(false);
    }

    private void Update() {
        if(Application.targetFrameRate != 60){
            Application.targetFrameRate = 60;
        }

        if(SceneManager.GetActiveScene().buildIndex == 0){return;}

        if(Input.GetKeyDown(KeyCode.Escape)){
            OnClick_ToggleSettingMenu(true);
        }
    }

    public void OnClick_ToggleSettingMenu(bool value){
        SettingMenu_ui.SetActive(value);
        
        try{
            CursorSettings.cursor.isOverride = value;
            CursorSettings.cursor.ToggleCursor(value);
        }
        catch{}
    }
}
