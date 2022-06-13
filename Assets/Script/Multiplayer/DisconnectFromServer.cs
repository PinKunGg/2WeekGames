using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class DisconnectFromServer : MonoBehaviourPunCallbacks
{
    public void Disconnect(){
        if(PhotonNetwork.IsMasterClient){
            
        }

        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom(){
        base.OnLeftRoom();

        PhotonNetwork.Disconnect();
        SceneManager.LoadSceneAsync(0);
    }
}
