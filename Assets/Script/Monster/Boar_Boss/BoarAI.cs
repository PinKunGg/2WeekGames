using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BoarAI : MonoBehaviourPunCallbacks
{
    Monster_Movement monsterMove;
    Boar_Attacker boarAttack;

    PlayerManager_Multiplayer playerManMulti;

    bool isChargeAttackReady = true;

    float TimeRandomChargeAttack = 5f;

    public void DelayStart(){
        //Fuck
        monsterMove = GetComponent<Monster_Movement>();
        boarAttack = GetComponent<Boar_Attacker>();
        playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();

        GetPlayerTarget();
        InvokeRepeating("CheckIsPlayerInRange",0.5f,1f);
        InvokeRepeating("CheckToChangePlayerTarget",10f,10f);
    }

    void GetPlayerTarget(){
        Transform targetPlayer = playerManMulti.GetRandomPlayer().transform;
        monsterMove.goToTarget = targetPlayer;
        monsterMove.lookAtTarget = targetPlayer;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.O)){
            // boarAttack.AttackSpecific(2);
        }
    }

    void CheckIsPlayerInRange(){
        if(!monsterMove.goToTarget){return;}

        if(monsterMove.isPlayerInRange){
            boarAttack.Attack();
        }
        else{
            if(TimeRandomChargeAttack <= 0){
                if(Random.value > 0.8f){
                    boarAttack.AttackSpecific(2);
                }
                TimeRandomChargeAttack = 5f;
            }
            else{
                TimeRandomChargeAttack--;
            }
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
        monsterMove.goToTarget = targetPlayer;
        monsterMove.lookAtTarget = targetPlayer;
    }

    void DelayChargeAttack(){
        isChargeAttackReady = true;
    }
}

