using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LootTable
{
    // IDs correspond to items in Chest.cs
    static List<int> loot = new() { 1, 2, 3, 4, 5, 6, 7, 8 }; 
    public static Stack<int> s = new Stack<int>();

    public static void Initialize()
    {
        int size = loot.Count;

        while (size > 0)
        {
            int rand = Random.Range(0, size);
            s.Push(loot[rand]);
            //Debug.Log("Pushed {s}!"+loot[rand]);
            loot.Remove(loot[rand]);
            size = loot.Count;
        }

    }
    // Update is called once per frame
    public static int GetItem()
    {
        return s.Pop();
    }

}
