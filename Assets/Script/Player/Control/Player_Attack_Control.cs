using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Cinemachine;

public class Player_Attack_Control : MonoBehaviour
{
    PhotonView photonView;
    Animator anim;
    Player_Move_Control player_Move_Control;
    Player_Inventory player_inventory;
    Tutorial_Control tutorial_Control;
    Player_Stat player_Stat;
    bool IsFormOther = false;
    public bool IsNoWeapon = false;

    public GameObject CameraAimBow;

    [Header("Draw Weapon Setting")]
    public bool IsDraw = false;
    public GameObject[] AllWeaponObj = new GameObject[9];
    public bool IsDrawed = false;
    bool IsDrawByOpenInven = false;
    public GameObject LookAtBow;
    public GameObject LookAtOrigin;

    [Header("Block Setting")]
    public bool IsBlock = false;
    public GameObject Magic_Block_Obj;

    public GameObject WeaponIsUse;
    public string AnimConName = "";
    // Start is called before the first frame update

    private void Awake()
    {
        tutorial_Control = Tutorial_Control.tutorial_Control;
        photonView = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();
        player_inventory = Player_Inventory.player_Inventory;
        player_Stat = GetComponent<Player_Stat>();
    }
    void Start()
    {
        player_Move_Control = GetComponent<Player_Move_Control>();
        if (photonView.IsMine)
        {
            //CheckWeaponUse();
            if (tutorial_Control.IsTutorial) { return; }
            PlayerListMenu.playerListMenu.AddAttackControl(this);
        }
        else if(!IsFormOther)
        {
            for (int x = 0; x < AllWeaponObj.Length; x++)
            {
                AllWeaponObj[x].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorial_Control.IsLobby) { return; }
        if (!photonView.IsMine) {return; }
        if (player_Stat.IsDie) { return; }
        if (player_inventory.InvenUI.activeSelf) 
        {
            if (IsDrawByOpenInven) 
            {
                KeepWeapon();
                CheckWhenEndAnimDraw();
            }
            return; 
        }
        if (IsNoWeapon) 
        { 
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
    public void IsPlayerNoWeapond(bool value) 
    {
        Debug.Log("No Weapond");
        IsNoWeapon = value;
        if (value) { photonView.RPC("RpcPlayerNoWeapond", RpcTarget.All); }

    }
    [PunRPC]
    void RpcPlayerNoWeapond() 
    {
        Debug.Log("No Weapond Run");
        for (int x = 0; x < AllWeaponObj.Length; x++)
        {
            AllWeaponObj[x].SetActive(false);
        }
    }
    void Attacking()
    {
        if (player_Move_Control.IsRun) { return; }
        if (Input.GetMouseButton(0))
        {
            Attack();
        }
        else
        {
            anim.SetBool("IsAttack", false);
            anim.SetBool("IsBowAttack", false);
        }

    }

    void Attack()
    {
        if (IsDraw)
        {
            if (anim.runtimeAnimatorController == player_inventory.animBow)
            {
                anim.SetBool("IsBowAttack", true);
            }
            else
            {
                if (!TutorialCheck(3)) { return; }
                anim.SetBool("IsAttack", true);
            }
        }
        else
        {
            if (!TutorialCheck(2)) { return; }
            IsDraw = true;
            IsDrawByOpenInven = true;
            anim.SetBool("Draw", true);
            anim.SetBool("IsDraw", IsDraw);
        }
    }


    public void CheckWeaponUse() 
    {
        for (int x = 0; x < AllWeaponObj.Length; x++)
        {
            AllWeaponObj[x].SetActive(false);
        }
        if (IsNoWeapon) 
        {
            Debug.Log("No Weapond");
            photonView.RPC("Rpc_CheckWeaponUse", RpcTarget.All, "NoWeapond");
            return; 
        }
        if (anim.runtimeAnimatorController == player_inventory.animAxe) { AnimConName = "Axe"; }
        else if (anim.runtimeAnimatorController == player_inventory.animSwordAndShield) { AnimConName = "Sword&Shield"; }
        else if (anim.runtimeAnimatorController == player_inventory.animMagic) { AnimConName = "Stuff"; }
        else if (anim.runtimeAnimatorController == player_inventory.animBow) { AnimConName = "Bow"; }
        photonView.RPC("Rpc_CheckWeaponUse", RpcTarget.All, AnimConName);
    }

    [PunRPC]
    void Rpc_CheckWeaponUse(string value) 
    {
        for (int x = 0; x < AllWeaponObj.Length; x++)
        {
            AllWeaponObj[x].SetActive(false);
        }
        if (value == "Axe")
        {
            AllWeaponObj[0].SetActive(true);
        }
        else if (value == "Sword&Shield")
        {
            AllWeaponObj[1].SetActive(true);
        }
        else if (value == "Stuff")
        {
            AllWeaponObj[2].SetActive(true);
        }
        else if (value == "Bow")
        {
            AllWeaponObj[7].SetActive(true);
        }
        else
        {
            Debug.Log("No Weapond");
            IsPlayerNoWeapond(true);
        }
        UpdateAnimForOther();
    }

    void KeepWeapon() 
    {
        if (!TutorialCheck(6)) { return; }
        IsDraw = false;
        anim.SetBool("Draw", true);
        anim.SetBool("IsDraw", IsDraw);
    }

    void CheckWhenEndAnimDraw() 
    {
        string _animConName = ""; 
        if (this.anim.GetCurrentAnimatorStateInfo(1).IsName("Draw") && IsDraw && !IsDrawed)
        {
            anim.SetBool("Draw", false);
            IsDrawed = true;
            if (anim.runtimeAnimatorController == player_inventory.animAxe)
            {
                _animConName = "Axe";
            }
            else if (anim.runtimeAnimatorController == player_inventory.animSwordAndShield)
            {
                _animConName = "SwordAndShield";
            }
            else if (anim.runtimeAnimatorController == player_inventory.animMagic)
            {
                _animConName = "Stuff";
            }
            else if (anim.runtimeAnimatorController == player_inventory.animBow)
            {
                _animConName = "Bow";
            }
            photonView.RPC("SwapWeapon", RpcTarget.All, true, _animConName);
        }
        else if (this.anim.GetCurrentAnimatorStateInfo(1).IsName("Draw") && !IsDraw && IsDrawed)
        {
            IsDrawByOpenInven = false;
            anim.SetBool("Draw", false);
            IsDrawed = false;
            photonView.RPC("SwapWeapon", RpcTarget.All, false,null);
        }
    }

    [PunRPC]
    public void SwapWeapon(bool value,string _animConName) 
    {
        Debug.Log("value : " + value + " amim : " + _animConName);
        if (anim == null) { GetComponent<Animator>(); }
        if (player_inventory == null) { player_inventory = Player_Inventory.player_Inventory; }
        for (int x = 0; x < AllWeaponObj.Length; x++)
        {
            AllWeaponObj[x].SetActive(false);
        }
        if (value)
        {
            if (_animConName == "Axe")
            {
                anim.runtimeAnimatorController = player_inventory.animAxe;
                if (value)
                {
                    AllWeaponObj[3].SetActive(true);
                }
            }
            else if (_animConName == "SwordAndShield")
            {
                anim.runtimeAnimatorController = player_inventory.animSwordAndShield;
                if (value)
                {
                    AllWeaponObj[4].SetActive(true);
                    AllWeaponObj[5].SetActive(true);
                }
            }
            else if (_animConName == "Stuff")
            {
                anim.runtimeAnimatorController = player_inventory.animMagic;
                if (value)
                {
                    AllWeaponObj[6].SetActive(true);
                }
            }
            else if (_animConName == "Bow")
            {
                anim.runtimeAnimatorController = player_inventory.animBow;
                if (value)
                {
                    AllWeaponObj[8].SetActive(true);
                }
            }
        }
        else 
        {
            if (anim.runtimeAnimatorController == player_inventory.animAxe)
            {
                AllWeaponObj[0].SetActive(true);
            }
            else if (anim.runtimeAnimatorController == player_inventory.animSwordAndShield)
            {
                AllWeaponObj[1].SetActive(true);
            }
            else if (anim.runtimeAnimatorController == player_inventory.animMagic)
            {
                AllWeaponObj[2].SetActive(true);
            }
            else if (anim.runtimeAnimatorController == player_inventory.animBow)
            {
                AllWeaponObj[7].SetActive(true);
            }
        }
    }

    void Block() 
    {
        if (player_Move_Control.IsDash) { return; }
        if (Input.GetMouseButton(1)) 
        {
            if (!TutorialCheck(4)) { return; }
            IsBlock = true;
            if (anim.runtimeAnimatorController == player_inventory.animBow)
            {
                //CameraAimBow.GetComponent<CinemachineFreeLook>().LookAt = LookAtBow.transform;
            }
            else if (anim.runtimeAnimatorController == player_inventory.animMagic && Input.GetMouseButtonDown(1) && IsDraw) 
            {
                photonView.RPC("Rpc_SetactiveMagicBlock", RpcTarget.All, true);
            }
        }
        else 
        {
            IsBlock = false;
            if (anim.runtimeAnimatorController == player_inventory.animBow)
            {
                //CameraAimBow.GetComponent<CinemachineFreeLook>().LookAt = LookAtOrigin.transform;
            }
            else if (anim.runtimeAnimatorController == player_inventory.animMagic && Input.GetMouseButtonUp(1))
            {
                photonView.RPC("Rpc_SetactiveMagicBlock", RpcTarget.All, false);
            }
        }

        if (!this.anim.GetCurrentAnimatorStateInfo(1).IsName("Basic_Attack")) 
        {
            anim.SetBool("IsBlock", IsBlock);
        }
    }

    [PunRPC]
    void Rpc_SetactiveMagicBlock(bool value) 
    {
        Magic_Block_Obj.SetActive(value);
    }

    public void SetDamage(float Damage , float CriRate , float CriDamage, float Damage_Multiply) 
    {
        for (int x = 0; x < AllWeaponObj.Length; x++) 
        {
            PlayerWeaponDamage playerWeapon = AllWeaponObj[x].GetComponent<PlayerWeaponDamage>();
            playerWeapon.Damage = Damage + (Damage * (Damage_Multiply/100));
            playerWeapon.CriRate = CriRate;
            playerWeapon.CriDamage = CriDamage;
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
            else if (anim.runtimeAnimatorController == player_inventory.animBow) { AnimConName = "Bow"; }
            bool[] IsSomeWeaponActive = new bool[9] { AllWeaponObj[0].activeSelf, AllWeaponObj[1].activeSelf, AllWeaponObj[2].activeSelf, AllWeaponObj[3].activeSelf, AllWeaponObj[4].activeSelf, AllWeaponObj[5].activeSelf , AllWeaponObj[6].activeSelf, AllWeaponObj[7].activeSelf, AllWeaponObj[8].activeSelf };
            photonView.RPC("UpdateAnim", RpcTarget.All, IsSomeWeaponActive, AnimConName,IsDraw);
        }
    }

    [PunRPC]
    public void UpdateAnim(bool[] data, string animConName,bool _IsDraw) 
    {
        if (!photonView) { photonView = GetComponent<PhotonView>(); }
        if (!photonView.IsMine)
        {
            for (int x = 0; x < AllWeaponObj.Length; x++) 
            {
                AllWeaponObj[x].SetActive(data[x]);
            }
            anim = GetComponent<Animator>();
            player_inventory = Player_Inventory.player_Inventory;
            Debug.Log("animCon Name : " + animConName);
            if (animConName == "Axe") { anim.runtimeAnimatorController = player_inventory.animAxe; }
            else if (animConName == "Sword&Shield") { anim.runtimeAnimatorController = player_inventory.animSwordAndShield; }
            else if (animConName == "Stuff") { anim.runtimeAnimatorController = player_inventory.animMagic; }
            else if (animConName == "Bow") { anim.runtimeAnimatorController = player_inventory.animBow; }
            IsFormOther = true;
            if (_IsDraw) 
            {
                anim.SetBool("Draw", true);
                anim.SetBool("IsDraw", true);
            }
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
}
