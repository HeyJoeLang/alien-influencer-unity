using Sharklib.UI;
using System.Collections;
using System.Collections.Generic;
using PilotoStudio;
using UnityEngine;

public class UFOLaser : MonoBehaviour
{
    public LayerMask raycastLayer;
    public LayerMask terrainLayer;
    public Vector3 deltaPosition = Vector3.zero;
    public Vector3 deltaDirection = Vector3.forward;
    public float rayLength = 10f;
    public Color hitColor = Color.green;
    public Color missColor = Color.yellow;
    private float laserDamage = 10f;
    public GameObject laserBeam;
    public Transform laserBeamImpact;

    public GameObject crosshair;
    Material crosshairMat;
    
    public GameObject missilePrefab;
    public float missileSpawnOffset = 1f;
    // Add these new fields at the top with other fields
    private float missileNextFireTime = 0f;
    private const float MISSILE_COOLDOWN = 0.5f;



    void Start()
    {
        crosshairMat = crosshair.GetComponent<MeshRenderer>().material;
        laserBeam.transform.position = transform.position + deltaPosition;
    }

    void FixedUpdate()
    {
        Vector3 rayDirection = transform.TransformDirection(deltaDirection);
        
        RaycastHit hit;
        Vector3 startPos = transform.position + deltaPosition;
        Vector3 endPos = startPos + rayDirection * rayLength;

        if (Physics.Raycast(startPos, rayDirection, out hit, rayLength, raycastLayer.value))
        {
            crosshair.SetActive(true);
            crosshairMat.color = hitColor;
            crosshair.transform.rotation = Quaternion.LookRotation(hit.normal);
            //Debug.Log(hit.collider.gameObject.name);
            endPos = hit.point;
           // BuildingHighlighter.Instance.HighlightObject(hit.collider.gameObject);
        }
        else if (Physics.Raycast(startPos, rayDirection, out hit, rayLength, terrainLayer.value))
        {
            
            crosshair.SetActive(true);
            endPos = hit.point;
            crosshairMat.color = missColor;
            crosshair.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            crosshairMat.color = missColor;
            crosshair.SetActive(false);
        }
        if(Input.GetButton("Fire2") || Input.GetKey(KeyCode.P))
        {
            laserBeam.SetActive(true);
            if (Physics.Raycast(startPos, rayDirection, out hit, rayLength, raycastLayer.value))
            {
                var building = hit.collider.transform.parent.GetComponent<Building>();
                if (building)
                {
                    building.AddDamage(laserDamage * Time.fixedDeltaTime); // Apply damage every physics update
                }
            }
           // BuildingHighlighter.Instance.SelectObject();
        }
        else
        {
            laserBeam.SetActive(false);
        }
        crosshair.transform.position = endPos - (endPos - startPos).normalized * 0.5f;
        laserBeamImpact.position = endPos - (endPos - startPos).normalized;
        
        if (Input.GetKeyDown(KeyCode.M) && Time.time >= missileNextFireTime)
        {
            LaunchMissile(startPos);
            missileNextFireTime = Time.time + MISSILE_COOLDOWN;
        }
    }
    
    private void LaunchMissile(Vector3 startPos)
    { 
        // Calculate the world-space direction from the UFO's local deltaDirection
        Vector3 worldDirection = transform.TransformDirection(deltaDirection);

        // Create a rotation that looks in that direction
        Quaternion missileRotation = Quaternion.LookRotation(worldDirection);

        // Instantiate the missile with the correct rotation
        GameObject missile = Instantiate(missilePrefab, startPos + worldDirection * missileSpawnOffset, missileRotation);

    }

}
