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

    [SerializeField] private bool startPoint = false;
    [SerializeField] private bool finishPoint = false;

    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;

    private QuestIcon questIcon;

    private void Awake()
    {
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

    /// <summary>
    /// NPC 퀘스트 시작, 종료 위치 설정
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="npcID"></param>
    public void NPCQuestSet(Quest quest, int npcID)
    {
        questForPoint = quest;

        questId = quest.info.id;
        bool start = npcID == quest.info.startNPCID;
        startPoint = start;
        bool finish = npcID == quest.info.finishNPCID;
        finishPoint = finish;
    }

    /// <summary>
    /// NPC 대화 -> 퀘스트 수행 또는 완료
    /// </summary>
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
