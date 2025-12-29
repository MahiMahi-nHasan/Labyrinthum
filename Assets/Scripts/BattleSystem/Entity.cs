using System;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public static float[,] weaknessMatrix = {
        {1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1}
    };

    public enum State
    {
        LOW_HEALTH_LOW_MANA = 0b00,
        LOW_HEALTH_HIGH_MANA = 0b01,
        HIGH_HEALTH_LOW_MANA = 0b10,
        HIGH_HEALTH_HIGH_MANA = 0b11
    }
    public enum Move
    {
        ATTACK,
        SPECIAL,
        DEFEND,
        RECHARGE
    }
    public enum Element
    {
        Fire = 0,
        Water = 1
    }

    // Contains all necessary data to train transition matrices
    [Serializable]
    public struct EntityState
    {
        public State hmHeuristic;
        public Move plannedMove;
        public Element element;
        public Entity target;
        public bool dead;
    }
    public Element element;

    public string entityName;
    public bool isPlayer;

    public int id;
    public int baseStrength;
    public int Strength
    {
        get
        {
            return baseStrength;
        }
    }
    public int baseDefense;
    public int Defense
    {
        get
        {
            return baseDefense;
        }
    }
    public int baseSpeed;
    public int Speed
    {
        get
        {
            return baseSpeed;
        }
    }
    public int health
    {
        get
        {
            return EntityManager.entities[id].health;
        }
        set
        {
            EntityData data = EntityManager.entities[id];
            data.health = value;
            EntityManager.entities[id] = data;
        }
    }
    public int maxHealth;
    public int mana
    {
        get
        {
            return EntityManager.entities[id].health;
        }
        set
        {
            EntityData data = EntityManager.entities[id];
            data.mana = value;
            EntityManager.entities[id] = data;
        }
    }
    public int maxMana;

    public EntityState state;

    public float lowHealthPercentThreshold = 0.35f;
    public int manaRequiredForSpecial = 5;
    public float rechargeManaPercent = 0.1f;
    public int defendModifier = 5;
    public bool isDefending;

    void Awake()
    {
        state.element = element;
        health = maxHealth;
    }

    protected void Update()
    {
        //Debug.Log(entityName + state.dead);

        if (state.dead)
            OnDeath();
    }

    public void SetHealthManaHeuristic()
    {
        int hmHeuristic = 0;
        if (health > maxHealth * lowHealthPercentThreshold)
            hmHeuristic |= 0b10;
        if (mana > manaRequiredForSpecial)
            hmHeuristic |= 0b01;
        state.hmHeuristic = (State)hmHeuristic;
    }

    public void SetPlannedMove(Move move) => state.plannedMove = move;

    public int BaseDamage()
    {
        Debug.Log("Entity " + entityName + " returned base damage");
        return (int)(Strength * weaknessMatrix[(int)state.element, (int)state.target.state.element]);
    }

    public void TakeDamage(int baseDamage)
    {
        // Change this later
        int damage = baseDamage - Defense;
        if (isDefending)
            // Change this later
            damage -= defendModifier;

        // Ensure damage is not negative
        damage = Math.Max(0, damage);

        Debug.Log("Entity " + entityName + " will take " + damage + " damage");

        health -= damage;
        state.dead = health <= 0;
    }

    public void Defend()
    {
        Debug.Log("Entity " + entityName + " is defending");
        isDefending = true;
    }

    public void Recharge()
    {
        Debug.Log("Entity " + entityName + " is recharging");
        mana = (int)(rechargeManaPercent * maxMana);

        mana = Mathf.Clamp(mana, 0, maxMana);
    }

    // Override this method with special behavior in subclasses
    public abstract int Special();
    public bool CanUseSpecial => mana >= manaRequiredForSpecial;

    public void SelectMove(Move move) => state.plannedMove = move;

    public void OnDeath()
    {
        gameObject.SetActive(false);
    }
}