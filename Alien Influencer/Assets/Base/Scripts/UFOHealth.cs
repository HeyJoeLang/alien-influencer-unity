using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOHealth : MonoBehaviour
{
    float health = 1;
    public ProgressBarPro healthBar;
    void Start()
    {
        healthBar.SetValue(health);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            collision.gameObject.GetComponent<Bullet>().Explode();
            health -= .1f;
            if (health <= 0)
            {
                GameManager.Instance.GameOver();
            }
            healthBar.SetValue(health);
        }
    }
    void Update()
    {
        
    }
}
