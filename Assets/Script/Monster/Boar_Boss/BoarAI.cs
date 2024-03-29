using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BoarAI : MonoBehaviour
{
    Monster_Movement monsterMove;
    Monster_Stat monsterStat;
    Boar_Attacker boarAttack;
    PlayerManager_Multiplayer playerManMulti;

    bool isChargeAttackReady = true;
    float TimeRandomChargeAttack = 5f;

    private void Awake() {
        monsterMove = GetComponent<Monster_Movement>();
        monsterStat = GetComponent<Monster_Stat>();
        boarAttack = GetComponent<Boar_Attacker>();
        playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
    }

    private void OnEnable()
    {
        if (!PhotonNetwork.IsMasterClient){ 
            enabled = false;
            return;
        }
        monsterMove.monsterStat = monsterStat;
        DelayStart();
    }
    public void DelayStart(){
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

        if(monsterStat.IsDie){
            CancelInvoke();
            return;
        }

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

