using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float distance = 10f;
    public float height = 5f;
    public float smootRot = 5f;


    private void LateUpdate()
    {
        float curAngleY = Mathf.LerpAngle(transform.eulerAngles.y, player.eulerAngles.y, smootRot * Time.deltaTime);

        Quaternion rot = Quaternion.Euler(0f, curAngleY, 0f);
    }
}
