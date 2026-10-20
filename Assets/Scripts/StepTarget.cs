using UnityEngine;

public class StepTarget : MonoBehaviour
{
    public string actionId; // doit correspondre à un StepData.actionId[cite: 8]

    [SerializeField] Color highlightColor = new Color(1f, 0.85f, 0.1f);
    [SerializeField] bool pulse = true;
    [SerializeField] float pulseSize = 0.08f;

    Renderer rend;
    Color originalColor;
    Vector3 baseScale;
    bool highlighted;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }
        baseScale = transform.localScale;
    }

    public void SetHighlight(bool on)
    {
        highlighted = on;
        if (rend != null)
        {
            rend.material.color = on ? highlightColor : originalColor;
        }
        if (!on) transform.localScale = baseScale;
    }

    void Update()
    {
        if (!highlighted || !pulse) return;
        float s = 1f + pulseSize * Mathf.Sin(Time.time * 6f);
        transform.localScale = baseScale * s;
    }
}