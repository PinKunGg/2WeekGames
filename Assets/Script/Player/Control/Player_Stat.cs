using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class Player_Stat : MonoBehaviour
{
    PhotonView photonView;
    public PlayerListMenu playerListMenu;
    public BasePlayerStat basePlayerStat;
    public PlayerNameControl playerNameControl;

    public string Player_Name;

    [Header("Player UI")]
    public TextMeshProUGUI PlayerNameUI;
    public Slider HealthBar;
    public GameObject Friend_HealthBar_Prefab;

    [Header("Current Stat")]
    public float Max_Current_HP = 0;
    public float Current_HP;

    [Header("Friend Health")]

    public Slider Friend_HealthBar;
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        playerListMenu = PlayerListMenu.playerListMenu;
        
        if (photonView.IsMine)
        {
            Player_Name = photonView.Owner.NickName;
            playerListMenu.AddPlayerStat(this);
            HealthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
            PlayerNameUI = GameObject.FindGameObjectWithTag("PlayerName").GetComponent<TextMeshProUGUI>();
            PlayerNameUI.text = Player_Name;
            SetCurrentStat();
        }
        //else
        //{
        //    if (!Friend_HealthBar) { CreateFriendHealthBar(null); }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Current_HP -= 5;
            photonView.RPC("UpdateHealthBar", RpcTarget.All, Current_HP, Player_Name);
        }
    }

    void SetCurrentStat()
    {
        Max_Current_HP = basePlayerStat.base_HP;
        Current_HP = Max_Current_HP;
        if (HealthBar)
        {
            HealthBar.maxValue = Max_Current_HP;
            HealthBar.value = Current_HP;
        }
        else if (Friend_HealthBar)
        {
            Friend_HealthBar.maxValue = Max_Current_HP;
            Friend_HealthBar.value = Current_HP;
        }
        UpdateHealthBarForOther();
    }

    public void UpdateHealthBarForOther() 
    {
        if (!photonView) { photonView = GetComponent<PhotonView>(); }
        if (photonView.IsMine) 
        {
            Debug.Log("Health F Run");
            photonView.RPC("UpdateHealthBar", RpcTarget.All, Current_HP,Player_Name);
        }
    }


    [PunRPC]
    void UpdateHealthBar(float value,string name) 
    {
        if (!photonView) { photonView = GetComponent<PhotonView>(); }
        Current_HP = value;
        if (photonView.IsMine)
        {
            Debug.Log("Health My : " + this.gameObject.name + value);
            HealthBar.value = value;
        }
        else
        {
            if (!Friend_HealthBar) { CreateFriendHealthBar(name);  }
            Friend_HealthBar.maxValue = 100f;
            Friend_HealthBar.value = value;
            Debug.Log("Health F : " + this.gameObject.name + value);
        }
    }

    void CreateFriendHealthBar(string name) 
    {
        Debug.Log("Create F HealthBar");
        //SetCurrentStat();
        playerNameControl = PlayerNameControl.playerNameControl;
        GameObject temp = Instantiate(Friend_HealthBar_Prefab);
        temp.transform.parent = GameObject.FindGameObjectWithTag("FriendHealthZone").transform;
        temp.transform.localScale = new Vector3(1, 1, 1);
        Friend_HealthBar = temp.GetComponentInChildren<Slider>();
        //temp.GetComponentInChildren<TextMeshProUGUI>().text = playerNameControl.GetOtherNamePlayer();
        temp.GetComponentInChildren<TextMeshProUGUI>().text = name;
        playerNameControl.AddHealthBar(temp);
    }
}

[System.Serializable]
public class BasePlayerStat 
{
    public float base_HP = 100;
    public float base_Armor = 0;
    public float base_Damage = 0;
    public float base_Cri_Rate = 0;
    public float base_Cri_Damage = 0;
}
