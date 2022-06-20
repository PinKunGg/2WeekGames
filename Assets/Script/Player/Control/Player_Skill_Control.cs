using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Player_Skill_Control : MonoBehaviour
{
    PhotonView photonView;
    Player_Move_Control player_Move_Control;
    Player_Attack_Control player_Attack_Control;
    Player_Inventory player_Inventory;
    Animator anim;

    [Header("Cooldown UI")]
    public Image Dash_cooldown_Ui,Skill1_cooldown_UI, Skill2_cooldown_UI;

    [Header("DashSetting")]
    [SerializeField] bool Isdash = false;
    [SerializeField] float dash_cooldown;
    [SerializeField] float temp_dash_cooldown;

    [Header("Skill1 Setting")]
    public bool IsSkill1 = false;
    public bool IsSkill1_Cooldown = false;
    [SerializeField] float skill1_cooldown = 10f;
    [SerializeField] float temp_skill1_cooldown;

    [Header("Skill2 Setting")]
    public bool IsSkill2 = false;
    public bool IsSkill2_Cooldown = false;
    [SerializeField] float skill2_cooldown = 10f;
    [SerializeField] float temp_skill2_cooldown;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        player_Move_Control = GetComponent<Player_Move_Control>();
        player_Attack_Control = GetComponent<Player_Attack_Control>();
        player_Inventory = Player_Inventory.player_Inventory;
        anim = GetComponent<Animator>();
        if (photonView.IsMine) 
        {
            Dash_cooldown_Ui = GameObject.FindGameObjectWithTag("DashUI").transform.GetChild(0).GetComponent<Image>();
            Skill1_cooldown_UI = GameObject.FindGameObjectWithTag("Skill1UI").transform.GetChild(0).GetComponent<Image>();
            Skill2_cooldown_UI = GameObject.FindGameObjectWithTag("Skill2UI").transform.GetChild(0).GetComponent<Image>();
        }
    }

    public void init() 
    {
        dash_cooldown = player_Move_Control.dash_cooldown;
        temp_dash_cooldown = 1 / dash_cooldown;
        temp_skill1_cooldown = skill1_cooldown;
        temp_skill2_cooldown = skill2_cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        CountCooldown();
        if (player_Attack_Control.IsBlock) { return; }
        if (Input.GetKeyDown(KeyCode.Q) && anim.GetBool("IsDraw") == false) 
        {
            if (player_Inventory.PotionSlot >= 1) 
            {
                player_Inventory.PotionSlot--;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q) && anim.GetBool("IsDraw") == true && temp_skill1_cooldown == skill1_cooldown && !anim.GetCurrentAnimatorStateInfo(1).IsName("Skill2") && !anim.GetCurrentAnimatorStateInfo(1).IsName("Dash"))
        {
            Skill1(true);
        }
        if (Input.GetKeyDown(KeyCode.E) && anim.GetBool("IsDraw") == true && temp_skill2_cooldown == skill2_cooldown && !anim.GetCurrentAnimatorStateInfo(1).IsName("Skill1") && !anim.GetCurrentAnimatorStateInfo(1).IsName("Dash"))
        {
            Skill2(true);
        }
        if (player_Move_Control.IsDash && !Isdash)
        {
            Isdash = true;
            Dash_cooldown_Ui.fillAmount = 1;
        }

    }

    void CountCooldown()
    {

        if (dash_cooldown > 0 && Isdash)
        {
            dash_cooldown -= Time.deltaTime;
            Dash_cooldown_Ui.fillAmount -= temp_dash_cooldown * Time.deltaTime;
        }
        else
        {
            Isdash = false;
            dash_cooldown = player_Move_Control.dash_cooldown;
        }

        if (temp_skill1_cooldown > 0 && IsSkill1_Cooldown)
        {
            temp_skill1_cooldown -= Time.deltaTime;
            Skill1_cooldown_UI.fillAmount -= (1/skill1_cooldown)* Time.deltaTime;

        }
        else
        {
            IsSkill1_Cooldown = false;
            temp_skill1_cooldown = skill1_cooldown;
        }
        if (anim.GetBool("IsSkill1") == true && anim.GetCurrentAnimatorStateInfo(1).IsName("Skill1"))
        {
            Skill1(false);
        }

        if (temp_skill2_cooldown > 0 && IsSkill2_Cooldown)
        {
            temp_skill2_cooldown -= Time.deltaTime;
            Skill2_cooldown_UI.fillAmount -= (1 / skill2_cooldown) * Time.deltaTime;

        }
        else
        {
            IsSkill2_Cooldown = false;
            temp_skill2_cooldown = skill1_cooldown;
        }
        if (anim.GetBool("IsSkill2") == true && anim.GetCurrentAnimatorStateInfo(1).IsName("Skill2"))
        {
            Skill2(false);
        }
    }

    void Skill1(bool value)
    {
        IsSkill1 = value;
        if (value) 
        { 
            Skill1_cooldown_UI.fillAmount = 1;
            IsSkill1_Cooldown = true;
        }
        anim.SetBool("IsSkill1", IsSkill1);
    }

    void Skill2(bool value)
    {
        IsSkill2 = value;
        if (value)
        {
            Skill2_cooldown_UI.fillAmount = 1;
            IsSkill2_Cooldown = true;
        }
        anim.SetBool("IsSkill2", IsSkill2);
    }
}
