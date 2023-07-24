using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;

    [SerializeField] QuestInfoSo[] allQuests;

    private Dictionary<string, Quest> questMap;

    private int currentPlayerLevel;       

    private int currentQuestIndex;
    public int CurrentQuestIndex { get => currentQuestIndex; }
    public int CurrentQuestStepIndex
    {
        get
        {
            switch (questMap[allQuests[currentQuestIndex].id].state)
            {
                case QuestState.REQUIREMENTS_NOT_MET:
                case QuestState.CAN_START:
                case QuestState.NOT_YET:
                    return 0;
                case QuestState.IN_PROGRESS:
                    return 1;
                case QuestState.CAN_FINISH:
                case QuestState.FINISHED:
                    return 2;
            }
            return -1;
        }
    }

    private string currentQuestNum = "CurrentQuestIndex";

    [SerializeField] private NPC[] npc;

    private void Awake()
    {
        questMap = CreateQuestMap();

        currentQuestIndex = PlayerPrefs.GetInt(currentQuestNum);
    }

    private void OnEnable()
    {
        GameEventsManager.instance.questEvents.onStartQuest += StartQuest;
        GameEventsManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest += FinishQuest;

        GameEventsManager.instance.questEvents.onQuestStepStateChange += QuestStepStateChange;

        GameEventsManager.instance.playerEvents.onPlayerLevelChange += PlayerLevelChange;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest -= FinishQuest;

        GameEventsManager.instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;

        GameEventsManager.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChange;
    }

    private void Start()
    {
        NextQuestNpc();
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }

            GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
    }

    private void Update()
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckrequirementsMet(quest) == true)
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameEventsManager.instance.questEvents.QuestStateChange(quest);
    }

    private void PlayerLevelChange(int level)
    {
        currentPlayerLevel = level;
    }

    private bool CheckrequirementsMet(Quest quest)
    {
        bool meetsRequirements = true;

        if (currentPlayerLevel < quest.info.levelRequirement)
        {
            meetsRequirements = false;
        }

        foreach (QuestInfoSo prerequisitesQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisitesQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }

        return meetsRequirements;
    }

    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);

        //Äù½ºÆ® ÀÌ¸§ º¯°æ
        UIPanelManager.instance.SetQuestText(quest.info.displayName);
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

        quest.MoveToNextStep();

        if (quest.CurrentStepExists() == true)
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);

        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);

        currentQuestIndex++;
        NextQuestNpc();
    }

    private void NextQuestNpc()
    {
        if (questMap.Count <= currentQuestIndex)
        {
            return;
        }

        for (int i = 0; i < npc.Length; i++)
        {
            if (npc[i].GetNPCID == questMap[allQuests[currentQuestIndex].id].info.startNPCID)
            {
                npc[i].Init(questMap[allQuests[currentQuestIndex].id]);
            }

            if (npc[i].GetNPCID == questMap[allQuests[currentQuestIndex].id].info.finishNPCID)
            {
                npc[i].Init(questMap[allQuests[currentQuestIndex].id]);
            }
        }
    }

    /// <summary>
    /// Äù½ºÆ® º¸»ó È¹µæ
    /// </summary>
    /// <param name="quest"></param>
    private void ClaimRewards(Quest quest)
    {
        GameEventsManager.instance.playerEvents.GoldGained(quest.info.goldReward);
        GameEventsManager.instance.playerEvents.ExperienceGained(quest.info.experienceReward);
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        QuestInfoSo[] allQuest = allQuests;

        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSo questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.Log("duplicate id found when creating quest map " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));                       //Äù½ºÆ® ·Îµå ¶Ç´Â »õ·Î »ý¼º
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("id not found in the quest map " + id);
        }
        return quest;
    }

    #region Save N Load
    private void OnApplicationQuit()
    {
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest)
    {
        try
        {
            QuestData questData = quest.GetQuestData();
            string serializedData = JsonUtility.ToJson(questData);
            PlayerPrefs.SetString(quest.info.id, serializedData);

            //Debug.Log(serializedData);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save quest with id " + quest.info.id + " : " + e);
        }

        PlayerPrefs.SetInt(currentQuestNum, currentQuestIndex);
    }

    private Quest LoadQuest(QuestInfoSo questInfo)
    {
        Quest quest = null;
        try
        {
            if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState == true)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            }
            else
            {
                quest = new Quest(questInfo);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.id + " : " + e);
        }
        return quest;
    }
    #endregion
}
