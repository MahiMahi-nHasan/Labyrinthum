using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : BattleEntity
{
    public override int Special()
    {
        return Specials.PummelingDamage(BaseDamage());
    }
}
