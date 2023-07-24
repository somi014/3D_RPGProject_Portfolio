using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private QuestManager questManager;
    private PlayerStateManager player;

    public GameObject talkPanel;
    public TextMeshProUGUI UINameText;
    public TextMeshProUGUI UITalkText;
    public TextMeshProUGUI UIInfoText;
    public Image portraitImg;
    public GameObject scanObject;

    private string charName;
    private int talkIndex;
    private int questTalkIndex;
    private int questStep;
    private bool isAction;       //��ȭâ Ȱ��ȭ ���� 
    private bool isQuestTalk;

    Dictionary<int, string[]> talkData;
    Dictionary<int, Sprite> portraitData;
    public Sprite[] portraitArr;
    const int playerface = 0;
    const int npcFemaleface = 1;
    const int npcMaleface = 2;

    void Awake()
    {
        questManager = FindObjectOfType<QuestManager>();
        player = FindObjectOfType<PlayerStateManager>();

        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();

        talkPanel.SetActive(isAction);
    }

    void GenerateData()
    {
        //��� ���� (obj id, ��ȭ )
        talkData.Add(1000, new string[] { "�ȳ��ϼ���:" + playerface, "���谡�ΰ�?:" + npcFemaleface, });
        talkData.Add(2000, new string[] { "�� ���� ���̱�:" + npcMaleface, "������ ã�� �־��\n���� ��������?:"+ playerface,
                                    "�� ������ �� ���� �����̾�\n������ ���Ͱ� ���� ���� ����:" + npcMaleface});


        //����Ʈ�� ��ȭ(obj id + quest id + questIndex(������ȣ))
        //����Ʈ�� ��ȭ(obj id + quest id)

        //10�� ����Ʈ 
        talkData.Add(1000 + 10, new string[] { "����..:" + npcFemaleface, "���� ������?:" +  playerface, "������� �����Ǿ����:"+ npcFemaleface,
                                        "������ ������ ������ ��ȭ�� �� �־�� \n�� �� �����ּ���:" + npcFemaleface,
                                        "������ ��Ƽ� �� ģ������ ������ �ּ���:" + npcFemaleface, });
        talkData.Add(1000 + 10 + 1, new string[] { "������ ����ּ��� \n��Ź�����:" + npcFemaleface, });
        talkData.Add(2000 + 10 + 1, new string[] { "���� �� ������ ���߱���:" + npcMaleface, });
        talkData.Add(2000 + 10 + 2, new string[] { "��� ��� ���̱��� \n�����ؿ�:" + npcMaleface, });

        //20�� ����Ʈ
        talkData.Add(2000 + 20, new string[] { "�ϳ� �� ��Ź�ϰ� ������ ���������?:" + npcMaleface, "���� ���ΰ���?:" + playerface, "������ ��ȭ���ּ���:" + npcMaleface, });
        talkData.Add(2000 + 20 + 1, new string[] { "��Ź�ؿ�:" + npcMaleface, }); //����Ʈ �Ϸ� ���� ��ȭ�� �ɾ��� ��
        talkData.Add(1000 + 20 + 1, new string[] { "������ ��ȭ�ϰ� ģ������ �˷��ּ���:" + npcFemaleface, }); //����Ʈ �Ϸ� ���� ��ȭ�� �ɾ��� ��
        //talkData.Add(300 + 20, new string[] { "���踦 ã�Ҵ�", });
        talkData.Add(2000 + 20 + 2, new string[] { "�����ؿ�!:" + npcMaleface, "�� ��������!:" + playerface, });


        //30�� ����Ʈ 
        //talkData.Add(1000 + 30, new string[] { "���踦 ã���༭ ������!:0", "�� ��������!:2", });

        //�ʻ�ȭ ���� (obj id + portrait number)
        portraitData.Add(1000 + 0, portraitArr[playerface]); //0�� �ε����� ����� �ʻ�ȭ�� id = 1000�� mapping
        portraitData.Add(1000 + 1, portraitArr[npcFemaleface]);
        portraitData.Add(1000 + 2, portraitArr[npcMaleface]); //2�� �ε����� ����� �ʻ�ȭ�� id = 1002�� mapping

        portraitData.Add(2000 + 0, portraitArr[0]);
        portraitData.Add(2000 + 1, portraitArr[1]);
        portraitData.Add(2000 + 2, portraitArr[2]);
    }

    public string GetTalk(int id, int talkIndex)
    {
        //1. �ش� ����Ʈ id���� ����Ʈindex(����)�� �ش��ϴ� ��簡 ����
        if (!talkData.ContainsKey(id))
        {
            //�ش� ����Ʈ ��ü�� ��簡 ���� �� -> �⺻ ��縦 �ҷ��� (��, ���� �ڸ� �κ� ���� )
            if (!talkData.ContainsKey(id - id % 10))
            {
                isQuestTalk = false;
                return GetTalk(id - id % 100, talkIndex);   //GET FIRST TALK
            }

            else
            {
                return GetTalk(id - id % 10, talkIndex);    //GET FIRST QUEST TALK
            }
        }

        //2. �ش� ����Ʈ id���� ����Ʈindex(����)�� �ش��ϴ� ��簡 ����
        if (talkIndex == talkData[id].Length)
            return null;
        else
        {
            return talkData[id][talkIndex]; //�ش� ���̵��� �ش�
        }
    }

    public Sprite GetPortrait(int id, int portraitIndex)
    {
        //id�� NPC�ѹ� , portraitIndex : ǥ����ȣ(?)
        return portraitData[id + portraitIndex];
    }

    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;

        NPC npcGo = scanObject.GetComponent<NPC>();
        charName = npcGo.NPCName;

        questTalkIndex = (questManager.CurrentQuestIndex + 1) * 10;        // ������ obj�� id�� �Ѱ� ����Ʈ id�� ��ȯ���� 
        questStep = questManager.CurrentQuestStepIndex;

        Talk(npcGo.GetNPCID);

        talkPanel.SetActive(isAction);
    }

    void Talk(int id, bool isNPC = true)
    { // id�� ������Ʈid 

        isQuestTalk = true;
        string talkData = GetTalk(id + questTalkIndex + questStep, talkIndex);              //id�� ����Ʈ id�� ���ϸ� -> �ش� id�� ���� ������Ʈ�� ���� ����Ʈ�� ��ȭ�� ��ȯ�ϰ� �����

        if (talkData == null)
        {
            isAction = false;
            talkIndex = 0;

            player.CamaraChange(false);

            if (isQuestTalk == true)
            {
                GameEventsManager.instance.inputEvents.SubmitPressed();         //NPC ����Ʈ ����
            }
        
            talkPanel.SetActive(isAction);
            return;
        }

        if (isNPC)
        {
            UIInfoText.text = "";
            UITalkText.text = talkData.Split(':')[0];           //�����ڷ� ������ ������  0: ��� 1:portraitIndex
            portraitImg.sprite = GetPortrait(id, int.Parse(talkData.Split(':')[1]));

            if (int.Parse(talkData.Split(':')[1]) == playerface)
            {
                UINameText.text = "";
            }
            else
            {
                UINameText.text = charName;
            }


            portraitImg.color = new Color(1, 1, 1, 1);

        }
        else
        {
            UINameText.text = "";
            UITalkText.text = "";
            UIInfoText.text = talkData;

            portraitImg.color = new Color(1, 1, 1, 0);
        }

        //���� ������ �������� ���� talkData�� �ε����� �ø�
        isAction = true; //��簡 ���������Ƿ� ��� ����Ǿ���� 
        talkIndex++;
    }

}