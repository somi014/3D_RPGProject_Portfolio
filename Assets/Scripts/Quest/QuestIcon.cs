using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 퀘스트 진행도에 따라 NPC 아이콘 표시
/// </summary>
public class QuestIcon : MonoBehaviour
{
    [Header("Icons")]
    [SerializeField]
    private GameObject requirementsNotMetToStartIcon;
    [SerializeField] 
    private GameObject canStartIcon;
    [SerializeField]
    private GameObject requirementsNotMetToFinishIcon;
    [SerializeField] 
    private GameObject canFinishIcon;

    public void SetState(QuestState state, bool startPoint, bool finishPoint)
    {
        requirementsNotMetToStartIcon.SetActive(false);
        canStartIcon.SetActive(false);
        requirementsNotMetToFinishIcon.SetActive(false);
        canFinishIcon.SetActive(false);

        switch (state)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
                if (startPoint == true)
                {
                    requirementsNotMetToStartIcon.SetActive(true);
                }
                break;
            case QuestState.CAN_START:
                if (startPoint == true)
                {
                    canStartIcon.SetActive(true);
                }
                break;
            case QuestState.IN_PROGRESS:
                if (finishPoint == true)
                {
                    requirementsNotMetToFinishIcon.SetActive(true);
                }
                break;
            case QuestState.CAN_FINISH:
                if (finishPoint == true)
                {
                    canFinishIcon.SetActive(true);
                }
                break;
            case QuestState.FINISHED:
                break;
            case QuestState.NOT_YET:
                break;
            default:
                break;
        }
    }

}
