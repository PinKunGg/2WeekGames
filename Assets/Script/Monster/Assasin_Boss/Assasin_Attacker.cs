using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class Assasin_Attacker : MonoBehaviour
{
    public int[] attackIndex;
    int attackIndexTemp, previousAttackIndex;
    public bool isCanAttack{get; private set;}
    bool isAttackSpecificInUse;
    bool isChargeAttackDone;
    public float NormalAttackRange;

    Monster_Animation monsterAnima;
    Monster_Hopping monsterHopping;

    Sequence AttackSqeuence;

    public Collider _collider;

    float disBetweenEnemyAndPlayer;

    private void Start() {
        monsterAnima = GetComponent<Monster_Animation>();
        monsterHopping = GetComponent<Monster_Hopping>();

        if(PhotonNetwork.IsMasterClient){
            isCanAttack = true;
        }

        // isCanAttack = true;
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

    int AttackSpecificIndex;

    public void AttackSpecific(int value){
        if(isAttackSpecificInUse){return;}

        AttackSpecificIndex = value;

        isAttackSpecificInUse = true;
        isCanAttack = false;

        AttackSqeuence = DOTween.Sequence();

        monsterAnima.PlayBoolAnimator("IsAttackFinish",false);

        switch(value){
            case 0:
            if(disBetweenEnemyAndPlayer > NormalAttackRange){
                ShootDarkEnergy();
                break;
            }

            NormalAttack();
            break;

            case 1:
            ShootDarkEnergy();
            break;

            case 2:
            monsterAnima.PlayBoolAnimator("IsSkill2",true);

            AnimationName = "IsSkill2";
            AttackSqeuence.AppendInterval(0.3f);
            AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
            AttackSqeuence.AppendInterval(4f);
            AttackSqeuence.AppendCallback(DelayCaculate);
            break;

            case 3:
            if(disBetweenEnemyAndPlayer <= NormalAttackRange){
                NormalAttack();
                break;
            }
            TeleportToPlayer();
            break;

            default:
            monsterAnima.PlayBoolAnimator("IsNormalAttack",true);
            
            AnimationName = "IsNormalAttack";
            AttackSqeuence.AppendInterval(0.3f);
            AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
            AttackSqeuence.AppendInterval(0.5f);
            AttackSqeuence.AppendCallback(DelayCaculate);
            break;
        }
    }

    void TeleportToPlayer(){
        _collider.enabled = false;
        monsterHopping.rb.isKinematic = true;
        monsterAnima.PlayBoolAnimator("IsTeleportUp",true);

        monsterHopping.SpawnShadowHopping();
        AnimationName = "IsTeleportUp";

        AttackSqeuence = DOTween.Sequence();
        AttackSqeuence.AppendInterval(0.3f);
        AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
        StartCoroutine(Jumping());
    }
    void NormalAttack(){
        monsterAnima.PlayBoolAnimator("IsNormalAttack",true);
            
        AnimationName = "IsNormalAttack";

        AttackSqeuence = DOTween.Sequence();
        AttackSqeuence.AppendInterval(0.5f);
        AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
        AttackSqeuence.AppendInterval(0.5f);
        AttackSqeuence.AppendCallback(DelayCaculate);
    }

    void ShootDarkEnergy(){
        monsterAnima.PlayBoolAnimator("IsSkill1",true);

        AnimationName = "IsSkill1";
        AttackSqeuence.AppendInterval(0.3f);
        AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
        AttackSqeuence.AppendInterval(0.5f);
        AttackSqeuence.AppendCallback(DelayCaculate);
    }

    void DelayCaculate(){
        monsterHopping.rb.isKinematic = false;
        Debug.Log(monsterAnima.GetCurrentAnimationTime() * 1.5f);
        AttackSqeuence = DOTween.Sequence();
        AttackSqeuence.AppendInterval(monsterAnima.GetCurrentAnimationTime() * 1.5f);
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
        yield return new WaitForSeconds(7.5f);

        monsterHopping.FindDropPoint(0f,0.5f);
        // yield return new WaitForSeconds(1f);

        monsterAnima.PlayBoolAnimator("IsTeleportDown",true);
        yield return new WaitForSeconds(0.1f);

        monsterAnima.PlayBoolAnimator("IsTeleportDown",false);
        yield return new WaitForSeconds(0.1f);
        _collider.enabled = true;
        monsterHopping.rb.isKinematic = false;

        monsterAnima.PlayBoolAnimator("IsNormalAttack",true);
        
        AnimationName = "IsNormalAttack";

        yield return new WaitForSeconds(0.5f);
        StopAttackerAnimationTween();
        yield return new WaitForSeconds(0.5f);
        DelayCaculate();
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, NormalAttackRange);
    }
}
