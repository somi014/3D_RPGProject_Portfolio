using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [HideInInspector] public int currency = 20;                   //�÷��̾� ���

    [SerializeField] private ItemGrid mainInventoryItemGrid;
    [SerializeField] private InventoryController inventoryController;

    [SerializeField] private List<EquipmentItemSlot> slots;

    private StatAttribute character;

    [SerializeField] private List<ItemData> itemsOnStart;       //���� ������

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
    /// ���� ����
    /// </summary>
    /// <param name="statsValue"></param>
    public void AddStats(List<StatsValue> statsValue)
    {
        character.AddStats(statsValue);
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    /// <param name="stats"></param>
    public void SubtractStats(List<StatsValue> stats)
    {
        character.SubtractStats(stats);
    }

    /// <summary>
    /// ��� �߰�
    /// </summary>
    /// <param name="amount"></param>
    public void AddCurrency(int amount)
    {
        currency += amount;
    }

    /// <summary>
    /// ��� ����
    /// </summary>
    /// <param name="amount"></param>
    public void SubstactCurrency(int amount)
    {
        currency -= amount;
    }

    /// <summary>
    /// ������ ȹ��, ���� �� �κ��丮�� �߰�
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
