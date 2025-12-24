using System.Collections.Generic;
using System.Linq;
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
        EntityData removed = entities[id];
        // Only destroy the gameobject if it is not currently spawned in
        if (removed.instance != null)
            MonoBehaviour.Destroy(removed.instance);
        entities.Remove(id);
    }

    public static void SpawnExistingEntity(int id)
    {
        EntityData data = entities[id];

        // Do not create a second instance of this entity
        if (data.instance != null) return;

        Debug.Log("Spawning entity with id " + id);

        GameObject instance = MonoBehaviour.Instantiate(data.prefab, data.position, data.rotation);
        instance.GetComponent<OverworldEntity>().id = id;

        data.instance = instance;
        // Update dictionary data to store instance
        entities[id] = data;
    }

    public static void SpawnAllEntities()
    {
        List<int> keys = entities.Keys.ToList();
        for (int i = 0; i < keys.Count; i++)
            SpawnExistingEntity(keys[i]);
    }
}

public struct EntityData
{
    public GameObject prefab;
    public GameObject battlePrefab;
    public GameObject instance;
    public Vector3 position;
    public Quaternion rotation;
    public int health { get; set; }
    public int mana { get; set; }
}