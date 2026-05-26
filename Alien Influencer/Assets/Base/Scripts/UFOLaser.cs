using Sharklib.UI;
using System.Collections;
using System.Collections.Generic;
using PilotoStudio;
using UnityEngine;
using System; // Added for Action
using FMODUnity;
using FMOD.Studio;

public class UFOLaser : MonoBehaviour
{
    // Add these variables at the top of the UFOLaser class
    [Header("FMOD Event")]
    [SerializeField] private EventReference eventLaser;
    [SerializeField] private EventReference eventMegaLaser;
    [SerializeField] private EventReference eventMissile;
    
    private EventInstance laserEventInstance;
    private EventInstance megaLaserEventInstance;
    private EventInstance missileEventInstance;
    
    [SerializeField] private float audioFadeTime = 0.2f; // Time to fade in/out
    [SerializeField] private float weaponVolume = 1f;
    public LayerMask raycastLayer;
    public LayerMask terrainLayer;
    public Vector3 deltaPosition = Vector3.zero;
    public Vector3 deltaDirection = Vector3.forward;
    public float rayLength = 10f;
    public Color hitColor = Color.green;
    public Color missColor = Color.yellow;
    private float laserDamage = 50f;
    public GameObject laserBeam;
    public GameObject LaserBeamImpactFlames;
    public GameObject megaLaserBeam;
    public Transform laserBeamImpact;
    public Transform megaImpact1, megaImpact2, megaImpact3;
    
    public GameObject crosshair;
    Material crosshairMat;
	public ProgressBarPro progressBar;
    private Animator progressBarAnimator;
    
    // Mega laser timer variables
    private float megaLaserTimer = 0f;
    private const float MEGA_LASER_DURATION = 10f;
    private bool isMegaLaserActive = false;
    [Range(0f, 1f)]
    public float megaLaserTimeRemainingPercent = 0f; // 0 = no time remaining, 1 = full time remaining
    
    // Mega laser charge system
    private int megaLaserCharges = 5; // Starting charges
    
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
    private int missileCharges = 5; // Starting missile charges
    
    // Public events for missile state changes
    public static event Action OnMissileLaunched;
    public static event Action OnMissileRechargeComplete;
    
    // Laser firing state tracking - separate for each laser type
    private bool isNormalLaserCurrentlyFiring = false;
    private bool isMegaLaserCurrentlyFiring = false;
    
    // Public events for laser firing state changes
    public static event Action OnLaserActivated;
    public static event Action OnLaserDeactivated;
    
    public static event Action OnMissileAdded;
    public static event Action OnMegaLaserAdded;
    
    // Fire pool variables
    private FirePool firePool;
    private float fireNextSpawnTime = 0f;
    private const float FIRE_COOLDOWN = 0.1f;

    // Add this method to handle audio setup
    private void SetupAudioSources()
    {
        
        laserEventInstance = RuntimeManager.CreateInstance(eventLaser);
        megaLaserEventInstance = RuntimeManager.CreateInstance(eventMegaLaser);
        missileEventInstance = RuntimeManager.CreateInstance(eventMissile);
        
        RuntimeManager.AttachInstanceToGameObject(laserEventInstance, transform);
        RuntimeManager.AttachInstanceToGameObject(megaLaserEventInstance, transform);
        RuntimeManager.AttachInstanceToGameObject(missileEventInstance, transform);
        
    }

    // Add this to the existing Start() method
    void Start()
    {
        crosshairMat = crosshair.GetComponent<MeshRenderer>().material;
        laserBeam.transform.position = transform.position + deltaPosition;
        progressBarAnimator = progressBar.GetComponent<Animator>();
        
        // Get reference to FirePool component on the same GameObject
        firePool = GetComponent<FirePool>();
        if (firePool == null)
        {
            Debug.LogError("FirePool component not found on " + gameObject.name);
        }
        progressBar.Start();
        SetupAudioSources();
    }

    // Add these methods for handling the audio
    private void StartLaserSound()
    {
        Debug.Log("Laser sound started");
        laserEventInstance.getPlaybackState(out var playbackState);

        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            laserEventInstance.start();
        }
    }

    private void StopLaserSound()
    {
        laserEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); 
    }

    private void StartMegaLaserSound()
    {
        
        megaLaserEventInstance.getPlaybackState(out var playbackState);

        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            megaLaserEventInstance.start();
        }
    }

    private void StopMegaLaserSound()
    {
        megaLaserEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); 
    }

    private void PlayMissileSound()
    {
        missileEventInstance.getPlaybackState(out var playbackState);

        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            missileEventInstance.start();
        }
        
    }

    private IEnumerator FadeAudio(AudioSource audioSource, float startVolume, float targetVolume, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }
        audioSource.volume = targetVolume;
        
        if (targetVolume == 0f)
        {
            audioSource.Stop();
        }
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
                // End mega laser session and consume a charge
                isMegaLaserActive = false;
                megaLaserTimer = 0f;
                megaLaserTimeRemainingPercent = 0f;
                progressBar.SetValue(megaLaserTimeRemainingPercent, true);
                
                // Consume one charge
                megaLaserCharges--;
                Debug.Log("Mega laser charge consumed. Remaining charges: " + megaLaserCharges);
                
                // If mega laser was currently firing, stop it
                if (isMegaLaserCurrentlyFiring)
                {
                    isMegaLaserCurrentlyFiring = false;
                    megaLaserBeam.SetActive(false);
                    StopMegaLaserSound();
                    OnMegaLaserDeactivated?.Invoke();
                }
                
                progressBarAnimator.SetTrigger("FadeToZero");
            }
        }
        else
        {
            // When mega laser is not active, percentage should be 0
            megaLaserTimeRemainingPercent = 0f;
        }
        
        bool didHitBuilding = false;
        Vector3 rayDirection = transform.TransformDirection(deltaDirection);
        RaycastHit hit;
        Vector3 startPos = transform.position + deltaPosition;
        Vector3 endPos = startPos + rayDirection * rayLength;

        if (Physics.Raycast(startPos, rayDirection, out hit, rayLength, raycastLayer.value))
        {
            crosshair.SetActive(true);
            crosshairMat.color = hitColor;
            crosshair.transform.rotation = Quaternion.LookRotation(hit.normal);
            endPos = hit.point;
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
        
        // Handle normal laser firing (Fire3 button)
        bool shouldNormalLaserFire = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.U);
        
        if (shouldNormalLaserFire)
        {
            if (!isNormalLaserCurrentlyFiring)
            {
                isNormalLaserCurrentlyFiring = true;
                StartLaserSound();
                OnLaserActivated?.Invoke();
            }
            
            laserBeam.SetActive(true);
            
            if (Physics.Raycast(startPos, rayDirection, out hit, rayLength, raycastLayer.value))
            {
                var building = hit.collider.transform.parent.GetComponent<Building>();
                if (building)
                {
                    didHitBuilding = true;
                    building.AddDamage(laserDamage * Time.fixedDeltaTime, false); // Apply normal laser damage
                    
                    // Create fire effect at hit location with cooldown
                    if (firePool != null && Time.time >= fireNextSpawnTime)
                    {
                        firePool.CreateFire(hit.point, hit.collider.transform);
                        fireNextSpawnTime = Time.time + FIRE_COOLDOWN;
                    }
                }
            }
        }
        else
        {
            if (isNormalLaserCurrentlyFiring)
            {
                isNormalLaserCurrentlyFiring = false;
                StopLaserSound();
                OnLaserDeactivated?.Invoke();
            }
            laserBeam.SetActive(false);
        }
        
        // Handle mega laser firing (Fire1 button)
        bool shouldMegaLaserFire = Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.J); // Changed to GetButtonDown for single press
        
        if (shouldMegaLaserFire)
        {
            if (isMegaLaserActive)
            {
                Debug.Log("Mega laser is already active! Wait for it to complete before using another charge.");
            }
            else if (megaLaserCharges > 0)
            {
                // Start mega laser session - it will fire continuously for the full duration
                isMegaLaserActive = true;
                isMegaLaserCurrentlyFiring = true;
                megaLaserTimer = 0f;
                megaLaserTimeRemainingPercent = 1f; // Start with full time remaining
                Debug.Log("Mega laser activated! Firing continuously for " + MEGA_LASER_DURATION + " seconds. Charges remaining after this use: " + (megaLaserCharges - 1));
                
                StartMegaLaserSound();
                OnMegaLaserActivated?.Invoke();
                progressBarAnimator.SetTrigger("FadeToOne");
            }
            else
            {
                Debug.Log("No mega laser charges remaining!");
            }
        }
        
        // Mega laser fires continuously while active
        if (isMegaLaserActive)
        {
            megaLaserBeam.SetActive(true);
            
            if (Physics.Raycast(startPos, rayDirection, out hit, rayLength, raycastLayer.value))
            {
                var building = hit.collider.transform.parent.GetComponent<Building>();
                if (building)
                {
                    didHitBuilding = true;
                    building.AddDamage(laserDamage * 5f * Time.fixedDeltaTime, false); // Apply mega laser damage (10x normal)
                    
                    // Create fire effect at hit location with cooldown
                    if (firePool != null && Time.time >= fireNextSpawnTime)
                    {
                        firePool.CreateFire(hit.point, hit.collider.transform);
                        fireNextSpawnTime = Time.time + FIRE_COOLDOWN;
                    }
                }
            }
        }
        else
        {
            megaLaserBeam.SetActive(false);
        }
        
        crosshair.transform.position = endPos - (endPos - startPos).normalized * 0.5f;
        laserBeamImpact.position = megaImpact1.position = megaImpact2.position = megaImpact3.position = endPos - (endPos - startPos).normalized;
        LaserBeamImpactFlames.SetActive(didHitBuilding);
        
        // Handle missile launching with recharge time and charge system
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.I))
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
                PlayMissileSound();
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
        OnMegaLaserAdded?.Invoke();
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
        OnMissileAdded?.Invoke();
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
    /// Check if normal laser is currently firing
    /// </summary>
    public bool IsNormalLaserCurrentlyFiring()
    {
        return isNormalLaserCurrentlyFiring;
    }
    
    /// <summary>
    /// Check if mega laser is currently firing
    /// </summary>
    public bool IsMegaLaserCurrentlyFiring()
    {
        return isMegaLaserCurrentlyFiring;
    }
    
    /// <summary>
    /// Check if any laser is currently firing (either normal or mega)
    /// </summary>
    public bool IsAnyLaserCurrentlyFiring()
    {
        return isNormalLaserCurrentlyFiring || isMegaLaserCurrentlyFiring;
    }
    
    /// <summary>
    /// Check if missiles are available to fire (has charges and not recharging)
    /// </summary>
    public bool CanFireMissile()
    {
        return missileCharges > 0 && Time.time >= missileNextFireTime;
    }

    // Add this to OnDestroy (create if it doesn't exist)
    private void OnDestroy()
    {
        // Unsubscribe from events
        OnLaserActivated -= StartLaserSound;
        OnLaserDeactivated -= StopLaserSound;
        OnMegaLaserActivated -= StartMegaLaserSound;
        OnMegaLaserDeactivated -= StopMegaLaserSound;
        OnMissileLaunched -= PlayMissileSound;
    }
}