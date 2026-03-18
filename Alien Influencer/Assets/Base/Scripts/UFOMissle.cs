using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class UFOMissile : MonoBehaviour
{
    public LayerMask raycastLayer;
    public LayerMask terrainLayer;
    public float speed = 20f;
    public float explosionRadius = 10f;
    public float explosionDamage = 50f;
    public GameObject explosionEffectPrefab;
    
    
    [Header("Audio Events")]
    [SerializeField] private EventReference eventHitNothing;

    private EventInstance hitNothingEventInstance;

    //private AudioSource audioSource;
    private bool hasExploded = false;

    private void Start()
    {
        hitNothingEventInstance = RuntimeManager.CreateInstance(eventHitNothing);
        RuntimeManager.AttachInstanceToGameObject(hitNothingEventInstance, transform);
    }

    private void Update()
    {
        if (hasExploded) return;

        RaycastHit hit;
        Vector3 rayDirection = transform.forward;
        float moveDistance = speed * Time.deltaTime;

        // Check for collisions with buildings
        if (Physics.Raycast(transform.position, rayDirection, out hit, moveDistance, raycastLayer))
        {
            transform.position = hit.point;
            Explode(true);
            return;
        }
        // Check for collisions with terrain
        else if (Physics.Raycast(transform.position, rayDirection, out hit, moveDistance, terrainLayer))
        {
            transform.position = hit.point;
            Explode(false);
            return;
        }

        // If no collision, move forward
        transform.Translate(Vector3.forward * moveDistance);
    }

    private void Explode(bool hasHitBuilding)
    {
        if (hasExploded) return;
        hasExploded = true;

        // Create explosion effect
        if (explosionEffectPrefab)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Find all colliders within explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, raycastLayer);

        foreach (Collider collider in colliders)
        {
            Building building = collider.GetComponentInParent<Building>();
            if (building != null)
            {
                float damage = explosionDamage;
                building.AddDamage(damage, true);
            }
        }

        if (colliders.Length <= 0)
        {
            hitNothingEventInstance.start();
        }

        // Make missile mesh invisible but keep the object for sound
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer) renderer.enabled = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        // Destroy the missile after sound plays
        //Destroy(gameObject, explosionSound ? explosionSound.length : 0.1f);
    }
    private void OnDestroy()
    {
        if (hitNothingEventInstance.isValid())
        {
            hitNothingEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            hitNothingEventInstance.release();
        }
    }
}