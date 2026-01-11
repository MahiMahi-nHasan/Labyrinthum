using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEnemy : BattleNPC
{
    public override int Special()
    {
        foreach (BattleEntity entity in BattleInterface.active.players)
            entity.TakeDamage((int)(BaseDamage() * 0.33));
        
        // Deal double damage to the target
        return Specials.Hurricane(BaseDamage());
    }
}
