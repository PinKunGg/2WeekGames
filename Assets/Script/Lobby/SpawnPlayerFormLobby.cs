using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class SpawnPlayerFormLobby : MonoBehaviour
{
    [SerializeField] PlayerListMenu playerListMenu;

    public GameObject playerPrefabs;
    public Transform spawnPoint;
    GameObject playerObj;
    static bool IsFistRun = false;
    void Start()
    {
        playerListMenu = FindObjectOfType<PlayerListMenu>();
        SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeIsFirstRun() 
    {
        IsFistRun = true;
    }

    void SpawnPlayer() 
    {
        Debug.Log("IsFistRun : " + IsFistRun);
        if (SceneManager.GetActiveScene().name == "Multiplayer_Lobby") { return; }
        playerObj = PhotonNetwork.Instantiate(playerPrefabs.name, spawnPoint.position, Quaternion.identity);
        foreach (PlayerListInfo value in playerListMenu.playerListingInfos) 
        {        
            playerObj.GetComponent<PhotonView>().TransferOwnership(value.info);
        }
    }
}
