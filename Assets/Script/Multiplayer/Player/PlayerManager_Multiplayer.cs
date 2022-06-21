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
        try{
            int userIndex = _allPlayerInCurrentRoom.FindIndex(x => x.GetComponent<PhotonView>().OwnerActorNr == ActorID);
            return userIndex;
        }catch{
            _allPlayerInCurrentRoom.RemoveAll(item => item == null);
            Debug.LogErrorFormat("Player Id '{0}' from list is null or empty",ActorID);
            return -1;
        }
    }

    public GameObject GetRandomPlayer(){
        int tempIndex = Random.Range(0,_allPlayerInCurrentRoom.Count);
        return _allPlayerInCurrentRoom[tempIndex];
    }

    public void AddPlayer(GameObject obj){
        _allPlayerInCurrentRoom.Add(obj);

        if(PhotonNetwork.IsMasterClient){
            _savePlayerData.LoadData(obj.GetComponent<PhotonView>());
        }
    }

    public void RemovePlayer(int ActorID){
        int indexPlayerListManager = GetPlayer(ActorID);

        if(indexPlayerListManager != -1){
           _allPlayerInCurrentRoom.RemoveAt(indexPlayerListManager);
        }
    }
}
