using TMPro;
using UnityEngine;

public class EquipDisplay : MonoBehaviour
{
    public int id;
    public TMP_Text statsField;

    void Update()
    {
        string stats = string.Format(
            "Strength: {0}\nDefense: {1}\nSpeed: {2}",
            EntityManager.entities[id].Strength,
            EntityManager.entities[id].Defense,
            EntityManager.entities[id].Speed
        );

        statsField.text = stats;
    }
}