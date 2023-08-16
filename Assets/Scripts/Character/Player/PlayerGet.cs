using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGet : MonoBehaviour
{
    private Inventory inventory;
    private StatAttribute stats;

    private UIPanelManager uiPanelUI;
    private StatsUI statUI;

    private void Awake()
    {
        stats = GetComponent<StatAttribute>();
        inventory = GetComponent<Inventory>();

        uiPanelUI = FindObjectOfType<UIPanelManager>();
        statUI = FindObjectOfType<StatsUI>();
    }

    private void OnEnable()
    {
        GameEventsManager.instance.playerEvents.onPlayerDead += PlayerDead;

        GameEventsManager.instance.playerEvents.onExperienceGained += ExperienceGained;
        GameEventsManager.instance.playerEvents.onPlayerLevelChange += PlayerLevelChange;

        GameEventsManager.instance.playerEvents.onGoldGained += GoldGained;
        GameEventsManager.instance.playerEvents.onGoldDeducted += GoldDeducted;

        GameEventsManager.instance.playerEvents.onStatChanged += StatChanged;
        GameEventsManager.instance.playerEvents.onAttributeChanged += AttributeChanged;

        StatsValue playerStat = stats.TakeStats(Statistic.Level);
        GameEventsManager.instance.playerEvents.PlayerLevelChange(playerStat.integer_value);     //레벨 설정
    }

    private void OnDisable()
    {
        GameEventsManager.instance.playerEvents.onPlayerDead -= PlayerDead;

        GameEventsManager.instance.playerEvents.onExperienceGained -= ExperienceGained;
        GameEventsManager.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChange;

        GameEventsManager.instance.playerEvents.onGoldGained -= GoldGained;
        GameEventsManager.instance.playerEvents.onGoldDeducted -= GoldDeducted;

        GameEventsManager.instance.playerEvents.onStatChanged -= StatChanged;
        GameEventsManager.instance.playerEvents.onAttributeChanged -= AttributeChanged;

    }

    private void PlayerDead()
    {
        uiPanelUI.OpenRestart();
    }

    private void ExperienceGained(int value)
    {
        stats.GetExperience(value);
    }

    private void PlayerLevelChange(int value)
    {
        if (value > 1)
        {
            UIPanelManager.instance.OpenLevelUpPanel(value);
        }
    }

    private void GoldGained(int gold)
    {
        inventory.AddGold(gold);
    }

    private void GoldDeducted(int gold)
    {
        inventory.SubstactGold(gold);
    }

    private void StatChanged(StatAttribute stat, Statistic statistic)
    {
        statUI.SetStats(stat, statistic);
    }

    private void AttributeChanged(StatAttribute stat, Attribute attribute)
    {
        statUI.SetAttribute(stat, attribute);
    }
}