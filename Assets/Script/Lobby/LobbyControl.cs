using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class LobbyControl : MonoBehaviour
{
    public static LobbyControl lobbyControl;

    public int PlayerCount = 0;
    public List<GameObject> AllPlayerObj;
    public GameObject[] Pos;
    PhotonView photonView;

    [Header("MasterUI")]
    public GameObject StartGameButton;
    public int IsotherReady_count;

    [Header("ClientUI")]
    public GameObject ReadyButton;
    public bool IsReady = false;
    // Start is called before the first frame update

    private void Awake()
    {
        lobbyControl = this;
    }
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (!Tutorial_Control.tutorial_Control.IsLobby) { return; }
        if (PhotonNetwork.IsMasterClient)
        {
            StartGameButton.SetActive(false);
            ReadyButton.SetActive(false);
        }
        else 
        {
            StartGameButton.SetActive(false);
            ReadyButton.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addPlayer(GameObject player) 
    {
        AllPlayerObj.Add(player);
        setPlayerPosition();
    }

    public void setPlayerPosition() 
    {
        if (AllPlayerObj.Count > 1) 
        {
            for (int x = 0; x < AllPlayerObj.Count; x++) 
            {
                if (AllPlayerObj[x] == null) 
                { 
                    AllPlayerObj.RemoveAt(x);
                    x--;
                }
            }
        }
        AllPlayerObj[PlayerCount].transform.position = Pos[PlayerCount].transform.position;
        AllPlayerObj[PlayerCount].transform.rotation = Pos[PlayerCount].transform.rotation;
        PlayerCount++;
        Check();
    }

    public void clientReady() 
    {
        photonView.RPC("Rpc_clientReady", RpcTarget.MasterClient,IsReady);
        if(!PhotonNetwork.IsMasterClient)
        if (!IsReady)
        {
            IsReady = true;
            ReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "UnReady";
        }
        else
        {
            IsReady = false;
            ReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
        }
    }
    public void UnReadyFormLeaveRoom() 
    {
        photonView.RPC("Rpc_clientReady", RpcTarget.MasterClient, true);
    }
    public void WhenPlayerLeftRoom() 
    {
        if (PhotonNetwork.IsMasterClient) 
        {
            PlayerCount--;
        }
        Check();
    }

    [PunRPC]
    public void Rpc_clientReady(bool value)
    {
        if (!value)
        {
            IsotherReady_count++;
        }
        else 
        {
            IsotherReady_count--;
        }
        Check();
    }

    void Check() 
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
        if (IsotherReady_count == PlayerCount - 1)
        {
            StartGameButton.SetActive(true);
        }
        else 
        {
            StartGameButton.SetActive(false);
        }

    }
}
