using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays celebration particle setups from a fixed list. Each entry is chosen uniformly at random
/// among entries not yet used this cycle; after every valid entry has played once, the cycle resets.
/// </summary>
public class CelebrationVisualsManager : Singleton<CelebrationVisualsManager>
{
    [Tooltip("Scene objects that have a ParticleSystem on them or in their hierarchy.")]
    public List<GameObject> celebrationVisuals = new List<GameObject>();

    ParticleSystem[] _particleSystems;
    readonly List<int> _availableIndices = new List<int>();
    bool _initialized;

    static ParticleSystem FindParticleSystem(GameObject go)
    {
        if (go == null)
            return null;
        var ps = go.GetComponent<ParticleSystem>();
        if (ps != null)
            return ps;
        return null;
    }

    void BuildCache()
    {
        int n = celebrationVisuals.Count;
        _particleSystems = new ParticleSystem[n];
        
        for (int i = 0; i < n; i++)
        {
            if (celebrationVisuals[i] != null)
            {
                // Try direct component first (fastest)
                _particleSystems[i] = celebrationVisuals[i].GetComponent<ParticleSystem>();
            
                // Only search children if not found on root
                if (_particleSystems[i] == null)
                    _particleSystems[i] = celebrationVisuals[i].GetComponentInChildren<ParticleSystem>(true);
            }
        }
    }

    void RefillAvailable()
    {
        _availableIndices.Clear();
        for (int i = 0; i < _particleSystems.Length; i++)
        {
            if (_particleSystems[i] != null)
                _availableIndices.Add(i);
        }
    }

    void EnsureInitialized()
    {
        if (_initialized)
            return;
        BuildCache();
        _initialized = true;
        RefillAvailable();
        if (_availableIndices.Count == 0 && celebrationVisuals.Count > 0)
            Debug.LogWarning("[CelebrationVisualsManager] No ParticleSystem found on any celebration visual.", this);
    }

    /// <summary>
    /// Moves the next unused celebration object to <paramref name="worldPosition"/> and plays its particle system (including children).
    /// </summary>
    public void PlayAt(Vector3 worldPosition)
    {
        if (_availableIndices.Count == 0)
            RefillAvailable();
        if (_availableIndices.Count == 0)
            return;

        // More efficient random selection with swap-and-pop
        int pick = Random.Range(0, _availableIndices.Count);
        int index = _availableIndices[pick];
        
        // Move last element to picked position and reduce count
        _availableIndices[pick] = _availableIndices[_availableIndices.Count - 1];
        _availableIndices.RemoveAt(_availableIndices.Count - 1);

        GameObject go = celebrationVisuals[index];
        ParticleSystem ps = _particleSystems[index];
        if (go == null || ps == null)
            return;

        // Consider using object pooling instead of SetActive if called frequently
        go.transform.position = worldPosition;
        ps.Play();
    }

    /// <summary>
    /// Refills the pool so every entry can be chosen again before reuse (same as after a full cycle completes).
    /// </summary>
    public void ResetUsageCycle()
    {
        EnsureInitialized();
        RefillAvailable();
    }

    void Start() // or Awake()
    {
        EnsureInitialized();
    }
}