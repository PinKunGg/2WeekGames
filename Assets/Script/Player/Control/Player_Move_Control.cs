using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player_Move_Control : MonoBehaviour
{
    PhotonView photonView;
    GameObject cam_main;
    Animator anim;
    Rigidbody rb;
    Player_Inventory player_inventory;

    public GameObject cam_player;
    Player_Attack_Control player_Attack_Control;
    public bool IsWalk, IsRun = false;
    Vector3 movedir;

    [Header("Speed Setting")]
    public float move_speed_run = 600f;
    public float move_speed = 400f;
    float temp_move_speed;

    [Header("Rotation Speed Setting")]
    public float trunsmoothtime = 0.1f;
    float trunsmoothvelocity;

    [Header("Dash Setting")]
    public float dash_speed = 10f;
    public float dash_cooldown = 2.5f;
    public float dash_time = 1f;
    float temp_dash_time = 0;
    float temp_dash_cooldown = 0;
    public float iframe_time = 0.5f;
    float delay_iframe = 0;
    public bool IsDash = false;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        player_Attack_Control = GetComponent<Player_Attack_Control>();
        cam_main = Camera.main.gameObject;
        rb = GetComponent<Rigidbody>();
        temp_move_speed = move_speed;
        player_inventory = Player_Inventory.player_Inventory;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    private void Start()
    {
        if (photonView.IsMine) { player_inventory.CameraPlayer = cam_player; }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            cam_player.SetActive(false);
            return;
        }
        Dash();
        CheckRollAnimIsRun();
    }

    void MoveMent() 
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(h, 0, v).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam_main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref trunsmoothvelocity, trunsmoothtime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            movedir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            rb.velocity = new Vector3(movedir.normalized.x * move_speed, rb.velocity.y, movedir.normalized.z * move_speed);
            IsWalk = true;
        }
        else if(!this.anim.GetCurrentAnimatorStateInfo(0).IsName("Rolling"))
        {
            rb.velocity = new Vector3(0,rb.velocity.y,0);
            IsWalk = false;
        }
    }

    void Run() 
    {
        if (player_Attack_Control.IsDraw) { return; }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            move_speed = move_speed_run;
            IsRun = true;
        }
        else
        {
            move_speed = temp_move_speed;
            IsRun = false;
        }
    }

    void Dash() 
    {
        if (!player_Attack_Control.IsDraw) { return; }

        anim.SetBool("IsRolling", IsDash);
        if (Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            IsDash = true;
            if (temp_dash_cooldown == 0) 
            {
                delay_iframe = (dash_time - iframe_time)/2;
            }
        }

        if (temp_dash_time < dash_time)
        {
            if (IsDash) 
            {
                rb.velocity = new Vector3(movedir.normalized.x*dash_speed, rb.velocity.y, movedir.normalized.z * dash_speed);
                temp_dash_time += Time.deltaTime;
                if (temp_dash_time >= delay_iframe && temp_dash_time < (delay_iframe+iframe_time)) 
                {
                    Debug.Log("Iframe on");
                }
                else { Debug.Log("Iframe off"); }
            }
        }
        else 
        {
            IsDash = false;
            if (temp_dash_cooldown < dash_cooldown)
            {
                temp_dash_cooldown += Time.deltaTime;
            }
            else 
            {
                temp_dash_time = 0;
                temp_dash_cooldown = 0;
            }
        }
    }

    void CheckRollAnimIsRun()
    {
        if (!this.anim.GetCurrentAnimatorStateInfo(0).IsName("Sprinting Forward Roll"))
        {
            if (player_Attack_Control.IsBlock && player_Attack_Control.IsDrawed)
            {
                RotationOnBlocked();
            }
            else 
            {
                MoveMent();
            }

            Run();
            anim.SetBool("IsWalk", IsWalk);
            anim.SetBool("IsRun", IsRun);
        }
    }

    void RotationOnBlocked() 
    {
        Vector3 direction = new Vector3(0, 0, 1).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam_main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref trunsmoothvelocity, trunsmoothtime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

        }
    }
};
