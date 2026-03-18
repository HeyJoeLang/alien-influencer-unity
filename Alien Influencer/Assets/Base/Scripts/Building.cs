using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class Building : MonoBehaviour
{
    #region Variables
    public enum BuildingState
    {
        Untouched,
        StartDamaged,
        IsDamaged,
        StartDestroyed,
        IsDestroyed
    }
    public BuildingState CurrentState = BuildingState.Untouched;
    public int maxDamage = 100;
    public float currentDamage = 0;

    public int scoreValue = 10;
    public GameObject buildingStanding, buildingDestroyed;
    public ParticleSystem damagedParticles, destroyedParticles;
    public Animator damageBarAnimator;
    public ProgressBarPro damageProgressBar;
    [SerializeField] private EventReference eventBuildingDestroyedRouble3D;
    [SerializeField] private EventReference eventBuildingDestrouyed2D;
    [SerializeField] private EventReference eventDamaged;
    [SerializeField] private EventReference eventBuildingMissileHitStillStanding;
    private EventInstance buildingDestroyedRouble3DEventInstance;   
    private EventInstance buildingDestroyed2DEventInstance;
    private EventInstance damagedEventInstance;
    private EventInstance missileHitStillStandingEventInstance;
    ParticleSystem sparksParticles;
    float distance = 0f;
    private Transform mainCameraTransform;
    #endregion
    #region Unity Methods

    private void Start()
    {
        buildingStanding.SetActive(true);
        buildingDestroyed.SetActive(false);
        damagedParticles.gameObject.SetActive(false);
        destroyedParticles.gameObject.SetActive(false);
        damageBarAnimator.gameObject.SetActive(false);
        
        // Cache the sparks particle system reference
        if (damagedParticles != null && damagedParticles.transform.childCount > 0)
        {
            sparksParticles = damagedParticles.transform.GetChild(0).GetComponent<ParticleSystem>();
        }
        buildingDestroyedRouble3DEventInstance = RuntimeManager.CreateInstance(eventBuildingDestroyedRouble3D);
        buildingDestroyed2DEventInstance = RuntimeManager.CreateInstance(eventBuildingDestrouyed2D);
        damagedEventInstance = RuntimeManager.CreateInstance(eventDamaged);
        missileHitStillStandingEventInstance = RuntimeManager.CreateInstance(eventBuildingMissileHitStillStanding);
        RuntimeManager.AttachInstanceToGameObject(damagedEventInstance, transform);
        RuntimeManager.AttachInstanceToGameObject(missileHitStillStandingEventInstance, transform);
        RuntimeManager.AttachInstanceToGameObject(buildingDestroyedRouble3DEventInstance, transform);

        mainCameraTransform = Camera.main.transform;
    }
    private void Update()
    {
        switch (CurrentState)
        {
            case BuildingState.StartDamaged:
                StartDamaged();
                break;
            case BuildingState.IsDamaged:
                IsDamaged();
                break;
            case BuildingState.StartDestroyed:
                StartDestroyed();
                break;
        }
    }

    #endregion
    #region State Functions

    private void StartDamaged()
    { 
        InitializeDamageVisuals();
        CurrentState = BuildingState.IsDamaged;
        damagedEventInstance.start();
    }

    private void InitializeDamageVisuals()
    {
        damageBarAnimator.gameObject.SetActive(true);
        damageBarAnimator.SetTrigger("Open");
        
        damagedParticles.gameObject.SetActive(true);
        damagedParticles.Play();
        damageProgressBar.SetValue(0);
        
        UpdateParticleEmissionRate();
    }
    private void IsDamaged()
    {
        if (mainCameraTransform == null) return;
        
        distance = Vector3.Distance(transform.position, mainCameraTransform.position);
        if (distance < 10f)
        {
            damagedEventInstance.setParameterByName("Distance", distance);
        }
    }

    private void StartDestroyed()
    {
        buildingDestroyedRouble3DEventInstance.start();
        buildingDestroyed2DEventInstance.start();
        
        if (damageBarAnimator)
        {
            damageBarAnimator.SetTrigger("Close");
        }
        
        if (destroyedParticles)
        {
            destroyedParticles.gameObject.SetActive(true);
            destroyedParticles.Play();
        }
        
        StartCoroutine(StallDisableStandingBuildingObjects());
        
        if (buildingDestroyed)
            buildingDestroyed.SetActive(true);
        if (buildingStanding)
            buildingStanding.SetActive(false);
        
        CurrentState = BuildingState.IsDestroyed;
        
        if (GameManager.Instance)
            GameManager.Instance.AddScore(scoreValue);
        
        if (PowerupSpawner.Instance)
            PowerupSpawner.Instance.SpawnAtLocation(new Vector3(transform.position.x, 8, transform.position.z));
    }

    #endregion
    #region Public Methods

    public void AddDamage(float amount, bool isMissile)
    {
        if (CurrentState == BuildingState.IsDestroyed)
        {
            return;
        }
        if(CurrentState == BuildingState.Untouched)
        {
            StartDamaged();
        }
        currentDamage += amount;
        damageProgressBar.SetValue(currentDamage, maxDamage, false);
        
        // Update particle emission rate when damage changes
        UpdateParticleEmissionRate();
        
        if (currentDamage >= maxDamage)
        {
            CurrentState = BuildingState.StartDestroyed;
        }
        else
        {
            if (isMissile)
            {
                missileHitStillStandingEventInstance.start();
            }
        }
    }

    #endregion
    #region Utility Functions

    private const float MIN_EMISSION_RATE = 10f;
    private const float MAX_EMISSION_RATE = 200f;
    private const float MIN_SPARKS_RATE = 1f;
    private const float MAX_SPARKS_RATE = 30f;

    void UpdateParticleEmissionRate()
    {
        if (damagedParticles || CurrentState == BuildingState.Untouched || CurrentState == BuildingState.IsDestroyed)
            return;
    
        float normalizedDamage = Mathf.Clamp(currentDamage, 1f, maxDamage);
        float damagePercent = (normalizedDamage - 1f) / (maxDamage - 1f);
    
        // Scale main particle emission
        float mainEmissionRate = Mathf.Lerp(MIN_EMISSION_RATE, MAX_EMISSION_RATE, damagePercent);
        var mainEmission = damagedParticles.emission;
        mainEmission.rateOverTime = mainEmissionRate;
    
        // Scale sparks particle emission
        if (!sparksParticles) return;
        var sparksEmissionRate = Mathf.Lerp(MIN_SPARKS_RATE, MAX_SPARKS_RATE, damagePercent);
        var sparksEmission = sparksParticles.emission;
        sparksEmission.rateOverTime = sparksEmissionRate;
    }

    private IEnumerator StallDisableStandingBuildingObjects()
    {
        yield return new WaitForSeconds(1f);
        damagedParticles.gameObject.SetActive(false);
        damageBarAnimator.gameObject.SetActive(false);
    }

    #endregion
    private void OnDestroy()
    {
        if (buildingDestroyedRouble3DEventInstance.isValid())
        {
            buildingDestroyedRouble3DEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            buildingDestroyedRouble3DEventInstance.release();
        }
        
        if (buildingDestroyed2DEventInstance.isValid())
        {
            buildingDestroyed2DEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            buildingDestroyed2DEventInstance.release();
        }
        
        if (damagedEventInstance.isValid())
        {
            damagedEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            damagedEventInstance.release();
        }
        
        if (missileHitStillStandingEventInstance.isValid())
        {
            missileHitStillStandingEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            missileHitStillStandingEventInstance.release();
        }
    }
}