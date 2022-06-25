using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class Assasin_Attacker : MonoBehaviour
{
    public int[] attackIndex;
    public bool isCanAttack{get; private set;}
    bool isAttackSpecificInUse;
    bool isChargeAttackDone;
    public float NormalAttackRange;

    Monster_Animation monsterAnima;
    Monster_Hopping monsterHopping;

    Sequence AttackSqeuence;

    float disBetweenEnemyAndPlayer;

    private void Start() {
        monsterAnima = GetComponent<Monster_Animation>();
        monsterHopping = GetComponent<Monster_Hopping>();

        if(PhotonNetwork.IsMasterClient){
            isCanAttack = true;
        }

        isCanAttack = true;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.O)){
            // Attack();
        }

        if(!monsterHopping.goToTarget){return;}

        disBetweenEnemyAndPlayer = Vector3.Distance(this.transform.position, monsterHopping.goToTarget.transform.localPosition);
    }

    public void Attack(){
        if(!isCanAttack){return;}

        isCanAttack = false;

        monsterAnima.PlayBoolAnimator("IsAttackFinish",false);
        CancelInvoke();

        AttackSpecific(Random.Range(0,attackIndex.Length));
    }

    public void AttackSpecific(int value){
        if(isAttackSpecificInUse){return;}
        isAttackSpecificInUse = true;
        isCanAttack = false;

        AttackSqeuence = DOTween.Sequence();

        monsterAnima.PlayBoolAnimator("IsAttackFinish",false);

        switch(value){
            case 0:
            if(disBetweenEnemyAndPlayer > NormalAttackRange){
                monsterAnima.PlayBoolAnimator("IsSkill1",true);

                AnimationName = "IsSkill1";
                AttackSqeuence.AppendInterval(0.3f);
                AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
                DelayCaculate();
                break;
            }

            monsterAnima.PlayBoolAnimator("IsNormalAttack",true);
            
            AnimationName = "IsNormalAttack";
            AttackSqeuence.AppendInterval(0.5f);
            AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
            DelayCaculate();
            break;

            case 1:
            monsterAnima.PlayBoolAnimator("IsSkill1",true);

            AnimationName = "IsSkill1";
            AttackSqeuence.AppendInterval(0.3f);
            AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
            DelayCaculate();
            break;

            case 2:
            monsterAnima.PlayBoolAnimator("IsSkill2",true);

            AnimationName = "IsSkill2";
            AttackSqeuence.AppendInterval(0.3f);
            AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
            DelayCaculate();
            break;

            case 3:
            monsterAnima.PlayBoolAnimator("IsJumpUp",true);

            monsterHopping.SpawnShadowHopping();
            AnimationName = "IsJumpUp";
            AttackSqeuence.AppendInterval(0.3f);
            AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
            StartCoroutine(Jumping());
            break;

            default:
            monsterAnima.PlayBoolAnimator("IsNormalAttack",true);
            
            AnimationName = "IsNormalAttack";
            AttackSqeuence.AppendInterval(0.3f);
            AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
            DelayCaculate();
            break;
        }
    }

    void DelayCaculate(){
        monsterHopping.rb.isKinematic = false;
        Debug.Log(monsterAnima.GetCurrentAnimationTime() * 0.9f);
        AttackSqeuence = DOTween.Sequence();
        AttackSqeuence.AppendInterval(monsterAnima.GetCurrentAnimationTime() * 0.9f);
        AttackSqeuence.AppendCallback(DelayAttacker);
    }

    string AnimationName;
    void StopAttackerAnimationTween(){
        monsterAnima.PlayBoolAnimator(AnimationName,false);
    }

    void DelayAttacker(){
        isCanAttack = true;
        isAttackSpecificInUse = false;
    }

    IEnumerator Jumping(){
        monsterHopping.rb.isKinematic = true;
        yield return new WaitForSeconds(0.7f);
        transform.DOMoveY(this.transform.position.y + 5f,0.5f);
        yield return new WaitForSeconds(2f);
        monsterHopping.FindDropPoint();
        yield return new WaitForSeconds(1f);

        transform.DOMoveY(this.transform.position.y - 5f,0.5f);
        yield return new WaitForSeconds(0.2f);
        monsterAnima.PlayBoolAnimator("IsJumpDown",true);
        DelayCaculate();
        yield return new WaitForSeconds(0.3f);
        monsterAnima.PlayBoolAnimator("IsJumpDown",false);
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, NormalAttackRange);
    }
}
