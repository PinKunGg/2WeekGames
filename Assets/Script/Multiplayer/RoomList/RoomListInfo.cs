using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class RoomListInfo : MonoBehaviour
{
    [SerializeField] TMP_InputField _playerName;

    [Space]
    [SerializeField] TMP_Text _roomName;
    [SerializeField] TMP_Text _roomPlayerCount;
    [SerializeField] TMP_Text _roomPing;

    public RoomInfo info {get; private set;}

    public void SetRoomInfo(RoomInfo roomInfo){
        info = roomInfo;
        _roomName.text = roomInfo.Name;
        _roomPlayerCount.text = string.Format("{0} / {1}",roomInfo.PlayerCount,roomInfo.MaxPlayers);
        
        _roomPing.text = string.Format("{0} ms",PhotonNetwork.GetPing());
    }

    public void ClickToJoin(){
        _playerName = GameObject.Find("PlayerName").GetComponent<TMP_InputField>();

        if(string.IsNullOrEmpty(_playerName.text)){
            return;
        }

        PhotonNetwork.NickName = _playerName.text;
        PhotonNetwork.JoinRoom(_roomName.text);
    }
}
