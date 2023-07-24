using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ItemDrop
{
    public ItemData item;
    public int weight = 1;
}


[CreateAssetMenu]
public class ItemDropList : ScriptableObject
{
    public List<ItemDrop> drops;
    public int totalWeight;

    [ContextMenu("Calculate weights")]
    public void CalculateTotalWeight()
    {
        totalWeight = 0;
        foreach (ItemDrop drop in drops)
        {
            totalWeight += drop.weight;
        }

        totalWeight += 1;
    }

    public string GetDropName()
    {
        ItemData itemData = GetDrop();
        return itemData.name;
    }

    public ItemData GetDrop()
    {
        CalculateTotalWeight();

        ItemData toDrop = drops[0].item;
        int roll = Random.Range(0, totalWeight);
        int i = 0;

        while (roll > 0)
        {
            roll -= drops[i].weight;
            toDrop = drops[i].item;
            i++;
        }

        return toDrop;
    }
}
