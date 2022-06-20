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
    public GameObject _createButton;
    public GameObject _createRoomUI;

    [Space]
    public GameObject _joinButton;
    public GameObject _joinRoomUI;

    [Space]
    public GameObject _roomListButton;
    public GameObject _roomListUI;

    [Space]
    public GameObject _mainMenuButton;

    private void Start() {
        CrateOrJoin_Toggle();
    }

    public void CrateOrJoin_Toggle(){
        LobbyType_tx.text = "";
        ToggleAllButton(true);

        _createRoomUI.SetActive(false);
        _joinRoomUI.SetActive(false);
        _roomListUI.SetActive(false);
    }
    public void CreateRoom_Toggle(){
        LobbyType_tx.text = "- Create room -";
        ToggleAllButton(false);

        _createRoomUI.SetActive(true);
        _joinRoomUI.SetActive(false);
        _roomListUI.SetActive(false);
    }
    public void JoinRoom_Toggle(){
        LobbyType_tx.text = "- Join room -";
        ToggleAllButton(false);

        _createRoomUI.SetActive(false);
        _joinRoomUI.SetActive(true);
        _roomListUI.SetActive(false);
    }

    public void RoomList_Toggle(){
        LobbyType_tx.text = "- Room list -";
        ToggleAllButton(false);

        _createRoomUI.SetActive(false);
        _joinRoomUI.SetActive(false);
        _roomListUI.SetActive(true);
    }

    public void ToggleAllButton(bool value){
        _createButton.SetActive(value);
        _joinButton.SetActive(value);
        _roomListButton.SetActive(value);
        _mainMenuButton.SetActive(value);
    }
}
