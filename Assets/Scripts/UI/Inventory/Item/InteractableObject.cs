using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ������ ȹ��, ����
/// </summary>
public class InteractableObject : MonoBehaviour
{
    public Action<Inventory> interact;

    public void Subscibe(Action<Inventory> action)
    {
        interact += action;
    }

    public void Delete(Action<Inventory> action)
    {
        interact -= action;
    }

    public void Interact(Inventory inventory)
    {
        interact?.Invoke(inventory);
    }
}
