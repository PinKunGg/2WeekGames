using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PlayerListMenu : MonoBehaviourPunCallbacks
{
    public static PlayerListMenu playerListMenu;
    public PlayerNameControl playerNameControl;
    [SerializeField] Transform _content;
    [SerializeField] PlayerListInfo _playerListInfo;

    private List<PlayerListInfo> _playerListingInfos = new List<PlayerListInfo>();
    public List<Player_Stat> allPlayerStat;

    public void Awake() 
    {
        playerListMenu = this;
    }
    public override void OnEnable(){
        base.OnEnable();

        GetAllCurrentPlayerInRoom();
        GetAllPlayerNameInRoom();
    }

    void GetAllCurrentPlayerInRoom(){
        if(!PhotonNetwork.IsConnected){return;}
        
        foreach(KeyValuePair<int,Player> playerInfo in PhotonNetwork.CurrentRoom.Players){
            AddPlayerList(playerInfo.Value);
        }
    }

    void GetAllPlayerNameInRoom() 
    {
        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            playerNameControl.AddOtherNamePlayer(playerInfo.Value.NickName);
        }
        playerNameControl.CountGetName++;
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

        playerNameControl.AddOtherNamePlayer(newPlayer.NickName);
        TriggerWhenPlayerJoin();

        Debug.LogFormat("Player '{0}' join the game",newPlayer.NickName);
        AddPlayerList(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){
        base.OnPlayerLeftRoom(otherPlayer);

        playerNameControl.RemoveHealthBar_ByName(otherPlayer.NickName);

        Debug.LogFormat("Player '{0}' is leave the game",otherPlayer.NickName);

        int index = _playerListingInfos.FindIndex(x => x.info.NickName == otherPlayer.NickName);
        if(index != -1){
            Destroy(_playerListingInfos[index].gameObject);
            _playerListingInfos.RemoveAt(index);
        }
    }

    public void AddPlayerStat(Player_Stat Value) 
    {
        allPlayerStat.Add(Value);
    }

    void TriggerWhenPlayerJoin() 
    {
        foreach (Player_Stat player_Stat in allPlayerStat) 
        {
            player_Stat.UpdateHealthBarForOther();
        }
    }
}
