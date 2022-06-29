using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

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
    public GameObject PlayerDieUI;
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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PlayerDieUI.SetActive(true);
        photonView.RPC("SentDeathCountToMaster", RpcTarget.MasterClient);
    }
    public void When_player_respawn()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerDieUI.SetActive(false);
        player.GetComponent<Player_Move_Control>().OnLobbySetUp(false);
        player.GetComponent<Player_Attack_Control>().CheckWeaponUse();
        player.GetComponent<Player_Attack_Control>().IsDraw = false;
        player.GetComponent<Player_Attack_Control>().IsDrawed = false;
        player.GetComponent<Player_Buff_Control>().StopCoroutine();
        player.GetComponent<Player_Stat>().WhenRespawn();
        player.GetComponentInChildren<CinemachineFreeLook>().m_XAxis.Value = 0;
        player.transform.position = pos.transform.position;
        player.transform.rotation = pos.transform.rotation;
        
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
        PlayerDieUI.SetActive(false);
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
        lobbyControl.EndGame();
    }
}
