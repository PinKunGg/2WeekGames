using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Monster_Animation : MonoBehaviour
{
    public Animator anima;

    public void PlayBoolAnimator(string parameterName, bool value){
        if(!PhotonNetwork.IsMasterClient){return;}

        anima.SetBool(parameterName,value);
    }
    public void PlayTriggerAnimator(string parameterName){
        if(!PhotonNetwork.IsMasterClient){return;}

        anima.SetTrigger(parameterName);
    }
    public float GetCurrentAnimationTime(){
        AnimatorStateInfo animationState = anima.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] animatorClip = anima.GetCurrentAnimatorClipInfo(0);
        float animateTime = animatorClip[0].clip.length * animationState.normalizedTime;
        return animateTime;
    }
}
