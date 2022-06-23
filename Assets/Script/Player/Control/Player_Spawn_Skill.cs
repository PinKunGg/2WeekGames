using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Player_Spawn_Skill : MonoBehaviour
{
    PhotonView photonView;
    Player_Attack_Control player_Attack_Control;
    public GameObject spawn_pos;

    [Header("Magic Skill Prefab")]
    public GameObject Magic_Normal_Attack;
    public GameObject Magic_Skill_1;
    public GameObject Magic_Skill_2;
    public bool[] IsMagicSKill = new bool[3] { false, false, false };
    public PlayerWeaponDamage stuff_weapondDamage;

    [Header("Bow Skill Prefab")]
    public GameObject Bow_Normal_Attack;
    public GameObject Bow_Skill_1;
    public GameObject Bow_Skill_2;
    public bool[] IsBowSkill = new bool[3] { false, false, false };
    public PlayerWeaponDamage bow_weapondDamage;

    GameObject[][] all_weapond_skill_obj = new GameObject[2][];
    public string animconName;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        player_Attack_Control = GetComponent<Player_Attack_Control>();
        init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void init() 
    {
        all_weapond_skill_obj[0] = new GameObject[3] { Magic_Normal_Attack, Magic_Skill_1, Magic_Skill_2 };
        all_weapond_skill_obj[1] = new GameObject[3] { Bow_Normal_Attack, Bow_Skill_1, Bow_Skill_2 };
        animconName = player_Attack_Control.AnimConName;
    }

    public void Spawn_Skill(int number_of_skill) 
    {
        animconName = player_Attack_Control.AnimConName;
        Debug.Log("Skill : " + number_of_skill);
        if (photonView.IsMine) 
        {
            if (animconName == "Stuff")
            {
                if (!IsMagicSKill[number_of_skill])
                {
                    IsMagicSKill[number_of_skill] = true;
                    photonView.RPC("Rpc_Spawn", RpcTarget.All, 0, number_of_skill);
                }
            }
            else if (animconName == "Bow")
            {
                if (!IsBowSkill[number_of_skill])
                {
                    IsBowSkill[number_of_skill] = true;
                    photonView.RPC("Rpc_Spawn", RpcTarget.All, 1, number_of_skill);
                }
            }
        }
    }

    [PunRPC]
    void Rpc_Spawn(int weapond,int number_of_skill) 
    {
        if (all_weapond_skill_obj[0][0] == null) { init(); }
        GameObject skill_obj = Instantiate(all_weapond_skill_obj[weapond][number_of_skill], spawn_pos.transform);
        skill_obj.transform.parent = null;
        PlayerWeaponDamage skill_weapond = skill_obj.GetComponentInChildren<PlayerWeaponDamage>();
        if (weapond == 0)
        {
            if (number_of_skill == 0)
            {
                skill_weapond.Damage = stuff_weapondDamage.Damage/3;
            }
            else if (number_of_skill == 1)
            {
                skill_weapond.Damage = stuff_weapondDamage.Damage/6;
                skill_weapond.IsDotDamage = true;
            }
            else if (number_of_skill == 2)
            {
                skill_weapond.Damage = stuff_weapondDamage.Damage*0.1f;
            }
            skill_weapond.CriRate = stuff_weapondDamage.CriRate;
            skill_weapond.CriDamage = stuff_weapondDamage.CriDamage;
        }
        else if(weapond == 1)
        {
            Debug.Log("Damage : " + bow_weapondDamage.Damage);
            Debug.Log("Damage : "+skill_weapond.name);
            if (number_of_skill == 0 )
            {
                skill_weapond.Damage = bow_weapondDamage.Damage;
            }
            else if(number_of_skill == 1)
            {
                skill_weapond.Damage = bow_weapondDamage.Damage / 50;
            }
            else if (number_of_skill == 2)
            {
                skill_weapond.Damage = bow_weapondDamage.Damage;
            }
            skill_weapond.CriRate = bow_weapondDamage.CriRate;
            skill_weapond.CriDamage = bow_weapondDamage.CriDamage;
        }

        Destroy(skill_obj, 5);
        StartCoroutine(Delay(weapond, number_of_skill));
    }

    IEnumerator Delay(int weapond, int number_of_skill) 
    {
        if (weapond == 1 && number_of_skill == 1)
        {
            yield return new WaitForSeconds(1);
        }
        else { yield return new WaitForSeconds(0.5f); }
        if (weapond == 0) { IsMagicSKill[number_of_skill] = false; }
        else if (weapond == 1) { IsBowSkill[number_of_skill] = false; }
    }


}