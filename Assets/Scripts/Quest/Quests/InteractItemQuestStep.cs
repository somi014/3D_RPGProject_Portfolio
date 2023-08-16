using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractItemQuestStep : QuestStep
{
    private int itemInteracted = 0;
    private int interactToComplete = 2;

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onItemInteracted += ItemInteracted;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onItemInteracted -= ItemInteracted;
    }

    /// <summary>
    /// 퀘스트 아이템 획득
    /// </summary>
    private void ItemInteracted()
    {
        if (itemInteracted < interactToComplete)
        {
            itemInteracted++;
            UpdateState();
        }

        if (itemInteracted >= interactToComplete)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = itemInteracted.ToString();
        ChangeState(state);
    }

    protected override void SetQuestStepState(string state)
    {
        itemInteracted = int.Parse(state);
        UpdateState();
    }
}