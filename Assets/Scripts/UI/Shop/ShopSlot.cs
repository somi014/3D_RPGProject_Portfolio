using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    private ItemData itemData;

    [SerializeField] 
    private Image itemImage;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private TextMeshProUGUI itemPrice;

    private InteractableObject interactItem;

    private void Awake()
    {
        interactItem = GetComponent<InteractableObject>();
    }

    private void Start()
    {
        interactItem.Subscibe(BoughtItemToInsert);              //구매한 아이템 인벤토리에 넣는 이벤트 추가
    }

    public void SetShopSlot(ItemData data)
    {
        itemData = data;

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