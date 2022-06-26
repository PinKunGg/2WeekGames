using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CreateAndJoinRoom_MenuToggle : MonoBehaviourPunCallbacks
{
    public TMP_Text LobbyType_tx;

    [Space]
    public GameObject createButton;
    public GameObject createRoomUI;

    [Space]
    public GameObject joinButton;
    public GameObject joinRoomUI;

    [Space]
    public GameObject roomListButton;
    public GameObject roomListUI;

    [Space]
    public GameObject mainMenuButton;

    private void Start() {
        CrateOrJoin_Toggle();
    }

    public void CrateOrJoin_Toggle(){
        LobbyType_tx.text = "";
        ToggleAllButton(true);

        createRoomUI.SetActive(false);
        joinRoomUI.SetActive(false);
        roomListUI.SetActive(false);
    }
    public void CreateRoom_Toggle(){
        LobbyType_tx.text = "- Create room -";
        ToggleAllButton(false);

        createRoomUI.SetActive(true);
        joinRoomUI.SetActive(false);
        roomListUI.SetActive(false);
    }
    public void JoinRoom_Toggle(){
        LobbyType_tx.text = "- Join room -";
        ToggleAllButton(false);

        createRoomUI.SetActive(false);
        joinRoomUI.SetActive(true);
        roomListUI.SetActive(false);
    }

    public void RoomList_Toggle(){
        LobbyType_tx.text = "- Room list -";
        ToggleAllButton(false);

        createRoomUI.SetActive(false);
        joinRoomUI.SetActive(false);
        roomListUI.SetActive(true);
    }

    public void ToggleAllButton(bool value){
        createButton.SetActive(value);
        joinButton.SetActive(value);
        roomListButton.SetActive(value);
        mainMenuButton.SetActive(value);
    }
}
