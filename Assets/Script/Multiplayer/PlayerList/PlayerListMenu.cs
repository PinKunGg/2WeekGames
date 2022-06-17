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

    public List<PlayerListInfo> _playerListingInfos = new List<PlayerListInfo>();
    public List<Player_Stat> allPlayerStat;
    public List<Player_Attack_Control> AllAttack_Controls;

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
            AddPlayerListMenu(playerInfo.Value);
        }
    }

    public void AddPlayerListMenu(Player player){
        int indexPlayerListMenu = _playerListingInfos.FindIndex(x => x.info.ActorNumber == player.ActorNumber);
        if(indexPlayerListMenu != -1){
            return;
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

    public void RemovePlayerListMenu(int ActorID){
        Debug.Log(ActorID);
        int indexPlayerListMenu = _playerListingInfos.FindIndex(x => x.info.ActorNumber == ActorID);
        if(indexPlayerListMenu != -1){
            Destroy(_playerListingInfos[indexPlayerListMenu].gameObject);
            _playerListingInfos.RemoveAt(indexPlayerListMenu);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
        base.OnPlayerEnteredRoom(newPlayer);

        playerNameControl.AddOtherNamePlayer(newPlayer.NickName);
        TriggerWhenPlayerJoin();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){
        base.OnPlayerLeftRoom(otherPlayer);

        playerNameControl.RemoveHealthBar_ByName(otherPlayer.NickName);
    }

    public void AddPlayerStat(Player_Stat Value) 
    {
        allPlayerStat.Add(Value);
    }
    public void AddAttackControl(Player_Attack_Control Value)
    {
        AllAttack_Controls.Add(Value);
    }

    void TriggerWhenPlayerJoin() 
    {
        foreach (Player_Stat player_Stat in allPlayerStat) 
        {
            player_Stat.UpdateHealthBarForOther();
        }
        foreach (Player_Attack_Control player_Attack_Control in AllAttack_Controls) 
        {
            player_Attack_Control.UpdateAnimForOther();
        }
    }
}
