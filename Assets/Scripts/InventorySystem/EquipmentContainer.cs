using UnityEngine;

public class EquipmentContainer : Interactable
{
    public Equipment equipment;

    public override void Interact()
    {
        InventoryManager.inventory.Add(equipment);
        Destroy(gameObject);
    }
}