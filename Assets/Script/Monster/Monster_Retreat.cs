using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Retreat : MonoBehaviour
{
    public Transform target;

    private void Update() {
        if(!target){return;}
        float dis = Vector3.Distance(this.transform.position,target.transform.position);

        if(dis >= 7.6f){
            Vector3 dir = target.position - transform.position;
            transform.Translate(dir * 1f * Time.deltaTime);
        }
        else if(dis <= 7.4f){
            Vector3 dir = transform.position - target.position;
            transform.Translate(dir * 1f * Time.deltaTime);
        }
    }

    private void OnDrawGizmos() {
        if(!target){return;}
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target.position, 7f);
    }
}
