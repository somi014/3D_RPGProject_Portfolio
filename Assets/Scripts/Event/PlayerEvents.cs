using System;
using UnityEngine;

public class PlayerEvents
{
    public event Action onPlayerDead;
    public void PlayerDead()
    {
        if(onPlayerDead != null)
        {
            onPlayerDead();
        }
    }

    public event Action<int> onExperienceGained;
    public void ExperienceGained(int experience)
    {
        if (onExperienceGained != null)
        {
            onExperienceGained(experience);
        }
    }

    public event Action<int> onPlayerLevelChange;
    public void PlayerLevelChange(int level)
    {
        if (onPlayerLevelChange != null)
        {
            onPlayerLevelChange(level);
        }
    }

    public event Action<int> onGoldGained;
    public void GoldGained(int gold)
    {
        if (onGoldGained != null)
        {
            onGoldGained(gold);
        }
    }

    public event Action<int> onGoldDeducted;
    public void GoldDeducted(int gold)
    {
        if (onGoldDeducted != null)
        {
            onGoldDeducted(gold);
        }
    }

    public event Action<StatAttribute, Statistic> onStatChanged;
    public void StatChanged(StatAttribute stat, Statistic statistic)
    {
        if (onStatChanged != null)
        {
            onStatChanged(stat, statistic);
        }
    }

    public event Action<StatAttribute, Attribute> onAttributeChanged;
    public void AttributeChanged(StatAttribute stat, Attribute attribute)
    {
        if (onAttributeChanged != null)
        {
            onAttributeChanged(stat, attribute);
        }
    }
}
