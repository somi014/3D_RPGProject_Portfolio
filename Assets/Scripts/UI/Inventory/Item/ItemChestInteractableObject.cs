using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChestInteractableObject : MonoBehaviour
{
    [SerializeField] ItemDropList dropList;
    [SerializeField] float itemDropRange = 2f;

    bool isOpened = false;

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GetComponent<InteractableObject>().Subscibe(OpenChest);
    }

    public void OpenChest(Inventory inventory)
    {
        if (isOpened == true)
            return;

        GetComponent<Collider>().enabled = false;

        isOpened = true;
        animator.SetBool("Open", true);
    }

    private Vector3 SelecteRandomPosition()
    {
        Vector3 pos = transform.position;

        pos += Vector3.right * UnityEngine.Random.Range(0, itemDropRange);
        pos += Vector3.forward * UnityEngine.Random.Range(0, itemDropRange);

        return pos;
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    private void OpenItemBox()
    {
        ItemSpawnManager.instance.SpawnItem(SelecteRandomPosition(), dropList.GetDrop());
    }
}
