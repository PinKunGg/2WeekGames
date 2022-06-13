using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRoom_MenuToggle : MonoBehaviourPunCallbacks
{
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
        _createButton.SetActive(true);
        _joinButton.SetActive(true);
        _roomListButton.SetActive(true);

        _createRoomUI.SetActive(false);
        _joinRoomUI.SetActive(false);
        _roomListUI.SetActive(false);
    }
    public void CreateRoom_Toggle(){
        _createButton.SetActive(false);
        _joinButton.SetActive(false);
        _roomListButton.SetActive(false);

        _createRoomUI.SetActive(true);
        _joinRoomUI.SetActive(false);
        _roomListUI.SetActive(false);
    }
    public void JoinRoom_Toggle(){
        _createButton.SetActive(false);
        _joinButton.SetActive(false);
        _roomListButton.SetActive(false);

        _createRoomUI.SetActive(false);
        _joinRoomUI.SetActive(true);
        _roomListUI.SetActive(false);
    }

    public void RoomList_Toggle(){
        _createButton.SetActive(false);
        _joinButton.SetActive(false);
        _roomListButton.SetActive(false);

        _createRoomUI.SetActive(false);
        _joinRoomUI.SetActive(false);
        _roomListUI.SetActive(true);
    }
}
