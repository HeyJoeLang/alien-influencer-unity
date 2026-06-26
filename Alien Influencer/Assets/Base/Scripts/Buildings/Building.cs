using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public delegate void DestroyedBuilding(Building building);
    public event DestroyedBuilding OnBuildingDestroyed;
    
    private AudioHandle damagedHandle;
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
        
        if (damagedParticles != null && damagedParticles.transform.childCount > 0)
        {
            sparksParticles = damagedParticles.transform.GetChild(0).GetComponent<ParticleSystem>();
        }

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
        damagedHandle = AudioManager.Instance.PlayLoop(AudioSoundIds.SoundDesign.Destruction.Fire, transform);
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
            AudioManager.Instance.SetLoopDistanceAttenuation(damagedHandle, distance, 10f);
        }
    }

    private void StartDestroyed()
    {
        AudioManager.Instance.Play(AudioSoundIds.SoundDesign.Destruction.BuildingDebris, transform);
        //AudioManager.Instance.Play2D(AudioSoundIds.SoundDesign.Destruction.BuildingExplosion);
        
        if (OnBuildingDestroyed != null)
        {
            Debug.Log($"Building: Building destroyed: {gameObject.name}");
            OnBuildingDestroyed(this);
        }
        
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
        
        UpdateParticleEmissionRate();
        
        if (currentDamage >= maxDamage)
        {
            CurrentState = BuildingState.StartDestroyed;
        }
        else
        {
            if (isMissile)
            {
                AudioManager.Instance.Play(AudioSoundIds.SoundDesign.Destruction.MissileImpactExplosion, transform);
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
    
        float mainEmissionRate = Mathf.Lerp(MIN_EMISSION_RATE, MAX_EMISSION_RATE, damagePercent);
        var mainEmission = damagedParticles.emission;
        mainEmission.rateOverTime = mainEmissionRate;
    
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
        AudioManager.Instance.StopLoop(ref damagedHandle, 0f);
    }
    public void ResetBuilding()
    {
        CurrentState = BuildingState.Untouched;
        currentDamage = 0;
        
        if (buildingStanding)
            buildingStanding.SetActive(true);
        if (buildingDestroyed)
            buildingDestroyed.SetActive(false);
        
        if (damagedParticles)
        {
            damagedParticles.Stop();
            damagedParticles.gameObject.SetActive(false);
        }
        if (destroyedParticles)
        {
            destroyedParticles.Stop();
            destroyedParticles.gameObject.SetActive(false);
        }
        
        if (damageBarAnimator)
        {
            damageBarAnimator.gameObject.SetActive(false);
        }
        if (damageProgressBar)
        {
            damageProgressBar.SetValue(0);
        }
        
        AudioManager.Instance.StopLoop(ref damagedHandle, 0f);
    }
}
