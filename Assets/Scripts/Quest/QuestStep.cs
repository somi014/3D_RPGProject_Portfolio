using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;
    private string questId;
    private int stepIndex;

    public void InitializeQuestStep(string id, int questIndex, string questStepState)
    {
        questId = id;
        stepIndex = questIndex;
        if(questStepState != null && questStepState != "")
        {
            SetQuestStepState(questStepState);
        }
    }

    protected void FinishQuestStep()
    {
        if(isFinished == false)
        {
            isFinished = true;

            GameEventsManager.instance.questEvents.AdvanceQuest(questId);       //퀘스트 매니저에서 완료 가능으로 

            Destroy(this.gameObject);
        }
    }

    protected void ChangeState(string newState)
    {
        GameEventsManager.instance.questEvents.QuestStepStateChange(questId, stepIndex, new QuestStepState(newState));
    }

    protected abstract void SetQuestStepState(string state);
}
