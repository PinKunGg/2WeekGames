using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Monster_Stat : MonoBehaviour
{
    Tutorial_Control tutorial_Control;
    public Player_Inventory Player_Inventory;
    PhotonView photonView;
    public BaseMonsterStat baseMonsterStat;
    [Header("Monster Setting")]
    public int BossStageNumber;
    public float Current_HP;
    public Slider Monster_HealthBar;

    public Item itemDrop;
    public int[] AmountRangeitemDrop = new int[2];
    PlayerWeaponDamage forDot;
    PlayerManager_Multiplayer playerManMulti;
    public GameObject WinUI;
    Vector3 spawnPos = Vector3.zero;

    public bool IsDie {get;private set;}
    int playerDamageID;

    void Start()
    {
        tutorial_Control = Tutorial_Control.tutorial_Control;
        if (tutorial_Control.IsTutorial) 
        {
            playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
        }
    }

    private void OnEnable()
    {
        photonView = GetComponent<PhotonView>();
        Player_Inventory = Player_Inventory.player_Inventory;
        UpdateMonsterCurrentStat();
        GetComponent<Boar_Attacker>()?.DelayAttacker();
        GetComponent<Arachne_Attacker>()?.DelayAttacker();
        GetComponent<Assasin_Attacker>()?.DelayAttacker();
        if (PhotonNetwork.IsMasterClient)
        {
            if (spawnPos != Vector3.zero) { this.gameObject.transform.position = spawnPos; }
            else { spawnPos = this.gameObject.transform.position; }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (forDot == null) 
        {
            CancelInvoke("Dotdamage");
        }
    }

    void UpdateMonsterCurrentStat() 
    {
        Current_HP = baseMonsterStat.base_HP;
        Monster_HealthBar = GameObject.FindGameObjectWithTag("MonsterHealthBar").GetComponent<Slider>();
        Monster_HealthBar.maxValue = Current_HP;
        Monster_HealthBar.value = Current_HP;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon")) 
        {
            Debug.Log(other.gameObject.name);
            float totoal_damage = 0;
            PlayerWeaponDamage playerWeaponDamage = other.GetComponent<PlayerWeaponDamage>();
            if (!playerWeaponDamage.IsDotDamage)
            {
                if (Random.value * 100 < playerWeaponDamage.CriRate)
                {
                    totoal_damage = playerWeaponDamage.Damage * ((playerWeaponDamage.CriDamage + 100) / 100);
                }
                else { totoal_damage = playerWeaponDamage.Damage; }
                photonView.RPC("RPC_SendDamageToPlayerManagerMultiplayer",RpcTarget.MasterClient,other.GetComponent<PhotonView>().OwnerActorNr,totoal_damage);
                photonView.RPC("TakeDamage", RpcTarget.All, totoal_damage);
            }
            else 
            {
                forDot = playerWeaponDamage;
                playerDamageID = other.GetComponent<PhotonView>().OwnerActorNr;
                InvokeRepeating("Dotdamage", 0.5f, 0.5f);
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            float totoal_damage = 0;
            PlayerWeaponDamage playerWeaponDamage = other.GetComponent<PlayerWeaponDamage>();
            if (!playerWeaponDamage.IsDotDamage)
            {
                if (Random.value * 100 < playerWeaponDamage.CriRate)
                {
                    totoal_damage = playerWeaponDamage.Damage * ((playerWeaponDamage.CriDamage + 100) / 100);
                }
                else { totoal_damage = playerWeaponDamage.Damage; }
                photonView.RPC("RPC_SendDamageToPlayerManagerMultiplayer",RpcTarget.MasterClient,other.GetComponent<PhotonView>().OwnerActorNr,totoal_damage);
                photonView.RPC("TakeDamage", RpcTarget.All, totoal_damage);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon"))
        {
            Debug.Log("Exit : "+other.gameObject.name);
            PlayerWeaponDamage playerWeaponDamage = other.GetComponent<PlayerWeaponDamage>();
            if (playerWeaponDamage.IsDotDamage)
            {
                CancelInvoke("Dotdamage");
            }
        }
    }

    void Dotdamage() 
    {
        float totoal_damage = 0;
        if (Random.value * 100 < forDot.CriRate)
        {
            totoal_damage = forDot.Damage * ((forDot.CriDamage + 100) / 100);
        }
        else { totoal_damage = forDot.Damage; }
        photonView.RPC("RPC_SendDamageToPlayerManagerMultiplayer",RpcTarget.MasterClient,playerDamageID,totoal_damage);
        photonView.RPC("TakeDamage", RpcTarget.All, totoal_damage);

    }

    [PunRPC]
    void TakeDamage(float damage) 
    {
        if (Monster_HealthBar.value > 0)
        {
            Current_HP -= damage;
            Monster_HealthBar.value = Current_HP;
        }
        else 
        {
            if (photonView.IsMine) 
            {
                if (IsDie == false) 
                {
                    checkIsdie();
                }
            }
        }
    }

    void checkIsdie() 
    {
        photonView.RPC("Rpc_monsterDie", RpcTarget.All);
    }

    [PunRPC]
    void Rpc_monsterDie() 
    {
        if (!tutorial_Control.IsTutorial) { WinUI.SetActive(true); }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        IsDie = true;
        GetComponent<Animator>().SetBool("IsDie", true);
        Monster_HealthBar.gameObject.SetActive(false);
        DropItem();
    }

    [PunRPC]
    void RPC_SendDamageToPlayerManagerMultiplayer(int playerID, float damage){
        playerManMulti = FindObjectOfType<PlayerManager_Multiplayer>();
        playerManMulti.AddPlayerDamage(playerID,damage);
    }

    void DropItem() 
    {
        if (!TutorialCheck(9)) { return; }
        Debug.Log("Drop : " + itemDrop.NameItem);
        Player_Inventory.BossUnlockStage = BossStageNumber;
        Player_Inventory.updateCraft();
        Player_Inventory.AddItem(itemDrop, Random.Range(AmountRangeitemDrop[0], AmountRangeitemDrop[1]));
        MonoBehaviour[] allscript = GetComponents<MonoBehaviour>();
        for (int x = 0; x < allscript.Length; x++) 
        {
            if (allscript[x] != this) { allscript[x].enabled = false; }
        }
        if (!tutorial_Control.IsTutorial) { Invoke("DelayBactToLobby", 5); }
    }

    void DelayBactToLobby() 
    {
        GetComponent<Animator>().SetBool("IsDie", false);
        WinUI.SetActive(false);
        Monster_HealthBar.gameObject.SetActive(false);
        this.gameObject.transform.position = spawnPos;
        this.gameObject.transform.rotation = Quaternion.Euler(0f,180f,0f);
        Current_HP = baseMonsterStat.base_HP;
        MonoBehaviour[] allscript = GetComponents<MonoBehaviour>();
        for (int x = 0; x < allscript.Length; x++)
        {
            if (allscript[x] != this) { allscript[x].enabled = true; }
        }
        photonView.RPC("SaveInven", RpcTarget.All);
        Invoke("DelayDisable",2f);
    }

    void DelayDisable(){
        IsDie = false;
        this.gameObject.SetActive(false);
    }

    [PunRPC]
    void SaveInven() 
    {
        Player_Inventory.SaveCloth();
        Player_Inventory.SaveItem();
        LobbyControl.lobbyControl.EndGame();
    }
    public void MultiPlyHealth(int multiply) 
    {
        Current_HP = baseMonsterStat.base_HP * multiply;
        Monster_HealthBar = GameObject.FindGameObjectWithTag("MonsterHealthBar").GetComponent<Slider>();
        Monster_HealthBar.maxValue = Current_HP;
        Monster_HealthBar.value = Current_HP;
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

[System.Serializable]
public class BaseMonsterStat
{
    public string Name_Monster;
    public float base_HP = 100;
    public float base_Armor = 5;
    public float base_Damage = 0;
}
