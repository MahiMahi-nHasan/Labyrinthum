using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    // Singleton system
    public static EntitySpawner instance;

    public GameObject target;
    private int t_id;

    public double spawnChance = .001;
    public int entityCap = 20;

    public int minPartySize = 1;
    public int maxPartySize = 4;

    public Entity entity;

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

        OverworldEntity[] entities = FindObjectsOfType<OverworldEntity>();
        foreach (OverworldEntity entity in entities)
        {
            int id = EntityManager.LinkEntity(entity.gameObject);
            if (target.GetComponent<OverworldEntity>() == entity)
                t_id = id;
        }
    }

    void FixedUpdate()
    {
        double comp = Utils.rand.NextDouble();

        if (comp < spawnChance && BattleRunner.active.gameState == BattleRunner.GameState.OVERWORLD)
        {
            TrySpawnParty(
                entity,
                GetRandomPointWithinRadiusFromOrigin(
                    EntityManager.entities[t_id].instance.transform.position,
                    20f
                )
            );
        }
    }

    public void TrySpawnParty(Entity entity, Vector3 position)
    {
        int partySize = Random.Range(minPartySize, maxPartySize);
        OverworldEntity[] entities = FindObjectsOfType<OverworldEntity>();

        if (entities.Length + partySize < entityCap)
            SpawnParty(entity, partySize, position);
    }

    public void SpawnParty(Entity entity, int size, Vector3 position)
    {
        OverworldEntity[] entities = new OverworldEntity[size];

        for (int i = 0; i < size; i++)
        {
            int id = EntityManager.CreateEntity(entity, GetRandomPointWithinRadiusFromOrigin(position, 5f));
            GameObject instance = EntityManager.entities[id].instance;
            entities[i] = instance.GetComponent<OverworldEntity>();
        }

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                entities[i].AddToParty(entities[j].id);
    }

    public Vector3 GetRandomPointWithinRadiusFromOrigin(Vector3 origin, float radius)
    {
        Vector2 rand = Random.insideUnitCircle * radius;
        return origin + new Vector3(rand.x, 0, rand.y);
    }
}