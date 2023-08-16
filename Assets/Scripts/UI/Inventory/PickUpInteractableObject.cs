using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInteractableObject : MonoBehaviour
{
    [SerializeField]
    private int coinCount;
    [SerializeField] 
    private ItemData itemData;

    private void Start()
    {
        GetComponent<InteractableObject>().Subscibe(PickUp);
    }

    public void PickUp(Inventory inventory)
    {
        GameEventsManager.instance.miscEvents.ItemCollected();           //아이템 획득 이벤트 알림

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