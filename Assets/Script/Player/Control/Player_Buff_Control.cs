using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player_Buff_Control : MonoBehaviour
{
    public Player_Stat player_Stat;
    Player_Move_Control player_Move_Control;
    PhotonView photonView;
    public Animator anim;

    public GameObject BuffZone;
    public GameObject BuffPrefab;
    public Sprite[] AllBuff = new Sprite[7];
    public float[] AllBuff_Time = new float[7];
    public float[] AllBuff_Cooldown = new float[7];

    public int Veil_Buff_int = 0;
    public int[] Veil_Buff = new int[5] {99,1,2,0,3};
    public float posion_damage = 5;

    // Start is called before the first frame update
    void Start()
    {
        //player_Stat = GetComponent<Player_Stat>();
        player_Move_Control = GetComponent<Player_Move_Control>();
        photonView = GetComponent<PhotonView>();
        BuffZone = GameObject.FindGameObjectWithTag("BuffZone");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) { return; }
        if (Input.GetKeyDown(KeyCode.Y)) 
        {
            CreateBuff(6);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            CreateBuff(0);
        }
    }
    public void When_Dash() 
    {
        if (!photonView.IsMine) { return; }
        CreateBuff(Veil_Buff[Veil_Buff_int]);
    }

    public void CreateBuff(int value) 
    {
        if (!photonView.IsMine) { return; }
        if (value == 99) { return; }
        if(AllBuff_Cooldown[value] != 0) 
        { 
            AllBuff_Cooldown[value] = AllBuff_Time[value];
            return;
        }
        GameObject buff = Instantiate(BuffPrefab);
        buff.GetComponent<Image>().sprite = AllBuff[value];
        buff.transform.parent = BuffZone.transform;
        buff.transform.localScale = Vector3.one;
        AllBuff_Cooldown[value] = AllBuff_Time[value];
        if (value == 0)
        {
            StartCoroutine(AttackBuff(AllBuff_Time[value]));
        }
        else if (value == 1)
        {
            StartCoroutine(ArmourBuff(AllBuff_Time[value]));
        }
        else if (value == 2)
        {
            StartCoroutine(ImuBuff(AllBuff_Time[value]));
        }
        else if (value == 3)
        {
            StartCoroutine(BarrierBuff(AllBuff_Time[value]));
        }
        else if (value == 4)
        {
            StartCoroutine(SlowDeBuff(AllBuff_Time[value]));
        }
        else if (value == 5)
        {
            StartCoroutine(AttackSpeedDeBuff(AllBuff_Time[value]));
        }
        else if (value == 6) 
        {
            StartCoroutine(PosionDeBuff(AllBuff_Time[value]));
        }
        StartCoroutine(Count_Time(buff.GetComponent<Image>(), value));
    }
    IEnumerator Count_Time(Image buff_obj,int value) 
    {
        while (AllBuff_Cooldown[value] > 0) 
        {
            yield return new WaitForSeconds(0.1f);
            AllBuff_Cooldown[value] -= 0.1f;
            buff_obj.fillAmount -= 1/(AllBuff_Time[value] / 0.1f);
        }
        AllBuff_Cooldown[value] = 0;
        Destroy(buff_obj.gameObject);
    }

    IEnumerator AttackBuff(float value) 
    {
        player_Stat.Damage_Multiplay = 20;
        player_Stat.setDamage(1);
        yield return new WaitForSeconds(value);
        player_Stat.Damage_Multiplay = 0;
        player_Stat.setDamage(1);
    }

    IEnumerator ArmourBuff(float value) 
    {
        player_Stat.Armour_Multiplay = 20;
        yield return new WaitForSeconds(value);
        player_Stat.Armour_Multiplay = 0;
    }

    IEnumerator ImuBuff(float value)
    {
        player_Stat.IsImu = true;
        yield return new WaitForSeconds(value);
        player_Stat.IsImu = false;
    }

    IEnumerator BarrierBuff(float value) 
    {
        float time = value;
        player_Stat.IsBarrierOn = true;
        player_Stat.BarrierHP = player_Stat.BarrierMaxHP;
        player_Stat.Rpc_SetactiveBarrier();
        while (player_Stat.BarrierHP > 0 && time > 0) 
        {
            yield return new WaitForSeconds(0.5f);
            time -= 0.5f;
        }
        player_Stat.IsBarrierOn = false;
        player_Stat.BarrierHP = 0;
        AllBuff_Cooldown[3] = 0;
        player_Stat.Rpc_SetactiveBarrier();
    }

    IEnumerator SlowDeBuff(float value) 
    {
        player_Move_Control.updateMoveSpeed(-60, -60);
        yield return new WaitForSeconds(value);
        player_Move_Control.updateMoveSpeed(0, 0);
    }

    IEnumerator AttackSpeedDeBuff(float value)
    {
        anim.SetFloat("AttackSpeed", 0.5f);
        yield return new WaitForSeconds(value);
        anim.SetFloat("AttackSpeed", 1f);
    }

    IEnumerator PosionDeBuff(float value)
    {
        float time = value;
        while (time > 0) 
        {
            player_Stat.Player_Take_Damage(posion_damage);
            time -= 1;
            yield return new WaitForSeconds(1);
        }
    }
}
