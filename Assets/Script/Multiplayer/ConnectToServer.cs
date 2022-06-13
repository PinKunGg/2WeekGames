using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public GameObject TitleUI;
    public GameObject LobbyUI;

    private void Start() {
        TitleUI.SetActive(true);
        LobbyUI.SetActive(false);
    }

    public override void OnConnectedToMaster(){
        base.OnConnectedToMaster();

        if(!PhotonNetwork.InLobby){
            Debug.LogFormat("Connected to server");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(DisconnectCause cause){
        base.OnDisconnected(cause);

        TitleUI.SetActive(true);
        LobbyUI.SetActive(false);
        Debug.LogFormat("Disconnected to server: {0}",cause.ToString());
    }

    public override void OnJoinedLobby(){
        base.OnJoinedLobby();

        TitleUI.SetActive(false);
        LobbyUI.SetActive(true);
    }

    public void Connect(){
        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.ConnectUsingSettings();
    }
}
