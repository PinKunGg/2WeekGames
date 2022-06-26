using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Animation : MonoBehaviour
{
    public Animator anima;

    public void PlayBoolAnimator(string parameterName, bool value){
        anima.SetBool(parameterName,value);
    }
    public void PlayTriggerAnimator(string parameterName){
        anima.SetTrigger(parameterName);
    }
}
