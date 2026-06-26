using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PikminSphere : MonoBehaviour
{
    Transform ufoTrans;
    NavMeshAgent agent;
    Vector3 groundPosition;
    enum PikmanState
    {
        Idle,
        StartFollowing,
        Following
    }
    PikmanState state = PikmanState.Idle;
    void Start()
    {
        ufoTrans = GameObject.FindWithTag("UFO").transform;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Terrain>() != null)
        {
            state = PikmanState.StartFollowing;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PikmanState.Idle:
                break;
            case PikmanState.StartFollowing:
                StartFollow();
                break;
            case PikmanState.Following:
                FollowUFO();
                break;
        }
    }
    void StartFollow()
    {
        if (agent == null)
        {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent == null)
            {
                agent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            }
        }
        state = PikmanState.Following;
    }
    void FollowUFO()
    {
        groundPosition = new Vector3(ufoTrans.position.x, transform.position.y, ufoTrans.position.z);
        agent.speed = Vector3.Distance(transform.position, groundPosition);
        agent.SetDestination(groundPosition);
    }
}
