using System.Collections;
using System.Collections.Generic;
using HomingMissile;
using UnityEngine;

public class UFOHealth : MonoBehaviour
{
    float health = 1;
    public ProgressBarPro healthBar;
    void Start()
    {
        healthBar.SetValue(health);
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "homing_missile")
        {
            collision.gameObject.GetComponent<homing_missile>().DestroyMe();
            health -= .1f;
            if (health <= 0)
            {
                GameManager.Instance.GameOver();
            }
            healthBar.SetValue(health);
        }
    }
}
