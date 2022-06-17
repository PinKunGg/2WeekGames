using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform _content;
    [SerializeField] PlayerListInfo _playerListInfo;

    public List<PlayerListInfo> _playerListingInfos = new List<PlayerListInfo>();

    public override void OnEnable(){
        base.OnEnable();
        
        GetAllCurrentPlayerInRoom();
    }

    void GetAllCurrentPlayerInRoom(){
        if(!PhotonNetwork.IsConnected){return;}
        
        foreach(KeyValuePair<int,Player> playerInfo in PhotonNetwork.CurrentRoom.Players){
            AddPlayerListMenu(playerInfo.Value);
        }
    }

    public void AddPlayerListMenu(Player player){
        int indexPlayerListMenu = _playerListingInfos.FindIndex(x => x.info.ActorNumber == player.ActorNumber);
        if(indexPlayerListMenu != -1){
            return;
        }

        PlayerListInfo playerInfo = Instantiate(_playerListInfo,_content);

        if(playerInfo != null){
            playerInfo.SetPlayerInfo(player);
            _playerListingInfos.Add(playerInfo);
        }
    }

    public void RemovePlayerListMenu(int ActorID){
        Debug.Log(ActorID);
        int indexPlayerListMenu = _playerListingInfos.FindIndex(x => x.info.ActorNumber == ActorID);
        if(indexPlayerListMenu != -1){
            Destroy(_playerListingInfos[indexPlayerListMenu].gameObject);
            _playerListingInfos.RemoveAt(indexPlayerListMenu);
        }
    }
}
