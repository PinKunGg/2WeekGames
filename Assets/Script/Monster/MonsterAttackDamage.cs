using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackDamage : MonoBehaviour
{
    Vector3 knockBackPos;

    static bool isKnockBackAlready;
    Player_Move_Control _playerMoveCon;
    Player_Attack_Control _player_Attack_Control;
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){

            if(isKnockBackAlready){
                return;
            }
            _player_Attack_Control = other.gameObject.GetComponent<Player_Attack_Control>();
            if (_player_Attack_Control.IsBlock) { 
                return; 
            }

            isKnockBackAlready = true;
            _playerMoveCon = other.gameObject.GetComponent<Player_Move_Control>();
            _playerMoveCon.enabled = false;

            Vector3 tempKnockback = this.transform.forward;
            knockBackPos = new Vector3(tempKnockback.x,other.transform.position.y * 5f,tempKnockback.z);
            other.gameObject.GetComponent<Rigidbody>().AddForce(knockBackPos * 10f,ForceMode.Impulse);

            Invoke("DelayKnockBack",1f);
        }
    }

    void DelayKnockBack(){
        _playerMoveCon.enabled = true;
        isKnockBackAlready = false;
    }
}
