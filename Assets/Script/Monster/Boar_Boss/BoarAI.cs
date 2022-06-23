using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BoarAI : MonoBehaviourPunCallbacks
{
    Monster_Movement _monsterMove;
    Monster_Attacker _monsterAttack;

    PlayerManager_Multiplayer _playerManMulti;

    bool isChargeAttackReady = true;

    private void Start() {
        _monsterMove = GetComponent<Monster_Movement>();
        _monsterAttack = GetComponent<Monster_Attacker>();
        _playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
        
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
        Transform targetPlayer = _playerManMulti.GetRandomPlayer().transform;
        _monsterMove.goToTarget = targetPlayer;
        _monsterMove.lookAtTarget = targetPlayer;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.O)){
            _monsterAttack.AttackSpecific(2);
        }
    }

    void CheckIsPlayerInRange(){
        if(!_monsterMove.goToTarget){return;}

        if(_monsterMove.isPlayerInRange){
            _monsterAttack.Attack();
        }
    }

    void CheckToChangePlayerTarget(){
        float hightestDamage = 0;
        int playerHightestDamage = 0;

        for(int i = 0; i < _playerManMulti._allPlayerInCurrentRoom.Count; i++){
            if(_playerManMulti._allPlayerInCurrentRoom[i]._playerDamageDealToBoss > hightestDamage){
                hightestDamage = _playerManMulti._allPlayerInCurrentRoom[i]._playerDamageDealToBoss;
                playerHightestDamage = i;
            }
        }

        Transform targetPlayer = _playerManMulti._allPlayerInCurrentRoom[playerHightestDamage]._playerGameObject.transform;
        _monsterMove.goToTarget = targetPlayer;
        _monsterMove.lookAtTarget = targetPlayer;
    }

    void DelayChargeAttack(){
        isChargeAttackReady = true;
    }
}

