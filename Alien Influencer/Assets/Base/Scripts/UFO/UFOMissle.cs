using UnityEngine;

public class UFOMissile : MonoBehaviour
{
    public LayerMask raycastLayer;
    public LayerMask terrainLayer;
    public float speed = 20f;
    public float explosionRadius = 10f;
    public float explosionDamage = 50f;
    public GameObject explosionEffectPrefab;
    public AudioSourceList deflectedMissileExplosionAudio;

    private bool hasExploded = false;

    private void Update()
    {
        if (hasExploded) return;

        RaycastHit hit;
        Vector3 rayDirection = transform.forward;
        float moveDistance = speed * Time.deltaTime;

        if (Physics.Raycast(transform.position, rayDirection, out hit, moveDistance, raycastLayer))
        {
            transform.position = hit.point;
            Explode(true);
            return;
        }
        else if (Physics.Raycast(transform.position, rayDirection, out hit, moveDistance, terrainLayer))
        {
            transform.position = hit.point;
            Explode(false);
            return;
        }

        transform.Translate(Vector3.forward * moveDistance);
    }

    private void Explode(bool hasHitBuilding)
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explosionEffectPrefab)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

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
            if(deflectedMissileExplosionAudio != null)
                deflectedMissileExplosionAudio.Play();
        }

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer) renderer.enabled = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
