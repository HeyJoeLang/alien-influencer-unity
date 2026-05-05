using UnityEngine;
using System.Collections.Generic;
using System;
using FMODUnity;

public class BuildingManager : MonoBehaviour
{
    public MissileSpawner missileSpawner;
    public Animator fadeInOutAnimator;
    public StudioEventEmitter studioEventEmitter;
    [SerializeField] private List<Building> allBuildings = new List<Building>();
    private List<Building> destroyedBuildings = new List<Building>();
    bool canTriggerScoreMultiplierIncrease = true;

    // Event that emits the percentage of destroyed buildings (0 to 1)
    public static event Action<float> BuildingsDestroyedPercentage;

    // Events for tracking rapid destruction
    public static event Action<int> OnMassDestruction; // Emitted when >5 buildings destroyed in <0.5s
    public static event Action<int> OnMinorDestruction; // Emitted when <=5 buildings destroyed in <0.5s

    // Tracking for rapid destruction
    private List<float> recentDestructionTimes = new List<float>();
    private const float destructionTimeWindow = 0.5f;

    [Range(0, 1)]
    public float nextPhaseDestructionPercent = .8f;

    public ProgressBarPro destructionPercentageBar;
    public float bgMusicIntensityStage1 = .1f;
    public float bgMusicIntensityStage2 = .2f;
    public float bgMusicIntensityStage3 = .45f;
    public float bgMusicIntensityStage4 = .8f;
    float bgMusicIntensity = 1;

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
            TrackRapidDestruction();
            CalculateAndEmitPercentage();
        }
    }

    /// <summary>
    /// Tracks destruction timing and emits events based on destruction rate
    /// </summary>
    private void TrackRapidDestruction()
    {
        float currentTime = Time.time;
        recentDestructionTimes.Add(currentTime);

        // Remove destruction times outside the time window
        recentDestructionTimes.RemoveAll(time => currentTime - time > destructionTimeWindow);

        // Check if we've accumulated enough destruction events
        int destructionCount = recentDestructionTimes.Count;

        if (destructionCount > 5)
        {
            OnMassDestruction?.Invoke(destructionCount);
            Debug.Log($"Mass Destruction! {destructionCount} buildings destroyed in {destructionTimeWindow}s");
        }
        else if (destructionCount > 0)
        {
            OnMinorDestruction?.Invoke(destructionCount);
            Debug.Log($"Minor Destruction: {destructionCount} buildings destroyed in {destructionTimeWindow}s");
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

        float percentage = destroyedBuildings.Count / (allBuildings.Count * nextPhaseDestructionPercent);
        percentage = Mathf.Clamp01(percentage); // Ensure value stays between 0 and 1

        BuildingsDestroyedPercentage?.Invoke(percentage);

        Debug.Log($"Buildings destroyed: {destroyedBuildings.Count}/{allBuildings.Count * nextPhaseDestructionPercent} ({percentage:P1})");
        destructionPercentageBar.SetValue(percentage);
        if(percentage >= bgMusicIntensityStage4)
        {
            bgMusicIntensity = 3.0f;
        }
        else if (percentage >= bgMusicIntensityStage3)
        {
            bgMusicIntensity = 2.0f;
        }
        else if (percentage >= bgMusicIntensityStage2)
        {
            bgMusicIntensity = 1.0f;
        }
        else if (percentage >= bgMusicIntensityStage1)
        {
            bgMusicIntensity = 0.0f;
        }

        // Set FMOD Intensity parameter based on percentage
        if (studioEventEmitter != null)
        {
            // Ensure the event is playing before setting the parameter
            if (!studioEventEmitter.IsPlaying())
            {
                studioEventEmitter.Play();
            }
            studioEventEmitter.SetParameter("Intensity", percentage*4f);
        }
        
        if (percentage >= 1f)
        {
            if (canTriggerScoreMultiplierIncrease)
            {
                canTriggerScoreMultiplierIncrease = false;
                GameManager.Instance.IncreaseScoreMultiplier();
            }
        }
    }

    /// <summary>
    /// Gets the current destruction percentage
    /// </summary>
    /// <returns>Percentage of destroyed buildings (0 to 1)</returns>
    public float GetDestructionPercentage()
    {
        if (allBuildings.Count == 0) return 0f;
        return Mathf.Clamp01((float)destroyedBuildings.Count / ((float)allBuildings.Count * nextPhaseDestructionPercent));
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
        recentDestructionTimes.Clear();

        RegisterAllBuildingsInScene();
        CalculateAndEmitPercentage();
        canTriggerScoreMultiplierIncrease = true;
    }
}