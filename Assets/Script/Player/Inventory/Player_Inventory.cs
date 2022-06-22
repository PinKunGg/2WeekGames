using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player_Inventory : MonoBehaviour
{
    public static Player_Inventory player_Inventory;

    private void Awake()
    {
        player_Inventory = this;
    }

    Tutorial_Control tutorial_Control;
    public Player_Stat player_Stat;
    public Sprite Defath_Sprite;

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

    Inventory temp_inven;
    int count_temp_inven = 0;

    [Header("armor stat")]
    public float HP = 0;
    public float Armor = 0;
    public float Damage = 0;
    public float Cri_Rate = 0;
    public float Cri_Damage = 0;

    public Item Test;
    // Start is called before the first frame update
    void Start()
    {
        tutorial_Control = Tutorial_Control.tutorial_Control;
        if (InvenUI.activeSelf == true && !IsFirstRun)
        {
            IsFirstRun = true;
            CreateInventory();
            LoadItem();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) 
        {
            OpenInven();
        }
        if (Input.GetKeyDown(KeyCode.K)) 
        {
            AddItem(Test, 1);
        }
        if (Input.GetKeyDown(KeyCode.L)) 
        {
            SaveItem();
            //RemoveItem(Test, 2);
        }
    }

    public void OpenInven() 
    {
        if (!TutorialCheck(10)) { return; }
        InvenUI.SetActive(!InvenUI.activeSelf);
        CameraPlayer.SetActive(!CameraPlayer.activeSelf);
        player_Move_Control.SwitchCursor(InvenUI.activeSelf);
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
            inventory.ItemNameArray[x] = null;
            inventory.ItemIndexArray[x] = 0;
            AllSlotPic.Add(slot.GetComponent<Image>());
        }
        InvenUI.SetActive(!InvenUI.activeSelf);
    }

    public void AddItem(Item _item,int amount) 
    {
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

    void UpdateSlot() 
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
                        break;
                    }
                }
            }
            else if(inventory.ItemNameArray[x] == null)
            {
                AllSlotPic[x].sprite = Defath_Sprite;
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
                    }
                    else if (ClothesSlot_Item[x].type_Weapon == Item.Type_Weapon.SwordAndShield)
                    {
                        player_Move_Control.gameObject.GetComponent<Animator>().runtimeAnimatorController = animSwordAndShield;
                    }
                    else if (ClothesSlot_Item[x].type_Weapon == Item.Type_Weapon.Stuff) 
                    {
                        player_Move_Control.gameObject.GetComponent<Animator>().runtimeAnimatorController = animMagic;
                    }
                    else if (ClothesSlot_Item[x].type_Weapon == Item.Type_Weapon.Bow)
                    {
                        player_Move_Control.gameObject.GetComponent<Animator>().runtimeAnimatorController = animBow;
                    }
                }
            }
        }
        player_Stat = player_Move_Control.gameObject.GetComponent<Player_Stat>();
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
                        ClothesSlot_pic[slotnumber].sprite = Defath_Sprite;
                    }
                    else if (inventory.ItemIndexArray[slotnumber] - 5 < 0)
                    {
                        PotionSlot = inventory.ItemIndexArray[slotnumber];
                        inventory.ItemNameArray[slotnumber] = null;
                        inventory.ItemIndexArray[slotnumber] = 0;
                        ClothesSlot_pic[slotnumber].sprite = Defath_Sprite;
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
        }
    }

    void LoadItem()
    {
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
        Debug.Log("Load : " + ClothesSlot_Item.Length);
        for (int y = 0; y < ClothesSlot_Item.Length; y++)
        {
            if (saveClothes.SaveClothesSlot_Item[y] != null)
            {
                Debug.Log("Load : " + saveClothes.SaveClothesSlot_Item[y].NameItem);
                ClothesSlot_Item[y] = saveClothes.SaveClothesSlot_Item[y];
                ClothesSlot_pic[y].sprite = saveClothes.SaveClothesSlot_Item[y].ItemPic;
                if (saveClothes.SaveClothesSlot_Item[y].type == Item.Type.Veil) 
                {
                    player_Move_Control.gameObject.GetComponent<Player_Buff_Control>().Veil_Buff_int = ClothesSlot_Item[y].Veil_Skill;
                    player_Move_Control.dash_cooldown = ClothesSlot_Item[y].Veil_Dash_Cooldown;
                    player_Move_Control.IsCanDash = true;
                    player_Move_Control.gameObject.GetComponent<Player_Skill_Control>().init();
                }
            }
        }
        UpdateStat();
    }

    void SaveItem() 
    {
        for (int x = 0; x < inventory.ItemNameArray.Length; x++)
        {
            if (inventory.ItemNameArray[x] != null) 
            {
                saveInventory.ItemNameArray[x] = inventory.ItemNameArray[x];
                saveInventory.ItemIndexArray[x] = inventory.ItemIndexArray[x];
            }
        }
    }

    bool TutorialCheck(int stage)
    {
        if (stage > 0)
        {
            if (tutorial_Control.IsTutorial)
            {
                if (tutorial_Control.Stage[stage - 1])
                {
                    tutorial_Control.CompleteStage(stage);
                    return true;
                }
                else { return false; }
            }
            return true;
        }
        else
        {
            tutorial_Control.CompleteStage(stage);
            return true;
        }
    }
}

[System.Serializable]
public class SaveClothes 
{
    public Item[] SaveClothesSlot_Item = new Item[8];
}

[System.Serializable]
public class SaveInventory
{
    public string[] ItemNameArray = new string[40];
    public int[] ItemIndexArray = new int[40];
}

[System.Serializable]
public class Inventory 
{
    public string[] ItemNameArray = new string[40];
    public int[] ItemIndexArray = new int[40];
}
