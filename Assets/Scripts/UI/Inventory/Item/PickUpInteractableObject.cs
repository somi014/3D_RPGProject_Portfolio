using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInteractableObject : MonoBehaviour
{
    [SerializeField] int coinCount;
    [SerializeField] ItemData itemData;

    private void Start()
    {
        GetComponent<InteractableObject>().Subscibe(PickUp);
    }

    public void PickUp(Inventory inventory)
    {
        GameEventsManager.instance.miscEvents.ItemCollected();           //æ∆¿Ã≈€ »πµÊ ¿Ã∫•∆Æ æÀ∏≤

        GameEventsManager.instance.playerEvents.GoldGained(coinCount);

        if(itemData != null)
        {
            inventory.AddItem(itemData);
        }

        Destroy(gameObject);
    }

    public void SetItem(ItemData itemToSpawn)
    {
        itemData = itemToSpawn;
    }
}
