using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    private Vector3 direction;
    public float lifetime = 5f;
    public ParticleSystem BulletDisappearEffect;

    private void Start()
    {
        if (target == null)
        {

            GameObject targetObject = GameObject.FindGameObjectWithTag("UFO");
            if (targetObject != null)
            {
                target = targetObject.transform;
            }
        }

        if (target != null)
        {
            direction = (target.position - transform.position).normalized;

            transform.Rotate(90, 0, 0);
            transform.LookAt(target.position);
        }

        if (BulletDisappearEffect != null)
        {
            Instantiate(BulletDisappearEffect, transform.position, Quaternion.identity);
        }

        DestroyBullet();
    }

    void Update()
    {
        if (target != null)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject, lifetime);
    }
}
