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

    private void Start() {
        _monsterAnima = GetComponent<Monster_Animation>();
        _monsterMove = GetComponent<Monster_Movement>();
    }

    private void Update() {
        if(!isCanAttack){return;}
        if(Input.GetKeyDown(KeyCode.O)){
            Attack();
        }
    }

    public void Attack(){
        isCanAttack = false;

        attackIndexTemp = Random.Range(0,attackIndex.Length);

        _monsterAnima.PlayBoolAnimator("IsAttackFinish",false);

        switch(attackIndexTemp){
            case 0:
            _monsterAnima.PlayBoolAnimator("IsNormalAttack",true);
            StartCoroutine(StopAttackerAnimation("IsNormalAttack",0.5f));
            CancelInvoke("DelayAttacker");
            Invoke("DelayAttacker",delayBetweenNextAttack[0]);
            break;
            case 1:
            _monsterAnima.PlayBoolAnimator("IsSkill1",true);
            StartCoroutine(StopAttackerAnimation("IsSkill1",0.5f));
            CancelInvoke("DelayAttacker");
            Invoke("DelayAttacker",delayBetweenNextAttack[1]);
            break;
            case 2:
            StartCoroutine(ChargeAttack());
            CancelInvoke("DelayAttacker");
            Invoke("ChargeAttackFinish",3f);
            _monsterAnima.PlayBoolAnimator("IsSkill2",true);
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
        yield return new WaitForSeconds(0.5f);
        _monsterAnima.PlayBoolAnimator("IsSkill2",false);
        yield return new WaitForSeconds(2f);
        while(!isCanAttack){
            _monsterMove.rb.drag = 50f;
            _monsterMove.rb.angularDrag = 50f;
            _monsterMove.rb.velocity = transform.forward * 50f;
            yield return null;
        }
    }

    void ChargeAttackFinish(){
        isCanAttack = true;
        _monsterMove.rb.drag = 1f;
        _monsterMove.rb.angularDrag = 1f;
        _monsterAnima.PlayBoolAnimator("IsSkill2",false);
        _monsterAnima.PlayBoolAnimator("IsAttackFinish",true);
        _monsterMove.isStopWalk = false;
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
