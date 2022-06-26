using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Monster_PlayParticle : MonoBehaviour
{
    public ParticleSystem[] particleSys;

    public void PlayParticle(int particleIndex){
        if(!PhotonNetwork.IsMasterClient){return;}

        particleSys[particleIndex].Play();
    }

    public void StopParticle(int particleIndex){
        if(!PhotonNetwork.IsMasterClient){return;}

        particleSys[particleIndex].Stop();
    }
}
