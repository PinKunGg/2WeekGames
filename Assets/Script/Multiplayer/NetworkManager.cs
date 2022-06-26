using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    PlayerListMenu _playerListMenu;
    SavePlayerData _savePlayerData;
    PlayerManager_Multiplayer _playerManMulti;
    SpawnPlayer _spawnPlayer;

    bool _isNotSaveThisPlayerData;
    bool _isCheckNameDone;

    public override void OnEnable(){
        base.OnEnable();

        _playerListMenu = FindObjectOfType<PlayerListMenu>();
        _playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
        _spawnPlayer = FindObjectOfType<SpawnPlayer>();
        _savePlayerData = GetComponent<SavePlayerData>();

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
            _isCheckNameDone = false;
            CheckPlayerName(newPlayer);

            StartCoroutine(WaitForCheckPlayerNameIsDone(newPlayer));
        }
    }

    IEnumerator WaitForCheckPlayerNameIsDone(Player newPlayer){
        while(!_isCheckNameDone){
            yield return null;
        }

        Debug.Log("CheckPlayerName Done!");

        if(_isNotSaveThisPlayerData){
            yield break;
        }

        base.photonView.RPC("RPC_SpawnNewPlayer",newPlayer);
        base.photonView.RPC("RPC_AddPlayerToPlayerListMenu",RpcTarget.All,newPlayer);
    }

    [PunRPC]
    void RPC_AddPlayerToPlayerListMenu(Player newPlayer){
        _playerListMenu.AddPlayerListMenu(newPlayer);
    }

    [PunRPC]
    void RPC_SpawnNewPlayer(){
        _spawnPlayer.SpawnNewPlayer();
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer){
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.LogFormat("Player '{0}' is leave the game",otherPlayer.NickName);

        if(_isNotSaveThisPlayerData){
            Debug.LogErrorFormat("Data from player '{0}' was not saved",otherPlayer.NickName);
            _isNotSaveThisPlayerData = false;
            return;
        }

        if(PhotonNetwork.IsMasterClient){
            _savePlayerData.SaveData(otherPlayer);

            StartCoroutine(WaitUntilSaveFinish(otherPlayer.ActorNumber));
        }
    }

    IEnumerator WaitUntilSaveFinish(int leavePlayerID){
        while(!_savePlayerData._isSaveDone){
            yield return null;
        }

        base.photonView.RPC("RPC_RemovePlayerFromPlayerList",RpcTarget.All,leavePlayerID);
        base.photonView.RPC("RPC_RemovePlayerFromPlayerListMenu",RpcTarget.All,leavePlayerID);
    }

    [PunRPC]
    void RPC_RemovePlayerFromPlayerListMenu(int otherPlayer){
        if(!_playerListMenu){
            _playerListMenu = FindObjectOfType<PlayerListMenu>();
        }
        _playerListMenu.RemovePlayerListMenu(otherPlayer);
    }
    [PunRPC]
    void RPC_RemovePlayerFromPlayerList(int otherPlayer){
        if(!_playerManMulti){
            _playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
        }

        _playerManMulti.RemovePlayer(otherPlayer);
    }

    [PunRPC]
    public void RPC_Disconnect(){
        Destroy(FindObjectOfType<PlayerListMenu>().gameObject);
        if (PhotonNetwork.IsMasterClient){
            base.photonView.RPC("RPC_Disconnect",RpcTarget.Others);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            StartCoroutine(PrepareHostDisconnect());

            return;
        }
        LobbyControl.lobbyControl.UnReady();
        PhotonNetwork.LeaveRoom();
    }

    IEnumerator PrepareHostDisconnect(){
        while(PhotonNetwork.CurrentRoom.PlayerCount > 1){
            yield return null;
        }

        _savePlayerData.SaveData(PhotonNetwork.LocalPlayer);

        while(!_savePlayerData._isSaveDone){
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

        for (int i = 0; i < _playerListMenu._playerListingInfos.Count; i++){
            if (_playerListMenu._playerListingInfos[i].info.NickName == newPlayer.NickName){
                Debug.Log("Kick " + newPlayer.NickName);
                    
                _isNotSaveThisPlayerData = true;

                base.photonView.RPC("RPC_Disconnect",newPlayer);
                break;
            }
        }

        _isCheckNameDone = true;
    }


}
