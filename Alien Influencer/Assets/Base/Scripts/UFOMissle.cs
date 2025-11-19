using UnityEngine;

public class UFOMissile : MonoBehaviour
{
    public LayerMask raycastLayer;
    public LayerMask terrainLayer;
    public float speed = 20f;
    public float explosionRadius = 10f;
    public float explosionDamage = 50f;
    public GameObject explosionEffectPrefab;
    public AudioClip flyingSound;
    public AudioClip explosionSound;
    

    private AudioSource audioSource;
    private bool hasExploded = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource && flyingSound)
        {
            audioSource.clip = flyingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
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
            Explode();
            return;
        }
        // Check for collisions with terrain
        else if (Physics.Raycast(transform.position, rayDirection, out hit, moveDistance, terrainLayer))
        {
            transform.position = hit.point;
            Explode();
            return;
        }

        // If no collision, move forward
        transform.Translate(Vector3.forward * moveDistance);
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Create explosion effect
        if (explosionEffectPrefab)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Play explosion sound
        if (audioSource && explosionSound)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.PlayOneShot(explosionSound);
        }

        // Find all colliders within explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, raycastLayer);

        foreach (Collider collider in colliders)
        {
            Building building = collider.GetComponentInParent<Building>();
            if (building != null)
            {
                float damage = explosionDamage;
                building.AddDamage(damage);
            }
        }

        // Make missile mesh invisible but keep the object for sound
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer) renderer.enabled = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        // Destroy the missile after sound plays
        Destroy(gameObject, explosionSound ? explosionSound.length : 0.1f);
    }

    private void OnDrawGizmos()
    {
        // Visualize explosion radius in editor
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}