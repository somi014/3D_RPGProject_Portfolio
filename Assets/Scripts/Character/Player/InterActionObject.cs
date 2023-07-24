using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InterActionObject : MonoBehaviour
{
    Inventory inventory;
    DialogueManager dialogueManager;
    PlayerStateManager player;

    private NPC npc;

    private bool npcTalk = false;
    public bool itemInterAction = false;
    public bool pickUp;


    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        player = FindObjectOfType<PlayerStateManager>();
    }

    /// <summary>
    /// 아이템, NPC 상호작용
    /// </summary>
    /// <param name="callbackContext"></param>
    public void InterActionObejctKey(InputAction.CallbackContext callbackContext)
    {
        if (npcTalk == true)
        {
            if (callbackContext.started == true)
            {
                player.CamaraChange(true);

                npc.NPCTalkStart();

                dialogueManager.Action(npc.gameObject);
            }
        }
        else if (itemInterAction == true)
        {
            pickUp = callbackContext.action.IsPressed();            //누르는 동안 아이템 상호작용
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out NPC npcComponent) == true)
        {
            npcTalk = true;
            npc = npcComponent;
        }

        if (other.transform.TryGetComponent(out InteractableObject item) == true)
        {
            itemInterAction = true;
            item.Interact(inventory);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent(out NPC npcComponent) == true)
        {
            npcTalk = false;
            npc = null;
        }


        if (other.gameObject.tag.Contains("Item") == true)
        {
            itemInterAction = false;
        }
    }
}
