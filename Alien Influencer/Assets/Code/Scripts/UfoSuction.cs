using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class UfoSuction : MonoBehaviour
{
    public float suctionRadius = 10f;
    public float suctionPower = 50f;
    public int scorePoint = 10;
    public LayerMask personLayer;
    public GameObject suctionEffectPrefab;
    public GameObject suctionEffectTrailPrefab;
    public ParticleSystem suctionConeEffect;
    public GameObject pikminSpherePrefab;
    public GameObject beamLight;
    UfoMovement ufoMovement;
    public CinemachineCamera virtualCamera;
    List<Civilian> civList;
    public AudioClip influencedSound;
    //AudioSource audioSource;
    public Vector3 deltaPositon = new Vector3(0, 10, -20);

    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
        ufoMovement = GetComponent<UfoMovement>();
    }

    private void Update()
    {
        if (Input.GetButton("Fire1") || Input.GetKey(KeyCode.Return))
        {
            SuckUpPeople();
            beamLight.SetActive(true);
            ufoMovement.moveSpeed = 10f;
        }
        else
        {
            if(civList != null)
            {
                foreach (var civ in civList)
                {
                    civ.StopFalling();
                }
                civList.Clear();
            }
            ufoMovement.moveSpeed = 25f;
            beamLight.SetActive(false);
            //transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, deltaPositon, Time.deltaTime * 2);
        }
    }

    private void SuckUpPeople()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, suctionRadius, personLayer);
        foreach (var hitCollider in hitColliders)
        {
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                UnityEngine.AI.NavMeshAgent agent = rb.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if(agent != null)
                {
                    agent.velocity = Vector3.zero;
                    agent.enabled = false;
                }
                Civilian civilian = hitCollider.GetComponent<Civilian>();
                if (civilian != null)
                {
                    if(civList == null)
                    {
                        civList = new List<Civilian>();
                    }
                    if (!civList.Contains(civilian))
                    {
                        civList.Add(civilian);
                    }
                    civilian.StartFalling();
                }
                Vector3 directionToUfo = (transform.position - hitCollider.transform.position).normalized;
                float distanceToUfo = Vector3.Distance(transform.position, hitCollider.transform.position);

                float speed = suctionPower * (1 - Mathf.Clamp01(distanceToUfo / suctionRadius));
                rb.linearVelocity = directionToUfo * speed;

                if (distanceToUfo <= 3f)
                {
                    Minion minion = hitCollider.GetComponent<Minion>();
                    if (minion != null)
                    {
                        //audioSource.PlayOneShot(influencedSound);
                        minion.enabled = true;
                        //InstantiateSuctionParticleEffect(hitCollider.transform.position);
                        minion.InfluenceMinion();
                        UFOLaser laser = GetComponent<UFOLaser>();
                        if(laser != null)
                        {
                            if (laser.enabled == false)
                            {
                                laser.enabled = true;
                            }
                        }
                    }
                    //Destroy(hitCollider.gameObject);
                }
            }
        }
    }

    private void InstantiateSuctionParticleEffect(Vector3 position)
    {
        Instantiate(suctionEffectPrefab, position, Quaternion.identity);
    }

    private void InstantiateSuctionTrail(Vector3 position)
    {
        GameObject trail = Instantiate(suctionEffectTrailPrefab, position, Quaternion.identity);
    }
}
