using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Player_Skill_Control : MonoBehaviour
{
    PhotonView photonView;
    Player_Move_Control player_Move_Control;

    [Header("Cooldown UI")]
    public Image Dash_cooldown_Ui;

    [SerializeField] bool Isdash = false;
    [SerializeField] float dash_cooldown;
    [SerializeField] float temp_dash_cooldown;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        player_Move_Control = GetComponent<Player_Move_Control>();
        if (photonView.IsMine) 
        {
            Dash_cooldown_Ui = GameObject.FindGameObjectWithTag("DashUI").transform.GetChild(0).GetComponent<Image>();
            dash_cooldown = player_Move_Control.dash_cooldown;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        CountCooldown();
    }

    void CountCooldown() 
    {
        if (player_Move_Control.IsDash && !Isdash)
        {
            Isdash = true;
            Dash_cooldown_Ui.fillAmount = 1;
            temp_dash_cooldown = 1/dash_cooldown;
        }

        if (dash_cooldown > 0 && Isdash)
        {
            dash_cooldown -= Time.deltaTime;
            Dash_cooldown_Ui.fillAmount -= temp_dash_cooldown*Time.deltaTime;
        }
        else 
        {
            Isdash = false;
            dash_cooldown = player_Move_Control.dash_cooldown;
            temp_dash_cooldown = 0;
            //Dash_cooldown_Ui.fillAmount = 0;
        }
    }
}
