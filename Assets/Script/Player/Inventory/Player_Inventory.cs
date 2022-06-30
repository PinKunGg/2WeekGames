using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Player_Inventory : MonoBehaviour
{
    public static Player_Inventory player_Inventory;

    private void Awake()
    {
        player_Inventory = this;
    }

    JsonSaveSystem js;
    Tutorial_Control tutorial_Control;
    public Player_Stat player_Stat;
    public Sprite Defath_Sprite;
    public TextMeshProUGUI PotionText;
    ItemSlot[] allItemSlot = new ItemSlot[40];
    public string player_name;
    public TextMeshProUGUI PlayerStat_Value;

    [Header("Animator For Change")]
    public RuntimeAnimatorController animAxe;
    public RuntimeAnimatorController animSwordAndShield;
    public RuntimeAnimatorController animMagic;
    public RuntimeAnimatorController animBow;

    [Header("invenSetting")]
    public GameObject InvenUI,CameraPlayer;
    public Player_Move_Control player_Move_Control;
    public Inventory inventory;
    public SaveInventory saveInventory;
    public GameObject prefab_invenSlot;
    public GameObject Inventory_zone;
    public Image[] ClothesSlot_pic = new Image[7];
    public Item[] ClothesSlot_Item = new Item[7];
    public SaveClothes saveClothes;
    public int PotionSlot = 0;
    public List<Image> AllSlotPic;
    public List<Item> AllItem;
    bool IsFirstRun = false;
    public int BossUnlockStage = 0;
    public Craft_Control craft_Control;

    Inventory temp_inven;
    int count_temp_inven = 0;

    [Header("armor stat")]
    public float HP = 0;
    public float Armor = 0;
    public float Damage = 0;
    public float Cri_Rate = 0;
    public float Cri_Damage = 0;

    [Header("SkillPic")]
    public Image[] SkillShow = new Image[3];
    public Sprite[] AxeSkillPic;
    public Sprite[] SwordAndShieldSkillPic;
    public Sprite[] BowSkillPic;
    public Sprite[] StuffSkillPic;
    public Sprite[] DashSkillPic;

    public Item Test;
    // Start is called before the first frame update
    void Start()
    {
        js = GetComponent<JsonSaveSystem>();
        tutorial_Control = Tutorial_Control.tutorial_Control;
        if (InvenUI.activeSelf == true && !IsFirstRun)
        {
            IsFirstRun = true;
            CreateInventory();
        }
    }

    public void LoadSave() 
    {
        LoadItem();
        LoadCloth();
    }

    // Update is called once per frame
    void Update()
    {
        PotionText.text = PotionSlot.ToString();
        if (Input.GetKeyDown(KeyCode.I) && (tutorial_Control.IsTutorial || tutorial_Control.IsLobby))
        {
            OpenInven();
        }
        //if (Input.GetKeyDown(KeyCode.K)) 
        //{
        //    AddItem(Test, 1);
        //}
        //if (Input.GetKeyDown(KeyCode.L)) 
        //{
        //    SaveItem();
        //    //RemoveItem(Test, 2);
        //}
    }

    public void OpenInven() 
    {
        if (!TutorialCheck(10)) { return; }
        InvenUI.SetActive(!InvenUI.activeSelf);
        if (tutorial_Control.IsLobby == false) 
        {
            CameraPlayer.SetActive(!CameraPlayer.activeSelf);
            CursorSettings.cursor.ToggleCursor(InvenUI.activeSelf);
        }
        FindObjectOfType<Craft_Control>().Main_Craft_UI.SetActive(false);
        UpdateSlot();
    }

    void CreateInventory() 
    {
        for (int x = 0; x < inventory.ItemNameArray.Length;x++) 
        {
            GameObject slot = Instantiate(prefab_invenSlot);
            slot.transform.parent = Inventory_zone.transform;
            slot.transform.localScale = Vector3.one;
            slot.GetComponent<ItemSlot>().NumberOfSlot = x;
            slot.GetComponent<Image>().sprite = Defath_Sprite;
            allItemSlot[x] = slot.GetComponent<ItemSlot>();
            inventory.ItemNameArray[x] = null;
            inventory.ItemIndexArray[x] = 0;
            AllSlotPic.Add(slot.GetComponent<Image>());
        }
        InvenUI.SetActive(!InvenUI.activeSelf);
    }

    public void AddItem(Item _item,int amount) 
    {
        if (_item == null) { return; }
        foreach (Item item in AllItem) 
        {
            if (_item.NameItem == item.NameItem) 
            {
                if (_item.type == Item.Type.Other || _item.type == Item.Type.Potion)
                {
                    if (FindSlotByName(_item.NameItem) == 99)
                    {
                        int pos = FindSlotByName("");
                        inventory.ItemNameArray[pos] = _item.NameItem;
                        inventory.ItemIndexArray[pos] = amount;
                        break;
                    }
                    else 
                    { 
                        int pos = FindSlotByName(_item.NameItem);
                        inventory.ItemNameArray[pos] = _item.NameItem;
                        inventory.ItemIndexArray[pos] += amount;
                        break;
                    }
                }
                else 
                {
                    int pos = FindSlotByName("");
                    inventory.ItemNameArray[pos] = _item.NameItem;
                    inventory.ItemIndexArray[pos] = amount;
                    break;
                }
            }
        }
        SortItem();
    }

    public int GetAmountItemByItem(Item _item)
    {
        int amount = 0;
        if (_item == null) 
        {
            return 101;
        }
        for (int x = 0; x < inventory.ItemNameArray.Length ; x++) 
        {
            if (inventory.ItemNameArray[x] == _item.NameItem) 
            {
                amount += inventory.ItemIndexArray[x];
            }
        }
        return amount;
    }

    public void RemoveItem(Item _item, int amount)
    {
        if (_item == null) { return; }
        int _amount = amount;
        int remove_count = 0;
        foreach (Item item in AllItem)
        {
            if (FindSlotByName(_item.NameItem) == 99)
            {
                break;
            }
            else 
            {
                int pos = FindSlotByName(_item.NameItem);
                if (inventory.ItemIndexArray[pos] - _amount > 0)
                {
                    inventory.ItemIndexArray[pos] = inventory.ItemIndexArray[pos] - _amount;
                    remove_count += _amount;
                    _amount = 0;
                }
                else if (inventory.ItemIndexArray[pos] - _amount <= 0)
                {
                    _amount = _amount - inventory.ItemIndexArray[pos];
                    remove_count += inventory.ItemIndexArray[pos];
                    inventory.ItemNameArray[pos] = "";
                    inventory.ItemIndexArray[pos] = 0;
                }
            }
            

            if (_amount == 0) { break; }
        }
        Debug.Log("Remove : " + _amount);
        if (_amount > 0)
        {
            AddItem(_item, remove_count);
        }
        else 
        {
            SortItem();
        }
    }

    void SortItem() 
    {
        temp_inven = new Inventory();
        count_temp_inven = 0;

        Item.Type[] types = new Item.Type[8] {Item.Type.Weapond, Item.Type.Clothes_Head ,
            Item.Type.Clothes_Body , Item.Type.Clothes_Pant , Item.Type.Clothes_Feet , Item.Type.Veil , Item.Type.Potion , Item.Type.Other };
        for (int cout_temp_type = 0; cout_temp_type < types.Length; cout_temp_type++) 
        {
            for (int x = 0; x < inventory.ItemNameArray.Length; x++)
            {
                foreach (Item item in AllItem)
                {
                    if (inventory.ItemNameArray[x] == item.NameItem)
                    {
                        if (item.type == types[cout_temp_type])
                        {
                            temp_inven.ItemNameArray[count_temp_inven] = inventory.ItemNameArray[x];
                            temp_inven.ItemIndexArray[count_temp_inven] = inventory.ItemIndexArray[x];
                            count_temp_inven++;
                        }
                        break;
                    }
                }
            }
        }
        inventory = temp_inven;
        UpdateSlot();
    }

    int FindSlotByName(string value) 
    {
        for (int x = 0; x < inventory.ItemIndexArray.Length; x++) 
        {
            if (inventory.ItemNameArray[x] == value || inventory.ItemNameArray[x] == null) 
            { 
                return x; 
            }
        }
        return 99;
    }

    public void UpdateSlot() 
    {
        for (int x = 0; x < inventory.ItemNameArray.Length; x++) 
        {
            if (inventory.ItemNameArray[x] != null)
            {
                for (int y = 0; y < AllItem.Count; y++)
                {
                    if (inventory.ItemNameArray[x] == AllItem[y].NameItem)
                    {
                        AllSlotPic[x].sprite = AllItem[y].ItemPic;
                        allItemSlot[x].UpdateText(inventory.ItemIndexArray[x]);
                        break;
                    }
                }
            }
            else if(inventory.ItemNameArray[x] == null)
            {
                AllSlotPic[x].sprite = Defath_Sprite;
                AllSlotPic[x].GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }

    void UpdateStat() 
    {
        HP = 0;
        Armor = 0;
        Damage = 0;
        Cri_Damage = 0;
        Cri_Rate = 0;
        bool is_player_has_weapond = false;
        for (int x = 0; x < ClothesSlot_Item.Length; x++) 
        {
            if (ClothesSlot_Item[x] != null) 
            {
                HP += ClothesSlot_Item[x].HP;
                Armor += ClothesSlot_Item[x].Armor;
                Damage += ClothesSlot_Item[x].Damage;
                Cri_Damage += ClothesSlot_Item[x].CriDamage;
                Cri_Rate += ClothesSlot_Item[x].CriRate;
                if (ClothesSlot_Item[x].type == Item.Type.Weapond)
                {
                    if (ClothesSlot_Item[x].type_Weapon == Item.Type_Weapon.Axe)
                    {
                        player_Move_Control.gameObject.GetComponent<Animator>().runtimeAnimatorController = animAxe;
                        SkillShow[1].sprite = AxeSkillPic[0];
                        SkillShow[2].sprite = AxeSkillPic[1];
                    }
                    else if (ClothesSlot_Item[x].type_Weapon == Item.Type_Weapon.SwordAndShield)
                    {
                        player_Move_Control.gameObject.GetComponent<Animator>().runtimeAnimatorController = animSwordAndShield;
                        SkillShow[1].sprite = SwordAndShieldSkillPic[0];
                        SkillShow[2].sprite = SwordAndShieldSkillPic[1];
                    }
                    else if (ClothesSlot_Item[x].type_Weapon == Item.Type_Weapon.Stuff)
                    {
                        player_Move_Control.gameObject.GetComponent<Animator>().runtimeAnimatorController = animMagic;
                        SkillShow[1].sprite = StuffSkillPic[0];
                        SkillShow[2].sprite = StuffSkillPic[1];
                    }
                    else if (ClothesSlot_Item[x].type_Weapon == Item.Type_Weapon.Bow)
                    {
                        player_Move_Control.gameObject.GetComponent<Animator>().runtimeAnimatorController = animBow;
                        SkillShow[1].sprite = BowSkillPic[0];
                        SkillShow[2].sprite = BowSkillPic[1];
                    }
                    is_player_has_weapond = true;
                }
                else if (ClothesSlot_Item[x].type == Item.Type.Veil) 
                {
                    SkillShow[0].sprite = ClothesSlot_Item[x].ItemPic;
                }
            }
            if (player_Stat == null) { player_Stat = player_Move_Control.gameObject.GetComponent<Player_Stat>(); }
            PlayerStat_Value.text = (HP+ player_Stat.basePlayerStat.base_HP).ToString() + '\n' + 
                (Armor+ player_Stat.basePlayerStat.base_Armor) + '\n' + (Damage + player_Stat.basePlayerStat.base_Damage) + '\n' + 
                (Cri_Rate + player_Stat.basePlayerStat.base_Cri_Rate) + '\n' + (Cri_Damage + player_Stat.basePlayerStat.base_Cri_Damage);
        }
        //player_Stat = player_Move_Control.gameObject.GetComponent<Player_Stat>();
        if (!is_player_has_weapond) { player_Stat.gameObject.GetComponent<Player_Attack_Control>().IsPlayerNoWeapond(true); }
        player_Stat.gameObject.GetComponent<Player_Attack_Control>().CheckWeaponUse();
        player_Stat.SetCurrentStat();
    }

    public void SelectSlot(int slotnumber) 
    {
        if (inventory.ItemNameArray[slotnumber] == null) { return; }
        foreach (Item item in AllItem) 
        {
            if (inventory.ItemNameArray[slotnumber] == item.NameItem) 
            {
                if (item.type == Item.Type.Other) { return; }
                else if (item.type == Item.Type.Potion)
                {
                    if (ClothesSlot_Item[6] != null) { return; }

                    ClothesSlot_pic[6].sprite = item.ItemPic;
                    ClothesSlot_Item[6] = item;
                    if (inventory.ItemIndexArray[slotnumber] - 5 > 0)
                    {
                        inventory.ItemIndexArray[slotnumber] = inventory.ItemIndexArray[slotnumber] - 5;
                        PotionSlot = 5;
                    }
                    else if (inventory.ItemIndexArray[slotnumber] - 5 == 0)
                    {
                        inventory.ItemNameArray[slotnumber] = null;
                        inventory.ItemIndexArray[slotnumber] = 0;
                        PotionSlot = 5;
                    }
                    else if (inventory.ItemIndexArray[slotnumber] - 5 < 0)
                    {
                        PotionSlot = inventory.ItemIndexArray[slotnumber];
                        inventory.ItemNameArray[slotnumber] = null;
                        inventory.ItemIndexArray[slotnumber] = 0;
                    }
                    break;
                }
                else if (item.type == Item.Type.Clothes_Head)
                {
                    if (ClothesSlot_Item[0] != null) { return; }
                    ClothesSlot_pic[0].sprite = item.ItemPic;
                    ClothesSlot_Item[0] = item;
                }
                else if (item.type == Item.Type.Clothes_Body)
                {
                    if (ClothesSlot_Item[1] != null) { return; }
                    ClothesSlot_pic[1].sprite = item.ItemPic;
                    ClothesSlot_Item[1] = item;
                }
                else if (item.type == Item.Type.Clothes_Pant)
                {
                    if (ClothesSlot_Item[2] != null) { return; }
                    ClothesSlot_pic[2].sprite = item.ItemPic;
                    ClothesSlot_Item[2] = item;
                }
                else if (item.type == Item.Type.Clothes_Feet)
                {
                    if (ClothesSlot_Item[3] != null) { return; }
                    ClothesSlot_pic[3].sprite = item.ItemPic;
                    ClothesSlot_Item[3] = item;
                }
                else if (item.type == Item.Type.Veil)
                {
                    if (ClothesSlot_Item[4] != null) { return; }
                    ClothesSlot_pic[4].sprite = item.ItemPic;
                    ClothesSlot_Item[4] = item;
                    player_Stat.gameObject.GetComponent<Player_Buff_Control>().Veil_Buff_int = item.Veil_Skill;
                    player_Move_Control.dash_cooldown = item.Veil_Dash_Cooldown;
                    player_Move_Control.IsCanDash = true;
                    player_Stat.gameObject.GetComponent<Player_Skill_Control>().init();
                }
                else if (item.type == Item.Type.Weapond)
                {
                    if (ClothesSlot_Item[5]!= null) { return; }
                    ClothesSlot_pic[5].sprite = item.ItemPic;
                    ClothesSlot_Item[5] = item;
                }
                inventory.ItemNameArray[slotnumber] = null;
                inventory.ItemIndexArray[slotnumber] = 0;
                player_Stat.gameObject.GetComponent<Player_Attack_Control>().IsPlayerNoWeapond(false);
                break;
            }
        }
        UpdateStat();
        SortItem();
    }

    public void RemoveFormClothesSlot(int slotNumber) 
    {
        if (slotNumber != 6)
        {
            AddItem(ClothesSlot_Item[slotNumber], 1);
            ClothesSlot_Item[slotNumber] = null;
            ClothesSlot_pic[slotNumber].sprite = Defath_Sprite;
            if (slotNumber == 5)
            {
                player_Stat.gameObject.GetComponent<Player_Attack_Control>().IsPlayerNoWeapond(true);
                SkillShow[1].sprite = Defath_Sprite;
                SkillShow[2].sprite = Defath_Sprite;
            }
            else if (slotNumber == 4) 
            {
                player_Move_Control.IsCanDash = false;
            }
            UpdateStat();
        }
        else 
        {
            AddItem(ClothesSlot_Item[slotNumber], PotionSlot);
            ClothesSlot_Item[slotNumber] = null;
            ClothesSlot_pic[slotNumber].sprite = Defath_Sprite;
            PotionSlot = 0;
        }
        LobbyControl.lobbyControl.UnReady();
    }

    public void updateCraft() 
    {
        Debug.Log("updateCraft");
        if (tutorial_Control.IsTutorial) { return; }
        craft_Control.SetUnlockCount = saveInventory.BossUnlockStage;
        craft_Control.UpdateSet();
        if (PhotonNetwork.IsMasterClient) { FindObjectOfType<LobbyControl>().LoadSelectStage(); }
    }

    void LoadItem()
    {
        string data = js.LoadJson(Application.persistentDataPath, "Saveinventory" + PhotonNetwork.MasterClient.NickName + player_name);
        JsonUtility.FromJsonOverwrite(data, saveInventory);
        updateCraft();
        for (int x = 0; x < saveInventory.ItemNameArray.Length; x++)
        {
            if (saveInventory.ItemNameArray[x] == "") { return; }
            foreach (Item item in AllItem)
            {
                if (saveInventory.ItemNameArray[x] == item.NameItem)
                {
                    AddItem(item, saveInventory.ItemIndexArray[x]);
                    break;
                }
            }
        }
    }

    public void LoadCloth() 
    {
        Debug.Log("SaveCloth" + player_name);
        string data = js.LoadJson(Application.persistentDataPath, "SaveCloth" + PhotonNetwork.MasterClient.NickName + player_name);
        JsonUtility.FromJsonOverwrite(data, saveClothes);
        for (int y = 0; y < ClothesSlot_Item.Length; y++)
        {
            if (saveClothes.ItemNameArray[y] != null)
            {
                foreach (Item item in AllItem)
                {
                    if (saveClothes.ItemNameArray[y] == item.NameItem)
                    {
                        ClothesSlot_Item[y] = item;
                        ClothesSlot_pic[y].sprite = item.ItemPic;
                        if (item.type == Item.Type.Veil)
                        {
                            player_Move_Control.gameObject.GetComponent<Player_Buff_Control>().Veil_Buff_int = item.Veil_Skill;
                            player_Move_Control.dash_cooldown = item.Veil_Dash_Cooldown;
                            player_Move_Control.IsCanDash = true;
                            player_Move_Control.gameObject.GetComponent<Player_Skill_Control>().init();
                        }
                        break;
                    }
                }
            }
            else 
            {
                ClothesSlot_Item[y] = null;
                ClothesSlot_pic[y].sprite = Defath_Sprite;
            }
        }
        UpdateStat();
    }

    public void SaveItem() 
    {
        for (int x = 0; x < inventory.ItemNameArray.Length; x++)
        {
            if (inventory.ItemNameArray[x] != null) 
            {
                saveInventory.ItemNameArray[x] = inventory.ItemNameArray[x];
                saveInventory.ItemIndexArray[x] = inventory.ItemIndexArray[x];
            }
        }
        saveInventory.BossUnlockStage[BossUnlockStage] = true;
        string savedata = JsonUtility.ToJson(saveInventory);
        js.SaveJson(Application.persistentDataPath, "Saveinventory" + PhotonNetwork.MasterClient.NickName + player_name, savedata);
    }

    public void SaveCloth() 
    {
        for (int y = 0; y < ClothesSlot_Item.Length; y++)
        {
            if (ClothesSlot_Item[y] != null) { saveClothes.ItemNameArray[y] = ClothesSlot_Item[y].NameItem; }
            else { saveClothes.ItemNameArray[y] = null; }
        }
        string savedata = JsonUtility.ToJson(saveClothes);
        js.SaveJson(Application.persistentDataPath, "SaveCloth" + PhotonNetwork.MasterClient.NickName + player_name, savedata);
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
public class SaveClothes 
{
    public string[] ItemNameArray = new string[8];
}

[System.Serializable]
public class SaveInventory
{
    public string[] ItemNameArray = new string[40];
    public int[] ItemIndexArray = new int[40];

    public bool[] BossUnlockStage = new bool[5] {true,false,false,false,false};
}

[System.Serializable]
public class Inventory 
{
    public string[] ItemNameArray = new string[40];
    public int[] ItemIndexArray = new int[40];
}
