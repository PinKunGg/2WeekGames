using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BoarAI : MonoBehaviourPunCallbacks
{
    Monster_Movement monsterMove;
    BoarAttacker boarAttack;

    PlayerManager_Multiplayer playerManMulti;

    bool isChargeAttackReady = true;

    private void Start() {
        monsterMove = GetComponent<Monster_Movement>();
        boarAttack = GetComponent<BoarAttacker>();
        playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
        
        if(PhotonNetwork.IsMasterClient){
            Invoke("DelayStart",1f);
        }

        // InvokeRepeating("CheckIsPlayerInRange",0.5f,1f);
    }

    void DelayStart(){
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

