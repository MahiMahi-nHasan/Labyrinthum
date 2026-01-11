using TMPro;
using UnityEngine;

public class ActionPopup : MonoBehaviour
{
    public TMP_Text action;

    [Header("Animation Settings")]
    public float lifetime = 1.6f;
    public float floatSpeed = 5f;
    public float fadeSpeed = 3f;

    private float timer;
    private Color startColor = new();

    public void Initialize(string action, Color color)
    {
        this.action.text = action;
        this.action.color = color;
        
        startColor = color;
    }

    void Update()
    {
        // Float upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        timer += Time.deltaTime;
        float t = timer / lifetime;

        action.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0), t);

        if (timer >= lifetime)
            Destroy(gameObject);

    }
}