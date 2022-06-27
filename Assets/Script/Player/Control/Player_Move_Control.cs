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
    Tutorial_Control tutorial_Control;
    Player_Skill_Control player_Skill_Control;

    public GameObject cam_lobby_pos;
    public GameObject cam_player, cam_player_inven;
    Player_Attack_Control player_Attack_Control;
    public bool IsWalk, IsRun = false;
    Vector3 movedir;

    public float SpeedMultiply = 0;

    [Header("Speed Setting")]
    public float move_speed_run = 600f;
    public float move_speed = 400f;
    float temp_move_speed_run;
    float temp_move_speed;
    float speed_multiply = 0;
    float speed_run_multiply = 0;

    [Header("Rotation Speed Setting")]
    public float trunsmoothtime = 0.1f;
    float trunsmoothvelocity;

    [Header("Dash Setting")]
    public bool IsCanDash = false;
    public float dash_speed = 10f;
    public float dash_cooldown = 2.5f;
    public float dash_time = 1f;
    float temp_dash_time = 0;
    float temp_dash_cooldown = 0;
    public float iframe_time = 0.5f;
    float delay_iframe = 0;
    public bool IsDash = false;
    public bool IsIframe = false;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        player_Attack_Control = GetComponent<Player_Attack_Control>();
        player_Skill_Control = GetComponent<Player_Skill_Control>();
        cam_lobby_pos = GameObject.FindGameObjectWithTag("CamLobbyPos");
        cam_main = Camera.main.gameObject;
        rb = GetComponent<Rigidbody>();
        temp_move_speed = move_speed;
        temp_move_speed_run = move_speed_run;
        player_inventory = Player_Inventory.player_Inventory;
        tutorial_Control = Tutorial_Control.tutorial_Control;
        if (!photonView.IsMine) { return; }
        if (tutorial_Control.IsLobby == false) 
        {
            SwitchCursor(false); 
        }
        else 
        {
            LobbyControl.lobbyControl.player_Move_Control = this;
            SwitchCursor(true);
            cam_player.SetActive(false);
            cam_player_inven.SetActive(false);
        }
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            player_inventory.player_Move_Control = this;
            player_inventory.CameraPlayer = cam_player;
            player_inventory.player_name = photonView.Owner.NickName;
            player_inventory.LoadSave();
        }
        else
        {
            cam_player.SetActive(false);
            cam_player_inven.SetActive(false);
        }
    }

    private void Update()
    {
        if (tutorial_Control.IsLobby) { return; }
        if (!photonView.IsMine)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            SwitchCursor(true);
        }
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Skill1") || anim.GetCurrentAnimatorStateInfo(1).IsName("Skill2"))
        {
            return;
        }
        if (player_inventory.InvenUI.activeSelf)
        {
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsRun", false);
            return;
        }
        Dash();
        CheckRollAnimIsRun();
    }

    public void OnLobbySetUp() 
    {
        anim.Play("Idle");
        cam_player.SetActive(false);
        cam_player_inven.SetActive(false);
        Debug.Log("EndRun");
        cam_main.transform.position = cam_lobby_pos.transform.position;
        cam_main.transform.rotation = cam_lobby_pos.transform.rotation;
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            anim.SetBool(parameter.name, false);
        }
        anim.Play("Standing Idle");
    }

    public void CameraOn() 
    {
        cam_player.SetActive(true);
        cam_player_inven.SetActive(true);
    }

    public void SwitchCursor(bool value)
    {
        if (tutorial_Control.IsLobby)
        {
            return;
        }
        if (!value)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (value)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void MoveMent()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(h, 0, v).normalized;

        if (direction.magnitude >= 0.1f)
        {
            if (!TutorialCheck(0)) { return; }
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam_main.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref trunsmoothvelocity, trunsmoothtime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            movedir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            rb.velocity = new Vector3(movedir.normalized.x * move_speed, rb.velocity.y, movedir.normalized.z * move_speed);
            IsWalk = true;
        }
        else if (!this.anim.GetCurrentAnimatorStateInfo(0).IsName("Rolling"))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            IsWalk = false;
        }
    }

    void Run()
    {
        if (player_Attack_Control.IsDraw) { return; }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!TutorialCheck(1)) { return; }
            move_speed = temp_move_speed_run + (temp_move_speed_run * (speed_run_multiply / 100));
            IsRun = true;
        }
        else
        {
            move_speed = temp_move_speed + (temp_move_speed * (speed_multiply / 100));
            IsRun = false;
        }
    }

    public void updateMoveSpeed(float walk,float run)
    {
        speed_multiply = walk;
        speed_run_multiply = run;
        move_speed = temp_move_speed + (temp_move_speed*(speed_multiply / 100)) ;
        move_speed_run = temp_move_speed_run + (temp_move_speed_run * (speed_run_multiply / 100));
    }

    void Dash() 
    {
        if (!IsCanDash) { return; }
        if (!player_Attack_Control.IsDraw) { return; }

        anim.SetBool("IsRolling", IsDash);
        if (Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            if (!TutorialCheck(5)) { return; }
            IsDash = true;
            if (temp_dash_cooldown == 0) 
            {
                GetComponent<Player_Buff_Control>().When_Dash();
                delay_iframe = (dash_time - iframe_time)/2;
            }
        }

        if (temp_dash_time < dash_time)
        {
            if (IsDash)
            {
                rb.velocity = new Vector3(movedir.normalized.x * dash_speed, rb.velocity.y, movedir.normalized.z * dash_speed);
                temp_dash_time += Time.deltaTime;
                if (temp_dash_time >= delay_iframe && temp_dash_time < (delay_iframe + iframe_time)) { IsIframe = true; }
                else { IsIframe = false; }
            }
        }
        else { IsDash = false; }
        if (!player_Skill_Control.Isdash) 
        {
            temp_dash_time = 0;
            temp_dash_cooldown = 0;
        }
    }

    void CheckRollAnimIsRun()
    {
        if (!this.anim.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
        {
            //Debug.Log("anim : " + anim.GetBool("IsBowAttack"));
            //if (anim.GetBool("IsBowAttack")) { return; }
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

    bool TutorialCheck(int stage)
    {
        if (tutorial_Control.IsTutorial)
        {
            if (stage > 0)
            {
                if (tutorial_Control.Stage[stage - 1])
                {
                    tutorial_Control.CompleteStage(stage);
                    return true;
                }
                else { return false; }
            }
            else
            {
                tutorial_Control.CompleteStage(stage);
                return true;
            }
        }
        else { return true; }
    }
};
