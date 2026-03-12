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
    /*
    public AudioClip buildingDestroyedSound;
    public AudioClip damagedSound;
    public FMODUnity.EventReference buildingDestroyedSoundEvent;
    public FMODUnity.EventReference damagedSoundEvent;
    FMODUnity.EmitterGameEvent audioSource;
    */
    [SerializeField] private EventReference eventBuildingDestroyedRouble3D;
    [SerializeField] private EventReference eventBuildingDestrouyed2D;
    [SerializeField] private EventReference eventDamaged;
    private EventInstance buildingDestroyedRouble3DEventInstance;   
    private EventInstance buildingDestroyed2DEventInstance;
    ParticleSystem sparksParticles;

    #endregion
    #region Unity Methods

    void Start()
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
    }
    void Update()
    {
        switch (CurrentState)
        {
            case BuildingState.Untouched:
                Untouched();
                break;
            case BuildingState.StartDamaged:
                StartDamaged();
                break;
            case BuildingState.IsDamaged:
                IsDamaged();
                break;
            case BuildingState.StartDestroyed:
                StartDestroyed();
                break;
            case BuildingState.IsDestroyed:
                IsDestroyed();
                break;
        }
    }

    #endregion
    #region State Functions

    void Untouched()
    {

    }
    void StartDamaged()
    { 
        damageBarAnimator.gameObject.SetActive(true);
        damageBarAnimator.SetTrigger("Open");

        damagedParticles.gameObject.SetActive(true);
        damagedParticles.Play();
        damageProgressBar.SetValue(0);
        
        // Set initial emission rate based on current damage
        UpdateParticleEmissionRate();

        CurrentState = BuildingState.IsDamaged;
    }
    void IsDamaged()
    {
    }
    void StartDestroyed()
    {
        buildingDestroyedRouble3DEventInstance.start();
        buildingDestroyed2DEventInstance.start();
        damageBarAnimator.SetTrigger("Close");

        destroyedParticles.gameObject.SetActive(true);
        destroyedParticles.Play();
        StartCoroutine(StallDisableStandingBuildingObjects());
        buildingDestroyed.SetActive(true);
        CurrentState = BuildingState.IsDestroyed;
        GameManager.Instance.AddScore(scoreValue);
        buildingStanding.SetActive(false);
        PowerupSpawner.Instance.SpawnAtLocation(new Vector3(transform.position.x, 8, transform.position.z));
    }
    void IsDestroyed()
    {
    }

    #endregion
    #region Public Methods

    public void AddDamage(float amount)
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
    }

    #endregion
    #region Utility Functions

    void UpdateParticleEmissionRate()
    {
        if (damagedParticles != null && CurrentState != BuildingState.Untouched && CurrentState != BuildingState.IsDestroyed)
        {
            float normalizedDamage = Mathf.Clamp(currentDamage, 1f, maxDamage);
            
            // Scale main particle emission rate: damage 1 = rate 10, damage 100 = rate 200
            float mainEmissionRate = 10f + (normalizedDamage - 1f) * 190f / 99f;
            var mainEmission = damagedParticles.emission;
            mainEmission.rateOverTime = mainEmissionRate;
            
            // Scale sparks particle emission rate: damage 1 = rate 1, damage 100 = rate 30
            if (!sparksParticles)
                return;
            float sparksEmissionRate = 1f + (normalizedDamage - 1f) * 29f / 99f;
            var sparksEmission = sparksParticles.emission;
            sparksEmission.rateOverTime = sparksEmissionRate;
        }
    }

    IEnumerator StallDisableStandingBuildingObjects()
    {
        yield return new WaitForSeconds(1f);
        damagedParticles.gameObject.SetActive(false);
        damageBarAnimator.gameObject.SetActive(false);
    }

    #endregion

}