using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    public Transform targetTrans;
    Transform ufoTrans;
    public float speed = 10.0f;
    public float zDistanceUFO = 5f;
    public GameObject bulletPrefab;
    public float attackCooldown = 1.5f;
    Vector3 dest, leavingDest;
    public int numShotsLeft = 3;
    public enum State { moving, attacking, startTurningToLeave, turningToLeave, startLeaving, leaving };
    
    public State myState = State.moving;

    void OnEnable()
    {
        ufoTrans = GameObject.Find("UFO").transform;
    }
    void Update()
    {
        dest = targetTrans.position;
        switch (myState)
        {
            case State.moving:
                UpdatePosition();
                break;
            case State.attacking:
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

        }
    }
    void UpdatePosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, dest , speed * Time.deltaTime);
        transform.LookAt(ufoTrans.position);
        if(Vector3.Distance(transform.position, dest) < 2f)
        {
            myState = State.attacking;
        }
    }
    void UpdateAttack()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0)
        {
            attackCooldown = 1.5f;
            Attack();
            numShotsLeft--;
            if (numShotsLeft <= 0)
            {
                myState = State.startTurningToLeave;
                return;
            }
        }
        if (Vector3.Distance(transform.position, dest) > 5f)
        {
            attackCooldown = 1.5f;
            myState = State.moving;
        }
    }
    void Attack()
    {
        Instantiate(bulletPrefab, transform.position, transform.rotation);
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
        myState = State.leaving;
        Destroy(gameObject, 5f);
    }
    void Leaving()
    {
        leavingDest = transform.position + (dest + transform.position).normalized * 2;
        transform.position = Vector3.MoveTowards(transform.position, leavingDest, speed * Time.deltaTime);
    }
}
