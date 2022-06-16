using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager_Multiplayer : MonoBehaviourPunCallbacks
{
    public List<GameObject> _allPlayerInCurrentRoom = new List<GameObject>();

    SavePlayerData _savePlayerData;

    public override void OnEnable(){
        base.OnEnable();

        _savePlayerData = FindObjectOfType<SavePlayerData>();
    }

    public int GetPlayer(int ActorID){
        int userIndex = _allPlayerInCurrentRoom.FindIndex(x => x.GetComponent<PhotonView>().OwnerActorNr == ActorID);
        return userIndex;
    }

    public void AddPlayer(GameObject obj){
        _allPlayerInCurrentRoom.Add(obj);
        _savePlayerData.LoadData(PhotonNetwork.LocalPlayer);
    }

    public void RemovePlayer(int ActorID){
        int indexPlayerListManager = GetPlayer(ActorID);

        if(indexPlayerListManager != -1){
           _allPlayerInCurrentRoom.RemoveAt(indexPlayerListManager);
        }
    }
}
