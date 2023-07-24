using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private StatSlot[] statSlot;
    [SerializeField] private StatSlot[] attributeSlot;

    public void SetStats(StatAttribute stat, Statistic statistic)
    {
        foreach (StatSlot slot in statSlot)
        {
            if (slot.isStat == true && slot.statistic == statistic)
            {
                slot.SetValue(stat);
                break;
            }
        }
    }

    public void SetAttribute(StatAttribute stat, Attribute attribute)
    {
        foreach (StatSlot slot in attributeSlot)
        {
            if (slot.isStat == false && slot.attribute == attribute)
            {
                slot.SetValue(stat);
                break;
            }
        }
    }

}
