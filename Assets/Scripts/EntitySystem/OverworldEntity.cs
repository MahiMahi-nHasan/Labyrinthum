using System.Collections.Generic;
using UnityEngine;

public class OverworldEntity : MonoBehaviour
{
    public int id;
    public GameObject prefab;
    public GameObject Prefab
    {
        get
        {
            if (prefab == null)
                return gameObject;
            else
                return prefab;
        }
    }
    public GameObject battlePrefab;
    public List<OverworldEntity> party;

    void OnDestroy()
    {
        EntityData data = EntityManager.entities[id];
        data.instance = null;
        EntityManager.entities[id] = data;
    }

    void Update()
    {
        // Modify this object then assign it to the allocated slot
        EntityData data = EntityManager.entities[id];

        data.position = transform.position;
        data.rotation = transform.rotation;

        EntityManager.entities[id] = data;
    }
}