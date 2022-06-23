using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_RotateToTarget : MonoBehaviour
{
    public Transform target;

    private void Update() {
        if(!target){return;}

        this.transform.LookAt(target);
    }
}
