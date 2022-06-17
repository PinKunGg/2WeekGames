using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class OwnerControllpanel : MonoBehaviourPunCallbacks
{
    [SerializeField]Button CloseServer_button;
    [SerializeField]Button OpenServer_button;

    private void Start() {
        try{
            if(!PhotonNetwork.IsMasterClient){
                CloseServer_button.gameObject.SetActive(false);
                OpenServer_button.gameObject.SetActive(false);
            }
            else{
                CloseServer_button.gameObject.SetActive(true);
                OpenServer_button.gameObject.SetActive(true);
            }
        }catch{}
    }

    public void OnClick_CloseServer(){
        if(PhotonNetwork.IsMasterClient){
            base.photonView.RPC("RPC_Disconnect",RpcTarget.Others);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    public void OnClick_OpenServer(){
        if(PhotonNetwork.IsMasterClient){
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }
    }
}
