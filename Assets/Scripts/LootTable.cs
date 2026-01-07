using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LootTable
{
    // IDs correspond to items in Chest.cs
    static List<int> loot = new() { 1, 2, 3, 4, 5, 6, 7, 8 }; 
    public static Stack<int> s = new Stack<int>();

    public static void RefillStack()
    {
        List<int> m_loot = new(loot);

        while (m_loot.Count > 0)
        {
            int rand = Random.Range(0, m_loot.Count);
            s.Push(loot[rand]);
            //Debug.Log("Pushed {s}!"+loot[rand]);
            loot.Remove(loot[rand]);
        }

    }
    // Update is called once per frame
    public static int GetItem()
    {
        if (s.Count == 0)
            RefillStack();
        
        return s.Pop();
    }

}
