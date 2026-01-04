using UnityEngine;

[CreateAssetMenu(fileName = "New Entity", menuName = "Labyrinthum/New Entity")]
public class Entity : ScriptableObject
{
    public GameObject prefab;
    public GameObject battlePrefab;
    
    public int baseStrength;
    public int baseDefense;
    public int baseSpeed;
}