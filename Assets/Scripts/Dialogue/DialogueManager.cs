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
    private bool isAction;       //대화창 활성화 상태 
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
        //대사 생성 (obj id, 대화 )
        talkData.Add(1000, new string[] { "안녕하세요:" + playerface, "모험가인가?:" + npcFemaleface, });
        talkData.Add(2000, new string[] { "못 보던 얼굴이군:" + npcMaleface, "마을을 찾고 있어요\n어디로 가야하죠?:"+ playerface,
                                    "이 앞으로 쭉 가면 마을이야\n하지만 몬스터가 길을 막고 있지:" + npcMaleface});


        //퀘스트용 대화(obj id + quest id + questIndex(순서번호))
        //퀘스트용 대화(obj id + quest id)

        //10번 퀘스트 
        talkData.Add(1000 + 10, new string[] { "저기..:" + npcFemaleface, "무슨 일이죠?:" +  playerface, "석상들이 오염되었어요:"+ npcFemaleface,
                                        "석상의 조각을 모으면 정화할 수 있어요 \n저 좀 도와주세요:" + npcFemaleface,
                                        "조각을 모아서 제 친구에게 가져다 주세요:" + npcFemaleface, });
        talkData.Add(1000 + 10 + 1, new string[] { "조각을 모아주세요 \n부탁드려요:" + npcFemaleface, });
        talkData.Add(2000 + 10 + 1, new string[] { "아직 다 모으지 못했군요:" + npcMaleface, });
        talkData.Add(2000 + 10 + 2, new string[] { "모두 모아 오셨군요 \n감사해요:" + npcMaleface, });

        //20번 퀘스트
        talkData.Add(2000 + 20, new string[] { "하나 더 부탁하고 싶은데 괜찮을까요?:" + npcMaleface, "무슨 일인가요?:" + playerface, "석상을 정화해주세요:" + npcMaleface, });
        talkData.Add(2000 + 20 + 1, new string[] { "부탁해요:" + npcMaleface, }); //퀘스트 완료 전에 대화를 걸었을 때
        talkData.Add(1000 + 20 + 1, new string[] { "석상을 정화하고 친구에게 알려주세요:" + npcFemaleface, }); //퀘스트 완료 전에 대화를 걸었을 때
        //talkData.Add(300 + 20, new string[] { "열쇠를 찾았다", });
        talkData.Add(2000 + 20 + 2, new string[] { "감사해요!:" + npcMaleface, "별 말씀을요!:" + playerface, });


        //30번 퀘스트 
        //talkData.Add(1000 + 30, new string[] { "열쇠를 찾아줘서 고마워요!:0", "별 말씀을요!:2", });

        //초상화 생성 (obj id + portrait number)
        portraitData.Add(1000 + 0, portraitArr[playerface]); //0번 인덱스에 저장된 초상화를 id = 1000과 mapping
        portraitData.Add(1000 + 1, portraitArr[npcFemaleface]);
        portraitData.Add(1000 + 2, portraitArr[npcMaleface]); //2번 인덱스에 저장된 초상화를 id = 1002과 mapping

        portraitData.Add(2000 + 0, portraitArr[0]);
        portraitData.Add(2000 + 1, portraitArr[1]);
        portraitData.Add(2000 + 2, portraitArr[2]);
    }

    public string GetTalk(int id, int talkIndex)
    {
        //1. 해당 퀘스트 id에서 퀘스트index(순서)에 해당하는 대사가 없음
        if (!talkData.ContainsKey(id))
        {
            //해당 퀘스트 자체에 대사가 없을 때 -> 기본 대사를 불러옴 (십, 일의 자리 부분 제거 )
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

        //2. 해당 퀘스트 id에서 퀘스트index(순서)에 해당하는 대사가 있음
        if (talkIndex == talkData[id].Length)
            return null;
        else
        {
            return talkData[id][talkIndex]; //해당 아이디의 해당
        }
    }

    public Sprite GetPortrait(int id, int portraitIndex)
    {
        //id는 NPC넘버 , portraitIndex : 표정번호(?)
        return portraitData[id + portraitIndex];
    }

    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;

        NPC npcGo = scanObject.GetComponent<NPC>();
        charName = npcGo.NPCName;

        questTalkIndex = (questManager.CurrentQuestIndex + 1) * 10;        // 조사한 obj의 id를 넘겨 퀘스트 id를 반환받음 
        questStep = questManager.CurrentQuestStepIndex;

        Talk(npcGo.GetNPCID);

        talkPanel.SetActive(isAction);
    }

    void Talk(int id, bool isNPC = true)
    { // id는 오브젝트id 

        isQuestTalk = true;
        string talkData = GetTalk(id + questTalkIndex + questStep, talkIndex);              //id에 퀘스트 id를 더하면 -> 해당 id를 가진 오브젝트가 가진 퀘스트의 대화를 반환하게 만들기

        if (talkData == null)
        {
            isAction = false;
            talkIndex = 0;

            player.CamaraChange(false);

            if (isQuestTalk == true)
            {
                GameEventsManager.instance.inputEvents.SubmitPressed();         //NPC 퀘스트 시작
            }
        
            talkPanel.SetActive(isAction);
            return;
        }

        if (isNPC)
        {
            UIInfoText.text = "";
            UITalkText.text = talkData.Split(':')[0];           //구분자로 문장을 나눠줌  0: 대사 1:portraitIndex
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

        //다음 문장을 가져오기 위해 talkData의 인덱스를 늘림
        isAction = true; //대사가 남아있으므로 계속 진행되어야함 
        talkIndex++;
    }

}