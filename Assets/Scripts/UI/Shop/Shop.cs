using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    [SerializeField]
    private List<ItemData> itemDatas;

    [SerializeField]
    private GameObject slotPrefab;
    [SerializeField]
    private Transform slotParent;

    private Inventory inventory;

    private void Awake()
    {
        instance = this;
        inventory = FindObjectOfType<Inventory>();
    }

    private void Start()
    {
        SetShopItem();
    }

    private void SetShopItem()
    {
        for (int i = 0; i < itemDatas.Count; i++)
        {
            GameObject clone = Instantiate(slotPrefab, slotParent);
            clone.GetComponent<ShopSlot>().SetShopSlot(itemDatas[i]);
        }
    }

   public void InsertItemToInventory(ShopSlot slot, int amount)
    {
        if (inventory.gold >= amount)
        {
            if (slot.TryGetComponent(out InteractableObject item) == true)
            {
                GameEventsManager.instance.playerEvents.GoldDeducted(amount);

                item.Interact(inventory);

                SoundManager.instance.Play(1);      
            }
        }
        else
        {
            Debug.Log("not enough currency");
        }
    }
}
