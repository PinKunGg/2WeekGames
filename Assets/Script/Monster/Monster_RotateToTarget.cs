using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Monster_RotateToTarget : MonoBehaviour
{
    public Transform target;

    private void Update() {
        if(!PhotonNetwork.IsMasterClient){return;}

        if(!target){return;}

        Vector3 newLookAt = new Vector3(target.position.x,target.position.y + 1f,target.position.z);

        this.transform.LookAt(newLookAt);
    }
}
