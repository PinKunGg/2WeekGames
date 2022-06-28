using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

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

    public TextMeshProUGUI SelectStage_output;
    public GameObject InvenUI, CraftUI, ShowClothUI,SelectBossUI,BossHeathUI;
    public Player_Move_Control player_Move_Control;
    public GameObject[] MapStage;
    string IsStageChange;

    public GameObject local_player;
    public GameObject[] AllBoss;
    // Start is called before the first frame update

    private void Awake()
    {
        lobbyControl = this;
    }
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (!Tutorial_Control.tutorial_Control.IsLobby)
        {
            return;
        }
        else 
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
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
        if (IsStageChange != SelectStage_output.text) 
        {
            IsStageChange = SelectStage_output.text;
            if (SelectStage_output.text == "Stage 1")
            {
                MapStage[0].SetActive(true);
                MapStage[1].SetActive(false);
                MapStage[2].SetActive(false);
                MapStage[3].SetActive(false);
                MapStage[4].SetActive(false);
            }
            else if (SelectStage_output.text == "Stage 2")
            {
                MapStage[0].SetActive(false);
                MapStage[1].SetActive(true);
                MapStage[2].SetActive(false);
                MapStage[3].SetActive(false);
                MapStage[4].SetActive(false);
            }
            else if (SelectStage_output.text == "Stage 3")
            {
                MapStage[0].SetActive(false);
                MapStage[1].SetActive(false);
                MapStage[2].SetActive(true);
                MapStage[3].SetActive(false);
                MapStage[4].SetActive(false);
            }
            else if (SelectStage_output.text == "Stage 4")
            {
                MapStage[0].SetActive(false);
                MapStage[1].SetActive(false);
                MapStage[2].SetActive(false);
                MapStage[3].SetActive(true);
                MapStage[4].SetActive(false);
            }
            else if (SelectStage_output.text == "Stage 5")
            {
                MapStage[0].SetActive(false);
                MapStage[1].SetActive(false);
                MapStage[2].SetActive(false);
                MapStage[3].SetActive(false);
                MapStage[4].SetActive(true);
            }
            photonView.RPC("Rpc_ChangeStage",RpcTarget.All, MapStage[0].activeSelf, MapStage[1].activeSelf, MapStage[2].activeSelf, MapStage[3].activeSelf, MapStage[4].activeSelf);
        }     
    }

    [PunRPC]
    void Rpc_ChangeStage(bool stage1, bool stage2, bool stage3, bool stage4, bool stage5) 
    {
        MapStage[0].SetActive(stage1);
        MapStage[1].SetActive(stage2);
        MapStage[2].SetActive(stage3);
        MapStage[3].SetActive(stage4);
        MapStage[4].SetActive(stage5);
    }   

    public void addPlayer(GameObject player) 
    {
        if (player.GetComponent<PhotonView>().IsMine) { local_player = player; }
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
    public void UnReady() 
    {
        if (IsReady == true) { photonView.RPC("Rpc_clientReady", RpcTarget.MasterClient, true); }
        IsReady = false;
        ReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
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
            if (IsotherReady_count > 0) { IsotherReady_count--; }
        }
        Check();
    }

    void Check() 
    {
        if (!Tutorial_Control.tutorial_Control.IsLobby) { return; }
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

    public void StartGame() 
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        IsotherReady_count = 0;
        photonView.RPC("Rpc_StartGame", RpcTarget.All,IsStageChange);
    }

    [PunRPC]
    void Rpc_StartGame(string stage) 
    {
        Tutorial_Control.tutorial_Control.IsLobby = false;
        InvenUI.SetActive(false);
        CraftUI.SetActive(false);
        StartGameButton.SetActive(false);
        ReadyButton.SetActive(false);
        ShowClothUI.SetActive(false);
        SelectBossUI.SetActive(false);
        BossHeathUI.SetActive(true);
        player_Move_Control.CameraOn();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        local_player.GetComponentInChildren<CinemachineFreeLook>().m_XAxis.Value = 0;
        if (stage == "Stage 1") 
        { 
            AllBoss[0].SetActive(true);
            //AllBoss[0].GetComponent<BoarAI>().DelayStart();
        }
        else if (stage == "Stage 2")
        { 
            AllBoss[1].SetActive(true);
            //AllBoss[1].GetComponent<ArachneAI>().DelayStart();
        }
        else if (stage == "Stage 3")
        { 
            AllBoss[2].SetActive(true);
            //AllBoss[2].GetComponent<AssasinAI>().DelayStart();
        }
    }

    public void EndGame() 
    {
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;
        photonView.RPC("Rpc_EndGame", RpcTarget.All);
    }

    [PunRPC]
    void Rpc_EndGame()
    {
        Tutorial_Control.tutorial_Control.IsLobby = true;
        local_player.GetComponent<Player_Move_Control>().OnLobbySetUp(true);
        local_player.GetComponent<Player_Attack_Control>().CheckWeaponUse();
        local_player.GetComponent<Player_Attack_Control>().IsDraw = false;
        local_player.GetComponent<Player_Attack_Control>().IsDrawed = false;
        InvenUI.SetActive(true);
        CraftUI.SetActive(true);
        ShowClothUI.SetActive(true);
        BossHeathUI.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (PhotonNetwork.IsMasterClient)
        {
            StartGameButton.SetActive(true);
            ReadyButton.SetActive(false);
            SelectBossUI.SetActive(true);
            Check();
        }
        else 
        {
            StartGameButton.SetActive(false);
            ReadyButton.SetActive(true);
            IsReady = false;
            ReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
        }
        resetPosition();
    }

    void resetPosition() 
    {
        for (int x = 0; x < AllPlayerObj.Count; x++)
        {
            if (AllPlayerObj[x].name == Player_Inventory.player_Inventory.player_name) 
            {
                AllPlayerObj[x].GetComponent<Rigidbody>().velocity = Vector3.zero;
                AllPlayerObj[x].transform.position = Pos[x].transform.position;
                AllPlayerObj[x].transform.rotation = Pos[x].transform.rotation;
            }
        }
    }

}
