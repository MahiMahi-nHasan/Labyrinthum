using UnityEngine;

public class Cleric : BattleEntity
{
    private readonly int healAmount = 10;

    public override int Special()
    {
        state.target = this;

        // Get all players and heal a certain amount
        foreach (BattleEntity entity in BattleInterface.active.players)
            entity.Heal(healAmount);

        // Healer does 0 damage, only heals teammates
        return 0;
    }
}