using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    public TMP_InputField _playerName;

    [Space]
    public TMP_InputField _createRoomName;
    public TMP_InputField _createRoomPassword;
    public Slider _createMaxPlayer;

    [Space]
    public Canvas _joinRoomList_ui;
    public TMP_InputField _joinRoomName;
    public TMP_InputField _joinRoomPassword;

    public void CreateRoom(){
        if(string.IsNullOrEmpty(_playerName.text) && string.IsNullOrEmpty(_createRoomName.text)){
            return;
        }
        
        if(!PhotonNetwork.IsConnected){
            return;
        }

        RoomOptions options = new RoomOptions(){
            MaxPlayers = byte.Parse(_createMaxPlayer.value.ToString()),
        };

        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        options.CustomRoomProperties.Add("roomPassword",_createRoomPassword.text);
        string[] _roomProperty = new string[] {"roomPassword"};
        options.CustomRoomPropertiesForLobby = _roomProperty;

        PreparePlayerData();

        PhotonNetwork.CreateRoom(_createRoomName.text,options,TypedLobby.Default);
        PhotonNetwork.CurrentRoom.SetPropertiesListedInLobby(_roomProperty);
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

        int index = infos.FindIndex(x => x.Name == _joinRoomName.text);

        if(index != -1){
            if(infos[index].CustomProperties["roomPassword"].ToString() == _joinRoomPassword.text){
                Debug.Log("Password Correct");
                PhotonNetwork.JoinRoom(_joinRoomName.text);
            }else{
                Debug.Log("Password Wrong");
            }
        }

        PreparePlayerData();
    }

    void PreparePlayerData(){
        PhotonNetwork.NickName = _playerName.text;
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

        PhotonNetwork.LoadLevel("Multiplayer_Game");
    }

    public override void OnJoinRoomFailed(short returnCode, string message){
        base.OnJoinRoomFailed(returnCode, message);

        Debug.LogFormat("Can't join room '{0}': {1}",_joinRoomName.text,message);
    }

    public void CloseJoinListUI(){
        _joinRoomList_ui.enabled = false;
    }
}
