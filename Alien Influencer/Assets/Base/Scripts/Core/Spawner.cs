using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject personPrefab;
    public Terrain terrain;
    private readonly int numCivilianMeshes = 19;
    private int civilianMeshIndex = 0;
    public int maxPeople = 500;
    public float spawnRadius = 250f;

    private void Start()
    {
        InitializeSpawner();
    }

    private void InitializeSpawner()
    {
        for (int i = 0; i < maxPeople; i++)
        {
            SpawnPerson();
        }

    }

    void SpawnPerson()
    {

        Vector3 randomPosition = GetRandomPointOnNavMesh(spawnRadius);
        GameObject person = Instantiate(personPrefab, randomPosition, Quaternion.identity);

        person.GetComponent<Civilian>().Initialize(randomPosition, civilianMeshIndex);
        civilianMeshIndex = civilianMeshIndex >= numCivilianMeshes ? 0 : civilianMeshIndex + 1;
    }
    Vector3 GetRandomPointOnNavMesh(float radius)
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * radius;
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, radius, UnityEngine.AI.NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero;
    }
}

