using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Monster_Attacker : MonoBehaviour
{
    public int[] attackIndex;
    public float[] delayBetweenNextAttack;
    int attackIndexTemp;
    public bool isCanAttack{get; private set;}
    bool isChargeAttackDone;

    Monster_Animation _monsterAnima;
    Monster_Movement _monsterMove;
    Monster_Retreat _monsterRetreat;

    float disBetweenEnemyAndPlayer;

    private void Start() {
        _monsterAnima = GetComponent<Monster_Animation>();
        _monsterMove = GetComponent<Monster_Movement>();
        _monsterRetreat = GetComponent<Monster_Retreat>();

        if(PhotonNetwork.IsMasterClient){
            isCanAttack = true;
        }

        // isCanAttack = true;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.O)){
            // Attack();
        }
    }

    public void Attack(){
        if(!isCanAttack){return;}

        isCanAttack = false;

        attackIndexTemp = Random.Range(0,attackIndex.Length);

        _monsterAnima.PlayBoolAnimator("IsAttackFinish",false);
        CancelInvoke();

        AttackSpecific(attackIndexTemp);
    }

    public void AttackSpecific(int value){
        attackIndexTemp = value;

        isCanAttack = false;

        switch(value){
            case 0:
            _monsterAnima.PlayBoolAnimator("IsNormalAttack",true);
            StartCoroutine(StopAttackerAnimation("IsNormalAttack",0.5f));
            Invoke("DelayAttacker",delayBetweenNextAttack[0]);
            break;
            case 1:
            _monsterAnima.PlayBoolAnimator("IsSkill1",true);
            StartCoroutine(StopAttackerAnimation("IsSkill1",0.5f));
            Invoke("DelayAttacker",delayBetweenNextAttack[1]);
            break;
            case 2:
            isChargeAttackDone = false;
            StartCoroutine(ChargeAttack());
            Invoke("ChargeAttackFinish",delayBetweenNextAttack[2]);
            break;
            case 3:
            _monsterMove.isStopWalk = true;
            _monsterRetreat.enabled = true;
            Invoke("DelayRetreat",delayBetweenNextAttack[3]);
            break;
        }
    }

    IEnumerator StopAttackerAnimation(string name, float time){
        yield return new WaitForSeconds(time);
        _monsterAnima.PlayBoolAnimator(name,false);
    }

    void DelayAttacker(){
        isCanAttack = true;
    }
    void DelayRetreat(){
        _monsterRetreat.enabled = false;
        _monsterMove.isStopWalk = false;
        Invoke("DelayAttacker",2f);
    }

    IEnumerator ChargeAttack(){
        _monsterMove.isStopWalk = true;
        _monsterRetreat.enabled = true;
        yield return new WaitForSeconds(1f);
        _monsterRetreat.enabled = false;
        yield return new WaitForSeconds(0.5f);
        _monsterAnima.PlayBoolAnimator("IsSkill2",true);
        yield return new WaitForSeconds(1f);
        _monsterAnima.PlayBoolAnimator("IsSkill2",false);
        _monsterMove.lookAtTarget = null;

        while(!isChargeAttackDone){
            _monsterMove.rb.drag = 20f;
            _monsterMove.rb.angularDrag = 20f;
            _monsterMove.rb.velocity = transform.forward * 50f;
            yield return null;
        }
    }

    void ChargeAttackFinish(){
        isChargeAttackDone = true;
        StopCoroutine(ChargeAttack());

        _monsterMove.rb.velocity = Vector3.zero;
        _monsterMove.rb.drag = 1f;
        _monsterMove.rb.angularDrag = 1f;

        _monsterAnima.PlayBoolAnimator("IsSkill2",false);
        _monsterAnima.PlayBoolAnimator("IsAttackFinish",true);

        _monsterMove.lookAtTarget = _monsterMove.goToTarget;
        _monsterMove.isStopWalk = false;
        Invoke("DelayAttacker",2f);
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log(other.gameObject.tag);

        if(other.gameObject.CompareTag("Player")){
            if(attackIndexTemp == 2){
                ChargeAttackFinish();   
            }
        }
    }
}
