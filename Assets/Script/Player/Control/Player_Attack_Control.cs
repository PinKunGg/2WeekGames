using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Player_Attack_Control : MonoBehaviour
{
    PhotonView photonView;
    Animator anim;
    Player_Move_Control player_Move_Control;
    Player_Inventory player_inventory;
    bool IsFormOther = false;

    [Header("Draw Weapon Setting")]
    public bool IsDraw = false;
    public GameObject Weapon_Back,Weapon_Hand;
    public GameObject Weapon_Shield,Weapon_Stuff;
    public bool IsDrawed = false;
    bool IsDrawByOpenInven = false;

    [Header("Block Setting")]
    public bool IsBlock = false;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        player_Move_Control = GetComponent<Player_Move_Control>();
        player_inventory = Player_Inventory.player_Inventory;

        if (photonView.IsMine)
        {
            Weapon_Hand.SetActive(false);
            Weapon_Shield.SetActive(false);
            Weapon_Stuff.SetActive(false);
            PlayerListMenu.playerListMenu.AddAttackControl(this);
        }
        else if(!IsFormOther)
        {
            Weapon_Hand.SetActive(false);
            Weapon_Shield.SetActive(false);
            Weapon_Stuff.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (player_inventory.InvenUI.activeSelf) 
        {
            if (IsDrawByOpenInven) 
            {
                KeepWeapon();
                CheckWhenEndAnimDraw();
            }
            return; 
        }
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Skill1") || anim.GetCurrentAnimatorStateInfo(1).IsName("Skill2")) { return; }
        if (Input.GetKeyDown(KeyCode.LeftControl) && IsDrawed)
        {
            KeepWeapon();
        }
        Block();
        Attacking();
        CheckWhenEndAnimDraw();
    }

    void Attacking() 
    {
        if (player_Move_Control.IsRun) { return; }
        if (Input.GetMouseButton(0))
        {
            Attack();
        }
        else { anim.SetBool("IsAttack", false); }

    }
    void Attack() 
    {
        if (IsDraw)
        {
            anim.SetBool("IsAttack", true);
        }
        else
        {
            IsDraw = true;
            IsDrawByOpenInven = true;
            anim.SetBool("Draw", true);
            anim.SetBool("IsDraw", IsDraw);
        }
    }

    void KeepWeapon() 
    {
        IsDraw = false;
        anim.SetBool("Draw", true);
        anim.SetBool("IsDraw", IsDraw);
    }

    void CheckWhenEndAnimDraw() 
    {
        if (this.anim.GetCurrentAnimatorStateInfo(1).IsName("Draw") && IsDraw && !IsDrawed)
        {
            anim.SetBool("Draw", false);
            IsDrawed = true;
            photonView.RPC("SwapWeapon", RpcTarget.All, true);
        }
        else if (this.anim.GetCurrentAnimatorStateInfo(1).IsName("Draw") && !IsDraw && IsDrawed)
        {
            IsDrawByOpenInven = false;
            anim.SetBool("Draw", false);
            IsDrawed = false;
            photonView.RPC("SwapWeapon", RpcTarget.All, false);
        }
    }
    [PunRPC]
    public void SwapWeapon(bool value) 
    {
        if (anim == null) { GetComponent<Animator>(); }
        if (player_inventory == null) { player_inventory = Player_Inventory.player_Inventory; }
        Debug.Log("It Run : " + this.gameObject.name);
        if (value)
        {
            if (anim.runtimeAnimatorController == player_inventory.animAxe)
            {
                Weapon_Back.SetActive(false);
                Weapon_Hand.SetActive(true);
                Weapon_Stuff.SetActive(false);
            }
            else if (anim.runtimeAnimatorController == player_inventory.animSwordAndShield) 
            {
                Weapon_Back.SetActive(false);
                Weapon_Hand.SetActive(true);
                Weapon_Shield.SetActive(true);
                Weapon_Stuff.SetActive(false);
            }
            else if (anim.runtimeAnimatorController == player_inventory.animMagic)
            {
                Weapon_Back.SetActive(false);
                Weapon_Hand.SetActive(false);
                Weapon_Shield.SetActive(false);
                Weapon_Stuff.SetActive(true);
            }
        }
        else 
        {
            Weapon_Back.SetActive(true);
            Weapon_Hand.SetActive(false);
            Weapon_Shield.SetActive(false);
            Weapon_Stuff.SetActive(false);
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

        if (!this.anim.GetCurrentAnimatorStateInfo(1).IsName("Basic_Attack")) 
        {
            anim.SetBool("IsBlock", IsBlock);
        }
    }

    public void UpdateAnimForOther() 
    {
        if (!photonView) { photonView = GetComponent<PhotonView>(); }
        if (photonView.IsMine)
        {
            string AnimConName = "";
            if (anim.runtimeAnimatorController == player_inventory.animAxe) { AnimConName = "Axe"; }
            else if (anim.runtimeAnimatorController == player_inventory.animSwordAndShield) { AnimConName = "Sword&Shield"; }
            else if (anim.runtimeAnimatorController == player_inventory.animMagic) { AnimConName = "Stuff"; }
            photonView.RPC("UpdateAnim", RpcTarget.All, Weapon_Hand.activeSelf, Weapon_Back.activeSelf,Weapon_Shield.activeSelf,Weapon_Stuff.activeSelf, AnimConName,IsDraw);
        }
    }

    [PunRPC]
    public void UpdateAnim(bool Isweapond_hand , bool Isweapon_back , bool Iswweapon_shield, bool Iswweapon_stuff, string animConName,bool _IsDraw) 
    {
        if (!photonView) { photonView = GetComponent<PhotonView>(); }
        if (!photonView.IsMine)
        {
            Weapon_Hand.SetActive(Isweapond_hand);
            Weapon_Back.SetActive(Isweapon_back);
            Weapon_Shield.SetActive(Iswweapon_shield);
            Weapon_Stuff.SetActive(Iswweapon_stuff);
            anim = GetComponent<Animator>();
            player_inventory = Player_Inventory.player_Inventory;
            if (animConName == "Axe") { anim.runtimeAnimatorController = player_inventory.animAxe; }
            else if (animConName == "Sword&Shield") { anim.runtimeAnimatorController = player_inventory.animSwordAndShield; }
            else if (animConName == "Stuff") { anim.runtimeAnimatorController = player_inventory.animMagic; }
            IsFormOther = true;
            if (_IsDraw) 
            {
                anim.SetBool("Draw", true);
                anim.SetBool("IsDraw", true);
            }
        }
    }
}
