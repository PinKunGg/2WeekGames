using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSettings : MonoBehaviour
{
    public static CursorSettings cursor;

    bool isCursorVisibe;
    public bool isInLobby;
    public bool isOverride;

    private void Awake() {
        cursor = this;
    }

    private void Update() {
        if(isOverride){return;}

        if(isInLobby){
            ToggleCursor(true);
            return;
        }

        if(Input.GetKeyDown(KeyCode.LeftAlt)){
            isCursorVisibe = !isCursorVisibe;
            ToggleCursor(isCursorVisibe);
        }
    }
    public void ToggleCursor(bool value){
        Cursor.visible = value;
        isCursorVisibe = value;
        
        if(value){
            Cursor.lockState = CursorLockMode.None;
        }
        else{
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
