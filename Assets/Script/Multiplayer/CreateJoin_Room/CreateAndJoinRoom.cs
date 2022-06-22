using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    public TMP_InputField _playerName;
    bool isTutorial = false;

    [Space]
    public TMP_InputField _createRoomName;
    public Button _PrivateRoom;
    public Button _PublicRoom;
    
    public Slider _createMaxPlayer;
    public TMP_InputField _createMaxPlayer_InputField;

    [Space]
    public Canvas _joinRoomList_ui;
    public TMP_InputField _joinRoomName;

    bool _isRoomVisible;
    float _maxPlayer = 2;

    private void Start() {
        OnClick_PublicRoom();
    }

    public void CreateRoom(){
        if(string.IsNullOrEmpty(_playerName.text) && string.IsNullOrEmpty(_createRoomName.text)){
            return;
        }
        
        if(!PhotonNetwork.IsConnected){
            return;
        }

        RoomOptions options = new RoomOptions(){
            MaxPlayers = byte.Parse(_createMaxPlayer_InputField.text.ToString()),
            IsVisible = _isRoomVisible
        };

        PreparePlayerData();

        PhotonNetwork.CreateRoom(_createRoomName.text,options,TypedLobby.Default);
    }

    public void OnClick_PrivateRoom(){
        _PrivateRoom.interactable = false;
        _PublicRoom.interactable = true;

        _isRoomVisible = false;
    }
    public void OnClick_PublicRoom(){
        _PrivateRoom.interactable = true;
        _PublicRoom.interactable = false;

        _isRoomVisible = true;
    }

    public void OnMaxPlayerValueChange(){
        _maxPlayer = _createMaxPlayer.value;
        _createMaxPlayer_InputField.text = _maxPlayer.ToString();
    }

    public void OnMaxPlayerValueChange_InputField(){
        if(float.Parse(_createMaxPlayer_InputField.text) > 4f){
            _maxPlayer = 4f;
            _createMaxPlayer_InputField.text = _maxPlayer.ToString();
        }
        else{
            _maxPlayer = float.Parse(_createMaxPlayer_InputField.text);
        }

        _createMaxPlayer.value = _maxPlayer;
    }

    List<RoomInfo> infos = new List<RoomInfo>();
    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        base.OnRoomListUpdate(roomList);

        infos = roomList;
    }

    public void JoinRoom(){
        if(string.IsNullOrEmpty(_playerName.text) && string.IsNullOrEmpty(_joinRoomName.text)){
            return;
        }

        if(!PhotonNetwork.IsConnected){
            return;
        }

        PreparePlayerData();

        PhotonNetwork.JoinRoom(_joinRoomName.text);
    }

    void PreparePlayerData(){
        if (!isTutorial)
        {
            PhotonNetwork.NickName = _playerName.text;
        }
        else if (isTutorial) 
        {
            PhotonNetwork.NickName = "Tutorial";
        }
    }

    public override void OnCreatedRoom(){
        base.OnCreatedRoom();

        Debug.LogFormat("Crated '{0}' room success",_createRoomName.text);
    }

    public override void OnCreateRoomFailed(short returnCode, string message){
        base.OnCreateRoomFailed(returnCode, message);

        Debug.LogFormat("Fail to crated '{0}' room: {1}",_createRoomName.text,message);
    }

    public override void OnJoinedRoom(){
        base.OnJoinedRoom();

        if (!isTutorial) 
        {
            PhotonNetwork.LoadLevel("Multiplayer_Game");
            // PhotonNetwork.LoadLevel("UtilitySystem");
        }
        else if (isTutorial)
        {
            PhotonNetwork.LoadLevel("Multiplayer_Tutorial");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message){
        base.OnJoinRoomFailed(returnCode, message);

        Debug.LogFormat("Can't join room '{0}': {1}",_joinRoomName.text,message);
    }

    public void CloseJoinListUI(){
        _joinRoomList_ui.enabled = false;
    }

    public void TutorialMode() 
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }
        isTutorial = true;
        RoomOptions options = new RoomOptions()
        {
            MaxPlayers = 1,
            IsVisible = false
        };

        PreparePlayerData();

        PhotonNetwork.CreateRoom("Tutorial"+System.DateTime.Now, options, TypedLobby.Default);
    }
}
