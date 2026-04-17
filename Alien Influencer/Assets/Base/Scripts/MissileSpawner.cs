using UnityEngine;

public class MissileSpawner : MonoBehaviour
{
    public Transform ufoTransform;
    public HomingMissile.shoot_missile_example[] missileLaunchers;
    
    [Header("Controls")]
    public bool pauseFiring = false;
    
    [Header("Difficulty Scaling")]
    public float baseFiringInterval = 5.0f; // Base time between missiles (seconds)
    public float minimumFiringInterval = 0.5f; // Fastest possible firing rate
    public float initialTimeAmount = 100f; // Set this to match your initial game time
    public DifficultyType difficultyType = DifficultyType.Exponential;
    public GameObject launch_effect_prefab;
    private ParticleSystem launch_effect;
    
    private float nextMissileTime;
    
    public enum DifficultyType
    {
        Linear,
        Exponential,
        Logarithmic,
        StepFunction,
        SineWave
    }
    
    void Start()
    {
        ufoTransform = GameObject.FindGameObjectWithTag("UFO").transform;
        missileLaunchers = FindObjectsByType<HomingMissile.shoot_missile_example>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        nextMissileTime = Time.time + baseFiringInterval;
        
        // Get initial time from GameManager if available
        if (GameManager.Instance != null)
        {
            initialTimeAmount = GameManager.Instance.timeRemaining;
        }
        launch_effect_prefab = Instantiate(launch_effect_prefab, transform.position, transform.rotation);
        launch_effect = launch_effect_prefab.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // Automatically fire missiles based on difficulty scaling
        if (!pauseFiring && Time.time >= nextMissileTime)
        {
            FireMissile();
            nextMissileTime = Time.time + GetCurrentFiringInterval();
        }
    }
    public void PauseFiring()
    {
        pauseFiring = true;
    }
    public void ResumeFiring()
    {
        pauseFiring = false;
    }
    
    private float GetCurrentFiringInterval()
    {
        if (GameManager.Instance == null)
        {
            return baseFiringInterval; // Fallback if GameManager not available
        }

        // Calculate time progression (0 = start, 1 = end of game)
        float timeProgress = 1f - (GameManager.Instance.timeRemaining / initialTimeAmount);
        timeProgress = Mathf.Clamp01(timeProgress); // Ensure it stays between 0 and 1

        float interval = baseFiringInterval;

        switch (difficultyType)
        {
            case DifficultyType.Linear:
                // Linear increase in difficulty as time runs out
                interval = baseFiringInterval * (1f - timeProgress * 0.8f);
                break;

            case DifficultyType.Exponential:
                // Exponential difficulty increase - gets intense near the end
                interval = baseFiringInterval * Mathf.Pow(1f - timeProgress, 3f);
                break;

            case DifficultyType.Logarithmic:
                // Quick ramp-up early, then more gradual
                interval = baseFiringInterval * (1f - Mathf.Log(timeProgress * 9f + 1f) / Mathf.Log(10f) * 0.8f);
                break;

            case DifficultyType.StepFunction:
                // Discrete difficulty levels based on time remaining percentage
                int difficultyLevel = Mathf.FloorToInt(timeProgress * 5f); // 5 difficulty levels
                interval = baseFiringInterval * (1f - difficultyLevel * 0.15f);
                break;

            case DifficultyType.SineWave:
                // Oscillating difficulty with overall increase
                float baseDecrease = timeProgress * 0.7f;
                float waveModifier = Mathf.Sin(timeProgress * Mathf.PI * 4f) * 0.1f;
                interval = baseFiringInterval * (1f - baseDecrease + waveModifier);
                break;
        }

        return Mathf.Max(interval, minimumFiringInterval);
    }
    
    // Get current difficulty metrics for UI display
    public float GetDifficultyMultiplier()
    {
        return baseFiringInterval / GetCurrentFiringInterval();
    }
    
    public float GetMissilesPerMinute()
    {
        return 60f / GetCurrentFiringInterval();
    }
    
    public float GetTimeProgress()
    {
        if (GameManager.Instance == null) return 0f;
        return 1f - (GameManager.Instance.timeRemaining / initialTimeAmount);
    }
    
    public void FireMissile()
    {
        if (ufoTransform == null)
        {
            Debug.LogWarning("UFO Transform is not assigned!");
            return;
        }
        
        if (missileLaunchers == null || missileLaunchers.Length == 0)
        {
            Debug.LogWarning("No missile launchers found!");
            return;
        }
        
        HomingMissile.shoot_missile_example closestLauncher = null;
        float closestDistance = float.MaxValue;
        
        foreach (var launcher in missileLaunchers)
        {
            if (launcher != null)
            {
                float distance = Vector3.Distance(launcher.transform.position, ufoTransform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLauncher = launcher;
                }
            }
        }
        
        if (closestLauncher != null)
        {
            launch_effect_prefab.transform.position = closestLauncher.transform.position;
            launch_effect.Play();
            closestLauncher.shoot_missile();
            Debug.Log($"Fired missile from launcher at position: {closestLauncher.transform.position} | Time Progress: {GetTimeProgress():P1}");
        }
        else
        {
            Debug.LogWarning("No valid missile launcher found!");
        }
    }
}