using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DamageDealer))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        DamageDealer fow = (DamageDealer)target;
        
        //view
        Handles.color = Color.blue;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
              
        //attack area
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.red;
        //foreach (Transform visible in fow.visibleTargets)
        foreach (GameObject visible in fow.hasDealtDamage)
        {
            Handles.DrawLine(fow.transform.position, visible.transform.position);
        }
    }
}