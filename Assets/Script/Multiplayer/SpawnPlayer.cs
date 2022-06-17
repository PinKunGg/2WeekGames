using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject playerPrefabs;

    private void Start() {
        if(!PhotonNetwork.IsMasterClient){return;}

        SpawnNewPlayer();
    }

    public void SpawnNewPlayer(){
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabs.name,Vector2.zero,Quaternion.identity);
        playerObj.name = PhotonNetwork.NickName;
    }
}
