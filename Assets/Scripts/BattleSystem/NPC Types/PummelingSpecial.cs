using UnityEngine;

public class PummelingSpecial : BattleNPC
{
    public override int Special()
    {
        // Pummeling

        return Specials.PummelingDamage(BaseDamage());
    }
}