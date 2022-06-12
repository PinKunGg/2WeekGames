using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class DisconnectFromServer : MonoBehaviourPunCallbacks
{
    public void Disconnect(){
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom(){
        base.OnLeftRoom();

        SceneManager.LoadSceneAsync(0);
    }
}
