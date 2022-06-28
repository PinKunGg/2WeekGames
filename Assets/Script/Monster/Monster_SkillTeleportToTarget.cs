using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Monster_SkillTeleportToTarget : MonoBehaviour
{
    public Transform startPoint;
    public Transform target;
    public GameObject[] SkillToTp;

    bool isSkillFollowTarget;

    private void OnEnable() {
        if (!PhotonNetwork.IsMasterClient){ 
            enabled = false;
            return;
        }
    }

    public void TeleportSkillToTarget(){
        if(!PhotonNetwork.IsMasterClient){return;}

        for(int i = 0; i < SkillToTp.Length; i++){
            SkillToTp[i].transform.position = startPoint.position;
        }

        isSkillFollowTarget = true;
    }

    private void Update() {
        if(!isSkillFollowTarget){return;}

        for(int i = 0; i < SkillToTp.Length; i++){
            SkillToTp[i].transform.position = new Vector3(target.position.x,target.position.y + 1f, target.position.z);
        }
    }

    public void SkillStopFollowTarget(){
        isSkillFollowTarget = false;
    }

    public void TeleportBack(){
        if(!PhotonNetwork.IsMasterClient){return;}

        isSkillFollowTarget = false;

        for(int i = 0; i < SkillToTp.Length; i++){
            SkillToTp[i].transform.position = startPoint.position;
        }
    }
}
