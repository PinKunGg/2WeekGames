using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class Boar_Attacker : MonoBehaviour
{
    public int[] attackIndex;
    int attackIndexTemp, previousAttackIndex;
    public bool isCanAttack{get; private set;}
    bool isAttackSpecificInUse;
    public float NormalAttackRange;

    Monster_Animation monsterAnima;
    Monster_Movement monsterMove;
    Monster_Stat monsterStat;

    Sequence AttackSqeuence, ChargeAttackContinueSqeuence;

    float disBetweenEnemyAndPlayer;

    private void Awake() {
        monsterAnima = GetComponent<Monster_Animation>();
        monsterMove = GetComponent<Monster_Movement>();
        monsterStat = GetComponent<Monster_Stat>();
    }
    private void OnEnable() {
        if(!PhotonNetwork.IsMasterClient){return;}

        isCanAttack = true;
    }

    private void Update() {
        if(!PhotonNetwork.IsMasterClient){return;}
        
        if(Input.GetKeyDown(KeyCode.O)){
            // Attack();
        }

        if(!monsterMove.goToTarget){return;}

        if(monsterStat.IsDie){
            if(AttackSqeuence.IsPlaying()){
                AttackSqeuence.Kill();
            }
            
            monsterAnima.PlayBoolAnimator("IsNormalAttack",false);
            monsterAnima.PlayBoolAnimator("IsSkill1",false);
            monsterAnima.PlayBoolAnimator("IsSkill2",false);
            monsterAnima.PlayBoolAnimator("IsAttackFinish",false);
            monsterAnima.PlayBoolAnimator("IsRun",false);
            return;
        }
        
        disBetweenEnemyAndPlayer = Vector3.Distance(this.transform.position, monsterMove.goToTarget.transform.localPosition);
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
        if(isAttackSpecificInUse){return;}

        attackIndexTemp = value;

        isAttackSpecificInUse = true;
        isCanAttack = false;

        monsterMove.isStopWalk = true;

        AttackSqeuence = DOTween.Sequence();

        monsterAnima.PlayBoolAnimator("IsAttackFinish",false);

        switch(value){
            case 0:
            if(disBetweenEnemyAndPlayer > NormalAttackRange){
                SpinAttack();
                break;
            }

            NormalAttack();
            break;

            case 1:
            SpinAttack();
            break;

            case 2:
            ChargeAttack();
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
        AttackSqeuence.AppendInterval(0.5f);
        AttackSqeuence.AppendCallback(DelayCaculate);
    }

    void SpinAttack(){
        monsterAnima.PlayBoolAnimator("IsSkill1",true);

        AnimationName = "IsSkill1";
        AttackSqeuence.AppendInterval(0.3f);
        AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
        AttackSqeuence.AppendInterval(0.5f);
        AttackSqeuence.AppendCallback(DelayCaculate);
    }

    void ChargeAttack(){
        monsterAnima.PlayBoolAnimator("IsRun",true);
        AnimationName = "IsRun";
        AttackSqeuence.Append(monsterMove.rb.DOMove(this.transform.localPosition + (this.transform.forward * -3f),1f));
        AttackSqeuence.AppendInterval(0.2f);
        AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
        AttackSqeuence.AppendInterval(0.3f);
        AnimationName = "IsSkill2";
        AttackSqeuence.AppendInterval(0.2f);
        AttackSqeuence.AppendCallback(PlayAttackerAnimationTween);
        AttackSqeuence.AppendInterval(0.2f);
        AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
        AttackSqeuence.AppendInterval(0.6f);
        AttackSqeuence.AppendCallback(ChargeAttackContinute);
    }

    void ChargeAttackContinute(){
        ChargeAttackContinueSqeuence = DOTween.Sequence();
        ChargeAttackContinueSqeuence.AppendCallback(ChargeAttackSetTargetToNull);
        ChargeAttackContinueSqeuence.AppendInterval(0.1f);
        ChargeAttackContinueSqeuence.Append(monsterMove.rb.DOMove(this.transform.localPosition + (this.transform.forward * 20f),1f));
        ChargeAttackContinueSqeuence.AppendInterval(0.3f);
        ChargeAttackContinueSqeuence.AppendCallback(ChargeAttackFinish);
    }

    void DelayCaculate(){
        Debug.Log(monsterAnima.GetCurrentAnimationTime() * 2.5f);
        AttackSqeuence = DOTween.Sequence();
        AttackSqeuence.AppendInterval(monsterAnima.GetCurrentAnimationTime() * 2.5f);
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

    void PlayAttackerAnimationTween(){
        monsterAnima.PlayBoolAnimator(AnimationName,true);
    }

    public void DelayAttacker(){
        isCanAttack = true;
        isAttackSpecificInUse = false;

        if(!monsterMove){monsterMove = GetComponent<Monster_Movement>();}
        monsterMove.isStopWalk = false;
    }
    void ChargeAttackSetTargetToNull(){
        monsterMove.lookAtTarget = null;
    }
    void ChargeAttackFinish(){
        ChargeAttackContinueSqeuence.Kill();
        monsterMove.rb.velocity = Vector3.zero;

        monsterMove.rb.DOMove(this.transform.localPosition + (this.transform.forward * -2f),1f);

        monsterAnima.PlayBoolAnimator("IsSkill2",false);
        monsterAnima.PlayBoolAnimator("IsAttackFinish",true);

        monsterMove.lookAtTarget = monsterMove.goToTarget;
        monsterMove.isStopWalk = false;
        Invoke("DelayAttacker",3f);
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log(other.gameObject.tag);

        if(!other.gameObject.CompareTag("Ground")){
            if(attackIndexTemp == 2){
                ChargeAttackFinish();   
            }
        }
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, NormalAttackRange);
    }
}
