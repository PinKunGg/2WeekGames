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

    public DisconnectFromServer disconnectFromServer;

    private void Start() {
        if(!PhotonNetwork.IsMasterClient){
            CloseServer_button.gameObject.SetActive(false);
            OpenServer_button.gameObject.SetActive(false);
        }
        else{
            CloseServer_button.gameObject.SetActive(true);
            OpenServer_button.gameObject.SetActive(true);
        }
    }

    public void OnClick_Disconnect(){
        if(PhotonNetwork.IsMasterClient){
            disconnectFromServer.OnClick_Disconnect();
        }
        else{
            Debug.Log(6.1f);
            PhotonNetwork.LeaveRoom();
        }
    }
    public void OnClick_CloseServer(){
        disconnectFromServer.OnClick_CloseServer();
    }
    public void OnClick_OpenServer(){
        disconnectFromServer.OnClick_OpenServer();
    }

    public override void OnLeftRoom(){
        base.OnLeftRoom();

        PhotonNetwork.Disconnect();
        SceneManager.LoadSceneAsync(0);
    }
}
