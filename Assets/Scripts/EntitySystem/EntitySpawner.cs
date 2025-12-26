using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    // Singleton system
    public static EntitySpawner instance;

    public List<SpawnData> spawnList;

    void Awake()
    {
        #region Singleton code
        // Ensure only one instance of this object exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion

        /*
        foreach (SpawnData data in spawnList)
        {
            EntityManager.CreateEntity(
                data.prefab,
                data.position,
                Quaternion.Euler(data.eulerAngles)
            );
        }
        */

        OverworldEntity[] entities = GameObject.FindObjectsOfType<OverworldEntity>();
        foreach (OverworldEntity entity in entities)
            EntityManager.LinkEntity(entity.gameObject);
    }
}

[System.Serializable]
public struct SpawnData
{
    public GameObject prefab;
    public Vector3 position;
    public Vector3 eulerAngles;
}