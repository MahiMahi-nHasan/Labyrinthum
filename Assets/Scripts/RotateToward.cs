using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotateToward : MonoBehaviour
{
    public Transform target;
    
    void Update()
    {
        if (target == null) return;
        
        Vector3 dir = target.position - transform.position;
        Quaternion rot = Quaternion.FromToRotation(transform.forward, dir);
        transform.rotation *= rot;
    }
}
