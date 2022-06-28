using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Arachne_SpiderMinion : MonoBehaviourPunCallbacks
{
    public float Current_HP;

    Monster_Animation monsterAnima;
    Monster_Movement monsterMove;
    Monster_Hopping monsterHopping;
    PlayerWeaponDamage forDot;
    ArachneAI arachneAI;
    bool isDeath;

    private void Awake() {
        monsterAnima = GetComponent<Monster_Animation>();
        monsterMove = GetComponent<Monster_Movement>();
        monsterHopping = FindObjectOfType<Monster_Hopping>();
        arachneAI = FindObjectOfType<ArachneAI>();
    }

    public override void OnEnable() {
        if(!PhotonNetwork.IsMasterClient){return;}

        monsterMove.goToTarget = monsterHopping.goToTarget;
        monsterMove.lookAtTarget = monsterHopping.lookAtTarget;
    }

    private void Update() {
        if(isDeath){return;}

        if(arachneAI.monsterStat.IsDie){
            photonView.RPC("TakeDamage", RpcTarget.All, 999f);
        }

        if (forDot == null) 
        {
            CancelInvoke("Dotdamage");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(isDeath){return;}

        if(other.gameObject.CompareTag("Player")){
            Death();
        }

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
                base.photonView.RPC("TakeDamage", RpcTarget.All, totoal_damage);
            }
            else 
            {
                forDot = playerWeaponDamage;
                InvokeRepeating("Dotdamage", 0.5f, 0.5f);
            }
        }
    }

    private void OnParticleCollision(GameObject other){
        if(isDeath){return;}

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
                photonView.RPC("TakeDamage", RpcTarget.All, totoal_damage);
            }
        }
    }
    private void OnTriggerExit(Collider other){
        if(isDeath){return;}

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
        if(isDeath){
            CancelInvoke("Dotdamage");
            return;
        }

        float totoal_damage = 0;
        if (Random.value * 100 < forDot.CriRate)
        {
            totoal_damage = forDot.Damage * ((forDot.CriDamage + 100) / 100);
        }
        else { totoal_damage = forDot.Damage; }
        photonView.RPC("TakeDamage", RpcTarget.All, totoal_damage);

    }

    [PunRPC]
    void TakeDamage(float Damage) {
        Current_HP -= Damage;

        if(Current_HP <= 0f){
            Death();
        }
    }

    void Death(){
        isDeath = true;
        monsterMove.isStopWalk = true;
        monsterMove.goToTarget = null;
        monsterMove.lookAtTarget = null;
        monsterAnima.PlayBoolAnimator("IsDeath",true);
        Invoke("DelayDisableAnimation",0.5f);

        if(!PhotonNetwork.IsMasterClient){return;}
        Invoke("DelayDestroy",4f);
    }

    void DelayDisableAnimation(){
        monsterAnima.PlayBoolAnimator("IsDeath",false);
        monsterAnima.PlayBoolAnimator("IsExplosion",false);
    }

    void DelayDestroy(){
        PhotonNetwork.Destroy(base.photonView);
    }
}
