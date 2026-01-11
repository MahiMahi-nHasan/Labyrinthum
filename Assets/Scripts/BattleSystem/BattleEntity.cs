using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleEntity : MonoBehaviour
{
    // Multiplies base damage
    // row = self element
    // col = target element
    public static float[,] weaknessMatrix = {
    // P    F    I    W
    {1f,  1f,  1f,  1f},   // Phys
    {1f,  1f,  1.5f, 0.5f},// Fire
    {1f,  0.5f,1f,   1.5f},// Ice   
    {1f,  1.5f,0.5f, 1f}   // Wind
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
    public Special[] specials = new Special[4];
    public Special chosenSpecial;

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
        if (Mana > chosenSpecial.manaCost)
            hmHeuristic |= 0b01;
        state.hmHeuristic = (State)hmHeuristic;
    }

    public void SetPlannedMove(Move move) => state.plannedMove = move;

    public int BaseDamage()
    {
        int baseDamage = (int)(Strength * weaknessMatrix[(int)state.element, (int)state.target.state.element]);
        Debug.Log("Entity " + baseEntity.entityName + " returned base damage " + baseDamage);
        Debug.Log($"multiplier is {weaknessMatrix[(int)state.element, (int)state.target.state.element]}");
        return baseDamage;
    }

    public void TakeDamage(int baseDamage, Element attackElem)
    {
        int damage = baseDamage * (1 - Defense / 100) * 5;
        if (isDefending)
            damage/=2;

        // Ensure damage is not negative
        damage = Math.Max(0, damage);

        Debug.Log("Entity " + baseEntity.entityName + " with health " + Health + " will take " + damage + " damage");

        Health -= damage;
        state.dead = Health <= 0;
        float multiplier = weaknessMatrix[(int)attackElem, (int)state.element];
        string tag = "";
        Color color = Color.white;

        if (multiplier > 1f)
        {
            tag = "WEAK!";
            color = Color.red;
        }
        else if (multiplier < 1f)
        {
            tag = "Resist...";
            color = Color.cyan;
        }
        PopupHandler.SpawnDamagePopup(this, damage, tag, color);
    }

    public void Heal(int amount)
    {
        if (state.dead)
            return;
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

    public virtual int Special()
    {
        if (chosenSpecial == null)
        {
            Debug.LogWarning($"{name} has no special assigned.");
            return 0;
        }

        if (!CanUseSpecial)
            return 0;

        Mana -= chosenSpecial.manaCost;
        List<BattleEntity> allies = isPlayer ? BattleInterface.active.players : BattleInterface.active.npcs;
        List<BattleEntity> enemies = isPlayer ? BattleInterface.active.npcs : BattleInterface.active.players;
        Debug.Log($"{name} used special {chosenSpecial.name}");

        return chosenSpecial.UseMove(
            this,
            state.target,
            allies,
            enemies
        );

    }
    public bool CanUseSpecial => chosenSpecial != null && Mana >= chosenSpecial.manaCost;
    public void SelectMove(Move move) => state.plannedMove = move;

    public void OnDeath()
    {
        gameObject.SetActive(false);
    }
}