using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    private InventoryItem[,] inventoryItemGrid;     //인벤토리 칸

    public const float TileSizeWidth = 32f;
    public const float TileSizeHeight = 32f;

    [SerializeField]
    private int gridSizeWidth;                      //인벤토리 가로 사이즈
    [SerializeField]
    private int gridSizeHeight;

    private RectTransform rectTransform;

    private Vector2 mousePositionOnTheGrid;
    private Vector2Int tileGridPosition = new Vector2Int();

    [SerializeField]
    private GameObject inventoryItemPrefab;

    public void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        inventoryItemGrid = new InventoryItem[gridSizeWidth, gridSizeHeight];

        Vector2 size = new Vector2();
        size.x = TileSizeWidth * gridSizeWidth;
        size.y = TileSizeHeight * gridSizeHeight;
        rectTransform.sizeDelta = size;
    }

    public InventoryItem GetItem(int x, int y)
    {
        return inventoryItemGrid[x, y];
    }

    /// <summary>
    /// 인벤토리 타일에 아이템 위치 시키기
    /// </summary>
    /// <param name="itemToPlace"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void PlaceItem(InventoryItem itemToPlace, int x, int y)
    {
        RectTransform itemRectTransform = itemToPlace.GetComponent<RectTransform>();
        itemRectTransform.SetParent(transform);

        for (int ix = 0; ix < itemToPlace.itemData.sizeWidth; ix++)
        {
            for (int iy = 0; iy < itemToPlace.itemData.sizeHeight; iy++)
            {
                inventoryItemGrid[x + ix, y + iy] = itemToPlace;
            }
        }
        itemToPlace.positionOnGridX = x;
        itemToPlace.positionOnGridY = y;

        itemRectTransform.localPosition = CalculatePositionOfObjectOnGrid(itemToPlace, x, y);
    }

    /// <summary>
    /// 아이템 추가할 위치 반환, 없을면 null
    /// </summary>
    /// <param name="itemData"></param>
    /// <returns></returns>
    public Vector2Int? FindSpaceForObject(ItemData itemData)
    {
        int height = gridSizeHeight - itemData.sizeHeight + 1;
        int width = gridSizeWidth - itemData.sizeWidth + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CheckAvailableSpace(x, y, itemData.sizeWidth, itemData.sizeHeight) == true)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 해당 위치에 아이템을 놓을 수 있는지 체크
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <param name="sizeWidth"></param>
    /// <param name="sizeHeight"></param>
    /// <returns></returns>
    private bool CheckAvailableSpace(int posX, int posY, int sizeWidth, int sizeHeight)
    {
        for (int x = 0; x < sizeWidth; x++)
        {
            for (int y = 0; y < sizeHeight; y++)
            {
                if (inventoryItemGrid[posX + x, posY + y] != null)      //인벤토리에 아이템이 있으면
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 아이템의 인벤토리에서 위치 반환
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x">타일 위치</param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector2 CalculatePositionOfObjectOnGrid(InventoryItem item, int x, int y)
    {
        Vector2 positionOnGrid = new Vector2();
        positionOnGrid.x = TileSizeWidth * x + TileSizeWidth * item.itemData.sizeWidth / 2;
        positionOnGrid.y = -(TileSizeHeight * y + TileSizeHeight * item.itemData.sizeHeight / 2);
        return positionOnGrid;
    }

    /// <summary>
    /// 마우스 위치를 인벤토리 타일 위치로 변환 반환
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        mousePositionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        mousePositionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        tileGridPosition.x = (int)(mousePositionOnTheGrid.x / TileSizeWidth);
        tileGridPosition.y = (int)(mousePositionOnTheGrid.y / TileSizeHeight);

        return tileGridPosition;
    }

    /// <summary>
    /// 겹치는 아이템 있는지 체크
    /// </summary>
    /// <param name="posX">위치</param>
    /// <param name="posY"></param>
    /// <param name="sizeWidth">아이템 사이즈</param>
    /// <param name="sizeHeight"></param>
    /// <param name="overlapItem">겹치는 아이템</param>
    /// <returns></returns>
    public bool CheckOverlap(int posX, int posY, int sizeWidth, int sizeHeight, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < sizeWidth; x++)
        {
            for (int y = 0; y < sizeHeight; y++)
            {
                if (inventoryItemGrid[posX + x, posY + y] == null)      //인벤토리에 아이템이 없으면 
                {
                    continue;
                }
              
                if (overlapItem == null)
                {
                    overlapItem = inventoryItemGrid[posX + x, posY + y];
                }
                else
                {
                    if (overlapItem != inventoryItemGrid[posX + x, posY + y])
                    {
                        return false;
                    }
                }

            }
        }

        return true;
    }

    /// <summary>
    /// 인벤토리에서 아이템 선택
    /// </summary>
    /// <param name="tilePositionOnGrid"></param>
    /// <returns></returns>
    public InventoryItem PickUpItem(Vector2Int tilePositionOnGrid)
    {
        InventoryItem pickedItem = inventoryItemGrid[tilePositionOnGrid.x, tilePositionOnGrid.y];
        if (pickedItem == null)
        {
            return null;
        }

        ClearGridFromItem(pickedItem);

        return pickedItem;
    }

    /// <summary>
    /// 인벤토리에서 해당 아이템 제거
    /// </summary>
    /// <param name="pickedItem"></param>
    public void ClearGridFromItem(InventoryItem pickedItem)
    {
        for (int ix = 0; ix < pickedItem.itemData.sizeWidth; ix++)
        {
            for (int iy = 0; iy < pickedItem.itemData.sizeHeight; iy++)
            {
                inventoryItemGrid[pickedItem.positionOnGridX + ix, pickedItem.positionOnGridY + iy] = null;
            }
        }
    }

    /// <summary>
    /// 인벤토리 칸 내부인지 체크
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool PositionCheck(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return false;
        }

        if (x >= gridSizeWidth || y >= gridSizeHeight)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 인벤토리에서 아이템 크기에 따라 범위 체크
    /// </summary>
    /// <param name="posX">마우스 위치</param>
    /// <param name="posY"></param>
    /// <param name="width">아이템 사이즈</param>
    /// <param name="height"></param>
    /// <returns></returns>
    public bool BoundrayCheck(int posX, int posY, int width, int height)
    {
        if (PositionCheck(posX, posY) == false)
        {
            return false;
        }

        posX += width - 1;
        posY += height - 1;

        if (PositionCheck(posX, posY) == false)
        {
            return false;
        }

        return true;
    }
}