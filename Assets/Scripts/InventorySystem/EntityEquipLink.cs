using UnityEngine;

public class EntityEquipLink : MonoBehaviour
{
    public Inventory master;

    public OverworldEntity entity;
    private int id;
    public EquipDisplay display;

    void Start()
    {
        id = entity.id;
        display.id = id;
    }

    public void Equip()
    {
        Equipment toEquip = master.selected;
        master.selected = null;
        EntityData data = EntityManager.entities[id];

        if (data.equipped != null)
            InventoryManager.inventory.Add(data.equipped);
        data.equipped = toEquip;
        InventoryManager.inventory.Remove(toEquip);

        EntityManager.entities[id] = data;

        master.RegenerateSlots();
    }
}