using System.Runtime.InteropServices;

public static class Specials
{
    // Player
    public static int SmashDamage(int baseDamage) => (int)(baseDamage * 2.0);

    // Phys
    public static int PummelingDamage(int baseDamage) => (int)(baseDamage * 1.5);
    public static int SlashDamage(int baseDamage) => baseDamage + 100;
    public static int ClawSwipeDamage(int baseDamage) => baseDamage + 50;
    public static int KickDamage(int baseDamage) => (int)(baseDamage * 1.2);

    // Fire
    public static int Immolate(int baseDamage) => (int)(baseDamage *1.2);
    // Ice
    public static int Snowstorm(int baseDamage) => (int)(baseDamage *1.2);
    // Wind
    public static int Hurricane(int baseDamage) => (int)(baseDamage * 1.2);
}