using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    // Singleton system
    public static EntitySpawner instance;

    public double spawnChance = .001;
    public int entityCap = 100;

    public int minPartySize = 1;
    public int maxPartySize = 4;

    public Entity[] possibleSpawns;

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
            EntityManager.LinkEntity(entity.gameObject);
    }

    void FixedUpdate()
    {
        double comp = Utils.rand.NextDouble();

        if (comp < spawnChance && BattleRunner.active.gameState == BattleRunner.GameState.OVERWORLD)
        {
            TrySpawnParty(
                GetRandomPointWithinRadiusFromOrigin(
                    GameObject.FindGameObjectWithTag("EnemySpawn").transform.position,
                    10f
                )
            );
        }
    }

    public void TrySpawnParty(Vector3 position)
    {
        int partySize = Random.Range(minPartySize, maxPartySize);
        OverworldEntity[] entities = FindObjectsOfType<OverworldEntity>();

        if (entities.Length + partySize < entityCap)
            SpawnParty(partySize, position);
    }

    public void SpawnParty(int size, Vector3 position)
    {
        OverworldEntity[] entities = new OverworldEntity[size];

        for (int i = 0; i < size; i++)
        {
            int id = EntityManager.CreateEntity(
                possibleSpawns[Random.Range(0, possibleSpawns.Length)],
                GetRandomPointWithinRadiusFromOrigin(position, 3f)
            );
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