using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO")]
public class QuestInfoSo : ScriptableObject
{
    public string id { get; set; }

    [Header("General")]
    public string displayName;

    [Header("Requirements")]
    public int levelRequirement;

    public QuestInfoSo[] questPrerequisites;        //선행 퀘스트

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    [Header("Rewards")]
    public int goldReward;
    public int experienceReward;

    [Header("NPC ID")]
    public int startNPCID;
    public int finishNPCID;

    /// <summary>
    /// 인스펙터 데이터 관리
    /// </summary>
    private void OnValidate()
    {
        if(string.IsNullOrEmpty(id) == true)
        {
            Debug.Log("Quest id is empty");
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }
}