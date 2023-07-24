using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSo questInfoForPoint;
    [SerializeField] private Quest questForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = false;
    [SerializeField] private bool finishPoint = false;

    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;

    private QuestIcon questIcon;

    private void Awake()
    {
        //questId = questInfoForPoint.id;
        questIcon = GetComponentInChildren<QuestIcon>();
    }

    private void Start()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
    }

    public void NPCQuestSet(Quest quest, int npcID)
    {
        questForPoint = quest;

        questId = quest.info.id;
        bool start = npcID == quest.info.startNPCID;
        startPoint = start;
        bool finish = npcID == quest.info.finishNPCID;
        finishPoint = finish;
    }

    private void SubmitPressed()
    {
        if (playerIsNear == false)
            return;

        if (currentQuestState.Equals(QuestState.CAN_START) == true && startPoint == true)
        {
            GameEventsManager.instance.questEvents.StartQuest(questId);
        }
        else if (currentQuestState.Equals(QuestState.CAN_FINISH) == true && finishPoint == true)
        {
            GameEventsManager.instance.questEvents.FinishQuest(questId);
        }
    }

    private void QuestStateChange(Quest quest)
    {
        if (quest.info.id.Equals(questId) == true)
        {
            currentQuestState = quest.state;
            questIcon.SetState(currentQuestState, startPoint, finishPoint);
        }
        //else
        //{
        //    Debug.Log("npc " + gameObject.name);
        //    currentQuestState = QuestState.NOT_YET;
        //    questIcon.SetState(currentQuestState, startPoint, finishPoint);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {

            playerIsNear = false;
        }
    }
}
