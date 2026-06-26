using System.Collections;
using System.Collections.Generic;
using HomingMissile;
using UnityEngine;

public class UFOHealth : MonoBehaviour
{
    float health = 1;
    public ProgressBarPro healthBar;
    public Animator hitAnimator;
    private bool damageSoundPlaying;
    
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
            else
            {
                hitAnimator.SetTrigger("Hit");
            }
            healthBar.SetValue(health);
            
            if (!damageSoundPlaying)
            {
                damageSoundPlaying = true;
                AudioManager.Instance.Play(AudioSoundIds.SoundDesign.Weapons.UfoDamage, transform);
                StartCoroutine(ResetDamageSoundFlag());
            }
        }
    }

    private IEnumerator ResetDamageSoundFlag()
    {
        yield return new WaitForSeconds(0.25f);
        damageSoundPlaying = false;
    }
}
