using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI tagText;

    [Header("Animation Settings")]
    public float lifetime = 1.6f;
    public float floatSpeed = 1.5f;
    public float fadeSpeed = 3f;
    public Vector2 randomOffset = new Vector2(0.3f, 0.3f);

    private float timer;
    private Color startColor;

    void Start()
    {
        // Slight random offset so popups don't overlap perfectly
        transform.position += new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(0, randomOffset.y),
            0
        );

        startColor = damageText.color;
    }

    public void Initialize(int amount, string tag, Color color)
    {
        damageText.text = amount.ToString();
        damageText.color = color;

        tagText.text = tag;
        tagText.color = color;
    }

    void Update()
    {
        // Float upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        timer += Time.deltaTime;
        float t = timer / lifetime;

        damageText.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0), t);

        if (timer >= lifetime)
            Destroy(gameObject);

    }
}