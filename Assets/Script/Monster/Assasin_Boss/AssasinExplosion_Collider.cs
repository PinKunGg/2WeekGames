using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssasinExplosion_Collider : MonoBehaviour
{
    public Collider _collider;

    public void EnableCollider(){
        _collider.enabled = true;
    }

    public void DisableCollider(){
        _collider.enabled = false;
    }
}
