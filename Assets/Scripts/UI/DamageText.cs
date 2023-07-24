using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private float moveSpeed;
    private float alphaSpeed;
    private float destroyTime;
    TextMeshPro text;
    Color alpha;
    public int damage;

    void Start()
    {
        moveSpeed = 2.0f;
        alphaSpeed = 2.0f;
        destroyTime = 2.0f;

        text = GetComponent<TextMeshPro>();
        alpha = text.color;
        text.text = damage.ToString();
        Invoke("DestroyObject", destroyTime);
    }

    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0)); // �ؽ�Ʈ ��ġ
        transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed); // �ؽ�Ʈ ���İ�
        text.color = alpha;
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}