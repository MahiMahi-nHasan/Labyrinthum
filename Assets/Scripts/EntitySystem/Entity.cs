using UnityEngine;

[CreateAssetMenu(fileName = "New Entity", menuName = "Labyrinthum/New Entity")]
public class Entity : ScriptableObject
{
    public string entityName;

    public Sprite sprite;

    public GameObject prefab;
    public GameObject battlePrefab;

    public int maxHealth;
    public int maxMana;
    public int baseStrength;
    public int baseDefense;
    public int baseSpeed;

    public BattleEntity.Element element;

    public float rechargeManaPercent = 0.1f;
    public int defendModifier = 5;
}