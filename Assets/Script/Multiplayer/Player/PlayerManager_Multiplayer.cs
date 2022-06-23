using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager_Multiplayer : MonoBehaviourPunCallbacks
{
    public List<AllPlayerInRoom> _allPlayerInCurrentRoom = new List<AllPlayerInRoom>();

    SavePlayerData _savePlayerData;

    public override void OnEnable(){
        base.OnEnable();

        _savePlayerData = FindObjectOfType<SavePlayerData>();
    }

    public int GetPlayer(int ActorID){
        try{
            int userIndex = _allPlayerInCurrentRoom.FindIndex(x => x._playerGameObject.GetComponent<PhotonView>().OwnerActorNr == ActorID);
            return userIndex;
        }catch{
            _allPlayerInCurrentRoom.RemoveAll(item => item == null);
            Debug.LogErrorFormat("Player Id '{0}' from list is null or empty",ActorID);
            return -1;
        }
    }

    public GameObject GetRandomPlayer(){
        int tempIndex = Random.Range(0,_allPlayerInCurrentRoom.Count);
        return _allPlayerInCurrentRoom[tempIndex]._playerGameObject;
    }

    public void AddPlayer(GameObject obj){
        AllPlayerInRoom a = new AllPlayerInRoom();
        a._playerGameObject = obj;
        _allPlayerInCurrentRoom.Add(a);

        if(PhotonNetwork.IsMasterClient){
            _savePlayerData.LoadData(a._playerGameObject.GetComponent<PhotonView>());
        }
    }

    public void RemovePlayer(int ActorID){
        int indexPlayerListManager = GetPlayer(ActorID);

        if(indexPlayerListManager != -1){
           _allPlayerInCurrentRoom.RemoveAt(indexPlayerListManager);
        }
    }

    public void AddPlayerDamage(int ActorID, float Damage){
        _allPlayerInCurrentRoom[GetPlayer(ActorID)]._playerDamageDealToBoss += Damage;
    }
}

[System.Serializable]
public class AllPlayerInRoom{
    public GameObject _playerGameObject;
    public float _playerDamageDealToBoss;
}
