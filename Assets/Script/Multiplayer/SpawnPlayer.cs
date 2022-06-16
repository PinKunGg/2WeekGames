using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject playerPrefabs;

    private void Start() {
        SpawnNewPlayer();
    }

    public void SpawnNewPlayer(){
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabs.name,Vector2.zero,Quaternion.identity);
        playerObj.name = PhotonNetwork.NickName;
    }
}
