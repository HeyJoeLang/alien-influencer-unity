using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{

    #region Variables
    Vector2 positionDelta;
    public enum MinionState
    {
        StartMovingToUFO,
        MovingToUFO,
        StartMovingToBuilding,
        MovingToBuilding,
        StartAttackingBuilding,
        AttackingBuilding,
        StartIdle,
        Idle
    }
    public MinionState state = MinionState.StartMovingToUFO;
    public enum AttackState
    {
        StartAttack,
        Attacking
    }
    public AttackState attackState = AttackState.StartAttack;
    public float DistanceToTarget;
    public Animator animator;
    public Transform houseTrans;
    public ParticleSystem attackParticles;
    public float walkingSpeed = 1.0f;
    public ParticleSystem influencedParticles;
    Civilian civilian;
    Vector3 destination;
    Transform ufoTrans;
    int damage = 1;
    Vector2 v2Pos, v2Dest;
    public int scoreValue = 1;

    #endregion
    #region Unity Methods

    void Start()
    {
        civilian = GetComponent<Civilian>();
    }
    void OnEnable()
    {
        MinionManager.OnNewBuildingSelected += MoveToBuilding;
    }
    void OnDisable()
    {
        MinionManager.OnNewBuildingSelected -= MoveToBuilding;
    }
    private void Update()
    {
        switch (state)
        {
            case MinionState.StartMovingToUFO:
                StartMovingToUFO();
                break;
            case MinionState.MovingToUFO:
                MovingToUFO();
                break;
            case MinionState.MovingToBuilding:
                MovingToBuilding();
                break;
            case MinionState.StartAttackingBuilding:
                StartAttackingBuilding();
                break;
            case MinionState.AttackingBuilding:
                AttackingBuilding();
                break;
            case MinionState.StartIdle:
                StartIdle();
                break;
            case MinionState.Idle:
                Idle();
                break;
            default:
                break;
        }
    }
    #endregion
    #region State Functions


    void StartMovingToUFO()
    {
        animator.SetTrigger("StartWalk");
        destination = GameObject.Find("UFO").transform.position;
        state = MinionState.MovingToUFO;
    }
    void MovingToUFO()
    {
        Vector3 ufoGroundPosition = new Vector3(ufoTrans.position.x + positionDelta[0], ufoTrans.position.y - 8, ufoTrans.position.z + positionDelta[1]);
        DistanceToTarget = Vector3.Distance(transform.position, ufoGroundPosition);
        transform.position = Vector3.MoveTowards(transform.position, ufoGroundPosition, walkingSpeed * DistanceToTarget * Time.deltaTime);
        transform.LookAt(new Vector3(ufoGroundPosition.x, transform.position.y, ufoGroundPosition.z));

        if (Vector3.Distance(transform.position, ufoGroundPosition) < 2.0f)
        {
            state = MinionState.StartIdle;
        }
    }

    public void MoveToBuilding()
    {
        destination = MinionManager.Instance.SelectedBuilding.transform.position;
        transform.LookAt(destination);
        animator.SetTrigger("StartWalk");
        state = MinionState.MovingToBuilding;
    }
    void MovingToBuilding()
    {
        DistanceToTarget = Vector3.Distance(transform.position, destination);
        transform.position = Vector3.MoveTowards(transform.position, destination, walkingSpeed * DistanceToTarget * Time.deltaTime);


        //Snap minion
        v2Pos.x = transform.position.x;
        v2Pos.y = transform.position.z;
        v2Dest.x = destination.x;
        v2Dest.y = destination.z;
        if(Vector2.Distance(v2Pos, v2Dest) < 8.0f) //if (Vector3.Distance(transform.position, destination) < 8.0f)
        {
            transform.position = new Vector3(transform.position.x, destination.y, transform.position.z);
            state = MinionState.StartAttackingBuilding;
        }
    }
    void StartAttackingBuilding()
    {
        state = MinionState.AttackingBuilding;
    }
    void AttackingBuilding()
    {
        if (!MinionManager.Instance.CanAttackBuilding())
        {
            state = MinionState.StartMovingToUFO;
            return;
        }
        switch (attackState)
        {
            case AttackState.StartAttack:
                StartCoroutine(StallAttack());
                attackState = AttackState.Attacking;
                break;
            case AttackState.Attacking:
                break;
        }
    }
    void StartIdle()
    {
        animator.SetTrigger("StartIdle");
        state = MinionState.Idle;
    }
    void Idle()
    {
        Vector3 ufoGroundPosition = new Vector3(ufoTrans.position.x, transform.position.y, ufoTrans.position.z);
        if (Vector3.Distance(transform.position, ufoGroundPosition) > 3.0f)
        {
            state = MinionState.StartMovingToUFO;
        }
    }

    #endregion
    #region Utility Functions

    public void InfluenceMinion()
    {
        if(civilian == null)
        {
            civilian = GetComponent<Civilian>();
        }
        if(ufoTrans == null)
        {
            ufoTrans = GameObject.Find("UFO").transform;
        }
        GameManager.Instance.AddScore(scoreValue);
        civilian.enabled = false;
        state = MinionState.StartMovingToUFO;
        gameObject.layer = LayerMask.NameToLayer("Default");
        positionDelta = MinionPlacement.MinonPlacementDelta();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = ufoTrans.position + new Vector3(positionDelta[0], -8, positionDelta[1]);
        influencedParticles.gameObject.SetActive(true);

    }

    IEnumerator StallAttack()
    {
        animator.SetTrigger("StartAttack");
        attackParticles.Play();
        MinionManager.Instance.AttackBuilding(damage);
        yield return new WaitForSeconds(1);
        attackState = AttackState.StartAttack;
    }

    #endregion

}
