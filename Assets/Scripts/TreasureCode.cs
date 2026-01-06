using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureCode : MonoBehaviour
{
    public LootTable money;
    // Start is called before the first frame update
    void Start()
    {
        LootTable.s.Pop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
