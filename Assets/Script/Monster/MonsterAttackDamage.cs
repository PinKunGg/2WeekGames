using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackDamage : MonoBehaviour
{
    Vector3 knockBackPos;

    static bool isKnockBackAlready;
    Player_Move_Control _playerMoveCon;
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){

            if(isKnockBackAlready){
                return;
            }

            isKnockBackAlready = true;
            _playerMoveCon = other.gameObject.GetComponent<Player_Move_Control>();
            _playerMoveCon.enabled = false;

            Vector3 tempKnockback = this.transform.forward;
            knockBackPos = new Vector3(tempKnockback.x,other.transform.position.y * 3f,tempKnockback.z);
            other.gameObject.GetComponent<Rigidbody>().AddForce(knockBackPos * 10f,ForceMode.Impulse);

            Invoke("DelayKnockBack",0.5f);
        }
    }

    void DelayKnockBack(){
        _playerMoveCon.enabled = true;
        isKnockBackAlready = false;
    }
}
