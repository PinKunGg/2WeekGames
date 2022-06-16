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

    public override void OnPlayerEnteredRoom(Player newPlayer){
        base.OnPlayerEnteredRoom(newPlayer);

        AddPlayerListMenu(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){
        base.OnPlayerLeftRoom(otherPlayer);

        RemovePlayerListMenu(otherPlayer.ActorNumber);
    }


    public void AddPlayerListMenu(Player player){
        PlayerListInfo playerInfo = Instantiate(_playerListInfo,_content);

        if(playerInfo != null){
            playerInfo.SetPlayerInfo(player);
            _playerListingInfos.Add(playerInfo);
        }
    }

    public void RemovePlayerListMenu(int ActorID){
        int indexPlayerListMenu = _playerListingInfos.FindIndex(x => x.info.ActorNumber == ActorID);
        if(indexPlayerListMenu != -1){
            Destroy(_playerListingInfos[indexPlayerListMenu].gameObject);
            _playerListingInfos.RemoveAt(indexPlayerListMenu);
        }
    }
}
