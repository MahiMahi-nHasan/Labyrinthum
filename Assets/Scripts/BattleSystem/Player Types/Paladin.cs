public class Paladin : BattleEntity
{
    // Smash
    public override int Special()
    {
        // Deal a little damage to every enemy
        foreach (BattleEntity entity in BattleInterface.active.npcs)
            entity.TakeDamage((int)(BaseDamage() * 0.5));

        // Deal double damage to the target
        return Specials.SmashDamage(BaseDamage());
    }
}