using System;
using UnityEngine;

public abstract class BattleEntity : MonoBehaviour
{
    // Multiplies base damage
    // row = self element
    // col = target element
    public static float[,] weaknessMatrix = {
        {1, 0.5f, 1, 1.5f},
        {1.5f, 1, 0.5f, 1},
        {1, 1.5f, 1, 0.5f},
        {0.5f, 1, 1.5f, 1}
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
        PHYS = 0,
        FIRE = 1,
        ICE = 2,
        WIND = 3
    }

    // Contains all necessary data to train transition matrices
    [Serializable]
    public struct EntityState
    {
        public State hmHeuristic;
        public Move plannedMove;
        public Element element;
        public BattleEntity target;
        public bool dead;
    }

    public Entity baseEntity;
    public bool isPlayer;

    public BarsManager barsManager;

    public int id;

    public int Strength => EntityManager.entities[id].Strength;
    public int Defense => EntityManager.entities[id].Defense;
    public int Speed => EntityManager.entities[id].Speed;
    public int Health
    {
        get
        {
            return EntityManager.entities[id].Health;
        }
        set
        {
            EntityData data = EntityManager.entities[id];
            data.Health = value;
            EntityManager.entities[id] = data;
        }
    }
    private int healthLastFrame;
    public int Mana
    {
        get
        {
            return EntityManager.entities[id].Mana;
        }
        set
        {
            EntityData data = EntityManager.entities[id];
            data.Mana = value;
            EntityManager.entities[id] = data;
        }
    }
    private int manaLastFrame;

    public EntityState state;

    public float lowHealthPercentThreshold = 0.35f;
    public bool isDefending;

    void Awake()
    {
        state.element = baseEntity.element;
        barsManager.Initialize(baseEntity.maxHealth, baseEntity.maxMana);
        barsManager.UpdateHealth(Health);
        barsManager.UpdateMana(Mana);
    }

    protected void Update()
    {
        //Debug.Log(entityName + state.dead);

        SetHealthManaHeuristic();

        if (state.dead)
            OnDeath();

        if (Health != healthLastFrame)
            barsManager.UpdateHealth(Health);
        if (Mana != manaLastFrame)
            barsManager.UpdateMana(Mana);

        healthLastFrame = Health;
        manaLastFrame = Mana;
    }

    public void SetHealthManaHeuristic()
    {
        int hmHeuristic = 0;
        if (Health > baseEntity.maxHealth * lowHealthPercentThreshold)
            hmHeuristic |= 0b10;
        if (Mana > baseEntity.manaRequiredForSpecial)
            hmHeuristic |= 0b01;
        state.hmHeuristic = (State)hmHeuristic;
    }

    public void SetPlannedMove(Move move) => state.plannedMove = move;

    public int BaseDamage()
    {
        int baseDamage = (int)(Strength * weaknessMatrix[(int)state.element, (int)state.target.state.element]);
        Debug.Log("Entity " + baseEntity.entityName + " returned base damage " + baseDamage);
        return baseDamage;
    }

    public void TakeDamage(int baseDamage)
    {
        int damage = baseDamage * (1 - Defense / 100) * 5;
        if (isDefending)
            damage/=2;

        // Ensure damage is not negative
        damage = Math.Max(0, damage);

        Debug.Log("Entity " + baseEntity.entityName + " with health " + Health + " will take " + damage + " damage");

        Health -= damage;
        state.dead = Health <= 0;
    }

    public void Heal(int amount)
    {
        Health += amount;
        Health = Math.Clamp(Health, 0, baseEntity.maxHealth);
    }

    public void Defend()
    {
        Debug.Log("Entity " + baseEntity.entityName + " is defending");
        isDefending = true;
    }

    public void Recharge()
    {
        Debug.Log("Entity " + baseEntity.entityName + " is recharging");
        Mana += (int)(baseEntity.rechargeManaPercent * baseEntity.maxMana);

        Mana = Mathf.Clamp(Mana, 0, baseEntity.maxMana);
    }

    public int BaseSpecial()
    {
        if (!CanUseSpecial)
            return 0;
        
        Mana -= baseEntity.manaRequiredForSpecial;
        return Special();
    }
    // Override this method with special behavior in subclasses
    public abstract int Special();
    public bool CanUseSpecial => Mana >= baseEntity.manaRequiredForSpecial;

    public void SelectMove(Move move) => state.plannedMove = move;

    public void OnDeath()
    {
        gameObject.SetActive(false);
    }
}