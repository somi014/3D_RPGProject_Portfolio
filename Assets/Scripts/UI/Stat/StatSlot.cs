using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatSlot : MonoBehaviour
{
    [Header("Stat Attribute Type")]
    public Statistic statistic;
    public Attribute attribute;

    private TextMeshProUGUI currentValueTxt;
    private TextMeshProUGUI extraValueTxt;

    [Header("Stats or Attribute")]
    [SerializeField] 
    public bool isStat;

    private void Awake()
    {
        currentValueTxt = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        extraValueTxt = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    public void SetValue(StatAttribute stat)
    {
        if (currentValueTxt == null)
        {
            return;
        }

        if (stat.TakeStats(statistic).typeFloat == true)
        {
            currentValueTxt.text = stat.TakeStats(statistic).float_value.ToString();
            extraValueTxt.text = "(+" + stat.TakeStats(statistic).extra_float_value.ToString() + ")";
        }
        else
        {
            currentValueTxt.text = stat.TakeStats(statistic).integer_value.ToString();
            extraValueTxt.text = "(+" + stat.TakeStats(statistic).extra_integer_value.ToString() + ")";
        }
    }
}
