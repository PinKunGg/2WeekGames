using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject playerPrefabs;
    public GameObject SpawnPos;
    DisconnectFromServer playerMan_Multi;

    private void Awake() {
        playerMan_Multi = FindObjectOfType<DisconnectFromServer>();
    }
    private void Start() {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabs.name, SpawnPos.transform.position, Quaternion.identity);
        playerObj.name = PhotonNetwork.NickName;
        playerObj.GetComponent<Player_Stat>().Player_Name = PhotonNetwork.NickName;
    }
}
