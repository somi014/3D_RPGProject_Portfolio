using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI goldText;
    [SerializeField]
    private Inventory playerInventory;

    private int gold = -1;

    private void Update()
    {
        if (gold != playerInventory.gold)
        {
            goldText.text = playerInventory.gold.ToString();
            gold = playerInventory.gold;
        }
    }

}
