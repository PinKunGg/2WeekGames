using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour ,IPointerClickHandler
{
    public int NumberOfSlot;
    public bool IsClothesSlot = false;
    Player_Inventory player_Inventory;
    // Start is called before the first frame update
    void Start()
    {
        player_Inventory = Player_Inventory.player_Inventory;
    }

    // Update is called once per frame
    void Update()
    {
        
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
