using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ArachneAI : MonoBehaviour
{
    Monster_Hopping monsterHopping;
    Monster_RotateToTarget monsterRotToTarget;
    Arachne_Attacker arachneAttack;
    PlayerManager_Multiplayer playerManMulti;

    bool isChargeAttackReady = true;

    private void OnEnable()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }
        DelayStart();
    }
    void DelayStart(){
        monsterHopping = GetComponent<Monster_Hopping>();
        arachneAttack = GetComponent<Arachne_Attacker>();
        playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
        monsterRotToTarget = GetComponentInChildren<Monster_RotateToTarget>();

        GetPlayerTarget();
        InvokeRepeating("CheckIsPlayerInRange",0.5f,1f);
        InvokeRepeating("CheckToChangePlayerTarget",10f,10f);
    }

    void GetPlayerTarget(){
        Transform targetPlayer = playerManMulti.GetRandomPlayer().transform;
        monsterHopping.goToTarget = targetPlayer;
        monsterHopping.lookAtTarget = targetPlayer;
        monsterRotToTarget.target = targetPlayer;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.O)){
            // arachneAttack.AttackSpecific(2);
        }
    }

    void CheckIsPlayerInRange(){
        if(!monsterHopping.goToTarget){return;}

        if(monsterHopping.isPlayerInRange){
            arachneAttack.Attack();
        }
        else{
            arachneAttack.AttackSpecific(3);
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
        monsterRotToTarget.target = targetPlayer;
    }

    void DelayChargeAttack(){
        isChargeAttackReady = true;
    }
}

