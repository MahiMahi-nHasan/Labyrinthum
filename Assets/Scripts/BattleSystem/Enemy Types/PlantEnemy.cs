using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantEnemy : BattleNPC
{
    private readonly int healAmount = 10;

    public override int Special()
    {
        state.target = this;

        // Get all enemies and heal a certain amount
        foreach (BattleEntity entity in BattleInterface.active.npcs)
            entity.Heal(healAmount);

        // Healer does 0 damage, only heals teammates
        return 0;
    }
}
