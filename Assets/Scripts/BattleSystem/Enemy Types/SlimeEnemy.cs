using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : BattleNPC
{
    public override int Special()
    {
        return Specials.PummelingDamage(BaseDamage());
    }
}
