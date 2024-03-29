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
    [SerializeField] Transform content;
    [SerializeField] PlayerListInfo playerListInfo;

    public List<PlayerListInfo> playerListingInfos = new List<PlayerListInfo>();
    public List<Player_Stat> allPlayerStat;
    public List<Player_Attack_Control> AllAttack_Controls;

    public GameObject SelectStage;
    public TextMeshProUGUI SelectStage_output;

    public void Awake() 
    {
        playerListMenu = this;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        //DontDestroyOnLoad(this);
        if (PhotonNetwork.IsMasterClient)
        {
            SelectStage.gameObject.SetActive(true);
        }
        else { SelectStage.gameObject.SetActive(false); }
    }
    public override void OnEnable(){
        base.OnEnable();
        
        GetAllCurrentPlayerInRoom();
    }

    void GetAllCurrentPlayerInRoom(){
        if(!PhotonNetwork.IsConnected){return;}
        
        foreach(KeyValuePair<int,Player> playerInfo in PhotonNetwork.CurrentRoom.Players){
            AddPlayerListMenu(playerInfo.Value);
            // playerNameControl.AddOtherNamePlayer(playerInfo.Value.NickName);
        }        
    }

    public void AddPlayerListMenu(Player player){
        int indexPlayerListMenu = playerListingInfos.FindIndex(x => x.info.ActorNumber == player.ActorNumber);
        if(indexPlayerListMenu != -1){
            return;
        }

        PlayerListInfo playerInfo = Instantiate(playerListInfo,content);

        if(playerInfo != null){
            playerInfo.SetPlayerInfo(player);
            playerListingInfos.Add(playerInfo);
        }

        playerNameControl.AddOtherNamePlayer(player.NickName);
        TriggerWhenPlayerJoin();
        playerNameControl.CountGetName++;
    }

    public void RemovePlayerListMenu(int ActorID){
        Debug.Log(ActorID);
        int indexPlayerListMenu = playerListingInfos.FindIndex(x => x.info.ActorNumber == ActorID);
        if(indexPlayerListMenu != -1){
            playerNameControl.RemoveHealthBar_ByName(playerListingInfos[indexPlayerListMenu].info.NickName);
            LobbyControl.lobbyControl.WhenPlayerLeftRoom();
            Destroy(playerListingInfos[indexPlayerListMenu].gameObject);
            playerListingInfos.RemoveAt(indexPlayerListMenu);
        }
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
        Debug.Log("trigger : run ");
        foreach (Player_Stat player_Stat in allPlayerStat) 
        {
            player_Stat.UpdateHealthBarForOther();
        }
        foreach (Player_Attack_Control player_Attack_Control in AllAttack_Controls)
        {
            player_Attack_Control.UpdateAnimForOther();
        }
        if (PhotonNetwork.IsMasterClient) { FindObjectOfType<LobbyControl>().UpdateStageToOther(); }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient) 
        {
            FindObjectOfType<SpawnPlayerFormLobby>().ChangeIsFirstRun();
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            base.photonView.RPC("Rpc_SaveGame", RpcTarget.All);
            string name_scene = "";
            if (SelectStage_output.text == "Stage 1") 
            {
                name_scene = "Multiplayer_Game";
            }
            else if (SelectStage_output.text == "Stage 2")
            {
                name_scene = "Multiplayer_Game";
            }
            else if (SelectStage_output.text == "Stage 3")
            {
                name_scene = "Multiplayer_Game";
            }
            else if (SelectStage_output.text == "Stage 4")
            {
                name_scene = "Multiplayer_Game";
            }
            else if (SelectStage_output.text == "Stage 5")
            {
                name_scene = "Multiplayer_Game";
            }
            PhotonNetwork.LoadLevel(name_scene);
        }
    }

    [PunRPC]
    void Rpc_SaveGame() 
    {
        Player_Inventory.player_Inventory.SaveItem();
        Player_Inventory.player_Inventory.SaveCloth();
    }
}
