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
    Player_Inventory player_Inventory;
    Player_Attack_Control player_Attack_Control;
    Player_Move_Control player_Move_Control;
    public bool IsImu = false;

    public bool IsBarrierOn = false;
    public float BarrierMaxHP = 100;
    public float BarrierHP = 0;
    public GameObject BarrierOBJ;

    public string Player_Name;
    public float Damage_Multiplay = 0;
    public float Armour_Multiplay = 0;

    [Header("Player UI")]
    public TextMeshProUGUI PlayerNameUI;
    public Slider HealthBar;
    public GameObject Friend_HealthBar_Prefab;

    [Header("Current Stat")]
    public float Max_Current_HP = 0;
    public float Current_HP;
    public float Current_Armour;

    [Header("Reduce Damage Whebblock %")]
    public float ReduceDamage_Axe = 50;
    public float ReduceDamage_SwordAndShield = 70;
    public float ReduceDamage_Stuff = 50;
    public float ReduceDamage_Bow = -10;

    public Slider Friend_HealthBar;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        playerListMenu = PlayerListMenu.playerListMenu;
        player_Attack_Control = GetComponent<Player_Attack_Control>();
        player_Move_Control = GetComponent<Player_Move_Control>();
    }
    void Start()
    {

        if (photonView.IsMine)
        {

            Player_Name = photonView.Owner.NickName;
            player_Inventory = Player_Inventory.player_Inventory;
            playerListMenu.AddPlayerStat(this);
            HealthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
            PlayerNameUI = GameObject.FindGameObjectWithTag("PlayerName").GetComponent<TextMeshProUGUI>();
            PlayerNameUI.text = Player_Name;
            player_Inventory.player_Stat = this;
            SetCurrentStat();
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
            Player_Take_Damage(10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Heal();
        }
    }

    void Heal()
    {
        if (player_Inventory.PotionSlot > 0)
        {
            player_Inventory.PotionSlot--;
        }
        else { return; }
        int total_heal = player_Inventory.ClothesSlot_Item[6].PotionHeal;
        if ((Current_HP + total_heal) < Max_Current_HP)
        {
            Current_HP += total_heal;
        }
        else if ((Current_HP + total_heal) >= Max_Current_HP) 
        {
            Current_HP = Max_Current_HP;
        }
        if (player_Inventory.PotionSlot == 0)
        {
            player_Inventory.ClothesSlot_Item[6] = null;
            player_Inventory.ClothesSlot_pic[6].sprite = player_Inventory.Defath_Sprite;
            player_Inventory.UpdateSlot();
        }
        photonView.RPC("UpdateHealthBar", RpcTarget.All, Current_HP,Max_Current_HP, Player_Name);
    }

    public void Rpc_SetactiveBarrier() 
    {
        photonView.RPC("Rpc_SetactiveBarrier", RpcTarget.All, IsBarrierOn);
    }

    [PunRPC]
    void Rpc_SetactiveBarrier(bool value) 
    {
        BarrierOBJ.SetActive(value);
    }

    public void SetCurrentStat()
    {
        if (player_Inventory == null) { player_Inventory = FindObjectOfType<Player_Inventory>(); }
        Max_Current_HP = basePlayerStat.base_HP + player_Inventory.HP;
        Current_HP = Max_Current_HP;
        Current_Armour = basePlayerStat.base_Armor + player_Inventory.Armor;
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
        setDamage();
        UpdateHealthBarForOther();
    }

    public void setDamage() 
    {
        Player_Attack_Control playerWeaponDamage = GetComponent<Player_Attack_Control>();
        playerWeaponDamage.SetDamage(basePlayerStat.base_Damage + player_Inventory.Damage, basePlayerStat.base_Cri_Rate + player_Inventory.Cri_Rate,
    basePlayerStat.base_Cri_Damage + player_Inventory.Cri_Damage, Damage_Multiplay);
    }

    public void UpdateHealthBarForOther() 
    {
        if (!photonView) { photonView = GetComponent<PhotonView>(); }
        if (photonView.IsMine) 
        {
            Player_Name = photonView.Owner.NickName;
            photonView.RPC("UpdateHealthBar", RpcTarget.All, Current_HP,Max_Current_HP,Player_Name);
        }
    }


    [PunRPC]
    void UpdateHealthBar(float value,float max_hp,string name) 
    {
        if (!photonView) { photonView = GetComponent<PhotonView>(); }
        if (HealthBar == null) {HealthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>(); }
        Current_HP = value;
        Debug.Log("Run : " + gameObject.name);
        if (photonView.IsMine)
        {
            HealthBar.value = value;
        }
        else
        {
            if (!Friend_HealthBar) { CreateFriendHealthBar(name);  }
            Friend_HealthBar.maxValue = max_hp;
            Friend_HealthBar.value = value;
        }
    }

    void CreateFriendHealthBar(string name) 
    {

        playerNameControl = PlayerNameControl.playerNameControl;
        GameObject temp = Instantiate(Friend_HealthBar_Prefab);
        temp.transform.parent = GameObject.FindGameObjectWithTag("FriendHealthZone").transform;
        temp.transform.localScale = new Vector3(1, 1, 1);
        Friend_HealthBar = temp.GetComponentInChildren<Slider>();
        temp.GetComponentInChildren<TextMeshProUGUI>().text = name;
        playerNameControl.AddHealthBar(temp);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BossAttack")) 
        {
            Player_Take_Damage(other.GetComponentInParent<Monster_Stat>().baseMonsterStat.base_Damage);
        }
    }

    public void Player_Take_Damage(float damage) 
    {
        if (IsImu) { return; }
        if (player_Move_Control.IsIframe) { return; }
        float totoal_damagee = 0;
        totoal_damagee = damage * ((100 - (Current_Armour + Armour_Multiplay)) / 100);
        if (player_Attack_Control.IsBlock) 
        {
            if (player_Attack_Control.AnimConName == "Axe")
            {
                totoal_damagee = totoal_damagee * ((100 - ReduceDamage_Axe) / 100);
            }
            else if (player_Attack_Control.AnimConName == "Sword&Shield")
            {
                totoal_damagee = totoal_damagee * ((100 - ReduceDamage_SwordAndShield) / 100);
            }
            else if (player_Attack_Control.AnimConName == "Stuff")
            {
                totoal_damagee = totoal_damagee * ((100 - ReduceDamage_Stuff) / 100);
            }
            else if (player_Attack_Control.AnimConName == "Bow")
            {
                totoal_damagee = totoal_damagee * ((100 - ReduceDamage_Bow) / 100);
            }
        }
        if (BarrierHP - totoal_damagee > 0)
        {
            float temp_damage = totoal_damagee;
            totoal_damagee -= BarrierHP;
            BarrierHP -= temp_damage;
        }
        else if (BarrierHP - totoal_damagee <= 0) 
        {
            totoal_damagee -= BarrierHP;
            BarrierHP = 0;
            if (IsBarrierOn) 
            {
                BarrierExplosive();
            }
            Current_HP -= totoal_damagee;
        }
        Debug.Log("Run");
        photonView.RPC("UpdateHealthBar", RpcTarget.All, Current_HP,Max_Current_HP, Player_Name);
    }

    void BarrierExplosive() 
    {
        Debug.Log("BarrierExplosiv : " + BarrierMaxHP);
        IsBarrierOn = false;
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
