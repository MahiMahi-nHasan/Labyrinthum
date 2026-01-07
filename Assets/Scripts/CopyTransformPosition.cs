using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransformPosition : MonoBehaviour
{
    public string targetTag = "Player";

    // Update is called once per frame
    void Update()
    {
        transform.position = GameObject.FindGameObjectWithTag(targetTag).transform.position;
    }
}
