using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private QuestPoint questPoint;
    private Animator anim;

    private Transform player;

    [Header("NPC Info")]
    [SerializeField]
    private string npcName;
    [SerializeField] 
    private int npcID;

    public string NPCName { get => npcName; }
    public int GetNPCID { get => npcID; }
   
    private bool look = false;


    private void Awake()
    {
        questPoint = GetComponent<QuestPoint>();
        anim = GetComponent<Animator>();

        player = FindObjectOfType<PlayerStateManager>().transform;
    }

    public void Init(Quest quest)
    {
        questPoint.NPCQuestSet(quest, npcID);
    }

    private void Update()
    {
        if (look == true)
        {
            LookPlayer();
        }
    }

    public void NPCTalkStart()
    {
        look = true;
        anim.SetBool("Move", true);
    }

    private void LookPlayer()
    {
        Vector3 dir = player.position - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        float gap = Quaternion.Angle(transform.rotation, targetRot);
        if (gap <= 0.1f)
        {
            look = false;
            anim.SetBool("Move", false);
            return;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10 * Time.deltaTime);
    }
}
