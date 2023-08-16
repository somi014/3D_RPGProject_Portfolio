using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestInfoSo info;

    public QuestState state;
    private int currentQuestStepIndex;
    private QuestStepState[] questStepStates;

    public Quest(QuestInfoSo questInfo)
    {
        info = questInfo;
        state = QuestState.REQUIREMENTS_NOT_MET;
        currentQuestStepIndex = 0;
        questStepStates = new QuestStepState[info.questStepPrefabs.Length];
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }

    public Quest(QuestInfoSo questInfo, QuestState questState, int stepIndex, QuestStepState[] stepStates)
    {
        info = questInfo;
        state = questState;
        currentQuestStepIndex = stepIndex;
        questStepStates = stepStates;

        if (questStepStates.Length != info.questStepPrefabs.Length)
        {
            Debug.Log("not same length Quest id " + info.id);
        }
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return (currentQuestStepIndex < info.questStepPrefabs.Length);      
    }

    /// <summary>
    /// 현재 진행 중인 퀘스트 오브젝트 만들기
    /// </summary>
    /// <param name="parentTransfom"></param>
    public void InstantiateCurrentQuestStep(Transform parentTransfom)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            QuestStep questStep = Object.Instantiate(questStepPrefab, parentTransfom).GetComponent<QuestStep>();          
            questStep.InitializeQuestStep(info.id, currentQuestStepIndex, questStepStates[currentQuestStepIndex].state);
        }
    }

    /// <summary>
    /// 현재 퀘스트 진행 중인 퀘스트 프리팹 반환
    /// </summary>
    /// <returns></returns>
    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExists() == true)
        {
            questStepPrefab = info.questStepPrefabs[currentQuestStepIndex];
        }
        else
        {
            Debug.Log("step not exist, id " + info.id + " , step index " + currentQuestStepIndex);
        }
        return questStepPrefab;
    }

    public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
    {
        if (stepIndex < questStepStates.Length)
        {
            questStepStates[stepIndex].state = questStepState.state;
        }
        else
        {
            Debug.Log("step index out of range Quest Id = " + info.id + ", Step Index = " + stepIndex);
        }
    }

    public QuestData GetQuestData()
    {
        return new QuestData(state, currentQuestStepIndex, questStepStates);
    }
}