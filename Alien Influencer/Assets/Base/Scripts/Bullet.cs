using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10.0f;
    public ParticleSystem explosion;
    void Start()
    {
        Destroy(gameObject, 5.0f); // Destroy the bullet after 2 seconds (if it doesn't hit anything before that
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    public void Explode()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
