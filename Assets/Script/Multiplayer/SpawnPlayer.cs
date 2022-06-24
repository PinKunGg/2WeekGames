using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject playerPrefabs;
    public Transform spawnPoint;
    public LobbyControl lobbyControl;
    PhotonView photonView;

    private void Start() {
        if (!PhotonNetwork.IsMasterClient){return;}

        SpawnNewPlayer();
    }

    public void SpawnNewPlayer(){
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabs.name,spawnPoint.position,Quaternion.identity);
        playerObj.name = PhotonNetwork.NickName;
    }
}
