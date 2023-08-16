using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItemSlot : MonoBehaviour
{
    [SerializeField]
    private EquipmentSlot equipmentSlot;

    private Inventory inventory;
    private InventoryItem itemInSlot;           //장비한 아이템
   
    private RectTransform slotRectTransform;

    private void Awake()
    {
        slotRectTransform = GetComponent<RectTransform>();
    }

    public void Init(Inventory inven)
    {
        inventory = inven;
    }

    public bool Check(InventoryItem itemToPlace)
    {
        return equipmentSlot == itemToPlace.itemData.equipmentSlot;
    }

    public InventoryItem ReplaceItem(InventoryItem itemToPlace)
    {
        InventoryItem replaceItem = itemInSlot;

        if (replaceItem != null)
        {
            inventory.SubtractStats(replaceItem.itemData.stats);
        }

        PlaceItem(itemToPlace);

        return replaceItem;
    }

    public void PlaceItem(InventoryItem itemToPlace)
    {
        itemInSlot = itemToPlace;
        inventory.AddStats(itemInSlot.itemData.stats);

        RectTransform rt = itemToPlace.GetComponent<RectTransform>();
        rt.SetParent(slotRectTransform);
        rt.position = slotRectTransform.position;
    }
}