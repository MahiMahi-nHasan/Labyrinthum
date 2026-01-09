using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle
{
    List<BattleEntity> entities;

    // Call this constructor in a MonoBehaviour to make a new list of entities
    public Battle(List<BattleEntity> entities)
    {
        this.entities = entities;
    }

    // Get an ordered queue of the entities in the battle based on speed
    public Queue<BattleEntity> GetOrderedEntities()
    {
        List<BattleEntity> bkp = new(entities);
        // Sort entities in order of speed
        Queue<BattleEntity> orderedEntities = new();
        while (entities.Count > 0)
        {
            int iMaxSpeed = 0;
            for (int i = 1; i < entities.Count; i++)
                if (entities[i].Speed > entities[iMaxSpeed].Speed)
                    iMaxSpeed = i;

            orderedEntities.Enqueue(entities[iMaxSpeed]);
            entities.RemoveAt(iMaxSpeed);
        }
        entities = bkp;

        return orderedEntities;
    }

    // Assume that all entities in entities list are alive
    public IEnumerator SimulateRound()
    {
        // Reset all entities to default state
        foreach (BattleEntity e in entities)
            e.isDefending = false;

        Queue<BattleEntity> orderedEntities = GetOrderedEntities();

        while (orderedEntities.Count > 0)
        {
            BattleEntity e = orderedEntities.Dequeue();

            if (e.state.dead)
            {
                Debug.Log(e.baseEntity.entityName + " is dead");
                continue;
            }

            yield return SimulateTurn(e, 0.5f);
        }
    }

    IEnumerator SimulateTurn(BattleEntity e, float waitTime)
    {
        switch (e.state.plannedMove)
        {
            case BattleEntity.Move.ATTACK:
                e.state.target.TakeDamage(e.BaseDamage());
                break;
            case BattleEntity.Move.DEFEND:
                e.Defend();
                break;
            case BattleEntity.Move.RECHARGE:
                e.Recharge();
                break;
            case BattleEntity.Move.SPECIAL:
                e.state.target.TakeDamage(e.Special());
                break;
        }

        yield return new WaitForSeconds(waitTime);
    }
}