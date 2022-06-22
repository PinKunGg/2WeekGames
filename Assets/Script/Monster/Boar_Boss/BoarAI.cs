using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BoarAI : MonoBehaviourPunCallbacks
{
    Monster_Movement _monsterMove;
    Monster_Attacker _monsterAttack;
    Monster_Retreat _monsterRetreat;

    PlayerManager_Multiplayer _playerManMulti;

    bool isChargeAttackReady = true;

    private void Start() {
        _monsterMove = GetComponent<Monster_Movement>();
        _monsterAttack = GetComponent<Monster_Attacker>();
        _monsterRetreat = GetComponent<Monster_Retreat>();
        _playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
        
        if(PhotonNetwork.IsMasterClient){
            Invoke("DelayStart",1f);
        }

        // InvokeRepeating("CheckIsPlayerInRange",0f,1f);
    }

    void DelayStart(){
        GetPlayerTarget();
        InvokeRepeating("CheckIsPlayerInRange",0.5f,1f);
    }

    void GetPlayerTarget(){
        Transform targetPlayer = _playerManMulti.GetRandomPlayer().transform;
        _monsterMove.goToTarget = targetPlayer;
        _monsterMove.lookAtTarget = targetPlayer;
        _monsterRetreat.target = targetPlayer;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.O)){
            // _monsterAttack.AttackSpecific(2);
        }
    }

    void CheckIsPlayerInRange(){
        if(!_monsterMove.goToTarget){return;}

        if(_monsterMove.isPlayerInRange){
            _monsterAttack.Attack();
        }
        else{
            // if(!isChargeAttackReady && !_monsterAttack.isCanAttack){return;}

            // float randChargeSkillAttack = Random.value;

            // if(randChargeSkillAttack >= 0.7f){
            //     isChargeAttackReady = false;
            //     _monsterAttack.AttackSpecific(2);
            //     Invoke("DelayChargeAttack",10f);
            // }
        }
    }

    void DelayChargeAttack(){
        isChargeAttackReady = true;
    }
}

