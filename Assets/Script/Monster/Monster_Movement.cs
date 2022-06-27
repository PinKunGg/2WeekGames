using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Photon.Pun;
using Photon.Realtime;

public class Monster_Movement : MonoBehaviour
{
    public float speed = 5f, StopDis = 7f;
    public bool startWalk;
    public bool isStopWalk;

    public Transform goToTarget;
    public Transform lookAtTarget;

    
    Path path;
    float nextWaypointDis = 3f;
    int currentWaypoint;

    Seeker seeker;
    Vector3 direc;
    Vector3 force;
    Monster_Animation monsterAnima;

    public Rigidbody rb {get;private set;}
    public bool isPlayerInRange {get;private set;}
    public float disBetweenEnemyAndPlayer {get;private set;}

    private void Awake(){
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody>();
        monsterAnima = GetComponent<Monster_Animation>();
    }

    void Start(){
        if(!PhotonNetwork.IsMasterClient){return;}

        startWalk = true;
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void OnPathComplete(Path p){
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    void UpdatePath(){
        try{
            if (seeker.IsDone())
            {
                seeker.StartPath(rb.position, goToTarget.localPosition, OnPathComplete);
            }
        }
        catch{}
    }
    private void Update(){
        if(goToTarget == null){return;}

        if (!goToTarget.gameObject.activeInHierarchy){
            this.enabled = false;
        }
        else{
            disBetweenEnemyAndPlayer = Vector3.Distance(this.transform.position, goToTarget.transform.localPosition);
        }
    }
    void FixedUpdate(){
        // UpdatePath();

        if (path == null){return;}
        if (currentWaypoint >= path.vectorPath.Count){return;}

        direc = ((Vector3)path.vectorPath[currentWaypoint] - rb.position).normalized;
        force = direc * speed;

        if (startWalk == true){
            GoToPlayer();

            float dis = Vector3.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (dis < nextWaypointDis){
                currentWaypoint++;
            }

            if(!lookAtTarget){return;}

            Vector3 targetPos = new Vector3(lookAtTarget.localPosition.x,this.transform.localPosition.y,lookAtTarget.transform.localPosition.z);

            Quaternion lookAt = Quaternion.LookRotation(targetPos - transform.position);

            this.transform.rotation = Quaternion.Slerp(transform.rotation,lookAt, 3f * Time.deltaTime);
        }
    }
    public void GoToPlayer(){
        if(isStopWalk){
            monsterAnima.PlayBoolAnimator("IsRun",false);
            return;
        }

        if (disBetweenEnemyAndPlayer > StopDis){
            monsterAnima.PlayBoolAnimator("IsRun",true);
            isPlayerInRange = false;

            rb.velocity = new Vector3(force.x, rb.velocity.y, force.z);
        }
        else{
            monsterAnima.PlayBoolAnimator("IsRun",false);
            isPlayerInRange = true;
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Ground")){
            if(PhotonNetwork.IsMasterClient){
                startWalk = true;
            }
        }
    }

    private void OnCollisionExit(Collision other) {
        if(other.gameObject.CompareTag("Ground")){
            if(PhotonNetwork.IsMasterClient){
                startWalk = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, StopDis);
    }
}
