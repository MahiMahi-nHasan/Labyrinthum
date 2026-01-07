using UnityEngine;

public class Chest : Interactable
{
    public Equipment[] lootIndex;

    public override void Interact()
    {
        int id = LootTable.GetItem();

        InventoryManager.inventory.Add(lootIndex[id]);

        Destroy(gameObject);
    }
}