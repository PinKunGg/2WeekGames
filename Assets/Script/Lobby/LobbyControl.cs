using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using Pathfinding;

public class LobbyControl : MonoBehaviour
{
    public static LobbyControl lobbyControl;

    public int PlayerCount = 0;
    public List<GameObject> AllPlayerObj;
    public GameObject[] Pos;
    PhotonView photonView;
    public bool[] SetUnlockCount = new bool[5] { true, false, false, false, false};
    public string[] nameStage = new string[5] { "Stage 1", "Stage 2", "Stage 3", "Stage 4", "Stage 5" };
    public Sprite[] imgStage = new Sprite[5];

    [Header("MasterUI")]
    public GameObject StartGameButton;
    public int IsotherReady_count;

    [Header("ClientUI")]
    public GameObject ReadyButton;
    public bool IsReady = false;

    public TMP_Dropdown SelectStage_DropDown;
    public TextMeshProUGUI selectstage_text;
    public GameObject InvenButUI, InvenMainUI, CraftButUI, CraftMainUI, ShowClothUI,SelectBossUI,BossHeathUI;
    public Player_Move_Control player_Move_Control;
    public GameObject[] MapStage;
    int stageIndex;

    public GameObject local_player;
    public GameObject[] AllBoss;

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

    public void OnStageChange(int value){
        stageIndex = value;

        if (value == 0)
            {
                MapStage[0].SetActive(true);
                MapStage[1].SetActive(false);
                MapStage[2].SetActive(false);
                MapStage[3].SetActive(false);
                MapStage[4].SetActive(false);
            }
            else if (value == 1)
            {
                MapStage[0].SetActive(false);
                MapStage[1].SetActive(true);
                MapStage[2].SetActive(false);
                MapStage[3].SetActive(false);
                MapStage[4].SetActive(false);
            }
            else if (value == 2)
            {
                MapStage[0].SetActive(false);
                MapStage[1].SetActive(false);
                MapStage[2].SetActive(true);
                MapStage[3].SetActive(false);
                MapStage[4].SetActive(false);
            }
            else if (value == 3)
            {
                MapStage[0].SetActive(false);
                MapStage[1].SetActive(false);
                MapStage[2].SetActive(false);
                MapStage[3].SetActive(true);
                MapStage[4].SetActive(false);
            }
            else if (value == 4)
            {
                MapStage[0].SetActive(false);
                MapStage[1].SetActive(false);
                MapStage[2].SetActive(false);
                MapStage[3].SetActive(false);
                MapStage[4].SetActive(true);
            }
            photonView.RPC("Rpc_ChangeStage",RpcTarget.All, MapStage[0].activeSelf, MapStage[1].activeSelf, MapStage[2].activeSelf, MapStage[3].activeSelf, MapStage[4].activeSelf);
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

    public void LoadSelectStage() 
    {
        SetUnlockCount = FindObjectOfType<Player_Inventory>().saveInventory.BossUnlockStage;
        SelectStage_DropDown.options.Clear();
        for (int x = 0; x < SetUnlockCount.Length; x++) 
        {
            if (x > 2) { break; }
            if (SetUnlockCount[x]) 
            {
                TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                data.text = nameStage[x];
                data.image = imgStage[x];
                SelectStage_DropDown.options.Add(data);
            }
        }
        if (SelectStage_DropDown.GetComponent<Image>().enabled == false) 
        {
            SelectStage_DropDown.GetComponent<Image>().enabled = true;
            SelectStage_DropDown.GetComponent<Image>().sprite = imgStage[0];
            selectstage_text.text = nameStage[0];
            OnStageChange(0);
        }
    }

    public void UpdateStageToOther() 
    {
        bool[] allmap = new bool[5] { MapStage[0].activeSelf, MapStage[1].activeSelf, MapStage[2].activeSelf, MapStage[3].activeSelf, MapStage[4].activeSelf };
        photonView.RPC("Rpc_UpdateStageToOther", RpcTarget.Others, allmap);
    }

    [PunRPC]
    void Rpc_UpdateStageToOther(bool[] allMapstage) 
    {
        Debug.Log("Run : wtf");
        MapStage[0].SetActive(allMapstage[0]);
        MapStage[1].SetActive(allMapstage[1]);
        MapStage[2].SetActive(allMapstage[2]);
        MapStage[3].SetActive(allMapstage[3]);
        MapStage[4].SetActive(allMapstage[4]);
    }

    public void StartGame() 
    {
        AstarPath.active.Scan();
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        IsotherReady_count = 0;
        photonView.RPC("Rpc_StartGame", RpcTarget.All,stageIndex);
    }

    [PunRPC]
    void Rpc_StartGame(int stage) 
    {
        Tutorial_Control.tutorial_Control.IsLobby = false;
        InvenButUI.SetActive(false);
        CraftButUI.SetActive(false);
        InvenMainUI.SetActive(false);
        CraftMainUI.SetActive(false);
        StartGameButton.SetActive(false);
        ReadyButton.SetActive(false);
        ShowClothUI.SetActive(false);
        SelectBossUI.SetActive(false);
        BossHeathUI.SetActive(true);
        player_Move_Control.CameraOn();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        local_player.GetComponentInChildren<CinemachineFreeLook>().m_XAxis.Value = 0;
        SoundBG soundbg = FindObjectOfType<SoundBG>();
        if (stage == 0) 
        { 
            AllBoss[0].SetActive(true);
            AllBoss[0].GetComponent<Monster_Stat>().MultiPlyHealth(AllPlayerObj.Count);
            soundbg.ChangeBGSound(0);
            //AllBoss[0].GetComponent<BoarAI>().DelayStart();
        }
        else if (stage == 1)
        { 
            AllBoss[1].SetActive(true);
            AllBoss[1].GetComponent<Monster_Stat>().MultiPlyHealth(AllPlayerObj.Count);
            soundbg.ChangeBGSound(1);
            //AllBoss[1].GetComponent<ArachneAI>().DelayStart();
        }
        else if (stage == 2)
        { 
            AllBoss[2].SetActive(true);
            AllBoss[2].GetComponent<Monster_Stat>().MultiPlyHealth(AllPlayerObj.Count);
            soundbg.ChangeBGSound(2);
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
        InvenButUI.SetActive(true);
        CraftButUI.SetActive(true);
        ShowClothUI.SetActive(true);
        BossHeathUI.SetActive(false);
        GetComponent<PlayerRespawn>().PlayerDieUI.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (PhotonNetwork.IsMasterClient)
        {
            StartGameButton.SetActive(true);
            ReadyButton.SetActive(false);
            SelectBossUI.SetActive(true);
            Check();
            LoadSelectStage();
        }
        else 
        {
            StartGameButton.SetActive(false);
            ReadyButton.SetActive(true);
            IsReady = false;
            ReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
        }
        FindObjectOfType<SoundBG>().ChangeToMainMenuBGSound();
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
