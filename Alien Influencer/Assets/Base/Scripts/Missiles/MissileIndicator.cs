using UnityEngine;
using UnityEngine.UI;

public class MissileIndicator : MonoBehaviour
{
       [Header("Target")]
    public GameObject lookAtMissile;

    [Header("Pointer")] public GameObject indicator;
    
    [Header("Distance Settings")]
    public float distanceStartVisible = 100f;
    public float distanceFullyVisible = 50f;
    public float distanceRed = 10f;

    [Header("Pulse & Scale (danger emphasis)")]
    public float pulseSpeed = 6f;
    public float pulseSpeedMax = 14f;
    public float pulseAmount = 0.35f;
    public float scaleMin = 1f;
    public float scaleMax = 1.6f;

    private Renderer indicatorRenderer;
    private Vector3 trackingLocalPosition;

    void Start()
    {
        trackingLocalPosition = new Vector3(0, 0, 3.5f);

        indicatorRenderer = indicator.GetComponent<Renderer>();
        // Ensure indicator starts hidden
        if (indicator != null)
        {
            Color color = indicatorRenderer.material.color;
            color.a = 0f;
            indicatorRenderer.material.color = color;
        }
    }

    void Update()
    {
        if (lookAtMissile == null || indicator == null)
        {
            // Hide indicator and stop rotation
            if (indicator != null)
            {
                Color color = indicatorRenderer.material.color;
                color.a = 0f;
                indicatorRenderer.material.color = color;
                indicatorRenderer.transform.localPosition = Vector3.zero;
                indicator.transform.localScale = Vector3.one;
            }
            return;
        }
        
        // Calculate world distance
        float distance = Vector3.Distance(transform.position, lookAtMissile.transform.position);
        
        // Handle rotation - point towards missile
        RotateTowardsMissile();
        
        // Handle indicator visibility and color based on distance
        UpdateIndicatorAppearance(distance);
    }
    
    private void RotateTowardsMissile()
    {
        transform.LookAt(lookAtMissile.transform.position, transform.up);
    }
    
    private void UpdateIndicatorAppearance(float distance)
    {
        Color newColor = indicatorRenderer.material.color;
        float danger = 0f;

        if (distance >= distanceStartVisible)
        {
            // Too far - completely hidden
            newColor.a = 0f;
            indicatorRenderer.transform.localPosition = Vector3.zero;

        }
        else if (distance >= distanceFullyVisible)
        {
            indicator.transform.localPosition = trackingLocalPosition;
            // Fade in using sine curve
            float t = (distanceStartVisible - distance) / (distanceStartVisible - distanceFullyVisible);
            float sineT = Mathf.Sin(t * Mathf.PI * 0.5f); // Sine curve from 0 to 1
            newColor.a = sineT;
            newColor.r = 1f;
            newColor.g = 1f;
            newColor.b = 1f;
        }
        else if (distance >= distanceRed)
        {
            indicator.transform.localPosition = trackingLocalPosition;
            // Fully visible, gradient from white to red
            newColor.a = 1f;
            float t = (distanceFullyVisible - distance) / (distanceFullyVisible - distanceRed);
            newColor.r = 1f;
            newColor.g = Mathf.Lerp(1f, 0f, t);
            newColor.b = Mathf.Lerp(1f, 0f, t);
            danger = t;
        }
        else
        {
            indicator.transform.localPosition = trackingLocalPosition;
            // Very close - fully red
            newColor.a = 1f;
            newColor.r = 1f;
            newColor.g = 0f;
            newColor.b = 0f;
            danger = 1f;
        }

        indicatorRenderer.material.color = newColor;
        ApplyPulseAndScale(danger);
    }

    private void ApplyPulseAndScale(float danger)
    {
        // Grows and starts to "heartbeat" as the missile gets more dangerous - static and
        // unobtrusive at long range, big and pulsing quickly right before impact.
        float currentPulseSpeed = Mathf.Lerp(pulseSpeed, pulseSpeedMax, danger);
        float pulse01 = 0.5f + 0.5f * Mathf.Sin(Time.time * currentPulseSpeed * Mathf.PI * 2f);

        float baseScale = Mathf.Lerp(scaleMin, scaleMax, danger);
        float wobble = 1f + danger * pulseAmount * (pulse01 - 0.5f) * 2f;

        indicator.transform.localScale = Vector3.one * (baseScale * wobble);
    }
}
