using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    public GameObject SettingMenu_ui;

    private void Start() {
        OnClick_ToggleSettingMenu(false);
    }

    public void OnClick_ToggleSettingMenu(bool value){
        SettingMenu_ui.SetActive(value);
    }
}
