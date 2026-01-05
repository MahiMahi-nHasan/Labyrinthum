using System.Collections.Generic;
using UnityEngine;

public class OverworldEntity : MonoBehaviour
{
    public int id;
    public Entity baseEntity;
    public List<int> party;

    void OnDestroy()
    {
        EntityData data = EntityManager.entities[id];
        data.instance = null;
        EntityManager.entities[id] = data;
    }

    void Update()
    {
        // Modify this object then assign it to the allocated slot
        EntityData data = EntityManager.entities[id];

        data.position = transform.position;
        data.rotation = transform.rotation;

        EntityManager.entities[id] = data;
    }

    public void Equip(Equipment equipment)
    {
        EntityData data = EntityManager.entities[id];
        data.equipped = equipment;
        EntityManager.entities[id] = data;

        InventoryManager.inventory.Remove(equipment);
    }

    public void AddToParty(int other)
    {
        party.Add(other);
        EntityManager.entities[id].party.Add(other);
    }
}