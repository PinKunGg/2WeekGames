using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SavePlayerData : MonoBehaviourPunCallbacks
{
    NetworkManager _networkManager;
    PlayerManager_Multiplayer _playerManMulti;
    JsonSaveSystem js;
    SaveManager sm;

    public bool _isSaveDone;
    public bool _isLoadDone;

    public override void OnEnable() {
        base.OnEnable();

        js = JsonSaveSystem.js;
        sm = SaveManager.sm;

        _networkManager = GetComponent<NetworkManager>();
        _playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
    }
    public void SaveData(Player player){
        _isSaveDone = false;
        int userIndex = _playerManMulti.GetPlayer(player.ActorNumber);

        SaveClass_PlayerData saveData = new SaveClass_PlayerData();
        saveData = _playerManMulti._allPlayerInCurrentRoom[userIndex]._playerGameObject.GetComponent<PlayerData>().sc;

        string data = JsonUtility.ToJson(saveData,true);
        js.SaveJson(Application.persistentDataPath,player.NickName, data);
        Debug.LogFormat("Save Data player '{0}'",player.NickName);

        _isSaveDone = true;
    }

    public void LoadData(PhotonView player){
        _isLoadDone = false;
        int userIndex = _playerManMulti.GetPlayer(player.OwnerActorNr);

        string data = js.LoadJson(Application.persistentDataPath,player.Owner.NickName);
        
        if(string.IsNullOrEmpty(data)){
            Debug.LogWarningFormat("[Master] Data from player '{0}' not found",player.Owner.NickName);
        }else{
            Debug.LogWarningFormat("[Master] Data from player '{0}' found",player.Owner.NickName);
            base.photonView.RPC("RPC_SendSaveDataToPlayer",RpcTarget.All,player.OwnerActorNr,data);
        }

        _isLoadDone = true;
    }

    [PunRPC]
    void RPC_SendSaveDataToPlayer(int playerID, string data){
        int userIndex = _playerManMulti.GetPlayer(playerID);
        _playerManMulti._allPlayerInCurrentRoom[userIndex]._playerGameObject.GetComponent<PlayerData>().ReciveSaveData(data);
    }
}
