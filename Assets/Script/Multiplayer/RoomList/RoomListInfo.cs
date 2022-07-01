using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class RoomListInfo : MonoBehaviour
{
    [SerializeField] TMP_InputField playerName;

    [Space]
    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text roomPlayerCount;
    [SerializeField] TMP_Text roomPing;

    public RoomInfo info {get; private set;}

    public void SetRoomInfo(RoomInfo roomInfo){
        info = roomInfo;
        roomName.text = roomInfo.Name;
        roomPlayerCount.text = string.Format("{0} / {1}",roomInfo.PlayerCount,roomInfo.MaxPlayers);
        
        roomPing.text = string.Format("{0} ms",PhotonNetwork.GetPing());
    }

    public void ClickToJoin(){
        playerName = GameObject.Find("PlayerName").GetComponent<TMP_InputField>();

        if(string.IsNullOrEmpty(playerName.text)){
            return;
        }

        LoadingScene.loading.OpenLoading();
        PhotonNetwork.NickName = playerName.text;
        PhotonNetwork.JoinRoom(roomName.text);
    }
}
