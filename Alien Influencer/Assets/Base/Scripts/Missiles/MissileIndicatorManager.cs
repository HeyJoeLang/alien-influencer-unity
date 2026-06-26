using System.Collections.Generic;
using UnityEngine;

public class MissileIndicatorManager : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject missileIndicatorPrefab;
    public int poolSize = 10;
    
    private Queue<MissileIndicator> indicatorPool = new Queue<MissileIndicator>();
    private Dictionary<GameObject, MissileIndicator> activeMissileIndicators = new Dictionary<GameObject, MissileIndicator>();
    
    void Start()
    {
        InitializePool();
        
        // Subscribe to missile creation event
        HomingMissile.shoot_missile_example.MissileCreated += OnMissileCreated;
    }
    
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        HomingMissile.shoot_missile_example.MissileCreated -= OnMissileCreated;
        
        // Unsubscribe from all active missile destruction events
        foreach (var kvp in activeMissileIndicators)
        {
            GameObject missileObj = kvp.Key;
            if (missileObj != null)
            {
                HomingMissile.homing_missile missile = missileObj.GetComponent<HomingMissile.homing_missile>();
                if (missile != null)
                {
                    missile.MissileDestroyed -= OnMissileDestroyed;
                }
            }
        }
    }
    
    private void InitializePool()
    {
        // Create pool of inactive missile indicators
        for (int i = 0; i < poolSize; i++)
        {
            GameObject indicatorObj = Instantiate(missileIndicatorPrefab, transform);
            MissileIndicator indicator = indicatorObj.GetComponent<MissileIndicator>();
            
            // Disable the indicator initially
            indicatorObj.SetActive(false);
            
            // Add to pool
            indicatorPool.Enqueue(indicator);
        }
    }
    
    private void OnMissileCreated(GameObject missile)
    {
        // Get an indicator from the pool
        if (indicatorPool.Count > 0)
        {
            MissileIndicator indicator = indicatorPool.Dequeue();
            
            // Activate and configure the indicator
            indicator.gameObject.SetActive(true);
            indicator.lookAtMissile = missile;
            
            // Track this missile-indicator pair
            activeMissileIndicators[missile] = indicator;
            
            // Subscribe to the missile's destruction event
            HomingMissile.homing_missile homingMissile = missile.GetComponent<HomingMissile.homing_missile>();
            if (homingMissile != null)
            {
                homingMissile.MissileDestroyed += OnMissileDestroyed;
            }
        }
        else
        {
            Debug.LogWarning("MissileIndicatorManager: No available indicators in pool!");
        }
    }
    
    private void OnMissileDestroyed(GameObject missile)
    {
        // Return indicator to pool when missile is destroyed
        if (activeMissileIndicators.TryGetValue(missile, out MissileIndicator indicator))
        {
            // Unsubscribe from this missile's destruction event
            HomingMissile.homing_missile homingMissile = missile.GetComponent<HomingMissile.homing_missile>();
            if (homingMissile != null)
            {
                homingMissile.MissileDestroyed -= OnMissileDestroyed;
            }
            
            ReturnIndicatorToPool(missile, indicator);
        }
    }
    
    private void ReturnIndicatorToPool(GameObject missile, MissileIndicator indicator)
    {
        // Remove from active tracking
        activeMissileIndicators.Remove(missile);
        
        // Reset indicator state
        indicator.lookAtMissile = null;
        indicator.gameObject.SetActive(false);
        
        // Return to pool
        indicatorPool.Enqueue(indicator);
    }
    
    // Optional: Method to manually return an indicator to pool (if needed)
    public void ReturnIndicatorToPool(GameObject missile)
    {
        if (activeMissileIndicators.TryGetValue(missile, out MissileIndicator indicator))
        {
            // Unsubscribe from missile destruction event
            HomingMissile.homing_missile homingMissile = missile.GetComponent<HomingMissile.homing_missile>();
            if (homingMissile != null)
            {
                homingMissile.MissileDestroyed -= OnMissileDestroyed;
            }
            
            ReturnIndicatorToPool(missile, indicator);
        }
    }
}