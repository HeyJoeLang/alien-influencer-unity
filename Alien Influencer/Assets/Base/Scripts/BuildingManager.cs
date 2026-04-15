using UnityEngine;
using System.Collections.Generic;
using System;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private List<Building> allBuildings = new List<Building>();
    private List<Building> destroyedBuildings = new List<Building>();
    
    // Event that emits the percentage of destroyed buildings (0 to 1)
    public static event Action<float> BuildingsDestroyedPercentage;
    
    void Awake()
    {
        destroyedBuildings = new List<Building>();
        
        // Find all buildings in the scene at start
        RegisterAllBuildingsInScene();
    }
    
    /// <summary>
    /// Registers all buildings found in the scene
    /// </summary>
    private void RegisterAllBuildingsInScene()
    {
        Building[] foundBuildings = FindObjectsOfType<Building>();
        foreach (Building building in foundBuildings)
        {
            RegisterBuilding(building);
        }
    }
    
    /// <summary>
    /// Registers a new building to be tracked
    /// </summary>
    /// <param name="building">The building to register</param>
    public void RegisterBuilding(Building building)
    {
        if (building != null && !allBuildings.Contains(building))
        {
            allBuildings.Add(building);
            building.OnBuildingDestroyed += DestroyedBuilding;
        }
    }
    
    /// <summary>
    /// Unregisters a building from tracking (use when building is removed without destruction)
    /// </summary>
    /// <param name="building">The building to unregister</param>
    public void UnregisterBuilding(Building building)
    {
        if (building != null)
        {
            allBuildings.Remove(building);
            building.OnBuildingDestroyed -= DestroyedBuilding;
            CalculateAndEmitPercentage();
        }
    }
    
    /// <summary>
    /// Called when a building is destroyed
    /// </summary>
    /// <param name="building">The destroyed building</param>
    public void DestroyedBuilding(Building building)
    {
        Debug.Log($"BuildingManager: Building destroyed: {building.name}");
        if (building != null && !destroyedBuildings.Contains(building))
        {
            destroyedBuildings.Add(building);
            CalculateAndEmitPercentage();
        }
    }
    
    /// <summary>
    /// Calculates the percentage of destroyed buildings and emits the event
    /// </summary>
    private void CalculateAndEmitPercentage()
    {
        if (allBuildings.Count == 0)
        {
            BuildingsDestroyedPercentage?.Invoke(0f);
            return;
        }
        
        float percentage = (float)destroyedBuildings.Count / (float)allBuildings.Count;
        percentage = Mathf.Clamp01(percentage); // Ensure value stays between 0 and 1
        
        BuildingsDestroyedPercentage?.Invoke(percentage);
        
        Debug.Log($"Buildings destroyed: {destroyedBuildings.Count}/{allBuildings.Count} ({percentage:P1})");
    }
    
    /// <summary>
    /// Gets the current destruction percentage
    /// </summary>
    /// <returns>Percentage of destroyed buildings (0 to 1)</returns>
    public float GetDestructionPercentage()
    {
        if (allBuildings.Count == 0) return 0f;
        return Mathf.Clamp01((float)destroyedBuildings.Count / (float)allBuildings.Count);
    }
    
    /// <summary>
    /// Gets the total number of buildings being tracked
    /// </summary>
    public int GetTotalBuildingsCount()
    {
        return allBuildings.Count;
    }
    
    /// <summary>
    /// Gets the number of destroyed buildings
    /// </summary>
    public int GetDestroyedBuildingsCount()
    {
        return destroyedBuildings.Count;
    }
    
    /// <summary>
    /// Resets the building manager (useful for level resets)
    /// </summary>
    public void Reset()
    {
        // Unsubscribe from all building events before clearing
        foreach (Building building in allBuildings)
        {
            if (building != null)
            {
                building.OnBuildingDestroyed -= DestroyedBuilding;
            }
        }
        
        foreach (Building building in destroyedBuildings)
        {
            if (building != null)
            {
                building.ResetBuilding();
            }
        }
        
        allBuildings.Clear();
        destroyedBuildings.Clear();
        
        RegisterAllBuildingsInScene();
        CalculateAndEmitPercentage();
    }
}