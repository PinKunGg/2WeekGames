using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AssasinAI : MonoBehaviour
{
    Monster_Hopping monsterHopping;
    Assasin_Attacker assasinAttack;
    PlayerManager_Multiplayer playerManMulti;
    Monster_SkillTeleportToTarget monsterSkillTp;

    bool isChargeAttackReady = true;

    private void Start() {
        monsterHopping = GetComponent<Monster_Hopping>();
        assasinAttack = GetComponent<Assasin_Attacker>();
        playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
        monsterSkillTp = GetComponent<Monster_SkillTeleportToTarget>();
        
        if(PhotonNetwork.IsMasterClient){
            Invoke("DelayStart",1f);
        }

        InvokeRepeating("CheckIsPlayerInRange",0.5f,1f);
    }

    void DelayStart(){
        GetPlayerTarget();
        InvokeRepeating("CheckIsPlayerInRange",0.5f,1f);
        InvokeRepeating("CheckToChangePlayerTarget",10f,10f);
    }

    void GetPlayerTarget(){
        Transform targetPlayer = playerManMulti.GetRandomPlayer().transform;
        monsterHopping.goToTarget = targetPlayer;
        monsterHopping.lookAtTarget = targetPlayer;
        monsterSkillTp.target = targetPlayer;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.O)){
            assasinAttack.AttackSpecific(3);
        }
    }

    void CheckIsPlayerInRange(){
        if(!monsterHopping.goToTarget){return;}

        if(monsterHopping.isPlayerInRange){
            assasinAttack.Attack();
        }
        else{
            assasinAttack.AttackSpecific(3);
        }
    }

    void CheckToChangePlayerTarget(){
        float hightestDamage = 0;
        int playerHightestDamage = 0;

        for(int i = 0; i < playerManMulti._allPlayerInCurrentRoom.Count; i++){
            if(playerManMulti._allPlayerInCurrentRoom[i]._playerDamageDealToBoss > hightestDamage){
                hightestDamage = playerManMulti._allPlayerInCurrentRoom[i]._playerDamageDealToBoss;
                playerHightestDamage = i;
            }
        }

        Transform targetPlayer = playerManMulti._allPlayerInCurrentRoom[playerHightestDamage]._playerGameObject.transform;
        monsterHopping.goToTarget = targetPlayer;
        monsterHopping.lookAtTarget = targetPlayer;
    }

    void DelayChargeAttack(){
        isChargeAttackReady = true;
    }
}
