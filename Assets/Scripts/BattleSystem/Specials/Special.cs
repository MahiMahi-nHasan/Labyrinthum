using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Special", menuName = "Labyrinthum/New Special")]
public class Special : ScriptableObject
{
    public enum SpecialType
    {
        Damage,
        Heal,
        AoEHeal,
        MultiHit,
        AoE,
        NukeAoE,
        Buff, //Don't worry about these two for now, they're for future expansion
        Debuff,
    }
    public enum TargetingType
    {
        SingleEnemy,
        SingleAlly,
        AllEnemies,
        AllAllies,
        Self
    }


    [Header("General Parameters")]
    [SerializeField] public string Name;
    [SerializeField] public int manaCost;
    [SerializeField] public BattleEntity.Element elem;
    [SerializeField] public int power;
    [SerializeField] public int hits;

    [Header("Behaviour")]
    [SerializeField] public SpecialType type;
    [SerializeField] public TargetingType targetingType;


    public int UseMove(
        BattleEntity user,
        BattleEntity target,
        List<BattleEntity> allies,
        List<BattleEntity> enemies)
    {
        switch (type)
        {
            case SpecialType.Damage:
                int dmg = DamageSingle(user, target); 
                target.TakeDamage(dmg, elem);
                return dmg;
            case SpecialType.Heal:
                target.Heal(power);
                return 0;
            case SpecialType.AoEHeal:
                foreach(BattleEntity ally in allies)
                {
                    if (ally != null && !ally.state.dead)
                        ally.Heal(power);
                }
                return 0;
            case SpecialType.MultiHit:
                int total = 0;
                for (int i = 0; i < hits; i++)
                {
                    dmg = DamageSingle(user, target);
                    target.TakeDamage(dmg, elem);
                    total += dmg;
                }
                return total;
            case SpecialType.AoE:
                return DamageAOE(user, enemies);
            case SpecialType.NukeAoE:
                return NukeAoE(user, enemies);
            case SpecialType.Buff:
                return 0;
            case SpecialType.Debuff:
                return 0;
            default:
                Debug.LogWarning("Special type not implemented");
                return 0;
        }
    }
    int DamageSingle(BattleEntity user, BattleEntity target)
    {
        float multiplier = BattleEntity.weaknessMatrix[(int)elem, (int)target.state.element];
        Debug.Log($"multiplier is {multiplier}");
        return Mathf.RoundToInt((user.Strength + power) * multiplier);
    }
    int DamageAOE(BattleEntity user, List<BattleEntity> targets) 
    {
        int total = 0;
        foreach (BattleEntity target in targets) 
        {
            if (target == null || target.state.dead)
                continue;

            int dmg = DamageSingle(user, target);
            target.TakeDamage(dmg, elem);
            total += dmg;

        }
        return total;
    }
    int NukeAoE(BattleEntity user, List<BattleEntity> enemies)
    {
        int total = 0;

        // 33% AoE
        foreach (BattleEntity target in enemies)
        {
            if (target == null || target.state.dead)
                continue;

            int dmg = Mathf.RoundToInt(DamageSingle(user, target) * 0.33f);
            target.TakeDamage(dmg, elem);
            total += dmg;
        }
        int single = DamageSingle(user, user.state.target);
        user.state.target.TakeDamage(single, elem);
        total += single;

        return total;
    }
}
