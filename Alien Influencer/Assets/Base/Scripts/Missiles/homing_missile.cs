using System;
using UnityEngine;

namespace HomingMissile
{
public class homing_missile : MonoBehaviour
{
    private enum MissileState
    {
        Idle,
        Launching,
        Armed,
        Homing,
        Destroyed
    }

    public int speed = 60;
    public int downspeed = 30;
    public int damage = 35;
    public int timebeforeactivition = 20;
    public int timebeforebursting = 40;
    public int timebeforedestruction = 450;
    public int timealive;
    public GameObject target;
    public GameObject shooter;
    public Rigidbody projectilerb;
    public Vector3 sleepposition;
    public GameObject targetpointer;
    public float turnSpeed = 0.035f;
    public GameObject smoke_obj;
    public ParticleSystem smoke;
    public GameObject smoke_position;
    public GameObject destroy_effect;
    public float distanceToUFO = 10;
    public float parameterMaxDistance = 10;
    public AudioSource launchAudio;
    public AudioSource flightAudio;
//    private AudioHandle flyHandle;
    private MissileState state = MissileState.Idle;

    public event Action<GameObject> MissileDestroyed;

    private void Start()
    {
        projectilerb = GetComponent<Rigidbody>();
    }

    public void setmissile()
    {
        timealive = 0;
        state = MissileState.Launching;

        if (shooter != null)
        {
            transform.position = shooter.transform.position;
        }
        launchAudio.Play();

        Debug.Log("Launching!");
    }

    public void StopFlySound()
    {
        flightAudio.Stop();
    }
    public void DestroyMe()
    {
        StopFlySound();
        if (state == MissileState.Destroyed)
        {
            return;
        }

        state = MissileState.Destroyed;
        timealive = 0;

        MissileDestroyed?.Invoke(gameObject);

        if (smoke != null)
        {
            smoke.Stop();
            //DestroyImmediate(smoke.gameObject);
        }

        if (projectilerb != null)
        {
            projectilerb.linearVelocity = Vector3.zero;
        }

        if (destroy_effect != null)
        {
            GameObject fx = Instantiate(destroy_effect, transform.position, transform.rotation);
            fx.SetActive(true);
            Destroy(fx, 3f);
        }
        transform.position = sleepposition;
        Destroy(gameObject);
    }

    public void usemissile()
    {
        setmissile();
    }

    private void FixedUpdate()
    {
        if (state == MissileState.Idle || state == MissileState.Destroyed)
        {
            return;
        }

        if (target == null || !target.activeInHierarchy)
        {
            DestroyMe();
            return;
        }

        timealive++;

        if (timealive >= timebeforedestruction)
        {
            DestroyMe();
            return;
        }

        switch (state)
        {
            case MissileState.Launching:
                HandleLaunching();
                break;
            case MissileState.Armed:
                HandleArmed();
                break;
            case MissileState.Homing:
                HandleHoming();
                break;
        }
    }

    private void HandleLaunching()
    {
        if (projectilerb != null)
        {
            projectilerb.linearVelocity = -transform.up * downspeed;
        }

        if (timealive > timebeforeactivition)
        {
            EnterArmedState();
        }
    }

    private void HandleArmed()
    {
        if (projectilerb != null)
        {
            projectilerb.linearVelocity = -transform.up * downspeed;
        }

        if (timealive >= timebeforebursting)
        {
            SpawnSmoke();
            EnterHomingState();
        }
    }

    private void HandleHoming()
    {
        if (targetpointer != null)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetpointer.transform.rotation,
                turnSpeed);
        }
        distanceToUFO = Vector3.Distance(transform.position, target.transform.position);
    //    AudioManager.Instance.SetLoopDistanceAttenuation(flyHandle, distanceToUFO, parameterMaxDistance);

        if (projectilerb != null)
        {
            projectilerb.linearVelocity = transform.forward * speed;
        }
    }

    private void EnterArmedState()
    {
        if (state != MissileState.Launching)
        {
            return;
        }

        state = MissileState.Armed;
        Debug.Log("Armed!");
    }

    private void EnterHomingState()
    {
        if (state == MissileState.Homing)
        {
            return;
        }

        state = MissileState.Homing;

        Debug.Log("Flying!");
        flightAudio.Play();
    }

    private void SpawnSmoke()
    {
        if (smoke_obj == null || smoke_position == null || smoke != null)
        {
            return;
        }

        smoke = Instantiate(
            smoke_obj,
            smoke_position.transform.position,
            smoke_position.transform.rotation)
            .GetComponent<ParticleSystem>();

        if (smoke != null)
        {
            smoke.Play();
            smoke.transform.SetParent(transform);
        }
    }
}
}
