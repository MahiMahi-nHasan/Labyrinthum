using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialButtonUI : MonoBehaviour
{
    public Button button;
    public TMP_Text label;
    public Image elementIcon;
    public TMP_Text manaCostText;

    [Header("ElementUI")]
    public Sprite phys;
    public Sprite fire;
    public Sprite ice;
    public Sprite wind;

    public void SetElement(BattleEntity.Element elem)
    {
        elementIcon.sprite = GetSprite(elem);
    }
    Sprite GetSprite(BattleEntity.Element elem)
    {
        return elem switch
        {
            BattleEntity.Element.PHYS => phys,
            BattleEntity.Element.FIRE => fire,
            BattleEntity.Element.ICE => ice,
            BattleEntity.Element.WIND => wind,
            _ => null
        };
    }
    public void SetAffordable(bool canAfford)
    {
        label.color = canAfford ? Color.black : Color.gray;
        manaCostText.color = canAfford ? Color.blue : Color.red;
    }

}