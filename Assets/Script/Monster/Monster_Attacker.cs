using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Attacker : MonoBehaviour
{
    public int[] attackIndex;
    public float[] delayBetweenNextAttack;
    int attackIndexTemp;
    bool isCanAttack = true;

    Monster_Animation _monsterAnima;
    Monster_Movement _monsterMove;
    Monster_Retreat _monsterRetreat;

    float disBetweenEnemyAndPlayer;

    private void Start() {
        _monsterAnima = GetComponent<Monster_Animation>();
        _monsterMove = GetComponent<Monster_Movement>();
        _monsterRetreat = GetComponent<Monster_Retreat>();
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
            StartCoroutine(ChargeAttack());
            Invoke("ChargeAttackFinish",4f);
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
        while(!isCanAttack){
            _monsterMove.rb.drag = 20f;
            _monsterMove.rb.angularDrag = 20f;
            _monsterMove.rb.velocity = transform.forward * 50f;
            yield return null;
        }
    }

    void ChargeAttackFinish(){
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
