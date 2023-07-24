using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO")]
public class QuestInfoSo : ScriptableObject
{
    [SerializeField] public string id { get; set; }

    [Header("General")]
    public string displayName;

    [Header("Requirements")]
    public int levelRequirement;

    public QuestInfoSo[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    [Header("Rewards")]
    public int goldReward;
    public int experienceReward;

    [Header("NPC ID")]
    public int startNPCID;
    public int finishNPCID;



    private void OnValidate()
    {
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);

    }
}