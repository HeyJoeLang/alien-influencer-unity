using Sharklib.UI;
using System.Collections;
using System.Collections.Generic;
using PilotoStudio;
using UnityEngine;
using System; // Added for Action

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
    public GameObject LaserBeamImpactFlames;
    public GameObject megaLaserBeam;
    public Transform laserBeamImpact;
    public Transform megaImpact1, megaImpact2, megaImpact3;
    public enum LaserBeamType { Mega, Normal }
    public LaserBeamType laserBeamType = LaserBeamType.Normal;
    private GameObject laserGameObject;
    public GameObject crosshair;
    Material crosshairMat;
	public ProgressBarPro progressBar;
    
    // Mega laser timer variables
    private float megaLaserTimer = 0f;
    private const float MEGA_LASER_DURATION = 10f;
    private bool isMegaLaserActive = false;
    [Range(0f, 1f)]
    public float megaLaserTimeRemainingPercent = 0f; // 0 = no time remaining, 1 = full time remaining
    
    // Mega laser charge system
    private int megaLaserCharges = 2; // Starting charges
    
    // Public events for mega laser state changes
    public static event Action OnMegaLaserActivated;
    public static event Action OnMegaLaserDeactivated;
    
    public GameObject missilePrefab;
    public float missileSpawnOffset = 1f;
    // Missile cooldown variables
    private float missileNextFireTime = 0f;
    private const float MISSILE_RECHARGE_TIME = 0.5f;
    private bool isMissileRecharging = false;
    
    // Missile charge system
    private int missileCharges = 2; // Starting missile charges
    
    // Public events for missile state changes
    public static event Action OnMissileLaunched;
    public static event Action OnMissileRechargeComplete;
    
    // Laser firing state tracking
    private bool isLaserCurrentlyFiring = false;
    
    // Public events for laser firing state changes
    public static event Action OnLaserActivated;
    public static event Action OnLaserDeactivated;
    
    // Fire pool variables
    private FirePool firePool;
    private float fireNextSpawnTime = 0f;
    private const float FIRE_COOLDOWN = 0.1f;

    void Start()
    {
        crosshairMat = crosshair.GetComponent<MeshRenderer>().material;
        laserBeam.transform.position = transform.position + deltaPosition;
        
        // Get reference to FirePool component on the same GameObject
        firePool = GetComponent<FirePool>();
        if (firePool == null)
        {
            Debug.LogError("FirePool component not found on " + gameObject.name);
        }
        progressBar.Start();
    }

    void FixedUpdate()
    {
        // Handle missile recharge completion check
        if (isMissileRecharging && Time.time >= missileNextFireTime)
        {
            isMissileRecharging = false;
            OnMissileRechargeComplete?.Invoke();
        }
        
        // Handle mega laser timer
        if (isMegaLaserActive)
        {
            megaLaserTimer += Time.fixedDeltaTime;
            
            // Calculate time remaining percentage (1 = full time, 0 = no time left)
            float timeRemaining = MEGA_LASER_DURATION - megaLaserTimer;
            megaLaserTimeRemainingPercent = Mathf.Clamp01(timeRemaining / MEGA_LASER_DURATION);
			progressBar.SetValue(megaLaserTimeRemainingPercent,true);
            
            if (megaLaserTimer >= MEGA_LASER_DURATION)
            {
                // Switch back to normal laser and consume a charge
                laserBeamType = LaserBeamType.Normal;
                isMegaLaserActive = false;
                megaLaserTimer = 0f;
                megaLaserTimeRemainingPercent = 0f;
                progressBar.SetValue(megaLaserTimeRemainingPercent, true);
                
                // Consume one charge
                megaLaserCharges--;
                Debug.Log("Mega laser charge consumed. Remaining charges: " + megaLaserCharges);
                
                // Trigger deactivation event
                OnMegaLaserDeactivated?.Invoke();
            }
        }
        else
        {
            // When mega laser is not active, percentage should be 0
            megaLaserTimeRemainingPercent = 0f;
        }
        
        // Handle laser type switching
        if (Input.GetKeyDown(KeyCode.L)) // Switch to normal laser
        {
            // Only allow switching to normal if mega laser is not currently active
            if (!isMegaLaserActive)
            {
                laserBeamType = LaserBeamType.Normal;
            }
            else
            {
                Debug.Log("Cannot switch laser types while mega laser is active!");
            }
        }
        else if (Input.GetKeyDown(KeyCode.K)) // Attempt to switch to mega laser
        {
            if (isMegaLaserActive)
            {
                Debug.Log("Mega laser is already active! Wait for it to complete before using another charge.");
            }
            else if (megaLaserCharges > 0)
            {
                // Start mega laser and activate timer
                laserBeamType = LaserBeamType.Mega;
                isMegaLaserActive = true;
                megaLaserTimer = 0f;
                megaLaserTimeRemainingPercent = 1f; // Start with full time remaining
                Debug.Log("Mega laser activated! Charges remaining after this use: " + (megaLaserCharges - 1));
                
                // Trigger activation event
                OnMegaLaserActivated?.Invoke();
            }
            else
            {
                Debug.Log("No mega laser charges remaining!");
            }
        }
        
        bool didHitBuilding = false;
        //LaserBeamImpactFlames.SetActive(false);
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
        
        // Check if laser should be firing (Fire2 button or P key)
        bool shouldLaserFire = Input.GetButton("Fire2") || Input.GetKey(KeyCode.P);
        
        if (shouldLaserFire)
        {
            // Trigger laser activation event if not already firing
            if (!isLaserCurrentlyFiring)
            {
                isLaserCurrentlyFiring = true;
                OnLaserActivated?.Invoke();
            }
            
            // Ensure only one laser type is active at a time
            if (laserBeamType == LaserBeamType.Mega)
            {
                megaLaserBeam.SetActive(true);
                laserBeam.SetActive(false);
                laserGameObject = megaLaserBeam;
            }
            else
            {
                laserBeam.SetActive(true);
                megaLaserBeam.SetActive(false);
                laserGameObject = laserBeam;
            }
            
            if (Physics.Raycast(startPos, rayDirection, out hit, rayLength, raycastLayer.value))
            {
                var building = hit.collider.transform.parent.GetComponent<Building>();
                if (building)
                {
                    didHitBuilding = true;
                    //LaserBeamImpactFlames.SetActive(false);
                    float damage = laserBeamType == LaserBeamType.Normal ? laserDamage : laserDamage * 10f;
                    building.AddDamage(damage * Time.fixedDeltaTime); // Apply damage every physics update
                    
                    // Create fire effect at hit location with cooldown
                    if (firePool != null && Time.time >= fireNextSpawnTime)
                    {
                        firePool.CreateFire(hit.point, hit.collider.transform);
                        fireNextSpawnTime = Time.time + FIRE_COOLDOWN;
                    }
                }
            }
           // BuildingHighlighter.Instance.SelectObject();
        }
        else
        {
            // Trigger laser deactivation event if currently firing
            if (isLaserCurrentlyFiring)
            {
                isLaserCurrentlyFiring = false;
                OnLaserDeactivated?.Invoke();
            }
            
            laserBeam.SetActive(false);
            megaLaserBeam.SetActive(false);
        }
        
        crosshair.transform.position = endPos - (endPos - startPos).normalized * 0.5f;
        laserBeamImpact.position = megaImpact1.position = megaImpact2.position = megaImpact3.position = endPos - (endPos - startPos).normalized;
        LaserBeamImpactFlames.SetActive(didHitBuilding);
        
        // Handle missile launching with recharge time and charge system
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (missileCharges <= 0)
            {
                Debug.Log("No missile charges remaining!");
            }
            else if (Time.time >= missileNextFireTime)
            {
                LaunchMissile(startPos);
                missileNextFireTime = Time.time + MISSILE_RECHARGE_TIME; // Set next fire time with 0.5 second recharge
                isMissileRecharging = true; // Set recharging flag
                
                // Consume one missile charge
                missileCharges--;
                
                Debug.Log("Missile launched! Charges remaining: " + missileCharges + ". Next missile ready in " + MISSILE_RECHARGE_TIME + " seconds.");
                
                // Trigger missile launched event
                OnMissileLaunched?.Invoke();
            }
            else
            {
                float timeRemaining = missileNextFireTime - Time.time;
                Debug.Log("Missile recharging... " + timeRemaining.ToString("F1") + " seconds remaining. Charges available: " + missileCharges);
            }
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
    
    /// <summary>
    /// Public function to add a charge to the mega laser
    /// </summary>
    public void AddMegaLaserCharge()
    {
        megaLaserCharges++;
        Debug.Log("Mega laser charge added! Total charges: " + megaLaserCharges);
    }
    
    /// <summary>
    /// Get the current number of mega laser charges
    /// </summary>
    public int GetMegaLaserCharges()
    {
        return megaLaserCharges;
    }
    
    /// <summary>
    /// Public function to add missile charges
    /// </summary>
    public void AddMissileCharges(int amount = 1)
    {
        missileCharges += amount;
        Debug.Log("Missile charges added! Amount: " + amount + ", Total charges: " + missileCharges);
    }
    
    /// <summary>
    /// Get the current number of missile charges
    /// </summary>
    public int GetMissileCharges()
    {
        return missileCharges;
    }
    
    /// <summary>
    /// Set the number of missile charges directly
    /// </summary>
    public void SetMissileCharges(int charges)
    {
        missileCharges = Mathf.Max(0, charges); // Ensure non-negative
        Debug.Log("Missile charges set to: " + missileCharges);
    }
    
    /// <summary>
    /// Check if missile is currently recharging
    /// </summary>
    public bool IsMissileRecharging()
    {
        return isMissileRecharging;
    }
    
    /// <summary>
    /// Get time remaining until missile recharge is complete
    /// </summary>
    public float GetMissileRechargeTimeRemaining()
    {
        if (!isMissileRecharging) return 0f;
        return Mathf.Max(0f, missileNextFireTime - Time.time);
    }
    
    /// <summary>
    /// Check if laser is currently firing (either normal or mega)
    /// </summary>
    public bool IsLaserCurrentlyFiring()
    {
        return isLaserCurrentlyFiring;
    }
    
    /// <summary>
    /// Check if missiles are available to fire (has charges and not recharging)
    /// </summary>
    public bool CanFireMissile()
    {
        return missileCharges > 0 && Time.time >= missileNextFireTime;
    }
}