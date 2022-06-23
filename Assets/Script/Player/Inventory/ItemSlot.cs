using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour ,IPointerClickHandler
{
    public int NumberOfSlot;
    public bool IsClothesSlot = false;
    Player_Inventory player_Inventory;
    TextMeshProUGUI amount_text;
    // Start is called before the first frame update
    void Start()
    {
        player_Inventory = Player_Inventory.player_Inventory;
        amount_text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText(int value) 
    {
        if (amount_text == null) { amount_text = GetComponentInChildren<TextMeshProUGUI>(); }
        Debug.Log("Slot : "+gameObject.name);
        amount_text.text = value.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsClothesSlot)
        {
            player_Inventory.SelectSlot(NumberOfSlot);
        }
        else if (IsClothesSlot) 
        {
            player_Inventory.RemoveFormClothesSlot(NumberOfSlot);
        }
    }
}
