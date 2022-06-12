using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public override void OnConnectedToMaster(){
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby(){
        base.OnJoinedLobby();

        SceneManager.LoadSceneAsync(1);
    }

    public void Connect(){
        PhotonNetwork.ConnectUsingSettings();
    }
}
