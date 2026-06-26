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
    public float speed = 1.0f;
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
        MinionManager.OnNewBuildingSelected += RecieveNewBuildingSelectedEvent;
    }
    void OnDisable()
    {
        MinionManager.OnNewBuildingSelected -= RecieveNewBuildingSelectedEvent;
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

    void RecieveNewBuildingSelectedEvent()
    {
        if (state != MinionState.AttackingBuilding)
        {
            StartMovingToBuilding();
        }
    }
    void StartMovingToUFO()
    {
        speed = 1f;
        animator.SetBool("StartWalking", true);
        animator.SetBool("StartIdling", false);
        animator.SetBool("StartAttacking", false);
        animator.SetBool("StartFalling", false);
        destination = GameObject.Find("UFO").transform.position;
        state = MinionState.MovingToUFO;
    }
    void MovingToUFO()
    {
        Vector3 ufoGroundPosition = new Vector3(ufoTrans.position.x + positionDelta[0], ufoTrans.position.y - 8, ufoTrans.position.z + positionDelta[1]);
        DistanceToTarget = Vector3.Distance(transform.position, ufoGroundPosition);
        transform.position = Vector3.MoveTowards(transform.position, ufoGroundPosition, speed * DistanceToTarget * Time.deltaTime);
        transform.LookAt(new Vector3(ufoGroundPosition.x, transform.position.y, ufoGroundPosition.z));

        if (Vector3.Distance(transform.position, ufoGroundPosition) < 2.0f)
        {
            state = MinionState.StartIdle;
        }
    }


    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;
    Vector3 startPosition;
    public void StartMovingToBuilding()
    {
        speed = 10f;
        destination = MinionManager.Instance.SelectedBuilding.transform.position;
        transform.LookAt(destination);
        animator.SetBool("StartWalking", true);
        animator.SetBool("StartIdling", false);
        animator.SetBool("StartAttacking", false);
        animator.SetBool("StartFalling", false);


        // Keep a note of the time the movement started.
        startTime = Time.time;
        startPosition = transform.position;
        // Calculate the journey length.
        journeyLength = Vector3.Distance(startPosition, destination);

        state = MinionState.MovingToBuilding;
    }
    void MovingToBuilding()
    {
        DistanceToTarget = Vector3.Distance(transform.position, destination);
        //transform.position = Vector3.Lerp(transform.position, destination, speed * DistanceToTarget * Time.deltaTime);

        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed equals current distance divided by total distance.
        float fractionOfJourney = distCovered / journeyLength;

        // Set our position as a fraction of the distance between the markers.
        transform.position = Vector3.Lerp(startPosition, destination, fractionOfJourney);

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
        animator.SetBool("StartIdling", true);
        animator.SetBool("StartWalking", false);
        animator.SetBool("StartAttacking", false);
        animator.SetBool("StartFalling", false);
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
        if (civilian == null)
        {
            civilian = GetComponent<Civilian>();
        }
        if (ufoTrans == null)
        {
            ufoTrans = GameObject.Find("UFO").transform;
        }
        GameManager.Instance.AddScore(scoreValue);
        civilian.StopFalling();
        civilian.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
        positionDelta = PositionDeltaManager.MinonPlacementDelta();
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        transform.position = ufoTrans.position + new Vector3(positionDelta[0], -8, positionDelta[1]);
        influencedParticles.gameObject.SetActive(true);
        if (MinionManager.Instance.CanAttackBuilding())
        {
            StartMovingToBuilding();
            state = MinionState.StartMovingToBuilding;
        }
        else
        {
            state = MinionState.StartMovingToUFO;
        }
    }
    IEnumerator StallDestroyNavMesh()
    {
        Civilian civ = GetComponent<Civilian>();
        if (civ != null)
        {
            Destroy(civ);
        }
        yield return new WaitForSeconds(1f);
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            Destroy(agent, 1f);
        }
    }
    IEnumerator StallAttack()
    {
        animator.SetBool("StartAttacking", true);
        animator.SetBool("StartWalking", false);
        animator.SetBool("StartIdling", false);
        animator.SetBool("StartFalling", false);
        attackParticles.Play();
        MinionManager.Instance.AttackBuilding(damage);
        yield return new WaitForSeconds(1);
        attackState = AttackState.StartAttack;
    }

    #endregion

}
