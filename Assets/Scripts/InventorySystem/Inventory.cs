using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : Menu
{
    public Equipment selected;
    public Transform contentPane;
    public GameObject slotPrefab;

    private GameObject[] slots;

    void Start()
    {
        slots = new GameObject[0];
    }

    protected override void OnOpenClose(InputAction.CallbackContext context)
    {
        base.OnOpenClose(context);

        if (opened)
            RegenerateSlots();
        else
            DestroyAllSlots();
    }

    public void RegenerateSlots()
    {
        // Get a reference to the global inventory
        List<Equipment> inventory = InventoryManager.inventory;
        // Clear slots list
        slots = new GameObject[inventory.Count];
        // Resize content pane
        RectTransform t = (RectTransform)contentPane.transform;
        t.sizeDelta = new Vector2(t.sizeDelta.x, 100 * inventory.Count);
        // Instantiate and initialize a prefab for each equipment
        for (int i = 0; i < inventory.Count; i++)
        {
            GameObject go = Instantiate(slotPrefab, contentPane);
            ((RectTransform)go.transform).anchoredPosition = new(0, -i * 100);
            go.GetComponent<EquipmentSlot>().Initialize(this, inventory[i]);

            // Save slot to slots array so it can be destroyed later
            slots[i] = go;
        }
    }

    public void DestroyAllSlots()
    {
        foreach (GameObject go in slots)
            Destroy(go);
    }
}
