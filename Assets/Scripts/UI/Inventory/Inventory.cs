using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [HideInInspector] 
    public int gold = 20;                   //플레이어 골드

    [SerializeField] 
    private ItemGrid mainInventoryItemGrid;
    [SerializeField] 
    private InventoryController inventoryController;

    [SerializeField]
    private List<EquipmentItemSlot> slots;

    private StatAttribute character;

    [SerializeField]
    private List<ItemData> itemsOnStart;       //시작 아이템

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

    /// <summary>
    /// 스탯 증가
    /// </summary>
    /// <param name="statsValue"></param>
    public void AddStats(List<StatsValue> statsValue)
    {
        character.AddStats(statsValue);
    }

    /// <summary>
    /// 스탯 차감
    /// </summary>
    /// <param name="stats"></param>
    public void SubtractStats(List<StatsValue> stats)
    {
        character.SubtractStats(stats);
    }

    /// <summary>
    /// 골드 추가
    /// </summary>
    /// <param name="amount"></param>
    public void AddCurrency(int amount)
    {
        gold += amount;
    }

    /// <summary>
    /// 골드 차감
    /// </summary>
    /// <param name="amount"></param>
    public void SubstactCurrency(int amount)
    {
        gold -= amount;
    }

    /// <summary>
    /// 아이템 획득, 구매 시 인벤토리에 추가
    /// </summary>
    /// <param name="itemData"></param>
    /// <returns></returns>
    public bool AddItem(ItemData itemData)
    {
        Vector2Int? positionToPlace = mainInventoryItemGrid.FindSpaceForObject(itemData);

        if (positionToPlace == null)
            return false;

        InventoryItem newItem = inventoryController.CreateNewInventoryItem(itemData);
        mainInventoryItemGrid.PlaceItem(newItem, positionToPlace.Value.x, positionToPlace.Value.y);

        return true;
    }
}
