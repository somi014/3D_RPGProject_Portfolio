using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    private ItemGrid selectedItemGrid;
    private EquipmentItemSlot selectedItemSlot;

    [SerializeField] private PlayerStateManager mouseInput;
    private Vector2 mousePosition;

    private Vector2Int positionOnGrid;
    private InventoryItem selectedItem;                             //������ ������
    private InventoryItem overlapItem;                              //�������� ������
    private RectTransform selectedItemRectTransform;                

    [SerializeField] private List<ItemData> itemDatas;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform targetCanvas;
                
    [SerializeField] private InventoryHighlight inventoryHighlight;
    [SerializeField] private RectTransform selectedItemParent;

    private InventoryItem itemToHighligt;

    private Vector2Int oldPosition;
    
    private bool isOverUIElement;
    public bool sellItem;                       //������ �Ǹ��� �� �ִ���

    public EquipmentItemSlot SelectedItemSlot
    {
        get => selectedItemSlot;
        set
        {
            selectedItemSlot = value;
        }
    }

    public ItemGrid SelectedItemGrid
    {
        get => selectedItemGrid;
        set
        {
            selectedItemGrid = value;
            inventoryHighlight.SetParent(value);
        }
    }

    #region ���콺 ��ġ & ���̶����� ������Ʈ
    private void Update()
    {
        isOverUIElement = EventSystem.current.IsPointerOverGameObject();            //�κ��丮 ���� ���콺�� �ִ���

        ProcessMousePosition();

        ProcessMouseInput();

        HandleHighlight();              
    }

    /// <summary>
    /// ���� ���콺 ��ġ ��
    /// </summary>
    private void ProcessMousePosition()
    {
        mousePosition = mouseInput.mouseInputPosition;
    }

    /// <summary>
    /// ������ ������ ��ġ ���콺 ��ġ ���󰡰�
    /// </summary>
    private void ProcessMouseInput()
    {
        if (selectedItem != null)
        {
            selectedItemRectTransform.position = mousePosition;
        }
    }

    /// <summary>
    /// ������ ���̶�����
    /// </summary>
    private void HandleHighlight()
    {
        if (selectedItemSlot != null)
        {
            inventoryHighlight.Show(false);
            return;
        }

        if (selectedItemGrid == null)
        {
            inventoryHighlight.Show(false);
            return;
        }

        Vector2Int positionOnGrid = GetTielGridPosition();
        if (positionOnGrid == oldPosition)
            return;

        if (selectedItemGrid.PositionCheck(positionOnGrid.x, positionOnGrid.y) == false)
            return;

        oldPosition = positionOnGrid;

        if (selectedItem == null)
        {
            itemToHighligt = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

            if (itemToHighligt != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighligt);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighligt);
            }
            else
            {
                inventoryHighlight.Show(false);
            }
        }
        else
        {
            inventoryHighlight.Show(selectedItemGrid.BoundrayCheck(positionOnGrid.x, positionOnGrid.y,
                                                                    selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight));
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }
    #endregion

    #region ���� ������ �߰�
    private void InsertRandomItem()
    {
        if (selectedItemGrid == null)
            return;

        CreateRandomItem();

        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }
    private void CreateRandomItem()
    {
        if (selectedItem != null)
            return;

        int selectedItemID = UnityEngine.Random.Range(0, itemDatas.Count);
        InventoryItem newItem = CreateNewInventoryItem(itemDatas[selectedItemID]);
        SelectItem(newItem);
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert.itemData);

        if (posOnGrid == null)
            return;

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }
    #endregion

    /// <summary>
    /// ������ ȹ��, ���� �� �� ������ ������Ʈ ����
    /// </summary>
    /// <param name="itemData"></param>
    /// <returns></returns>
    public InventoryItem CreateNewInventoryItem(ItemData itemData)
    {
        GameObject newItemGO = Instantiate(inventoryItemPrefab, targetCanvas);

        InventoryItem newInventoryItem = newItemGO.GetComponent<InventoryItem>();

        RectTransform newItemRectTransform = newItemGO.GetComponent<RectTransform>();
        newItemRectTransform.SetParent(targetCanvas);

        newInventoryItem.Set(itemData);

        return newInventoryItem;
    }

    /// <summary>
    /// ������ ������ ���� ������Ʈ
    /// </summary>
    /// <param name="inventoryItem"></param>
    public void SelectItem(InventoryItem inventoryItem)
    {
        selectedItem = inventoryItem;
        selectedItemRectTransform = inventoryItem.GetComponent<RectTransform>();
        selectedItemRectTransform.SetParent(selectedItemParent);
    }

    /// <summary>
    /// ���콺 Ŭ������ ��
    /// </summary>
    /// <param name="context"></param>
    public void ProcessLeftMouseButtonPress(InputAction.CallbackContext context)
    {
        if (sellItem == true)                                           //���� �ȱ⿡ ���콺�� ���� ��
        {
            SellInventoryItem();
            return;
        }

        if (selectedItemGrid == null && selectedItemSlot == null)       //�κ��丮 �Ǵ� ��� â�� ���콺�� ���� ��
        {
            if (isOverUIElement)
            {
                return;
            }
            ThrowItemAwayProcess();
        }

        if (selectedItemGrid != null)
        {
            ItemGridInput();
        }

        if (selectedItemSlot != null)
        {
            ItemSlotInput();
        }
    }

    /// <summary>
    /// ������ �Ǹ��ϱ�
    /// </summary>
    private void SellInventoryItem()
    {
        if (selectedItem != null)
        {
            GameEventsManager.instance.playerEvents.GoldGained(selectedItem.itemData.price);

            SoundManager.instance.Play(1);

            DestroySelectedItemObject();
            NullSelectedItem();
        }
    }
    
    /// <summary>
    /// ������ ������
    /// </summary>
    private void ThrowItemAwayProcess()
    {
        if (selectedItem == null)
            return;

        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        playerPos.x += UnityEngine.Random.Range(-1f, 2f);
        playerPos.z += UnityEngine.Random.Range(-1f, 2f);

        ItemSpawnManager.instance.SpawnItem(playerPos, selectedItem.itemData);
        DestroySelectedItemObject();
        NullSelectedItem();
    }

    private void DestroySelectedItemObject()
    {
        Destroy(selectedItemRectTransform.gameObject);
    }

    /// <summary>
    /// ������ ����ϱ�
    /// </summary>
    private void ItemSlotInput()
    {
        if (selectedItem != null)
        {
            PlaceItemIntoSlot();
        }
    }

    private void PlaceItemIntoSlot()
    {
        if (selectedItemSlot.Check(selectedItem) == false)
            return;

        SoundManager.instance.Play(0);

        InventoryItem replaceItem = selectedItemSlot.ReplaceItem(selectedItem);
        if (replaceItem == null)        //�����ϰ� �ִ� �������� ������
        {
            NullSelectedItem();
        }
        else
        {
            SelectItem(replaceItem);
        }
    }

    /// <summary>
    /// ������ ������ �ʱ�ȭ
    /// </summary>
    private void NullSelectedItem()
    {
        selectedItem = null;
        selectedItemRectTransform = null;
    }

    /// <summary>
    /// �κ��丮�� ������ ����
    /// </summary>
    private void ItemGridInput()
    {
        positionOnGrid = GetTielGridPosition();
        if (selectedItem == null)
        {
            InventoryItem itemToSlect = selectedItemGrid.PickUpItem(positionOnGrid);
            if (itemToSlect != null)
            {
                SelectItem(itemToSlect);
            }
        }
        else
        {
            PlaceItemInput();
        }
    }

    /// <summary>
    /// ���콺 ��ġ�� ������ ũ�⿡ ���� �� ��ȯ �� �κ��丮���� ��ġ ��ȯ
    /// </summary>
    /// <returns></returns>
    Vector2Int GetTielGridPosition()
    {
        Vector2 position = mousePosition;
        if (selectedItem != null)
        {
            position.x -= (selectedItem.itemData.sizeWidth - 1) * ItemGrid.TileSizeWidth / 2;
            position.y += (selectedItem.itemData.sizeHeight - 1) * ItemGrid.TileSizeHeight / 2;
        }

        return selectedItemGrid.GetTileGridPosition(position);
    }

    /// <summary>
    /// ������ ��ġ ��Ű��
    /// </summary>
    private void PlaceItemInput()
    {
        if (selectedItemGrid.BoundrayCheck(positionOnGrid.x, positionOnGrid.y,
                                           selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight) == false)
            return;

        if (selectedItemGrid.CheckOverlap(positionOnGrid.x, positionOnGrid.y,
            selectedItem.itemData.sizeWidth, selectedItem.itemData.sizeHeight, ref overlapItem) == false)
        {
            overlapItem = null;
            return;
        }

        if (overlapItem != null)                    //��ġ�� �������� �κ��丮���� ����
        {
            selectedItemGrid.ClearGridFromItem(overlapItem);
        }

        selectedItemGrid.PlaceItem(selectedItem, positionOnGrid.x, positionOnGrid.y);
        NullSelectedItem();

        if (overlapItem != null)
        {
            selectedItem = overlapItem;             //��ġ�� �������� ������ ���������� (�ű��)
            selectedItemRectTransform = selectedItem.GetComponent<RectTransform>();
            selectedItemRectTransform.SetParent(selectedItemParent);
            overlapItem = null;
        }
    }
}