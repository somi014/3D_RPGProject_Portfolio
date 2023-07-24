using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    InventoryItem[,] inventoryItemGrid;     //�κ��丮 ĭ

    public const float TileSizeWidth = 32f;
    public const float TileSizeHeight = 32f;

    [SerializeField] int gridSizeWidth;     //�κ��丮 ���� ������
    [SerializeField] int gridSizeHeight;

    RectTransform rectTransform;

    Vector2 mousePositionOnTheGrid;
    Vector2Int tileGridPosition = new Vector2Int();

    [SerializeField] GameObject inventoryItemPrefab;

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
    /// �κ��丮 Ÿ�Ͽ� ������ ��ġ ��Ű��
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
    /// ������ �߰��� ��ġ ��ȯ, ������ null
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
                if(CheckAvailableSpace(x, y, itemData.sizeWidth, itemData.sizeHeight) == true)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    /// <summary>
    /// �ش� ��ġ�� �������� ���� �� �ִ��� üũ
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
                if (inventoryItemGrid[posX + x, posY + y] != null)      //�κ��丮�� �������� ������
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// �������� �κ��丮���� ��ġ ��ȯ
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x">Ÿ�� ��ġ</param>
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
    /// ���콺 ��ġ�� �κ��丮 Ÿ�� ��ġ�� ��ȯ ��ȯ
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
    /// ��ġ�� ������ �ִ��� üũ
    /// </summary>
    /// <param name="posX">��ġ</param>
    /// <param name="posY"></param>
    /// <param name="sizeWidth">������ ������</param>
    /// <param name="sizeHeight"></param>
    /// <param name="overlapItem">��ġ�� ������</param>
    /// <returns></returns>
    public bool CheckOverlap(int posX, int posY, int sizeWidth, int sizeHeight, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < sizeWidth; x++)
        {
            for (int y = 0; y < sizeHeight; y++)
            {
                if (inventoryItemGrid[posX + x, posY + y] != null)      //�κ��丮�� �������� ������
                {
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
        }

        return true;
    }

    /// <summary>
    /// �κ��丮���� ������ ����
    /// </summary>
    /// <param name="tilePositionOnGrid"></param>
    /// <returns></returns>
    public InventoryItem PickUpItem(Vector2Int tilePositionOnGrid)
    {
        InventoryItem pickedItem = inventoryItemGrid[tilePositionOnGrid.x, tilePositionOnGrid.y];
        if (pickedItem == null)
            return null;

        ClearGridFromItem(pickedItem);

        return pickedItem;
    }

    /// <summary>
    /// �κ��丮���� �ش� ������ ����
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
    /// �κ��丮 ���� ������ üũ
    /// </summary>
    /// <param name="posX">���콺 ��ġ</param>
    /// <param name="posY"></param>
    /// <param name="width">������ ������</param>
    /// <param name="height"></param>
    /// <returns></returns>
    public bool BoundrayCheck(int posX, int posY, int width, int height)
    {
        if (PositionCheck(posX, posY) == false)
            return false;

        posX += width - 1;
        posY += height - 1;

        if (PositionCheck(posX, posY) == false)
            return false;

        return true;
    }
}
