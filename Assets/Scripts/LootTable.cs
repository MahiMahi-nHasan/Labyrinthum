using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
    // Start is called before the first frame update
    List<int> loot = new() {1,2,3,4,5,6,7,8}; 
    // In order: Healing Potion, Mana Potion, Potent Potion, Demon Armor, Sage Armor, Nightmare Mace, Magic Bomb, Angel Blessing
    public static Stack<int> s = new Stack<int>();
    [SerializeField] private int totalChests;
    private GameObject[] chests;
    
    void Start()
    {
        int size = loot.Count;

        while (size>0)
        {
            int rand = Random.Range(0,size);
            s.Push(loot[rand]);
            Debug.Log("Pushed {s}!"+loot[rand]);
            loot.Remove(loot[rand]);
            size = loot.Count;


        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void ChestGen()
    {
        
    }
    
}
