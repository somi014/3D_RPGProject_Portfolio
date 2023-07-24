using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int currency = 20;
    [SerializeField] ItemGrid mainInventoryItemGrid;
    [SerializeField] InventoryController inventoryController;

    [SerializeField] List<EquipmentItemSlot> slots;

    StatAttribute character;

    [SerializeField] List<ItemData> itemsOnStart;

    private void Start()
    {
        mainInventoryItemGrid.Init();

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Init(this);
        }

        character = GetComponent<StatAttribute>();

        if (itemsOnStart == null)
            return;
        for (int i = 0; i < itemsOnStart.Count; i++)
        {
            AddItem(itemsOnStart[i]);
        }
    }

    public void SubtractStats(List<StatsValue> stats)
    {
        character.SubtractStats(stats);
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
    }

    public void SubstactCurrency(int amount)
    {
        currency -= amount;
    }

    public bool AddItem(ItemData itemData)
    {
        Vector2Int? positionToPlace = mainInventoryItemGrid.FindSpaceForObject(itemData);

        if (positionToPlace == null)
            return false;

        InventoryItem newItem = inventoryController.CreateNewInventoryItem(itemData);
        mainInventoryItemGrid.PlaceItem(newItem, positionToPlace.Value.x, positionToPlace.Value.y);

        return true;
    }

    public void AddStats(List<StatsValue> statsValue)
    {
        character.AddStats(statsValue);
    }
}
