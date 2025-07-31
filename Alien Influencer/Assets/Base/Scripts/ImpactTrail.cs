using System;
using UnityEngine;

public class ImpactTrail : MonoBehaviour
{
    private GameObject impactPointGo;
    public Transform impactPoint;

    private void Start()
    {
        impactPointGo = impactPoint.gameObject;
    }

    void Update()
    {
        if(impactPointGo.activeInHierarchy)
            transform.position = impactPoint.position;
    }
}
