using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FirePool : MonoBehaviour
{
    [Header("Fire Pool Settings")]
    public GameObject firePrefab;
    public int poolSize = 50;
    
    private Queue<GameObject> firePool;
    private List<GameObject> activeFires;
    private int currentIndex = 0;
    private GameObject firePoolContainer;

    void Start()
    {
        InitializePool();
    }
    
    private void InitializePool()
    {
        firePool = new Queue<GameObject>();
        activeFires = new List<GameObject>();
        
        // Create a container GameObject for all fire objects
        firePoolContainer = new GameObject("FirePool");
        firePoolContainer.transform.SetParent(transform);
        
        // Create all fire objects and add them to the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject fireObject = Instantiate(firePrefab, firePoolContainer.transform);
            fireObject.SetActive(false);
            
            // Add callback component to handle when fire is disabled
            FireObjectCallback callback = fireObject.AddComponent<FireObjectCallback>();
            callback.Initialize(this);
            
            firePool.Enqueue(fireObject);
        }
    }
    
    public void CreateFire(Vector3 location, Transform parent = null)
    {
        GameObject fireToActivate;
        
        // If pool has available objects, use one from the pool
        if (firePool.Count > 0)
        {
            fireToActivate = firePool.Dequeue();
        }
        else
        {
            // Pool is empty, cycle through active fires (oldest to newest)
            fireToActivate = activeFires[currentIndex];
            activeFires.RemoveAt(currentIndex);
            
            // Reset index if we've reached the end
            if (currentIndex >= activeFires.Count && activeFires.Count > 0)
            {
                currentIndex = 0;
            }
        }
        
        // Position and activate the fire object
        
        //fireToActivate.GetComponent<Fire>().StartFire();
        fireToActivate.transform.position = location;
        fireToActivate.transform.SetParent(parent);
        fireToActivate.SetActive(true);

        
        // Add to active fires list
        activeFires.Add(fireToActivate);
    }
    
    // Called by FireObjectCallback when a fire object is disabled
    public void OnFireObjectDisabled(GameObject fireObject)
    {
        // If activeSelf is still true, this OnDisable was triggered by a parent being
        // deactivated rather than an explicit SetActive(false) on the fire itself.
        // Unity forbids SetParent while a parent is mid-activation/deactivation, so defer.
        if (fireObject.activeSelf)
        {
            StartCoroutine(ReturnToPoolNextFrame(fireObject));
            return;
        }

        fireObject.transform.SetParent(firePoolContainer.transform);
        activeFires.Remove(fireObject);
        firePool.Enqueue(fireObject);
    }

    private IEnumerator ReturnToPoolNextFrame(GameObject fireObject)
    {
        yield return null;

        if (fireObject == null) yield break;

        // Suppress the recursive OnDisable callback triggered by SetActive(false) below
        FireObjectCallback callback = fireObject.GetComponent<FireObjectCallback>();
        if (callback != null) callback.returningToPool = true;

        fireObject.transform.SetParent(firePoolContainer.transform);
        fireObject.SetActive(false);
        activeFires.Remove(fireObject);
        firePool.Enqueue(fireObject);
    }
    
    // Optional: Method to return a fire object to the pool manually if needed
    private void ReturnFireToPool(GameObject fireObject)
    {
        //fireObject.GetComponent<Fire>().StopFire();
        fireObject.SetActive(false);
        // OnFireObjectDisabled will be called automatically when SetActive(false) is called
    }
    
    // Optional: Clear all active fires and return them to pool
    public void ClearAllFires()
    {
        for (int i = activeFires.Count - 1; i >= 0; i--)
        {
            ReturnFireToPool(activeFires[i]);
        }
        currentIndex = 0;
    }
}

// Helper component to detect when fire objects are disabled
public class FireObjectCallback : MonoBehaviour
{
    private FirePool parentPool;

    // Set to true before an intentional SetActive(false) during deferred pool return
    // so OnDisable doesn't re-enter pool logic for that call.
    [HideInInspector] public bool returningToPool = false;

    public void Initialize(FirePool pool)
    {
        parentPool = pool;
    }

    void OnDisable()
    {
        if (returningToPool)
        {
            returningToPool = false;
            return;
        }

        if (parentPool != null)
        {
            parentPool.OnFireObjectDisabled(gameObject);
        }
    }
}