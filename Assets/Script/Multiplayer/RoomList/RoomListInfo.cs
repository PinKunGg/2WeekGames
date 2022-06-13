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

    [Space]
    [SerializeField] Image _privateRoomIcon;
    [SerializeField] Canvas _roomPasswordInputUI;   
    [SerializeField] TMP_InputField _roomPasswordInput;

    public RoomInfo info {get; private set;}

    public void SetRoomInfo(RoomInfo roomInfo){
        info = roomInfo;
        _roomName.text = roomInfo.Name;
        _roomPlayerCount.text = string.Format("{0} / {1}",roomInfo.PlayerCount,roomInfo.MaxPlayers);
        
        _roomPing.text = string.Format("{0} ms",PhotonNetwork.GetPing());

        if(!string.IsNullOrEmpty(info.CustomProperties["roomPassword"].ToString())){
            _privateRoomIcon.gameObject.SetActive(true);

            _roomPasswordInputUI = GameObject.Find("RoomListPasswordInput_ui").GetComponent<Canvas>();
            _roomPasswordInput = GameObject.Find("RoomListPasswordInput").GetComponent<TMP_InputField>();
            Button joinButton = GameObject.Find("JoinRoomList_Button").GetComponent<Button>();
            joinButton.onClick.AddListener(JoinRoom);
        }
    }

    public void ClickToJoin(){
        _playerName = GameObject.Find("PlayerName").GetComponent<TMP_InputField>();

        if(string.IsNullOrEmpty(_playerName.text)){
            return;
        }

        if(!string.IsNullOrEmpty(info.CustomProperties["roomPassword"].ToString())){
            _roomPasswordInputUI.enabled = true;
            return;
        }

        PhotonNetwork.NickName = _playerName.text;
        PhotonNetwork.JoinRoom(_roomName.text);
    }

    public void JoinRoom(){
        if(info.CustomProperties["roomPassword"].ToString() == _roomPasswordInput.text){
            Debug.Log("Password Correct");
            PhotonNetwork.NickName = _playerName.text;
            PhotonNetwork.JoinRoom(_roomName.text);
        }else{
            Debug.Log("Password Wrong");
        }
    }
}
