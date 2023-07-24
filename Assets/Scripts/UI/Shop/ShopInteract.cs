using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController inventoryController;

    private void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.sellItem = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.sellItem = false;
    }    
}
