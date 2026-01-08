using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarsManager : MonoBehaviour
{
    public Slider healthBar;
    public TMP_Text healthText;
    private int maxHealth;

    public Slider manaBar;
    public TMP_Text manaText;
    private int maxMana;

    public void Initialize(int maxHealth, int maxMana)
    {
        this.maxHealth = maxHealth;
        this.maxMana = maxMana;

        healthBar.maxValue = maxHealth;
        manaBar.maxValue = maxMana;
    }

    public void UpdateHealth(int health)
    {
        healthBar.value = health;

        healthText.text = string.Format("{0} / {1}", health, maxHealth);
    }

    public void UpdateMana(int mana)
    {
        manaBar.value = mana;
        
        manaText.text = string.Format("{0} / {1}", mana, maxMana);
    }
}
