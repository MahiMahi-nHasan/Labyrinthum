using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DKEnemy : BattleNPC
{
    // Start is called before the first frame update

    public override int Special()
    {
        foreach (BattleEntity entity in BattleInterface.active.players)
            entity.TakeDamage((int)(BaseDamage() * 0.4), BattleEntity.Element.FIRE);
        
        // Deal double damage to the target
        return Specials.Immolate((int)1.2*BaseDamage());
    }
}
