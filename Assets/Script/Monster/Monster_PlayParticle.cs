using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_PlayParticle : MonoBehaviour
{
    public ParticleSystem[] particleSys;

    public void PlayerParticle(int particleIndex){
        particleSys[particleIndex].Play();
    }
}
