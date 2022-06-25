using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Photon.Pun;
using Photon.Realtime;

public class Monster_Movement : MonoBehaviour
{
    #region private variable
    float nextWaypointDis = 3f;
    int currentWaypoint;
    Seeker seeker;
    public Rigidbody rb {get;private set;}
    Vector3 direc;
    Vector3 force;
    Monster_Animation monsterAnima;
    #endregion

    #region SerializeField variable
    [SerializeField] Path path;
    public bool startWalk;
    public bool isStopWalk;
    public bool isPlayerInRange {get;private set;}
    public float speed = 5f, StopDis = 7f;
    public Transform goToTarget;
    public Transform lookAtTarget;
    public float disBetweenEnemyAndPlayer {get;private set;}
    #endregion

    //Get all component
    private void Awake(){
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody>();
        monsterAnima = GetComponent<Monster_Animation>();
    }

    //Prepare all setting
    void Start(){
        //Make path and update it
        if(PhotonNetwork.IsMasterClient){
            startWalk = true;
            InvokeRepeating("UpdatePath", 0f, 0.1f);
        }

        // startWalk = true;
        // InvokeRepeating("UpdatePath", 0f, 0.1f);
    }

    void OnPathComplete(Path p){
        //I don't know what the fuck is this
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    void UpdatePath(){
        //yeah Update Pathfinding
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
        //I don't know what the fuck is this
        if (path == null){
            return;
        }
        if (currentWaypoint >= path.vectorPath.Count){
            return;
        }

        //check path enemy and path waypoint & make force
        direc = ((Vector3)path.vectorPath[currentWaypoint] - rb.position).normalized;

        force = new Vector3(direc.x * speed, direc.y * speed, direc.z * speed);

        //if enemy hit ground and player in detectrage
        if (startWalk == true){
            GoToPlayer();

            //check if it have any waypoint left
            float dis = Vector3.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (dis < nextWaypointDis){
                currentWaypoint++;
            }

            if(!lookAtTarget){return;}

            Vector3 targetPos = new Vector3(lookAtTarget.localPosition.x,this.transform.localPosition.y,lookAtTarget.transform.localPosition.z);

            Quaternion lookAt = Quaternion.LookRotation(targetPos - transform.position);

            this.transform.rotation = Quaternion.Slerp(transform.rotation,lookAt, 2f * Time.deltaTime);
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
            startWalk = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        if(other.gameObject.CompareTag("Ground")){
            if(PhotonNetwork.IsMasterClient){
                startWalk = false;
            }
            startWalk = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, StopDis);
    }
}
