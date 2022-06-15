using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class PlayerListMenu : MonoBehaviourPunCallbacks
{
    PlayerNameInRoomCollector playerNameInRoomCollector;

    [SerializeField] Transform _content;
    [SerializeField] PlayerListInfo _playerListInfo;

    private List<PlayerListInfo> _playerListingInfos = new List<PlayerListInfo>();

    public override void OnEnable(){
        base.OnEnable();

        GetAllCurrentPlayerInRoom();
        playerNameInRoomCollector = GetComponent<PlayerNameInRoomCollector>();
    }

    void GetAllCurrentPlayerInRoom(){
        if(!PhotonNetwork.IsConnected){return;}
        
        foreach(KeyValuePair<int,Player> playerInfo in PhotonNetwork.CurrentRoom.Players){
            AddPlayerList(playerInfo.Value);
        }
    }

    void AddPlayerList(Player player){
        PlayerListInfo playerInfo = Instantiate(_playerListInfo,_content);

            if(playerInfo != null){
                playerInfo.SetPlayerInfo(player);
                _playerListingInfos.Add(playerInfo);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
        base.OnPlayerEnteredRoom(newPlayer);

        //playerNameInRoomCollector.AddPlayerName(newPlayer.NickName);

        Debug.LogFormat("Player '{0}' join the game",newPlayer.NickName);
        AddPlayerList(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.LogFormat("Player '{0}' is leave the game",otherPlayer.NickName);

        int index = _playerListingInfos.FindIndex(x => x.info.NickName == otherPlayer.NickName);
        if(index != -1){
            Destroy(_playerListingInfos[index].gameObject);
            _playerListingInfos.RemoveAt(index);
        }
    }
}
