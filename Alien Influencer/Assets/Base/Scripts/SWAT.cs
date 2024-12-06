using Synty.Interface.SciFiSoldierHUD.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helicopter;

public class SWAT : MonoBehaviour
{

    public GameObject bulletPrefab;
    float attackCooldown = 1.5f;
    public Transform ufoTrans;
    public Transform bulletSpawnTrans;
    Animator animator;
    Vector3 innerCircleDest, outterCircleDest, leavingDest;
    public Vector2 outterCircleDelta;
    public GameObject shotFiredParticles;
    public int numShotsLeft = 3;
    public enum State
    {
        startWalkingToOuterTarget,
        walkingToOuterTarget,
        startWalkingToInnerTarget,
        walkingToInnerTarget,
        startShooting,
        shooting,
        startTurningToLeave,
        turningToLeave,
        startLeaving,
        leaving
    }
    public State myState;
    public float speed = 4f;
    void Start()
    {
        ufoTrans = GameObject.Find("UFO").transform;
        animator = GetComponent<Animator>();
        outterCircleDelta = PositionDeltaManager.SwatTargetDelta();

        Debug.LogFormat("My Delta: {0}", outterCircleDelta);
    }

    // Update is called once per frame
    void Update()
    {
        innerCircleDest = ufoTrans.position + new Vector3(0, -8, 0);
        outterCircleDest = ufoTrans.position + new Vector3(outterCircleDelta[0], -8, outterCircleDelta[1]);
        switch (myState)
        {
            case State.startWalkingToOuterTarget:
                StartWalkingToOuterTarget();
                break;
            case State.walkingToOuterTarget:
                WalkingToOuterTarget();
                break;
            case State.startWalkingToInnerTarget:
                StartWalkingToInnerTarget();
                break;
            case State.walkingToInnerTarget:
                WalkingToInnerTarget();
                break;
            case State.startShooting:
                StartShooting();
                break;
            case State.shooting:
                UpdateAttack();
                break;
            case State.startTurningToLeave:
                StartTurningToLeave();
                break;
            case State.turningToLeave:
                TurningToLeave();
                break;

            case State.startLeaving:
                StartLeaving();
                break;
            case State.leaving:
                Leaving();
                break;
            default:
                break;
        }
    }
    void StartWalkingToOuterTarget()
    {
        animator.SetBool("StartShooting", false);
        animator.SetBool("StartRunning", true);
        myState = State.walkingToOuterTarget;
    }
    void WalkingToOuterTarget()
    {
        transform.LookAt(outterCircleDest);
        if (Vector3.Distance(transform.position, outterCircleDest) < 2f)
        {
            myState = State.startWalkingToInnerTarget;
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, outterCircleDest, speed * Time.deltaTime);
    }
    void StartWalkingToInnerTarget()
    {
        animator.SetBool("StartShooting", false);
        animator.SetBool("StartRunning", true);
        myState = State.walkingToInnerTarget;
    }
    void WalkingToInnerTarget()
    {
        transform.LookAt(innerCircleDest);
        if (Vector3.Distance(transform.position, innerCircleDest) < 5f)
        {
            myState = State.startShooting;
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, innerCircleDest, speed * Time.deltaTime);
    }
    void StartShooting()
    {
        myState = State.shooting;
    }
    void UpdateAttack()
    {
        transform.LookAt(innerCircleDest);
        if (Vector3.Distance(transform.position, innerCircleDest) > 15f)
        {
            attackCooldown = 1.5f;
            myState = State.startWalkingToOuterTarget;
            return;
        }
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0)
        {
            attackCooldown = 1.5f;
            Attack();
        }
    }
    void Attack()
    {
        animator.SetBool("StartShooting", true);
        animator.SetBool("StartRunning", false);
        bulletSpawnTrans.LookAt(ufoTrans);
        Instantiate(shotFiredParticles, bulletSpawnTrans.position, bulletSpawnTrans.rotation);
        Instantiate(bulletPrefab, bulletSpawnTrans.position, bulletSpawnTrans.rotation);
        numShotsLeft--;
        if(numShotsLeft <= 0)
        {
            myState = State.startTurningToLeave;
        }
    }
    float startTurnTime = 0;
    void StartTurningToLeave()
    {
        startTurnTime = Time.time;
        myState = State.turningToLeave;
    }
    void TurningToLeave()
    {
        float rotationSpeed = 180f; // Degrees per second
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        if (Time.time - startTurnTime > 1)
        {
            myState = State.startLeaving;
        }
    }
    void StartLeaving()
    {
        animator.SetBool("StartShooting", false);
        animator.SetBool("StartRunning", true);
        myState = State.leaving;
        Destroy(gameObject, 5f);
    }
    void Leaving()
    {
        leavingDest = transform.position + (innerCircleDest + transform.position).normalized * 2;
        transform.position = Vector3.MoveTowards(transform.position, leavingDest, speed * Time.deltaTime);
    }
}
