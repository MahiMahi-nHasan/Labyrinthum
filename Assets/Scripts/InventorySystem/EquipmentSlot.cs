using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    private Inventory master;
    private Equipment equipment;

    public Image image;
    public TMP_Text nameField;
    public TMP_Text descField;

    // Call this method when instantiating a new instance of this prefab
    public void Initialize(Inventory master, Equipment equipment)
    {
        image.sprite = equipment.sprite;
        nameField.text = equipment.equipmentName;
        descField.text = equipment.description;

        this.master = master;
        this.equipment = equipment;
    }

    public void Select()
    {
        if (master == null || equipment == null)
        {
            Debug.LogError("Equipment slot has not been initialized!");
            return;
        }

        master.selected = equipment;
    }
}
