using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Labyrinthum/New Equipment")]
public class Equipment : ScriptableObject
{
    public string equipmentName;
    public string description;
    public Sprite sprite;
    public int strengthModifier;
    public int defenseModifier;
    public int speedModifier;
}