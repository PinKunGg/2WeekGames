using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arachne_SpiderMinion : MonoBehaviour
{
    Monster_Animation monsterAnima;
    bool isDeath;

    private void Start() {
        monsterAnima = GetComponent<Monster_Animation>();
    }

    private void OnTriggerEnter(Collider other) {
        if(isDeath){return;}

        if(other.gameObject.CompareTag("Player")){
            isDeath = true;
            monsterAnima.PlayBoolAnimator("IsDeath",true);
            Invoke("DelayDisableAnimation",0.5f);
        }
    }

    void DelayDisableAnimation(){
        monsterAnima.PlayBoolAnimator("IsDeath",false);
    }
}
