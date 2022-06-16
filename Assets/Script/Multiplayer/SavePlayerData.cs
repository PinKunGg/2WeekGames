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
        _playerManMulti = GetComponent<PlayerManager_Multiplayer>();
    }
    public void SaveData(Player player){
        _isSaveDone = false;
        int userIndex = _playerManMulti.GetPlayer(player.ActorNumber);

        SaveClass_PlayerData saveData = new SaveClass_PlayerData();
        saveData = _playerManMulti._allPlayerInCurrentRoom[userIndex].GetComponent<PlayerData>().sc;

        string data = JsonUtility.ToJson(saveData,true);
        js.SaveJson(Application.persistentDataPath,player.NickName, data);
        Debug.LogFormat("Save Data player '{0}'",player.NickName);

        _isSaveDone = true;
    }

    public void LoadData(Player player){
        _isLoadDone = false;
        int userIndex = _playerManMulti.GetPlayer(player.ActorNumber);

        string data = js.LoadJson(Application.persistentDataPath,player.NickName);

        if(string.IsNullOrEmpty(data)){
            Debug.LogWarningFormat("[Master] Data from player '{0}' not found",player.NickName);
        }else{
            Debug.LogWarningFormat("[Master] Data from player '{0}' found",player.NickName);
            _playerManMulti._allPlayerInCurrentRoom[userIndex].GetComponent<PlayerData>().ReciveSaveData(data);
        }

        _isLoadDone = true;
    }
}
