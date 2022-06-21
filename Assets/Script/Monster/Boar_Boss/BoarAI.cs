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

    private void Start() {
        _monsterMove = GetComponent<Monster_Movement>();
        _monsterAttack = GetComponent<Monster_Attacker>();
        _monsterRetreat = GetComponent<Monster_Retreat>();
        
        if(PhotonNetwork.IsMasterClient){
            InvokeRepeating("CheckIsPlayerInRange",0f,1f);
        }

        InvokeRepeating("CheckIsPlayerInRange",0f,1f);
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
            float randChargeSkillAttack = Random.value;

            if(randChargeSkillAttack >= 0.7f){
                _monsterAttack.AttackSpecific(2);
            }
        }
    }
}

