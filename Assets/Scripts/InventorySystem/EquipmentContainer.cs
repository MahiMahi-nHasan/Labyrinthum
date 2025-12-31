using UnityEngine;

public class EquipmentContainer : MonoBehaviour
{
    public Equipment equipment;

    public void Pickup()
    {
        InventoryManager.inventory.Add(equipment);
        Destroy(gameObject);
    }
}