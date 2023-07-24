using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currencyText;
    [SerializeField] Inventory playerInventory;

    int currency = -1;

    private void Update()
    {
        if (currency != playerInventory.currency)
        {
            currencyText.text = playerInventory.currency.ToString();
            currency = playerInventory.currency;
        }
    }

}
