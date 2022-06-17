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

    [Header("Animator For Change")]
    public RuntimeAnimatorController animAxe;
    public RuntimeAnimatorController animSwordAndShield;
    public RuntimeAnimatorController animMagic;

    [Header("invenSetting")]
    public GameObject InvenUI,CameraPlayer;
    public Player_Move_Control player_Move_Control;
    public Inventory inventory;
    public GameObject prefab_invenSlot;
    public GameObject Inventory_zone;
    public Image[] ClothesSlot_pic = new Image[6];
    public Item[] ClothesSlot_Item = new Item[6];
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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InvenUI.activeSelf == true && !IsFirstRun) 
        {
            IsFirstRun = true;
            CreateInventory();
        }

        if (Input.GetKeyDown(KeyCode.I)) 
        {
            OpenInven();
        }
        if (Input.GetKeyDown(KeyCode.K)) 
        {
            AddItem(Test, 1);
        }
    }

    public void OpenInven() 
    {
        InvenUI.SetActive(!InvenUI.activeSelf);
        CameraPlayer.SetActive(!CameraPlayer.activeSelf);
        UpdateSlot();
    }

    void CreateInventory() 
    {
        for(int x = 0; x < inventory.ItemNameArray.Length;x++) 
        {
            GameObject slot = Instantiate(prefab_invenSlot);
            slot.transform.parent = Inventory_zone.transform;
            slot.transform.localScale = Vector3.one;
            slot.GetComponent<ItemSlot>().NumberOfSlot = x;
            AllSlotPic.Add(slot.GetComponent<Image>());
        }
    }

    void AddItem(Item _item,int amount) 
    {
        foreach (Item item in AllItem) 
        {
            if (_item.NameItem == item.NameItem) 
            {
                if (_item.type == Item.Type.Other)
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

    void SortItem() 
    {
        temp_inven = new Inventory();
        count_temp_inven = 0;

        Item.Type[] types = new Item.Type[7] {Item.Type.Weapond, Item.Type.Clothes_Head ,
            Item.Type.Clothes_Body , Item.Type.Clothes_Pant , Item.Type.Clothes_Feet , Item.Type.Veil , Item.Type.Other };
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
            Debug.Log(inventory.ItemNameArray[x] + " == " + value + " > ");
            Debug.Log(inventory.ItemNameArray[x] == value);
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
            if (inventory.ItemNameArray[x] != "") 
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
                    if (ClothesSlot_Item[x].NameItem == "Axe")
                    {
                        player_Move_Control.gameObject.GetComponent<Animator>().runtimeAnimatorController = animAxe;
                    }
                    else if (ClothesSlot_Item[x].NameItem == "Sword&Shield")
                    {
                        player_Move_Control.gameObject.GetComponent<Animator>().runtimeAnimatorController = animSwordAndShield;
                    }
                    else if (ClothesSlot_Item[x].NameItem == "Stuff") 
                    {
                        player_Move_Control.gameObject.GetComponent<Animator>().runtimeAnimatorController = animMagic;
                    }
                }
            }
        }
    }

    public void SelectSlot(int slotnumber) 
    {
        if (inventory.ItemNameArray[slotnumber] == "") { return; }
        foreach (Item item in AllItem) 
        {
            if (inventory.ItemNameArray[slotnumber] == item.NameItem) 
            {
                if (item.type == Item.Type.Other) { return; }
                else if (item.type == Item.Type.Clothes_Head)
                {
                    ClothesSlot_pic[0].sprite = item.ItemPic;
                    ClothesSlot_Item[0] = item;
                }
                else if (item.type == Item.Type.Clothes_Body)
                {
                    ClothesSlot_pic[1].sprite = item.ItemPic;
                    ClothesSlot_Item[1] = item;
                }
                else if (item.type == Item.Type.Clothes_Pant)
                {
                    ClothesSlot_pic[2].sprite = item.ItemPic;
                    ClothesSlot_Item[2] = item;
                }
                else if (item.type == Item.Type.Clothes_Feet)
                {
                    ClothesSlot_pic[3].sprite = item.ItemPic;
                    ClothesSlot_Item[3] = item;
                }
                else if (item.type == Item.Type.Veil)
                {
                    ClothesSlot_pic[4].sprite = item.ItemPic;
                    ClothesSlot_Item[4] = item;
                }
                else if (item.type == Item.Type.Weapond)
                {
                    ClothesSlot_pic[5].sprite = item.ItemPic;
                    ClothesSlot_Item[5] = item;
                }
                break;
            }
        }
        UpdateStat();
    }

    //void LoadItem() 
    //{
    //    for (int x = 0; x< inventory.ItemIndexArray.Length;x++) 
    //    {
    //        if (inventory.ItemNameArray[x] == "") { return; }
    //        foreach (Item item in AllItem)
    //        {
    //            if (inventory.ItemNameArray[x] == item.NameItem)
    //            {
    //                AddItem(item, inventory.ItemIndexArray[x]);
    //                break;
    //            }
    //        }
    //    }
    //}
}
[System.Serializable]
public class Inventory 
{
    public string[] ItemNameArray = new string[40];
    public int[] ItemIndexArray = new int[40];
}
