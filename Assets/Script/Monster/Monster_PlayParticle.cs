using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Monster_PlayParticle : MonoBehaviour
{
    public ParticleSystem[] particleSys;

    public void PlayParticle(int particleIndex){
        particleSys[particleIndex].Play();
    }

    public void StopParticle(int particleIndex){
        particleSys[particleIndex].Stop();
    }
}
