using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    PlayerListMenu playerListMenu;
    SavePlayerData savePlayerData;
    PlayerManager_Multiplayer playerManMulti;
    SpawnPlayer spawnPlayer;

    bool isNotSaveThisPlayerData;
    bool isCheckNameDone;

    public override void OnEnable(){
        base.OnEnable();

        playerListMenu = FindObjectOfType<PlayerListMenu>();
        playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
        spawnPlayer = FindObjectOfType<SpawnPlayer>();
        savePlayerData = GetComponent<SavePlayerData>();

        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable(){
        base.OnDisable();

        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent){

    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
        base.OnPlayerEnteredRoom(newPlayer);

        Debug.LogFormat("Player '{0}' join the game",newPlayer.NickName);

        if(PhotonNetwork.IsMasterClient){
            isCheckNameDone = false;
            CheckPlayerName(newPlayer);

            StartCoroutine(WaitForCheckPlayerNameIsDone(newPlayer));
        }
    }

    IEnumerator WaitForCheckPlayerNameIsDone(Player newPlayer){
        while(!isCheckNameDone){
            yield return null;
        }

        Debug.Log("CheckPlayerName Done!");

        if(isNotSaveThisPlayerData){
            yield break;
        }

        base.photonView.RPC("RPC_SpawnNewPlayer",newPlayer);
        base.photonView.RPC("RPC_AddPlayerToPlayerListMenu",RpcTarget.All,newPlayer);
    }

    [PunRPC]
    void RPC_AddPlayerToPlayerListMenu(Player newPlayer){
        playerListMenu.AddPlayerListMenu(newPlayer);
    }

    [PunRPC]
    void RPC_SpawnNewPlayer(){
        spawnPlayer.SpawnNewPlayer();
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer){
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.LogFormat("Player '{0}' is leave the game",otherPlayer.NickName);

        if(isNotSaveThisPlayerData){
            Debug.LogErrorFormat("Data from player '{0}' was not saved",otherPlayer.NickName);
            isNotSaveThisPlayerData = false;
            return;
        }

        if(PhotonNetwork.IsMasterClient){
            savePlayerData.SaveData(otherPlayer);

            StartCoroutine(WaitUntilSaveFinish(otherPlayer.ActorNumber));
        }
    }

    IEnumerator WaitUntilSaveFinish(int leavePlayerID){
        while(!savePlayerData.isSaveDone){
            yield return null;
        }

        base.photonView.RPC("RPC_RemovePlayerFromPlayerList",RpcTarget.All,leavePlayerID);
        base.photonView.RPC("RPC_RemovePlayerFromPlayerListMenu",RpcTarget.All,leavePlayerID);
    }

    [PunRPC]
    void RPC_RemovePlayerFromPlayerListMenu(int otherPlayer){
        if(!playerListMenu){
            playerListMenu = FindObjectOfType<PlayerListMenu>();
        }
        playerListMenu.RemovePlayerListMenu(otherPlayer);
    }
    [PunRPC]
    void RPC_RemovePlayerFromPlayerList(int otherPlayer){
        if(!playerManMulti){
            playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
        }

        playerManMulti.RemovePlayer(otherPlayer);
    }

    [PunRPC]
    public void RPC_Disconnect(){
        LoadingScene.loading.OpenLoading();

        Destroy(FindObjectOfType<PlayerListMenu>().gameObject);
        if (PhotonNetwork.IsMasterClient){
            base.photonView.RPC("RPC_HostDisconnect",RpcTarget.Others,PhotonNetwork.CurrentRoom.Name);
            base.photonView.RPC("RPC_Disconnect",RpcTarget.Others);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            FindObjectOfType<Player_Inventory>().SaveCloth();
            FindObjectOfType<Player_Inventory>().SaveItem();
            FindObjectOfType<SoundBG>().ChangeToMainMenuBGSound();
            StartCoroutine(PrepareHostDisconnect());

            return;
        }
        CursorSettings.cursor.isOverride = false;
        CursorSettings.cursor.isInLobby = false;
        CursorSettings.cursor.ToggleCursor(true);
        FindObjectOfType<Player_Inventory>().SaveCloth();
        FindObjectOfType<Player_Inventory>().SaveItem();
        LobbyControl.lobbyControl.UnReady();

        PhotonNetwork.LeaveRoom();
    }

    public override void OnMasterClientSwitched(Player newMasterClient){
        base.OnMasterClientSwitched(newMasterClient);

        FindObjectOfType<GameAlert_Nortification>().SetAlert("Disconnected from the server",string.Format("The game '{0}' no longer exists.",PhotonNetwork.CurrentRoom.Name),true);

        CursorSettings.cursor.isOverride = false;
        CursorSettings.cursor.isInLobby = false;
        CursorSettings.cursor.ToggleCursor(true);
        FindObjectOfType<Player_Inventory>().SaveCloth();
        FindObjectOfType<Player_Inventory>().SaveItem();
        LobbyControl.lobbyControl.UnReady();

        PhotonNetwork.LeaveRoom();        
    }

    [PunRPC]
    public void RPC_HostDisconnect(string roomName){
        FindObjectOfType<GameAlert_Nortification>().SetAlert("Disconnected from the server",string.Format("The game '{0}' no longer exists.",roomName),true);
    }

    IEnumerator PrepareHostDisconnect(){
        while(PhotonNetwork.CurrentRoom.PlayerCount > 1){
            yield return null;
        }

        savePlayerData.SaveData(PhotonNetwork.LocalPlayer);

        while(!savePlayerData.isSaveDone){
            yield return null;
        }

        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom(){
        base.OnLeftRoom();

        PhotonNetwork.Disconnect();
        SceneManager.LoadSceneAsync(0);
    }

    void CheckPlayerName(Player newPlayer){
        if(!PhotonNetwork.IsMasterClient){return;}

        for (int i = 0; i < playerListMenu.playerListingInfos.Count; i++){
            if (playerListMenu.playerListingInfos[i].info.NickName == newPlayer.NickName){
                Debug.Log("Kick " + newPlayer.NickName);
                    
                isNotSaveThisPlayerData = true;

                FindObjectOfType<GameAlert_Nortification>().SetAlert("Disconnected from the server",string.Format("That name is already taken."),true);
                base.photonView.RPC("RPC_Disconnect",newPlayer);
                break;
            }
        }

        isCheckNameDone = true;
    }


}
