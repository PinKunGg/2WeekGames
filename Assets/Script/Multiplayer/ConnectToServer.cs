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
        Debug.LogFormat("Disconnected From The Server: {0}",cause.ToString());
        FindObjectOfType<GameAlert_Nortification>().SetAlert("Disconnected from the server",string.Format(cause.ToString()),true);
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
        FindObjectOfType<GameAlert_Nortification>().SetAlert("Exiting the game?",string.Format("Are you sure?"),false);

        StartCoroutine(FindObjectOfType<GameAlert_Nortification>().WaiteForCallBack(ConfirmExitGame));
    }

    void ConfirmExitGame(bool value){
        if(value){
            Debug.Log("Exiting the game");
            PhotonNetwork.Disconnect();
            Application.Quit();
        }
        else{
            Debug.Log("Exiting the game abort");
        }
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
        Invoke("OnEndCreditAnimation", 36f);
    }

    public void OnEndCreditAnimation() 
    {
        CreditUI.SetActive(false);
    }
}
