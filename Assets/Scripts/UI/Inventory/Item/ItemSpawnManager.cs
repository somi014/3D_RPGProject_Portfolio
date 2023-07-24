using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    public static ItemSpawnManager instance;

    [SerializeField] GameObject itemPrefab;
    [SerializeField] LayerMask terrainLayerMask;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnItem(Vector3 position, ItemData itemToSpawn)
    {
        position += Vector3.up * 20f;       //xz ��ǥ���� �ٴ� ���̾� üũ�� y ���߱�

        Ray findSurfaceRay = new Ray(position, Vector3.down);
        RaycastHit hit;

        if(Physics.Raycast(findSurfaceRay, out hit, Mathf.Infinity, terrainLayerMask))
        {
            GameObject newItemOnGround = Instantiate(itemPrefab, hit.point, Quaternion.identity);
            newItemOnGround.GetComponent<PickUpInteractableObject>().SetItem(itemToSpawn);
        }
    }
}
