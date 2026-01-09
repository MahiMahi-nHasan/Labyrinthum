using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LootTable
{
    // IDs correspond to items in Chest.cs
    static readonly List<int> loot = new() { 0, 1, 2, 3, 4, 5, 6, 7 }; 
    public static Stack<int> s = new Stack<int>();

    public static void RefillStack()
    {
        List<int> m_loot = new(loot);

        for (int i = 0; i < loot.Count; i++)
        {
            int rand = Random.Range(0, m_loot.Count);
            s.Push(loot[rand]);
            Debug.Log("Pushed " + loot[rand]);
            m_loot.Remove(loot[rand]);
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
