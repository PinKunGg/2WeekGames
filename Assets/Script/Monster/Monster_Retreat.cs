using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Retreat : MonoBehaviour
{
    public Transform target;
    Vector3 dir,newDir;

    public bool isGroundTouch;

    Monster_Animation _monsterAnima;
    Monster_Movement _monsterMove;

    private void Start() {
        _monsterAnima = GetComponent<Monster_Animation>();
        _monsterMove = GetComponent<Monster_Movement>();
    }

    private void OnDisable() {
        _monsterAnima.PlayBoolAnimator("IsRun",false);
    }

    private void Update() {
        if(!target || !isGroundTouch){return;}

        float dis = Vector3.Distance(this.transform.position,target.transform.position);

        if(dis <= 8f){
            dir = transform.position - target.position;
            _monsterAnima.PlayBoolAnimator("IsRun",true);
            _monsterMove.rb.velocity = -transform.forward * 5f;
        }
        else if(dis >= 12f){
            dir = target.position - transform.position;
            _monsterAnima.PlayBoolAnimator("IsRun",true);
            _monsterMove.rb.velocity = transform.forward * 5f;
        }
        else{
            _monsterAnima.PlayBoolAnimator("IsRun",false);
        }
    }

    private void OnDrawGizmos() {
        if(!target){return;}
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target.position, 10f);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Ground")){
            isGroundTouch = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        if(other.gameObject.CompareTag("Ground")){
            isGroundTouch = false;
        }
    }
}
