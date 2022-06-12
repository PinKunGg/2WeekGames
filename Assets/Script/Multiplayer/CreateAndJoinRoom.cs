using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    public TMP_InputField createRoom;
    public TMP_InputField joinRoom;

    public void CreateRoom(){
        PhotonNetwork.CreateRoom(createRoom.text);
    }

    public void JoinRom(){
        PhotonNetwork.JoinRoom(joinRoom.text);
    }

    public override void OnJoinedRoom(){
        base.OnJoinedRoom();

        PhotonNetwork.LoadLevel("Multiplayer_Game");
    }
}
