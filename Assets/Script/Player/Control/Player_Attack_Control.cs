using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player_Attack_Control : MonoBehaviour
{
    PhotonView photonView;
    Animator anim;
    Player_Move_Control player_Move_Control;

    [Header("Draw Weapon Setting")]
    public bool IsDraw = false;
    public GameObject Weapon_Back,Weapon_Hand;
    public bool IsDrawed = false;

    [Header("Bloc Setting")]
    public bool IsBlock = false;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        player_Move_Control = GetComponent<Player_Move_Control>();
        Weapon_Hand.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        Attacking();
        KeepWeapon();
        Block();
        CheckWhenEndAnimDraw();
    }

    void Attacking() 
    {
        if (player_Move_Control.IsRun) { return; }
        if (Input.GetMouseButton(0))
        {
            if (IsDraw)
            {
                anim.SetBool("IsAttack", true);
            }
            else 
            {
                IsDraw = true;
                anim.SetBool("Draw", true);
                anim.SetBool("IsDraw", IsDraw);
            }
        }
        else { anim.SetBool("IsAttack", false); }

    }

    void KeepWeapon() 
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && IsDrawed) 
        {
            IsDraw = false;
            anim.SetBool("Draw", true);
            anim.SetBool("IsDraw", IsDraw);
        }
    }

    void CheckWhenEndAnimDraw() 
    {
        if (this.anim.GetCurrentAnimatorStateInfo(1).IsName("Draw A Great Sword 1") && IsDraw && !IsDrawed)
        {
            anim.SetBool("Draw", false);
            IsDrawed = true;
            photonView.RPC("SwapWeapon", RpcTarget.All, true);
        }
        else if (this.anim.GetCurrentAnimatorStateInfo(1).IsName("Draw A Great Sword 1") && !IsDraw && IsDrawed)
        {
            anim.SetBool("Draw", false);
            IsDrawed = false;
            photonView.RPC("SwapWeapon", RpcTarget.All, false);
        }
    }
    [PunRPC]
    public void SwapWeapon(bool value) 
    {
        if (value)
        {
            Weapon_Back.SetActive(false);
            Weapon_Hand.SetActive(true);
        }
        else 
        {
            Weapon_Back.SetActive(true);
            Weapon_Hand.SetActive(false);
        }
    }

    void Block() 
    {
        if (Input.GetMouseButton(1)) 
        {
            IsBlock = true;
        }
        else 
        {
            IsBlock = false;
        }

        if (!this.anim.GetCurrentAnimatorStateInfo(1).IsName("Standing Melee Combo Attack Ver_ 3 (1)")) 
        {
            anim.SetBool("IsBlock", IsBlock);
        }
    }
}
