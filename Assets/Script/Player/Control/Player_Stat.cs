using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Player_Stat : MonoBehaviour
{
    PhotonView photonView;
    public BasePlayerStat basePlayerStat;

    [Header("Player UI")]
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
        if (photonView.IsMine)
        {
            HealthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
            UpdateCurrentStat();
        }
        else 
        {
            GameObject temp = Instantiate(Friend_HealthBar_Prefab);
            temp.transform.parent = GameObject.FindGameObjectWithTag("FriendHealthZone").transform;
            temp.transform.localScale = new Vector3(1, 1, 1);
            Friend_HealthBar = temp.GetComponentInChildren<Slider>();
            UpdateFriend_CurrentStat();
        }
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
            photonView.RPC("UpdateHealthBar", RpcTarget.All, Current_HP);
        }
    }

    void UpdateCurrentStat() 
    {
        Max_Current_HP = basePlayerStat.base_HP;
        Current_HP = Max_Current_HP;
        HealthBar.maxValue = Max_Current_HP;

        photonView.RPC("UpdateHealthBar", RpcTarget.All, Current_HP);
    }

    void UpdateFriend_CurrentStat() 
    {
        Max_Current_HP = basePlayerStat.base_HP;
        Current_HP = Max_Current_HP;
        Friend_HealthBar.maxValue = Max_Current_HP;

        photonView.RPC("UpdateHealthBar", RpcTarget.All, Current_HP);
    }

    [PunRPC]
    void UpdateHealthBar(float value) 
    {
        if (photonView.IsMine)
        {
            HealthBar.value = value;
        }
        else 
        {
            Friend_HealthBar.value = value;
        }
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
