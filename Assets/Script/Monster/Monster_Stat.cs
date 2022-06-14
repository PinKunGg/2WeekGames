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
    public Slider Monster_HealthBar;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    void TakeDamage() 
    {
    
    }
}

[System.Serializable]
public class BaseMonsterStat
{
    public string Name_Monster;
    public float base_HP = 100;
    public float base_Armor = 0;
    public float base_Damage = 0;
}
