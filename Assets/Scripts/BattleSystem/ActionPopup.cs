using TMPro;
using UnityEngine;

public class ActionPopup : MonoBehaviour
{
    public TMP_Text action;

    [Header("Animation Settings")]
    public float slideInTime = 0.25f;
    public float holdTime = 0.8f;
    public float slideOutTime = 0.35f;
    public float slideDistance = 200f;

    private RectTransform rect;
    private Vector2 startPos;
    private Vector2 endPos;
    private float timer;
    private enum State { SlideIn, Hold, SlideOut }
    private State state;

    public void Initialize(string text, Color color)
    {
        action.text = text;
        action.color = color;
        rect = GetComponent<RectTransform>();
        endPos = rect.anchoredPosition;
        startPos = endPos + Vector2.left * slideDistance;

        rect.anchoredPosition = startPos;
        state = State.SlideIn;
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case State.SlideIn:
                float tIn = timer / slideInTime;
                rect.anchoredPosition = Vector2.Lerp(startPos, endPos, tIn);

                if (tIn >= 1f)
                {
                    timer = 0f;
                    state = State.Hold;
                }
                break;

            case State.Hold:
                if (timer >= holdTime)
                {
                    timer = 0f;
                    state = State.SlideOut;
                }
                break;

            case State.SlideOut:
                float tOut = timer / slideOutTime;
                rect.anchoredPosition = Vector2.Lerp(endPos, endPos + Vector2.right * slideDistance, tOut);
                action.alpha = 1f - tOut;

                if (tOut >= 1f)
                    Destroy(gameObject);
                break;
        }
    }
}