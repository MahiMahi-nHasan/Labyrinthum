using UnityEngine;

public class Chest : Interactable
{
    public Equipment[] lootIndex;
    private bool opened = false;

    // For use with Billboard shader graph
    public SpriteRenderer rendererObject;
    public Sprite closedSprite;
    public Sprite openSprite;

    void Start()
    {
        rendererObject.sprite = closedSprite;
    }

    public override void Interact()
    {
        if (!opened)
        {
            int id = LootTable.GetItem();

            InventoryManager.inventory.Add(lootIndex[id]);

            rendererObject.sprite = openSprite;
        }

        opened = true;
    }
}