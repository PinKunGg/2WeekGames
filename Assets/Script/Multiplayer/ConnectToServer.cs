using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public GameObject TitleUI;
    public GameObject MainMenuUI;
    public GameObject LobbyUI;
    public GameObject CreditUI;

    private void Start() {
        TitleUI.SetActive(true);
        MainMenuUI.SetActive(false);
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
        MainMenuUI.SetActive(false);
        LobbyUI.SetActive(false);
        Debug.LogFormat("Disconnected to server: {0}",cause.ToString());
    }

    public override void OnJoinedLobby(){
        base.OnJoinedLobby();

        TitleUI.SetActive(false);
        MainMenuUI.SetActive(true);
        LobbyUI.SetActive(false);
    }

    public void OnClick_StartGame(){
        TitleUI.SetActive(false);
        MainMenuUI.SetActive(false);
        LobbyUI.SetActive(true);

    }
    public void OnClick_MainMenu(){
        TitleUI.SetActive(false);
        MainMenuUI.SetActive(true);
        LobbyUI.SetActive(false);
    }
    public void OnClick_ExitGame(){
        PhotonNetwork.Disconnect();
        Application.Quit();
    }

    public void Connect(){
        FindObjectOfType<SoundBG>().OnclickToStart();
        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClickCredit() 
    {
        CreditUI.SetActive(true);
        FindObjectOfType<SoundBG>().ChangeToMainMenuBGSound();
        Invoke("OnEndCreditAnimation", 36);
    }

    public void OnEndCreditAnimation() 
    {
        CreditUI.SetActive(false);
    }
}
