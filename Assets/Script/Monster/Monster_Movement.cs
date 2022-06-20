using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Monster_Movement : MonoBehaviour
{
    #region private variable
    float nextWaypointDis = 3f;
    float disBetweenEnemyAndPlayer;
    public float speed = 5, StopDis = 7;
    int currentWaypoint;
    bool startWalk;
    Seeker seeker;
    Rigidbody rb;
    Vector3 direc;
    Vector3 force;
    Monster_Animation monsterAnima;
    #endregion

    #region SerializeField variable

    [SerializeField] Transform target;
    [SerializeField] Path path;
    #endregion

    #region Class Property

    #region Set Property
    public float SetSpeed{set{speed = value;}}
    public float SetStopDis{set{StopDis = value;}}
    #endregion

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
        seeker.StartPath(rb.position, target.position, OnPathComplete);
        InvokeRepeating("UpdatePath", 0f, 0.1f);

        startWalk = true;
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
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
        }
        catch{}
    }
    private void Update()
    {
        if (!target.gameObject.activeInHierarchy){
            this.enabled = false;
        }
        else{
            disBetweenEnemyAndPlayer = Vector3.Distance(this.transform.position, target.transform.position);
        }
    }
    void FixedUpdate()
    {
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

            //Rotate Graphic
            // if (target.position.x > transform.position.x){
            //     this.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            // }
            // else if (target.position.x < transform.position.x){
            //     this.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            // }

            Vector3 tempLookAt = new Vector3(target.position.x,this.transform.position.y,target.transform.position.z);

            this.transform.LookAt(tempLookAt);
        }
    }
    public void GoToPlayer(){
        if (disBetweenEnemyAndPlayer > StopDis){
            rb.velocity = new Vector3(force.x, rb.velocity.y, force.z);
            monsterAnima.PlayBoolAnimator("IsRun",true);
        }
        else{
            monsterAnima.PlayBoolAnimator("IsRun",false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, StopDis);
    }
}
