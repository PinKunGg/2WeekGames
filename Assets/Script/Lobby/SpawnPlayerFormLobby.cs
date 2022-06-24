using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPlayerFormLobby : MonoBehaviour
{
    [SerializeField] PlayerListMenu playerListMenu;

    public GameObject playerPrefabs;
    public Transform spawnPoint;
    GameObject playerObj;
    void Start()
    {
        playerListMenu = FindObjectOfType<PlayerListMenu>();
        SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnPlayer() 
    {
        Debug.Log("y : run");
        playerObj = PhotonNetwork.Instantiate(playerPrefabs.name, spawnPoint.position, Quaternion.identity);
        foreach (PlayerListInfo value in playerListMenu._playerListingInfos) 
        {        
            playerObj.GetComponent<PhotonView>().TransferOwnership(value.info);
            Debug.Log("y : " + playerObj.name);
        }
    }
}
