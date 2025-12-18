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
    public struct EntityState
    {
        public State hmHeuristic;
        public Move plannedMove;
        public Element element;
        public Entity target;
        public bool alive;
    }

    public string entityName;

    public int strength;
    public int defense;
    public int speed;
    public int health;
    public int maxHealth;
    public int mana;
    public int maxMana;

    public EntityState state;

    public float lowHealthPercentThreshold = 0.35f;
    public int manaRequiredForSpecial = 5;
    public float rechargeManaPercent = 0.1f;
    public int defendModifier = 5;
    public bool isDefending;

    void Awake()
    {
        if (entityName == "")
            entityName = name;
            
        state.alive = true;
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
        return (int)(strength * weaknessMatrix[(int)state.element, (int)state.target.state.element]);
    }

    public void TakeDamage(int baseDamage)
    {
        int damage = baseDamage - defense;
        if (isDefending)
            damage -= defendModifier;

        Debug.Log("Entity " + entityName + " took " + damage + " damage");

        health -= damage;

        state.alive = health > 0;
    }

    public void Defend()
    {
        Debug.Log("Entity " + entityName + " is defending");
        isDefending = true;
    }

    public void Recharge()
    {
        Debug.Log("Entity " + entityName + " is recharging");
        mana += (int)(rechargeManaPercent * maxMana);
        // Clamp value to maximum
        if (mana > maxMana)
            mana = maxMana;
    }

    // Override this method with special behavior in subclasses
    public abstract int Special();
    public bool CanUseSpecial => mana >= manaRequiredForSpecial;

    public void SelectMove(Move move) => state.plannedMove = move;
}