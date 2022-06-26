using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class Monster_Hopping : MonoBehaviour
{
    public SpriteRenderer HoppingShadow;
    public float StopDis = 7f;
    public bool isPlayerInRange;
    // public bool isPlayerInRange {get;private set;}

    public float disBetweenEnemyAndPlayer {get;private set;}

    public Transform goToTarget;
    public Transform lookAtTarget;

    public Bounds HoppingBounds;

    public Rigidbody rb {get;private set;}

    private void OnEnable() {
        rb = GetComponent<Rigidbody>();
        if(PhotonNetwork.IsMasterClient){
            HoppingShadow.enabled = false;
        }

        HoppingShadow.enabled = false;
    }

    private void Update() {
        if(!goToTarget){return;}

        disBetweenEnemyAndPlayer = Vector3.Distance(this.transform.position, goToTarget.transform.localPosition);

        if (disBetweenEnemyAndPlayer > StopDis){
            isPlayerInRange = false;
        }
        else{
            isPlayerInRange = true;
        }

        if(!lookAtTarget){return;}

        Vector3 targetPos = new Vector3(lookAtTarget.localPosition.x,this.transform.localPosition.y,lookAtTarget.transform.localPosition.z);

        Quaternion lookAt = Quaternion.LookRotation(targetPos - transform.position);

        this.transform.rotation = Quaternion.Slerp(transform.rotation,lookAt, 2f * Time.deltaTime);
    }

    public void SpawnShadowHopping(){
        HoppingShadow.transform.localPosition = new Vector3(this.transform.position.x,0.3f,this.transform.position.z);
        HoppingShadow.enabled = true;
        Sequence FadeShadow = DOTween.Sequence();
        Vector3 fullScale = new Vector3(6,6,6);
        FadeShadow.Append(HoppingShadow.transform.DOScale(fullScale,0.3f));
        FadeShadow.Join(HoppingShadow.DOFade(0.8f,0.5f));
        FadeShadow.AppendInterval(1f);
        fullScale = new Vector3(1,1,1);
        FadeShadow.Append(HoppingShadow.transform.DOScale(fullScale,0.3f));
        FadeShadow.Join(HoppingShadow.DOFade(0f,0.5f));
        FadeShadow.AppendInterval(0.2f);
        FadeShadow.AppendCallback(DelayDisableHoppingShadow);
    }

    public void DelayDisableHoppingShadow(){
        HoppingShadow.enabled = false;
    }
    public void FindDropPoint(){
        this.transform.position = new Vector3(goToTarget.position.x + Random.Range(-1f,2f),this.transform.position.y,goToTarget.position.z + Random.Range(-1f,2f));
        SpawnShadowHopping();
    }

    Vector3 GetRandomPointInBounds(){
        return HoppingBounds.center + new Vector3(
            ((Random.value - 0.5f) * HoppingBounds.size.x),
            this.transform.position.y,
            ((Random.value - 0.5f) * HoppingBounds.size.z));
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, StopDis);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(HoppingBounds.center,HoppingBounds.size);
    }
}
