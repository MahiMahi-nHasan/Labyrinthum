using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EntityManager
{
    public static Dictionary<int, EntityData> entities = new();
    public static int id = -1;

    public static int CreateEntity(Entity baseEntity, Vector3 localPosition = new(), Quaternion rotation = new(), bool spawnEntity = true)
    {
        id++;

        Debug.Log("Creating entity with id " + id);

        EntityData data = new()
        {
            baseEntity = baseEntity,
            position = localPosition,
            rotation = rotation,
            party = new()
        };

        entities.Add(id, data);

        if (spawnEntity)
            SpawnExistingEntity(id);

        return id;
    }

    public static int LinkEntity(GameObject instance)
    {
        id++;

        Debug.Log("Linking entity with id " + id);

        EntityData data = new()
        {
            baseEntity = instance.GetComponent<OverworldEntity>().baseEntity,
            instance = instance,
            position = instance.transform.position,
            rotation = instance.transform.rotation,
            party = new()
        };

        entities.Add(id, data);

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

        GameObject instance = MonoBehaviour.Instantiate(data.baseEntity.prefab, data.position, data.rotation);
        instance.GetComponent<OverworldEntity>().id = id;

        data.instance = instance;
        // Update dictionary data to store instance
        entities[id] = data;

        foreach (int pid in data.party)
            instance.GetComponent<OverworldEntity>().party.Add(pid);
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
    public Entity baseEntity;
    public GameObject instance;
    public Vector3 position;
    public Quaternion rotation;
    public int Health { get; set; }
    public int Mana { get; set; }
    public Equipment equipped;
    public List<int> party;
    public int Strength
    {
        get
        {
            int strength = baseEntity.baseStrength;
            if (equipped != null)
                strength += equipped.strengthModifier;
            return strength;
        }
    }
    public int Defense
    {
        get
        {
            int defense = baseEntity.baseDefense;
            if (equipped != null)
                defense += equipped.defenseModifier;
            return defense;
        }
    }
    public int Speed
    {
        get
        {
            int speed = baseEntity.baseSpeed;
            if (equipped != null)
                speed += equipped.speedModifier;
            return speed;
        }
    }
}