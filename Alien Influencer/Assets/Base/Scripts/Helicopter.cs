using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    public Transform targetTrans;
    Transform ufoTrans;
    public float speed = 10.0f;
    public float positionDeltaX = -1;
    public float zDistanceUFO = 5f;
    public GameObject bulletPrefab;
    public float attackCooldown = 1.5f;
    Vector3 dest;
    public enum state { moving, attacking };
    
    public state myState = state.moving;

    void OnEnable()
    {
        ufoTrans = GameObject.Find("UFO").transform;
        if (positionDeltaX == -1)
        {
            positionDeltaX = MinionPlacement.HelicopterPlacementDeltaX();
        }
    }
    void Update()
    {
        dest = targetTrans.position;
        switch (myState)
        {
            case state.moving:
                UpdatePosition();
                break;
            case state.attacking:
                UpdateAttack();
                break;
        }
        UpdatePosition();
    }
    void UpdatePosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, dest , speed * Time.deltaTime);
        transform.LookAt(ufoTrans.position);
        if(Vector3.Distance(transform.position, dest) < 2f)
        {
            myState = state.attacking;
        }
    }
    void UpdateAttack()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0)
        {
            attackCooldown = 1.5f;
            Attack();
        }
        if (Vector3.Distance(transform.position, dest) > 5f)
        {
            attackCooldown = 1.5f;
            myState = state.moving;
        }
    }
    void Attack()
    {
        Instantiate(bulletPrefab, transform.position, transform.rotation);
    }
}
