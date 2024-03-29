using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class Arachne_Attacker : MonoBehaviourPunCallbacks
{
    public int[] attackIndex;
    int attackIndexTemp, previousAttackIndex;
    public bool isCanAttack{get; private set;}
    bool isAttackSpecificInUse;
    bool isChargeAttackDone;
    public float NormalAttackRange;

    Monster_Animation monsterAnima;
    Monster_Hopping monsterHopping;
    Monster_Stat monsterStat;

    Sequence AttackSqeuence;

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
            monsterAnima.PlayBoolAnimator("IsJumpUp",false);
            monsterAnima.PlayBoolAnimator("IsJumpDown",false);
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

    public void AttackSpecific(int value){
        if(isAttackSpecificInUse){return;}

        isAttackSpecificInUse = true;
        isCanAttack = false;

        AttackSqeuence = DOTween.Sequence();

        monsterAnima.PlayBoolAnimator("IsAttackFinish",false);

        switch(value){
            case 0:
            if(disBetweenEnemyAndPlayer > NormalAttackRange){
                ShootVemon();
                break;
            }

            NormalAttack();
            break;

            case 1:
            ShootVemon();
            break;

            case 2:
            monsterAnima.PlayBoolAnimator("IsSkill2",true);

            AnimationName = "IsSkill2";
            AttackSqeuence.AppendInterval(0.3f);
            AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
            AttackSqeuence.AppendInterval(0.5f);
            AttackSqeuence.AppendCallback(DelayCaculate);
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

    void ShootVemon(){
        monsterAnima.PlayBoolAnimator("IsSkill1",true);

        AnimationName = "IsSkill1";
        AttackSqeuence.AppendInterval(0.3f);
        AttackSqeuence.AppendCallback(StopAttackerAnimationTween);
        AttackSqeuence.AppendInterval(0.5f);
        AttackSqeuence.AppendCallback(DelayCaculate);   
    }

    void DelayCaculate(){
        base.photonView.RPC("RPC_ToggleRigiBody",RpcTarget.All,false);
        Debug.Log(monsterAnima.GetCurrentAnimationTime() * 4f);
        AttackSqeuence = DOTween.Sequence();
        AttackSqeuence.AppendInterval(monsterAnima.GetCurrentAnimationTime() * 4f);
        AttackSqeuence.AppendCallback(DelayAttacker);
    }

    [PunRPC]
    void RPC_ToggleRigiBody(bool value){
        monsterHopping.rb.isKinematic = value;
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
        yield return new WaitForSeconds(0.7f);
        transform.DOMoveY(this.transform.position.y + 15f,0.5f);
        yield return new WaitForSeconds(2f);
        
        while(!monsterHopping.FindDropPoint(-1f,2f)){
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        transform.DOMoveY(this.transform.position.y - 15f,0.5f);
        yield return new WaitForSeconds(0.2f);
        monsterAnima.PlayBoolAnimator("IsJumpDown",true);
        yield return new WaitForSeconds(0.5f);
        DelayCaculate();
        yield return new WaitForSeconds(0.3f);
        monsterAnima.PlayBoolAnimator("IsJumpDown",false);
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, NormalAttackRange);
    }
}
