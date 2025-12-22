using System.Collections.Generic;
using UnityEngine;

public static class EntityManager
{
    public static Dictionary<int, EntityData> entities = new();
    public static int id = -1;

    public static int CreateEntity(GameObject prefab, Vector3 localPosition = new(), Quaternion rotation = new(), bool spawnEntity = true)
    {
        id++;

        Debug.Log("Creating entity with id " + id);

        EntityData data = new()
        {
            prefab = prefab,
            battlePrefab = prefab.GetComponent<OverworldEntity>().battlePrefab,
            position = localPosition,
            rotation = rotation
        };

        entities.Add(id, data);

        if (spawnEntity)
            SpawnExistingEntity(id);

        return id;
    }

    public static void RemoveEntity(int id)
    {
        entities.Remove(id);
    }

    public static void SpawnExistingEntity(int id)
    {
        EntityData data = entities[id];

        GameObject go = MonoBehaviour.Instantiate(data.prefab, data.position, data.rotation);
        go.GetComponent<OverworldEntity>().id = id;

        data.instance = go;
        // Update dictionary data to store instance
        entities[id] = data;
    }

    public static void SpawnAllEntities()
    {
        foreach (int id in entities.Keys)
            SpawnExistingEntity(id);
    }
}

public struct EntityData
{
    public GameObject prefab;
    public GameObject battlePrefab;
    public GameObject instance;
    public Vector3 position;
    public Quaternion rotation;
}