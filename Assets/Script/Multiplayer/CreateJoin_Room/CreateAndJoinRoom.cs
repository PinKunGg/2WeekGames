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
    public TMP_InputField playerName;
    bool isTutorial = false;

    [Space]
    public TMP_InputField createRoomName;
    public Button PrivateRoom;
    public Button PublicRoom;
    
    public Slider createMaxPlayer;
    public TMP_InputField createMaxPlayer_InputField;

    public TMP_InputField joinRoomName;

    bool isRoomVisible;
    float maxPlayer = 2;

    private void Start() {
        OnClick_PublicRoom();
    }

    public void CreateRoom(){
        if(string.IsNullOrEmpty(playerName.text) && string.IsNullOrEmpty(createRoomName.text)){
            return;
        }
        
        if(!PhotonNetwork.IsConnected){
            return;
        }

        RoomOptions options = new RoomOptions(){
            MaxPlayers = byte.Parse(createMaxPlayer_InputField.text.ToString()),
            IsVisible = isRoomVisible
        };

        PreparePlayerData();

        PhotonNetwork.CreateRoom(createRoomName.text,options,TypedLobby.Default);
    }

    public void OnClick_PrivateRoom(){
        PrivateRoom.interactable = false;
        PublicRoom.interactable = true;

        isRoomVisible = false;
    }
    public void OnClick_PublicRoom(){
        PrivateRoom.interactable = true;
        PublicRoom.interactable = false;

        isRoomVisible = true;
    }

    public void OnMaxPlayerValueChange(){
        maxPlayer = createMaxPlayer.value;
        createMaxPlayer_InputField.text = maxPlayer.ToString();
    }

    public void OnMaxPlayerValueChange_InputField(){
        if(float.Parse(createMaxPlayer_InputField.text) > 4f){
            maxPlayer = 4f;
            createMaxPlayer_InputField.text = maxPlayer.ToString();
        }
        else{
            maxPlayer = float.Parse(createMaxPlayer_InputField.text);
        }

        createMaxPlayer.value = maxPlayer;
    }

    List<RoomInfo> infos = new List<RoomInfo>();
    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        base.OnRoomListUpdate(roomList);

        infos = roomList;
    }

    public void JoinRoom(){
        if(string.IsNullOrEmpty(playerName.text) && string.IsNullOrEmpty(joinRoomName.text)){
            return;
        }

        if(!PhotonNetwork.IsConnected){
            return;
        }

        PreparePlayerData();

        PhotonNetwork.JoinRoom(joinRoomName.text);
    }

    void PreparePlayerData(){
        if (!isTutorial)
        {
            PhotonNetwork.NickName = playerName.text;
        }
        else if (isTutorial) 
        {
            PhotonNetwork.NickName = "Tutorial";
        }
    }

    public override void OnCreatedRoom(){
        base.OnCreatedRoom();

        Debug.LogFormat("Crated '{0}' room success",createRoomName.text);
    }

    public override void OnCreateRoomFailed(short returnCode, string message){
        base.OnCreateRoomFailed(returnCode, message);

        Debug.LogFormat("Fail to crated '{0}' room: {1}",createRoomName.text,message);
    }

    public override void OnJoinedRoom(){
        base.OnJoinedRoom();

        if (!isTutorial) 
        {
            PhotonNetwork.LoadLevel("Multiplayer_Lobby");
            // PhotonNetwork.LoadLevel("UtilitySystem");
        }
        else if (isTutorial)
        {
            PhotonNetwork.LoadLevel("Multiplayer_Tutorial");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message){
        base.OnJoinRoomFailed(returnCode, message);

        Debug.LogFormat("Can't join room '{0}': {1}",joinRoomName.text,message);
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
