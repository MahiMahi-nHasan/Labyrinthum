using System.Collections.Generic;
using UnityEngine;

public class Battle
{
    List<Entity> entities;

    // Call this constructor in a MonoBehaviour to make a new list of entities
    public Battle(List<Entity> entities)
    {
        this.entities = entities;
    }

    // Assume that all entities in entities list are alive
    public void SimulateRound()
    {
        // Reset all entities to default state
        foreach (Entity e in entities)
            e.isDefending = false;

        // Sort entities in order of speed
        Queue<Entity> orderedEntities = new();
        while (entities.Count > 0)
        {
            int iMaxSpeed = 0;
            for (int i = 1; i < entities.Count; i++)
                if (entities[i].speed > entities[iMaxSpeed].speed)
                    iMaxSpeed = i;

            orderedEntities.Enqueue(entities[iMaxSpeed]);
            entities.RemoveAt(iMaxSpeed);
        }

        while (orderedEntities.Count > 0)
        {
            Entity e = orderedEntities.Dequeue();

            if (e.state.dead)
            {
                Debug.Log(e.entityName + " is dead");
                continue;
            }

            SimulateTurn(e);
        }
    }

    void SimulateTurn(Entity e)
    {
        Debug.Log("Health = " + e.health);
        switch (e.state.plannedMove)
        {
            case Entity.Move.ATTACK:
                e.state.target.TakeDamage(e.BaseDamage());
                break;
            case Entity.Move.DEFEND:
                e.Defend();
                break;
            case Entity.Move.RECHARGE:
                e.Recharge();
                break;
            case Entity.Move.SPECIAL:
                e.state.target.TakeDamage(e.Special());
                break;
        }
    }
}