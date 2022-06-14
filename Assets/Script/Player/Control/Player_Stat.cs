using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Player_Stat : MonoBehaviour
{
    PhotonView photonView;
    public BaseStat baseStat;

    [Header("Player UI")]
    public Slider Health_Bar;
    public GameObject Friend_HealthBar_Prefab;

    [Header("Current Stat")]
    public float Max_Current_HP = 0;
    public float Current_HP;
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            Health_Bar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
            UpdateCurrentStat();
        }
        else 
        {
            GameObject temp = Instantiate(Friend_HealthBar_Prefab);
            temp.transform.parent = GameObject.FindGameObjectWithTag("FriendHealthZone").transform;
            temp.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        UpdateHealthBar();
    }

    void UpdateCurrentStat() 
    {
        Max_Current_HP = baseStat.base_HP;
        Current_HP = Max_Current_HP;
        Health_Bar.maxValue = Max_Current_HP;
    }

    void UpdateHealthBar() 
    {
        Health_Bar.value = Current_HP;
    }
}

[System.Serializable]
public class BaseStat 
{
    public float base_HP = 100;
    public float base_Armor = 0;
    public float base_Damage = 0;
    public float base_Cri_Rate = 0;
    public float vase_Cri_Damage = 0;
}
