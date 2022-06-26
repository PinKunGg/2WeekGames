using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class BoarAttacker : MonoBehaviour
{
    public int[] attackIndex;
    int attackIndexTemp, previousAttackIndex;
    public bool isCanAttack{get; private set;}
    bool isChargeAttackDone;

    Monster_Animation monsterAnima;
    Monster_Movement monsterMove;

    Sequence AttackSqeuence;

    float disBetweenEnemyAndPlayer;

    private void Start() {
        monsterAnima = GetComponent<Monster_Animation>();
        monsterMove = GetComponent<Monster_Movement>();

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

        if(attackIndexTemp == previousAttackIndex){
            attackIndexTemp++;
            previousAttackIndex = attackIndexTemp;
        }else{
            previousAttackIndex = attackIndexTemp;
        }

        monsterAnima.PlayBoolAnimator("IsAttackFinish",false);
        CancelInvoke();

        AttackSpecific(attackIndexTemp);
    }

    public void AttackSpecific(int value){
        attackIndexTemp = value;

        isCanAttack = false;
        monsterMove.isStopWalk = true;

        AttackSqeuence = DOTween.Sequence();

        monsterAnima.PlayBoolAnimator("IsAttackFinish",false);

        switch(value){
            case 0:
            NormalAttack();
            break;

            case 1:
            monsterAnima.PlayBoolAnimator("IsSkill1",true);

            AnimationName = "IsSkill1";
            AttackSqeuence.AppendInterval(0.3f);
            AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
            DelayCaculate();
            break;

            case 2:
            isChargeAttackDone = false;
            Invoke("ChargeAttackFinish",4f);
            StartCoroutine(ChargeAttack());
            break;

            default:
            NormalAttack();
            break;
        }
    }

    void NormalAttack(){
        monsterAnima.PlayBoolAnimator("IsNormalAttack",true);
            
        AnimationName = "IsNormalAttack";
        AttackSqeuence.AppendInterval(0.3f);
        AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
        DelayCaculate();
    }

    void DelayCaculate(){
        AttackSqeuence = DOTween.Sequence();
        AttackSqeuence.AppendInterval(monsterAnima.anima.GetCurrentAnimatorStateInfo(0).length / 2f);
        AttackSqeuence.AppendCallback(DelayAttacker);
    }

    IEnumerator StopAttackerAnimation(string name, float time){
        yield return new WaitForSeconds(time);
        monsterAnima.PlayBoolAnimator(name,false);
    }

    string AnimationName;
    void StopAttackerAnimationTween(){
        monsterAnima.PlayBoolAnimator(AnimationName,false);
    }

    void DelayAttacker(){
        isCanAttack = true;
        monsterMove.isStopWalk = false;
    }

    IEnumerator ChargeAttack(){
        monsterAnima.PlayBoolAnimator("IsSkill2",true);
        yield return new WaitForSeconds(1f);

        monsterAnima.PlayBoolAnimator("IsSkill2",false);
        monsterMove.lookAtTarget = null;

        while(!isChargeAttackDone){
            monsterMove.rb.drag = 20f;
            monsterMove.rb.angularDrag = 20f;
            monsterMove.rb.velocity = transform.forward * 30f;
            yield return null;
        }
    }

    void ChargeAttackFinish(){
        isChargeAttackDone = true;
        StopCoroutine(ChargeAttack());

        monsterMove.rb.velocity = Vector3.zero;
        monsterMove.rb.drag = 1f;
        monsterMove.rb.angularDrag = 1f;

        monsterAnima.PlayBoolAnimator("IsSkill2",false);
        monsterAnima.PlayBoolAnimator("IsAttackFinish",true);

        monsterMove.lookAtTarget = monsterMove.goToTarget;
        monsterMove.isStopWalk = false;
        Invoke("DelayAttacker",1f);
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
