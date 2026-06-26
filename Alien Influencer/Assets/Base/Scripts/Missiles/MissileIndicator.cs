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
        }
        else
        {
            indicator.transform.localPosition = trackingLocalPosition;
            // Very close - fully red
            newColor.a = 1f;
            newColor.r = 1f;
            newColor.g = 0f;
            newColor.b = 0f;
        }
        
        indicatorRenderer.material.color = newColor;
    }
}
