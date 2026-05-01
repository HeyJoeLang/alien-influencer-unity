using System.Collections;
using System.Collections.Generic;
using HomingMissile;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class UFOHealth : MonoBehaviour
{
    float health = 1;
    public ProgressBarPro healthBar;
    public Animator hitAnimator;
    
    [SerializeField] private EventReference eventDamage;
    private EventInstance damageEventInstance;
    
    void Start()
    {
        healthBar.SetValue(health);
        
        damageEventInstance = RuntimeManager.CreateInstance(eventDamage);
        RuntimeManager.AttachInstanceToGameObject(damageEventInstance, transform);
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
            
            damageEventInstance.getPlaybackState(out var playbackState);
            if (playbackState != PLAYBACK_STATE.PLAYING)
            {
                damageEventInstance.start();
            }
        }
    }
}
