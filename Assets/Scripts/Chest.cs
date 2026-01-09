using UnityEngine;

public class Chest : Interactable
{
    public Equipment[] lootIndex;
    private bool opened = false;

    public override void Interact()
    {
        if (!opened)
        {
            int id = LootTable.GetItem();

            InventoryManager.inventory.Add(lootIndex[id]);
        }

        opened = true;
    }
}