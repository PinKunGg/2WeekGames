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
        _spawnPlayer = FindObjectOfType<SpawnPlayer>();
        _savePlayerData = GetComponent<SavePlayerData>();
        _playerManMulti = GetComponent<PlayerManager_Multiplayer>();

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
        }
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer){
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.LogFormat("Player '{0}' is leave the game",otherPlayer.NickName);

        if(_isNotSaveThisPlayerData){
            Debug.LogErrorFormat("Data from player '{0}' was not saved",otherPlayer.NickName);

            _playerManMulti.RemovePlayer(otherPlayer.ActorNumber);
            _isNotSaveThisPlayerData = false;
            return;
        }

        if(PhotonNetwork.IsMasterClient){
            _savePlayerData.SaveData(otherPlayer);

            StartCoroutine(WaitUntilSaveFinish(otherPlayer));
        }
    }

    IEnumerator WaitUntilSaveFinish(Player otherPlayer){
        while(!_savePlayerData._isSaveDone){
            yield return null;
        }

        _playerManMulti.RemovePlayer(otherPlayer.ActorNumber);
    }

    [PunRPC]
    public void RPC_Disconnect(){
        if (PhotonNetwork.IsMasterClient){
            base.photonView.RPC("RPC_Disconnect",RpcTarget.Others);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            StartCoroutine(PrepareHostDisconnect());

            return;
        }

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

        for (int i = 0; i < _playerListMenu._playerListingInfos.Count - 1; i++){
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
