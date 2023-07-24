using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    ItemData itemData;

    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemPrice;

    InteractableObject interactItem;

    private void Awake()
    {
        interactItem = GetComponent<InteractableObject>();
    }

    private void Start()
    {
        interactItem.Subscibe(BoughtItemToInsert);              //������ ������ �κ��丮�� �ִ� �̺�Ʈ �߰�
    }

    public void SetShopSlot(ItemData itemData)
    {
        this.itemData = itemData;

        itemImage.sprite = itemData.icon;
        itemName.text = itemData.name;
        itemPrice.text = itemData.price.ToString();
    }

    public void BoughtItemToInsert(Inventory inventory)
    {
        if (itemData != null)
        {
            inventory.AddItem(itemData);
        }
    }

    public void BuyButton()
    {     
        Shop.instance.InsertItemToInventory(this, itemData.price);
    }

}
