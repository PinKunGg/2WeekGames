using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class Monster_Attacker : MonoBehaviour
{
    public int[] attackIndex;
    int attackIndexTemp;
    public bool isCanAttack{get; private set;}
    bool isChargeAttackDone;

    Monster_Animation _monsterAnima;
    Monster_Movement _monsterMove;

    Sequence AttackSqeuence;

    float disBetweenEnemyAndPlayer;

    private void Start() {
        _monsterAnima = GetComponent<Monster_Animation>();
        _monsterMove = GetComponent<Monster_Movement>();

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
        _monsterMove.isStopWalk = true;

        AttackSqeuence = DOTween.Sequence();

        _monsterAnima.PlayBoolAnimator("IsAttackFinish",false);

        switch(value){
            case 0:
            _monsterAnima.PlayBoolAnimator("IsNormalAttack",true);
            
            AnimationName = "IsNormalAttack";
            AttackSqeuence.AppendInterval(0.5f);
            AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
            DelayCaculate();
            break;

            case 1:
            _monsterAnima.PlayBoolAnimator("IsSkill1",true);

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
            _monsterAnima.PlayBoolAnimator("IsNormalAttack",true);
            
            AnimationName = "IsNormalAttack";
            AttackSqeuence.AppendInterval(0.3f);
            AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
            DelayCaculate();
            break;
        }
    }

    void DelayCaculate(){
        AttackSqeuence = DOTween.Sequence();
        AttackSqeuence.AppendInterval(_monsterAnima.anima.GetCurrentAnimatorStateInfo(0).length / 2f);
        AttackSqeuence.AppendCallback(DelayAttacker);
    }

    IEnumerator StopAttackerAnimation(string name, float time){
        yield return new WaitForSeconds(time);
        _monsterAnima.PlayBoolAnimator(name,false);
    }

    string AnimationName;
    void StopAttackerAnimationTween(){
        _monsterAnima.PlayBoolAnimator(AnimationName,false);
    }

    void DelayAttacker(){
        isCanAttack = true;
        _monsterMove.isStopWalk = false;
    }
    void DelayRetreat(){
        _monsterMove.isStopWalk = false;
        Invoke("DelayAttacker",2f);
    }

    IEnumerator ChargeAttack(){
        _monsterAnima.PlayBoolAnimator("IsSkill2",true);
        yield return new WaitForSeconds(1f);

        _monsterAnima.PlayBoolAnimator("IsSkill2",false);
        _monsterMove.lookAtTarget = null;

        while(!isChargeAttackDone){
            _monsterMove.rb.drag = 20f;
            _monsterMove.rb.angularDrag = 20f;
            _monsterMove.rb.velocity = transform.forward * 30f;
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
