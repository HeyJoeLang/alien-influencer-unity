using System;
using UnityEngine;

public class ImpactTrail : MonoBehaviour
{
    private GameObject impactPointGo;
    public Transform impactPoint;
    public TrailRenderer[] trailRenderers;

    private void Start()
    {
        impactPointGo = impactPoint.gameObject;
        trailRenderers = GetComponentsInChildren<TrailRenderer>();
    }

    void Update()
    {
        if(impactPointGo.activeInHierarchy)
            transform.position = impactPoint.position;
    }

    private void OnDisable()
    {
        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.Clear();
        }
    }
}
