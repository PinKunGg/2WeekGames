using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject playerPrefabs;
    DisconnectFromServer playerMan_Multi;

    private void Awake() {
        playerMan_Multi = FindObjectOfType<DisconnectFromServer>();
    }
    private void Start() {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabs.name,Vector2.zero,Quaternion.identity);
        playerObj.name = PhotonNetwork.NickName;
    }
}
