using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class Assasin_Attacker : MonoBehaviourPunCallbacks
{
    public int[] attackIndex;
    int attackIndexTemp, previousAttackIndex;
    public bool isCanAttack{get; private set;}
    bool isAttackSpecificInUse;
    public float NormalAttackRange;

    Monster_Animation monsterAnima;
    Monster_Hopping monsterHopping;
    Monster_Stat monsterStat;

    Sequence AttackSqeuence;

    public Collider _collider;

    float disBetweenEnemyAndPlayer;

    private void Awake() {
        monsterAnima = GetComponent<Monster_Animation>();
        monsterHopping = GetComponent<Monster_Hopping>();
        monsterStat = GetComponent<Monster_Stat>();
    }

    public override void OnEnable() {
        base.OnEnable();

        if(!PhotonNetwork.IsMasterClient){return;}

        isCanAttack = true;
    }

    private void Update() {
        if(!PhotonNetwork.IsMasterClient){return;}

        if(Input.GetKeyDown(KeyCode.O)){
            // Attack();
        }

        if(!monsterHopping.goToTarget){return;}

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
        base.photonView.RPC("RPC_ToggleRigiBody",RpcTarget.All,false);
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
        base.photonView.RPC("RPC_ToggleRigiBody",RpcTarget.All,false);
        Debug.Log(monsterAnima.GetCurrentAnimationTime() * 1.5f);
        AttackSqeuence = DOTween.Sequence();
        AttackSqeuence.AppendInterval(monsterAnima.GetCurrentAnimationTime() * 1.5f);
        AttackSqeuence.AppendCallback(DelayAttacker);
    }

    [PunRPC]
    void RPC_ToggleRigiBody(bool value){
        monsterHopping.rb.isKinematic = value;
        _collider.enabled = !value;
    }

    string AnimationName;
    void StopAttackerAnimationTween(){
        monsterAnima.PlayBoolAnimator(AnimationName,false);
    }

    public void DelayAttacker(){
        isCanAttack = true;
        isAttackSpecificInUse = false;
    }

    IEnumerator Jumping(){
        base.photonView.RPC("RPC_ToggleRigiBody",RpcTarget.All,true);
        yield return new WaitForSeconds(7.5f);

        while(!monsterHopping.FindDropPoint(0f,0.5f)){
            yield return null;
        }
        // yield return new WaitForSeconds(1f);

        monsterAnima.PlayBoolAnimator("IsTeleportDown",true);
        yield return new WaitForSeconds(0.1f);

        monsterAnima.PlayBoolAnimator("IsTeleportDown",false);
        yield return new WaitForSeconds(0.1f);
        base.photonView.RPC("RPC_ToggleRigiBody",RpcTarget.All,false);

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
