using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Monster_Stat : MonoBehaviour
{
    Tutorial_Control tutorial_Control;
    public Player_Inventory Player_Inventory;
    PhotonView photonView;
    public BaseMonsterStat baseMonsterStat;
    [Header("Monster Setting")]
    public float Current_HP;
    public Slider Monster_HealthBar;

    public Item itemDrop;
    public int[] AmountRangeitemDrop = new int[2];
    PlayerWeaponDamage forDot;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        tutorial_Control = Tutorial_Control.tutorial_Control;
        UpdateMonsterCurrentStat();
    }

    // Update is called once per frame
    void Update()
    {
        if (forDot == null) 
        {
            CancelInvoke("Dotdamage");
        }
    }

    void UpdateMonsterCurrentStat() 
    {
        Current_HP = baseMonsterStat.base_HP;
        Monster_HealthBar = GameObject.FindGameObjectWithTag("MonsterHealthBar").GetComponent<Slider>();
        Monster_HealthBar.maxValue = Current_HP;
        Monster_HealthBar.value = Current_HP;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon")) 
        {
            Debug.Log(other.gameObject.name);
            float totoal_damage = 0;
            PlayerWeaponDamage playerWeaponDamage = other.GetComponent<PlayerWeaponDamage>();
            if (!playerWeaponDamage.IsDotDamage)
            {
                if (Random.value * 100 < playerWeaponDamage.CriRate)
                {
                    totoal_damage = playerWeaponDamage.Damage * ((playerWeaponDamage.CriDamage + 100) / 100);
                }
                else { totoal_damage = playerWeaponDamage.Damage; }
                Current_HP -= totoal_damage;
                photonView.RPC("TakeDamage", RpcTarget.All, Current_HP);
            }
            else 
            {
                forDot = playerWeaponDamage;
                InvokeRepeating("Dotdamage", 0.5f, 0.5f);
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            float totoal_damage = 0;
            PlayerWeaponDamage playerWeaponDamage = other.GetComponent<PlayerWeaponDamage>();
            if (!playerWeaponDamage.IsDotDamage)
            {
                if (Random.value * 100 < playerWeaponDamage.CriRate)
                {
                    totoal_damage = playerWeaponDamage.Damage * ((playerWeaponDamage.CriDamage + 100) / 100);
                }
                else { totoal_damage = playerWeaponDamage.Damage; }
                Current_HP -= totoal_damage;
                photonView.RPC("TakeDamage", RpcTarget.All, Current_HP);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            Debug.Log("Exit : "+other.gameObject.name);
            PlayerWeaponDamage playerWeaponDamage = other.GetComponent<PlayerWeaponDamage>();
            if (playerWeaponDamage.IsDotDamage)
            {
                CancelInvoke("Dotdamage");
            }
        }
    }

    void Dotdamage() 
    {
        float totoal_damage = 0;
        if (Random.value * 100 < forDot.CriRate)
        {
            totoal_damage = forDot.Damage * ((forDot.CriDamage + 100) / 100);
        }
        else { totoal_damage = forDot.Damage; }
        Current_HP -= totoal_damage;
        photonView.RPC("TakeDamage", RpcTarget.All, Current_HP);

    }

    [PunRPC]
    void TakeDamage(float Damage) 
    {
        if (Monster_HealthBar.value > 0)
        {
            Monster_HealthBar.value = Damage;
        }
        else 
        {
            if (photonView.IsMine) 
            {
                DropItem();
            }
        }
    }

    void DropItem() 
    {
        if (!TutorialCheck(9)) { return; }
        Player_Inventory.AddItem(itemDrop, Random.Range(AmountRangeitemDrop[0], AmountRangeitemDrop[1]));
    }

    bool TutorialCheck(int stage)
    {
        if (tutorial_Control.IsTutorial)
        {
            if (stage > 0)
            {
                if (tutorial_Control.Stage[stage - 1])
                {
                    tutorial_Control.CompleteStage(stage);
                    return true;
                }
                else { return false; }
            }
            else
            {
                tutorial_Control.CompleteStage(stage);
                return true;
            }
        }
        else { return true; }
    }
}

[System.Serializable]
public class BaseMonsterStat
{
    public string Name_Monster;
    public float base_HP = 100;
    public float base_Armor = 5;
    public float base_Damage = 0;
}
