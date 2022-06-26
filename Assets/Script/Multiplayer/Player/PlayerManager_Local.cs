using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager_Local : MonoBehaviour
{

    bool isCursorEnable;
    private void OnEnable() {
        FindObjectOfType<PlayerManager_Multiplayer>().AddPlayer(this.gameObject);
    }

    private void Start() {
        isCursorEnable = false;
        //ToggleCursor(isCursorEnable);
    }

    private void Update() {
        //if(Input.GetKeyDown(KeyCode.LeftAlt)){
        //    isCursorEnable = !isCursorEnable;
        //    ToggleCursor(isCursorEnable);
        //}
    }

    void ToggleCursor(bool value){
        Cursor.visible = value;
        
        if(value){
            Cursor.lockState = CursorLockMode.None;
        }
        else{
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
