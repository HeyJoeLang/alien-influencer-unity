using UnityEngine;
using System.Collections.Generic;

public class PowerupSpawner : Singleton<PowerupSpawner>
{
    [System.Serializable]
    public class PowerupPool
    {
        public GameObject prefab;
        public Queue<GameObject> pool = new Queue<GameObject>();
        public int poolSize = 5;
    }

    [Header("Powerup Prefabs")]
    [SerializeField] private PowerupPool megaLaserPool;
    [SerializeField] private PowerupPool missilesPool;
    
    [Header("Spawn Settings")]
    [SerializeField] private float spawnChance = 0.25f; // 1 in 10 chance

    private void Start()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        // Initialize MegaLaser pool
        for (int i = 0; i < megaLaserPool.poolSize; i++)
        {
            GameObject powerup = Instantiate(megaLaserPool.prefab);
            powerup.SetActive(false);
            megaLaserPool.pool.Enqueue(powerup);
        }

        // Initialize Missiles pool
        for (int i = 0; i < missilesPool.poolSize; i++)
        {
            GameObject powerup = Instantiate(missilesPool.prefab);
            powerup.SetActive(false);
            missilesPool.pool.Enqueue(powerup);
        }
    }

    public void SpawnAtLocation(Vector3 locationToSpawn)
    {
        // Check if we should spawn (1 in 10 chance)
        if (Random.value > spawnChance)
            return;

        // Randomly choose which powerup to spawn
        PowerupPool selectedPool = Random.value < 0.5f ? megaLaserPool : missilesPool;
        PowerupPool alternatePool = selectedPool == megaLaserPool ? missilesPool : megaLaserPool;

        GameObject powerup = GetFromPool(selectedPool);
        
        // If primary pool is empty, try alternate pool
        if (powerup == null)
        {
            powerup = GetFromPool(alternatePool);
            if (powerup == null)
                return; // Both pools are empty
        }

        // Set position and activate
        powerup.transform.position = locationToSpawn;
        powerup.SetActive(true);
        CelebrationVisualsManager.Instance.PlayAt(locationToSpawn);

        // Get the powerup component and trigger startup animation
        //if (powerup.TryGetComponent<PowerupBase>(out var powerupComponent))
        //{
       //     powerupComponent.OnSpawn();
        //}
    }

    private GameObject GetFromPool(PowerupPool pool)
    {
        if (pool.pool.Count > 0)
        {
            return pool.pool.Dequeue();
        }
        return null;
    }

    public void ReturnToPool(GameObject powerup)
    {
        powerup.SetActive(false);
        
        // Determine which pool to return to
        if (powerup.CompareTag("MegaLaser"))
        {
            megaLaserPool.pool.Enqueue(powerup);
        }
        else if (powerup.CompareTag("Missiles"))
        {
            missilesPool.pool.Enqueue(powerup);
        }
    }
}