using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HomingMissile
{
public class homing_missile : MonoBehaviour
{
    public int speed = 60;
    public int downspeed = 30;
    public int damage = 35;
    public bool fully_active = false;
    public int timebeforeactivition = 20;
    public int timebeforebursting = 40;
    public int timebeforedestruction = 450;
    public int timealive;
    public GameObject target;
    public GameObject shooter;
    public Rigidbody projectilerb;
    public bool isactive = false;
    public Vector3 sleepposition;
    public GameObject targetpointer;
    public float turnSpeed = 0.035f;
    public AudioSource launch_sound;
    public AudioSource thrust_sound;
    public GameObject smoke_obj;
    public ParticleSystem smoke;
    public GameObject smoke_position;
    public GameObject destroy_effect;
    public event Action <GameObject> MissileDestroyed;
    private void Start()
    {
        projectilerb = this.GetComponent<Rigidbody>();
    }
    public void call_destroy_effects()
    {
        Instantiate(destroy_effect, transform.position, transform.rotation);
    }
    public void setmissile()
    {
        timealive = 0;
        transform.rotation = shooter.transform.rotation;
        transform.Rotate(0, 90, 0);
        transform.position = shooter.transform.position;
    }
    public void DestroyMe()
    {
        MissileDestroyed?.Invoke(this.gameObject);
        isactive = false;
        fully_active = false;
        timealive = 0;
        smoke.transform.SetParent(null);
        smoke.Pause();
        smoke.transform.position =sleepposition;
        smoke.Play();
        projectilerb.linearVelocity = Vector3.zero;
        thrust_sound.Pause();
        call_destroy_effects();
        transform.position = sleepposition;
        Destroy(smoke.gameObject,3);
        Destroy(this.gameObject);
    }
    public void usemissile()
    {
        launch_sound.Play();
        isactive = true;
        setmissile();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (isactive)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (other.gameObject == shooter)
                {
                    if (fully_active)
                    {
                    //damege the shooter;
                    DestroyMe();
                    }
                }
                else
                {
                    //damage the enemy;
                    DestroyMe();
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (isactive)
        {
            if (!target.activeInHierarchy)
            {
                DestroyMe();
            }
            if (timealive == timebeforeactivition)
            {
                fully_active = true;
                thrust_sound.Play();
            }
            timealive++;
            if (timealive < timebeforebursting)
            {
                projectilerb.linearVelocity = transform.up * -1 * downspeed;
            }
            if (timealive == timebeforebursting)
            {
                smoke=(Instantiate(smoke_obj,smoke_position.transform.position,smoke_position.transform.rotation)).GetComponent<ParticleSystem>();
                smoke.Play();
                smoke.transform.SetParent(this.transform);
            }
            if (timealive == timebeforedestruction)
            {
                DestroyMe();
            }
            if (timealive >= timebeforebursting && timealive < timebeforedestruction)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetpointer.transform.rotation, turnSpeed);
                projectilerb.linearVelocity = transform.forward * speed;
            }
        }
    }
}
}