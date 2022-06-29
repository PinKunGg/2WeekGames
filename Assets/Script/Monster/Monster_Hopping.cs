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

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        if(!PhotonNetwork.IsMasterClient){
            enabled = false;
            return;
        }
        
        if(!HoppingShadow){return;}
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
        if(!HoppingShadow){return;}

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

    public GameObject checkDropPoint;
    public Vector3 checkDropPoint_radius;
    Collider[] collideList = new Collider[2];
    Vector3 dropPos;
    public LayerMask layer;
    int limiteLoop = 10;
    public bool FindDropPoint(float minOffset, float maxOffset){
        for(int i = 0; i < limiteLoop;){
            Debug.Log(1111);
            dropPos = new Vector3(goToTarget.position.x + Random.Range(minOffset,maxOffset),this.transform.position.y,goToTarget.position.z + Random.Range(minOffset,maxOffset));
            checkDropPoint.transform.position = new Vector3(dropPos.x,0f,dropPos.z);
            collideList = new Collider[2];
            int collideCount = Physics.OverlapBoxNonAlloc(checkDropPoint.transform.position,checkDropPoint_radius / 2f,collideList,Quaternion.identity,layer);

            if(collideCount > 0){
                i++;
            }
            else{
                Debug.Log(2222);
                ConfirmDropPoint(dropPos);
                return true;
            }
        }

        Debug.Log(3333);
        CheckBoundDropPoint();
        return true;
    }

    void CheckBoundDropPoint(){
        for(int i = 0; i < limiteLoop;){
            Debug.Log(4444);
            Vector3 pos = GetRandomPointInBounds();
            dropPos = pos;
            checkDropPoint.transform.position = new Vector3(dropPos.x,0f,dropPos.z);
            collideList = new Collider[2];
            int collideCount = Physics.OverlapBoxNonAlloc(checkDropPoint.transform.position,checkDropPoint_radius / 2f,collideList,Quaternion.identity,layer);

            if(collideCount > 0){
                i++;
            }
            else{
                Debug.Log(5555);
                ConfirmDropPoint(dropPos);
                return;
            }
        }

        Debug.Log(6666);
    }

    public void ConfirmDropPoint(Vector3 pos){
        this.transform.position = pos;
        
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(checkDropPoint.transform.position,checkDropPoint_radius);
    }
}
