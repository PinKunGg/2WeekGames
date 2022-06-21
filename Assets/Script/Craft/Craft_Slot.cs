using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Craft_Slot : MonoBehaviour , IPointerClickHandler
{
    public Craft_Control craft_Control;
    public Image item_pic;
    public Item item;
    void Start()
    {
        item_pic.sprite = item.ItemPic;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        craft_Control.SelectFormCraftSlot(item);
    }
}
