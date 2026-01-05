using UnityEngine;

public class SlashSpecial : BattleNPC
{
    public override int Special()
    {
        return Specials.SlashDamage(BaseDamage());
    }
}