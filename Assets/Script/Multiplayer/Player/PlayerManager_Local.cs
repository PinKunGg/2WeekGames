using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager_Local : MonoBehaviourPunCallbacks
{
    bool isCursorEnable;
    public override void OnEnable() {
        base.OnEnable();

        FindObjectOfType<PlayerManager_Multiplayer>().AddPlayer(this.gameObject);

        if(!base.photonView.IsMine){
            this.enabled = false;
            return;
        }
    }

    private void Start() {
        isCursorEnable = false;
        // ToggleCursor(isCursorEnable);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.L)){
            Vector3 tempSpawnPos = new Vector3(this.transform.position.x,this.transform.position.y + 2f,this.transform.position.z + 2f);
            PhotonNetwork.Instantiate("Arrow_NormalAttack",tempSpawnPos,Quaternion.identity);
        }
    }

    // private void Update() {
    //     if(Input.GetKeyDown(KeyCode.LeftAlt)){
    //         isCursorEnable = !isCursorEnable;
    //         ToggleCursor(isCursorEnable);
    //     }
    // }

    // void ToggleCursor(bool value){
    //     Cursor.visible = value;
        
    //     if(value){
    //         Cursor.lockState = CursorLockMode.None;
    //     }
    //     else{
    //         Cursor.lockState = CursorLockMode.Locked;
    //     }
    // }
}
