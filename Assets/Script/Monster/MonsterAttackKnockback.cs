using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackKnockback : MonoBehaviour
{
    Vector3 knockBackPos;

    static bool isKnockBackAlready;
    Player_Move_Control playerMoveCon;
    Player_Attack_Control player_Attack_Control;
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){

            if(isKnockBackAlready){
                return;
            }
            player_Attack_Control = other.gameObject.GetComponent<Player_Attack_Control>();
            if (player_Attack_Control.IsBlock) { 
                return; 
            }

            isKnockBackAlready = true;
            playerMoveCon = other.gameObject.GetComponent<Player_Move_Control>();
            playerMoveCon.enabled = false;

            Vector3 tempKnockback = this.transform.forward;
            knockBackPos = new Vector3(tempKnockback.x,other.transform.position.y * 3f,tempKnockback.z);
            other.gameObject.GetComponent<Rigidbody>().AddForce(knockBackPos * 10f,ForceMode.Impulse);

            Invoke("DelayKnockBack",0.5f);
        }
    }

    void DelayKnockBack(){
        playerMoveCon.enabled = true;
        isKnockBackAlready = false;
    }
}
