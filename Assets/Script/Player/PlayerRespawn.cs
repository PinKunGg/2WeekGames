using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerRespawn : MonoBehaviour
{
    public static PlayerRespawn playerRespawn;

    private void Awake()
    {
        playerRespawn = this;
    }
    PhotonView photonView;
    public int DeathCount = 0;
    public GameObject GameOverUI;
    public GameObject pos;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void player_respawn() 
    {
        player.transform.position = pos.transform.position;
        player.transform.rotation = pos.transform.rotation;
        photonView.RPC("SentDeathCountToMaster", RpcTarget.MasterClient);
    }

    [PunRPC]
    void SentDeathCountToMaster() 
    {
        if (DeathCount > 2)
        {
            photonView.RPC("RpcShowBGGameObver", RpcTarget.All);
            DeathCount = 0;
        }
        else 
        {
            DeathCount++;
        }
    }

    [PunRPC]
    void RpcShowBGGameObver() 
    {
        GameOverUI.SetActive(true);
        Tutorial_Control.tutorial_Control.IsLobby = true;
        Invoke("BackToLobby", 5);
    }
    void BackToLobby() 
    {
        photonView.RPC("RpcBackToLobby", RpcTarget.All);
    }

    [PunRPC]
    void RpcBackToLobby() 
    {
        GameOverUI.SetActive(false);
        LobbyControl lobbyControl = LobbyControl.lobbyControl;
        lobbyControl.local_player.GetComponent<Player_Move_Control>().OnLobbySetUp();
        lobbyControl.local_player.GetComponent<Player_Attack_Control>().CheckWeaponUse();
        lobbyControl.EndGame();
    }
}
