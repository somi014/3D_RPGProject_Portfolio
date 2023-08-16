using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject[] enemyObject;

    private Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();

    private void Start()
    {
        StartCoroutine(IESpwanEnemy());
    }

    public GameObject GetObject()
    {
        if (poolingObjectQueue.Count > 0)
        {
            GameObject obj = poolingObjectQueue.Dequeue();
            return obj;
        }
        else
        {
            return null;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        poolingObjectQueue.Enqueue(obj);
    }

    IEnumerator IESpwanEnemy()
    {
        while (true)
        {
            if (poolingObjectQueue.Count > 0)
            {
                yield return new WaitForSeconds(10f);
                
                GameObject spawnObj = GetObject();
                float posX = Random.Range(-2f, 2f);
                float posZ = Random.Range(-2f, 2f);
                spawnObj.transform.localPosition = new Vector3(posX, spawnObj.transform.position.y, posZ);
                
                spawnObj.SetActive(true);

            }
            yield return null;
        }
    }
}
