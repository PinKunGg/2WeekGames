using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenControl : MonoBehaviour
{
    public GameObject ControlUI;

    private void Start() {
        ToggleControlUI(false);
    }

    public void ToggleControlUI(bool value){
        ControlUI.SetActive(value);
    }
}
