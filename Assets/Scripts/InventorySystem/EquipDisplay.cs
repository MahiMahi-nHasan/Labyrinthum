using TMPro;
using UnityEngine;

public class EquipDisplay : MonoBehaviour
{
    public int id;
    public Inventory master;
    public TMP_Text nameField;
    public TMP_Text statsField;
    public TMP_Text equippedField;
    public TMP_Text buttonText;

    void Update()
    {
        EntityData data = EntityManager.entities[id];

        nameField.text = data.baseEntity.entityName;

        string stats = string.Format(
            "Strength: {0}\nDefense: {1}\nSpeed: {2}",
            data.Strength,
            data.Defense,
            data.Speed
        );

        statsField.text = stats;

        if (data.equipped != null)
            equippedField.text = data.equipped.equipmentName;
        else
            equippedField.text = "";

        if (master.selected != null)
            buttonText.text = "Equip";
        else
            buttonText.text = "Unequip";
    }
}