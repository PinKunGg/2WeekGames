using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ArachneAI : MonoBehaviourPunCallbacks
{
    Monster_Hopping monsterHopping;
    Arachne_Attacker arachneAttack;
    PlayerManager_Multiplayer playerManMulti;
    public Monster_Stat monsterStat {get;private set;}
    public Transform Skill1SpawnPos;
    Transform targetPlayer;

    bool isChargeAttackReady = true;

    private void Awake() {
        monsterHopping = GetComponent<Monster_Hopping>();
        arachneAttack = GetComponent<Arachne_Attacker>();
        playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
        monsterStat = GetComponent<Monster_Stat>();
    }
    
    public override void OnEnable()
    {
        if (!PhotonNetwork.IsMasterClient){return;}

        DelayStart();
    }
    void DelayStart(){
        GetPlayerTarget();
        InvokeRepeating("CheckIsPlayerInRange",0.5f,1f);
        InvokeRepeating("CheckToChangePlayerTarget",10f,10f);
    }

    void GetPlayerTarget(){
        targetPlayer = playerManMulti.GetRandomPlayer().transform;
        monsterHopping.goToTarget = targetPlayer;
        monsterHopping.lookAtTarget = targetPlayer;
    }

    private void Update() {
        try{
            Vector3 newLookAt = new Vector3(targetPlayer.position.x,targetPlayer.position.y + 1f,targetPlayer.position.z);

            base.photonView.RPC("RPC_RotateSkill1SpawnPos",RpcTarget.All,newLookAt);
        }
        catch{}

        if(!PhotonNetwork.IsMasterClient){return;}

        if(Input.GetKeyDown(KeyCode.O)){
            // arachneAttack.AttackSpecific(2);
            // arachneAttack.Attack();
        }
    }

    [PunRPC]
    void RPC_RotateSkill1SpawnPos(Vector3 rot){
        Skill1SpawnPos.transform.LookAt(rot);
    }

    void CheckIsPlayerInRange(){
        if(!monsterHopping.goToTarget){return;}

        if(monsterStat.IsDie){
            CancelInvoke();
            return;
        }

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

        targetPlayer = playerManMulti._allPlayerInCurrentRoom[playerHightestDamage]._playerGameObject.transform;
        monsterHopping.goToTarget = targetPlayer;
        monsterHopping.lookAtTarget = targetPlayer;
    }

    void DelayChargeAttack(){
        isChargeAttackReady = true;
    }
}

