using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SavePlayerData : MonoBehaviourPunCallbacks
{
    NetworkManager networkManager;
    PlayerManager_Multiplayer playerManMulti;
    JsonSaveSystem js;
    SaveManager sm;

    public bool isSaveDone;
    public bool isLoadDone;

    public override void OnEnable() {
        base.OnEnable();

        js = JsonSaveSystem.js;
        sm = SaveManager.sm;

        networkManager = GetComponent<NetworkManager>();
        playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
    }
    public void SaveData(Player player){
        isSaveDone = false;
        int userIndex = playerManMulti.GetPlayer(player.ActorNumber);

        SaveClass_PlayerData saveData = new SaveClass_PlayerData();
        saveData = playerManMulti._allPlayerInCurrentRoom[userIndex]._playerGameObject.GetComponent<PlayerData>().sc;

        string data = JsonUtility.ToJson(saveData,true);
        js.SaveJson(Application.persistentDataPath,player.NickName, data);
        Debug.LogFormat("Save Data player '{0}'",player.NickName);

        isSaveDone = true;
    }

    public void LoadData(PhotonView player){
        isLoadDone = false;
        int userIndex = playerManMulti.GetPlayer(player.OwnerActorNr);

        string data = js.LoadJson(Application.persistentDataPath,player.Owner.NickName);
        
        if(string.IsNullOrEmpty(data)){
            Debug.LogWarningFormat("[Master] Data from player '{0}' not found",player.Owner.NickName);
        }else{
            Debug.LogWarningFormat("[Master] Data from player '{0}' found",player.Owner.NickName);
            base.photonView.RPC("RPC_SendSaveDataToPlayer",RpcTarget.All,player.OwnerActorNr,data);
        }

        isLoadDone = true;
    }

    [PunRPC]
    void RPC_SendSaveDataToPlayer(int playerID, string data){
        int userIndex = playerManMulti.GetPlayer(playerID);
        playerManMulti._allPlayerInCurrentRoom[userIndex]._playerGameObject.GetComponent<PlayerData>().ReciveSaveData(data);
    }
}
