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

    private void Start() {
        CrateOrJoin_Toggle();
    }

    public void CrateOrJoin_Toggle(){
        LobbyType_tx.text = "";
        _createButton.SetActive(true);
        _joinButton.SetActive(true);
        _roomListButton.SetActive(true);

        _createRoomUI.SetActive(false);
        _joinRoomUI.SetActive(false);
        _roomListUI.SetActive(false);
    }
    public void CreateRoom_Toggle(){
        LobbyType_tx.text = "- Create room -";
        _createButton.SetActive(false);
        _joinButton.SetActive(false);
        _roomListButton.SetActive(false);

        _createRoomUI.SetActive(true);
        _joinRoomUI.SetActive(false);
        _roomListUI.SetActive(false);
    }
    public void JoinRoom_Toggle(){
        LobbyType_tx.text = "- Join room -";
        _createButton.SetActive(false);
        _joinButton.SetActive(false);
        _roomListButton.SetActive(false);

        _createRoomUI.SetActive(false);
        _joinRoomUI.SetActive(true);
        _roomListUI.SetActive(false);
    }

    public void RoomList_Toggle(){
        LobbyType_tx.text = "- Room list -";
        _createButton.SetActive(false);
        _joinButton.SetActive(false);
        _roomListButton.SetActive(false);

        _createRoomUI.SetActive(false);
        _joinRoomUI.SetActive(false);
        _roomListUI.SetActive(true);
    }
}
