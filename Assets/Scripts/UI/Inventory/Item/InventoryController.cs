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
    private InventoryItem selectedItem;                             //선택한 아이템
    private InventoryItem overlapItem;                              //겹쳐지는 아이템
    private RectTransform selectedItemRectTransform;                

    [SerializeField] private List<ItemData> itemDatas;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform targetCanvas;
                
    [SerializeField] private InventoryHighlight inventoryHighlight;
    [SerializeField] private RectTransform selectedItemParent;

    private InventoryItem itemToHighligt;

    private Vector2Int oldPosition;
    
    private bool isOverUIElement;
    public bool sellItem;                       //아이템 판매할 수 있는지

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

    #region 마우스 위치 & 하이라이터 업데이트
    private void Update()
    {
        isOverUIElement = EventSystem.current.IsPointerOverGameObject();            //인벤토리 위에 마우스가 있는지

        ProcessMousePosition();

        ProcessMouseInput();

        HandleHighlight();              
    }

    /// <summary>
    /// 현재 마우스 위치 값
    /// </summary>
    private void ProcessMousePosition()
    {
        mousePosition = mouseInput.mouseInputPosition;
    }

    /// <summary>
    /// 선택한 아이템 위치 마우스 위치 따라가게
    /// </summary>
    private void ProcessMouseInput()
    {
        if (selectedItem != null)
        {
            selectedItemRectTransform.position = mousePosition;
        }
    }

    /// <summary>
    /// 아이템 하이라이터
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

    #region 랜덤 아이템 추가
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
    /// 아이템 획득, 구매 시 새 아이템 오브젝트 생성
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
    /// 선택한 아이템 정보 업데이트
    /// </summary>
    /// <param name="inventoryItem"></param>
    public void SelectItem(InventoryItem inventoryItem)
    {
        selectedItem = inventoryItem;
        selectedItemRectTransform = inventoryItem.GetComponent<RectTransform>();
        selectedItemRectTransform.SetParent(selectedItemParent);
    }

    /// <summary>
    /// 마우스 클릭했을 때
    /// </summary>
    /// <param name="context"></param>
    public void ProcessLeftMouseButtonPress(InputAction.CallbackContext context)
    {
        if (sellItem == true)                                           //상점 팔기에 마우스가 있을 때
        {
            SellInventoryItem();
            return;
        }

        if (selectedItemGrid == null && selectedItemSlot == null)       //인벤토리 또는 장비 창에 마우스가 없을 때
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
    /// 아이템 판매하기
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
    /// 아이템 버리기
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
    /// 아이템 장비하기
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
        if (replaceItem == null)        //장착하고 있던 아이템이 없으면
        {
            NullSelectedItem();
        }
        else
        {
            SelectItem(replaceItem);
        }
    }

    /// <summary>
    /// 선택한 아이템 초기화
    /// </summary>
    private void NullSelectedItem()
    {
        selectedItem = null;
        selectedItemRectTransform = null;
    }

    /// <summary>
    /// 인벤토리에 아이템 놓기
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
    /// 마우스 위치를 아이템 크기에 따라 값 변환 후 인벤토리에서 위치 반환
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
    /// 아이템 위치 시키기
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

        if (overlapItem != null)                    //겹치는 아이템을 인벤토리에서 제거
        {
            selectedItemGrid.ClearGridFromItem(overlapItem);
        }

        selectedItemGrid.PlaceItem(selectedItem, positionOnGrid.x, positionOnGrid.y);
        NullSelectedItem();

        if (overlapItem != null)
        {
            selectedItem = overlapItem;             //겹치는 아이템을 선택한 아이템으로 (옮기기)
            selectedItemRectTransform = selectedItem.GetComponent<RectTransform>();
            selectedItemRectTransform.SetParent(selectedItemParent);
            overlapItem = null;
        }
    }
}