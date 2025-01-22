using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Civilian : MonoBehaviour
{
    Animator animator;
    private float speed;
    public bool isFalling = false;
    public float wanderRadius = 10f;
    public float wanderTimer = 5;

    private UnityEngine.AI.NavMeshAgent agent;
    private float timer;

    public void Initialize(Vector3 spawnPosition, int meshIndex)
    {
        transform.GetChild(meshIndex).gameObject.SetActive(true);
        speed = Random.Range(5.0f, 200.0f);
        animator = GetComponent<Animator>();
        wanderRadius = Random.Range(5.0f, 20.0f);
        wanderTimer = Random.Range(5.0f, 7.0f);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    private void OnCollisionExit(Collision collision)
    {
    }
    public void StartFalling()
    {
        isFalling = true;
        animator.SetBool("StartFalling", true);
        animator.SetBool("StartAttacking", false);
        animator.SetBool("StartWalking", false);
        animator.SetBool("StartIdling", false);
    }
    public void StopFalling()
    {
        if (agent.enabled == false)
        {
            agent.enabled = true;
        }
        isFalling = false;
        animator.SetBool("StartWalking", true);
        animator.SetBool("StartFalling", false);
        animator.SetBool("StartAttacking", false);
        animator.SetBool("StartIdling", false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        StopFalling();
    }
    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        timer = wanderTimer - 1f;
    }

    // Update is called once per frame
    void Update()
    {

        if (isFalling)
        {
            return;
        }
        timer += Time.deltaTime;
        if(agent == null)
        {
            return;
        }
        if (Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance)
        {
            transform.LookAt(agent.destination);
            animator.SetBool("StartWalking", false);
            animator.SetBool("StartAttacking", false);
            animator.SetBool("StartIdling", true);
        }
        else
        {
            animator.SetBool("StartWalking", true);
            animator.SetBool("StartAttacking", false);
            animator.SetBool("StartIdling", false);
        }
        if (timer >= wanderTimer)
        {
            if (!agent.isOnNavMesh)
            { 
                return; 
            }
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        UnityEngine.AI.NavMeshHit navHit;

        UnityEngine.AI.NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}