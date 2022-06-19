using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Monster_Stat : MonoBehaviour
{
    PhotonView photonView;
    public BaseMonsterStat baseMonsterStat;
    [Header("Monster Setting")]
    public float Current_HP;
    public Slider Monster_HealthBar;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        UpdateMonsterCurrentStat();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            float totoal_damage = 0;
            PlayerWeaponDamage playerWeaponDamage = other.GetComponent<PlayerWeaponDamage>();
            if (Random.value * 100 < playerWeaponDamage.CriRate)
            {
                totoal_damage = playerWeaponDamage.Damage * ((playerWeaponDamage.CriDamage+100)/100);
            }
            else { totoal_damage = playerWeaponDamage.Damage; }
            Current_HP -= totoal_damage;
            photonView.RPC("TakeDamage", RpcTarget.All,Current_HP);
        }
    }

    [PunRPC]
    void TakeDamage(float Damage) 
    {
        Monster_HealthBar.value = Damage;
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
