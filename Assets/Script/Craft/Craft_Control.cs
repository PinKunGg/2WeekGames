using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Craft_Control : MonoBehaviour
{
    public Player_Inventory player_inventory;
    public Craft_Slot FirstSlot;

    public static int SetUnlockCount;
    public GameObject[] AllItemSet = new GameObject[12];

    public GameObject Main_Craft_UI;
    public GameObject Info_UI;
    public TextMeshProUGUI Name_Item,Hp,Armour,Damage,Cri_Rate,Cri_Damage;
    public TextMeshProUGUI CraftItemamount1,CraftItemamount2;
    public Image Item_Pic,Craft_Pic1,Craft_Pic2;
    public Item Craft_Item1, Craft_Item2;
    public int Craft_amount1,Craft_amount2;
    Item item_selected;

    bool Isopen = false;

    // Start is called before the first frame update
    void Start()
    {
        Main_Craft_UI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) 
        {
            Main_Craft_UI.SetActive(!Main_Craft_UI.activeSelf);
        }

        if (Main_Craft_UI.activeSelf && !Isopen)
        {
            Isopen = true;
            SelectFormCraftSlot(FirstSlot.item);
            UpdateSet();
        }
        else if (!Main_Craft_UI.activeSelf && Isopen)
        {
            Isopen = false;
        }
    }

    void UpdateSet() 
    {
        foreach (GameObject set in AllItemSet) 
        {
            set.SetActive(false);
        }
        if (SetUnlockCount == 5)
        {
            AllItemSet[11].SetActive(true);
            AllItemSet[10].SetActive(true);
        }
        if (SetUnlockCount == 4) 
        {
            AllItemSet[9].SetActive(true);
            AllItemSet[8].SetActive(true);
        }
        if (SetUnlockCount == 3)
        {
            AllItemSet[7].SetActive(true);
            AllItemSet[6].SetActive(true);
        }
        if (SetUnlockCount == 2)
        {
            AllItemSet[5].SetActive(true);
            AllItemSet[4].SetActive(true);
        }
        if (SetUnlockCount == 1)
        {
            AllItemSet[3].SetActive(true);
            AllItemSet[2].SetActive(true);
        }
        if (SetUnlockCount == 0)
        {
            AllItemSet[1].SetActive(true);
            AllItemSet[0].SetActive(true);
        }
    }

    public void SelectFormCraftSlot(Item item) 
    {
        Name_Item.text = item.NameItem;
        Hp.text = "HP : " + item.HP;
        Armour.text = "Def : " + item.Armor;
        Damage.text = "Atk : " + item.Damage;
        Cri_Rate.text = "Cri R : " + item.CriRate;
        Cri_Damage.text = "Cri D : " + item.CriDamage;
        Item_Pic.sprite =  item.ItemPic;
        item_selected = item;
        if (item.ItemForCraft[0] != null)
        {
            CraftItemamount1.gameObject.SetActive(true);
            Craft_Pic1.gameObject.SetActive(true);
            Craft_Pic1.sprite = item.ItemForCraft[0].ItemPic;
            Craft_Item1 = item.ItemForCraft[0];
            Craft_amount1 = item.CountItemForCraft[0];
            CraftItemamount1.text = player_inventory.GetAmountItemByItem(Craft_Item1) + "/" + Craft_amount1;
        }
        else 
        {
            Craft_Item1 = null;
            CraftItemamount1.gameObject.SetActive(false);
            Craft_Pic1.gameObject.SetActive(false);
        }
        if (item.ItemForCraft[1] != null)
        {
            CraftItemamount2.gameObject.SetActive(true);
            Craft_Pic2.gameObject.SetActive(true);
            Craft_Pic2.sprite = item.ItemForCraft[1].ItemPic;
            Craft_Item2 = item.ItemForCraft[1];
            Craft_amount2 = item.CountItemForCraft[1];
            CraftItemamount2.text = player_inventory.GetAmountItemByItem(Craft_Item2) + "/" + Craft_amount2;
        }
        else
        {
            Craft_Item2 = null;
            CraftItemamount2.gameObject.SetActive(false);
            Craft_Pic2.gameObject.SetActive(false);

        }
    }

    public void Craft() 
    {
        if (Craft_amount1 != 0) 
        {
            if (Craft_amount1 > player_inventory.GetAmountItemByItem(Craft_Item1) && player_inventory.GetAmountItemByItem(Craft_Item1) != 101)
            {
                return;
            }
        }
        if (Craft_amount2 != 0) 
        {
            if (Craft_amount2 > player_inventory.GetAmountItemByItem(Craft_Item2) && player_inventory.GetAmountItemByItem(Craft_Item2) != 101)
            {
                return;
            }
        }
        player_inventory.RemoveItem(Craft_Item1, Craft_amount1);
        player_inventory.RemoveItem(Craft_Item2, Craft_amount2);
        player_inventory.AddItem(item_selected, 1);
        Debug.Log("Crafted");


    }
}
