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
    public AudioClip buildingDestroyedSound;
    public AudioClip damagedSound;
    AudioSource audioSource;


    #endregion
    #region Unity Methods

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        buildingStanding.SetActive(true);
        buildingDestroyed.SetActive(false);
        damagedParticles.gameObject.SetActive(false);
        destroyedParticles.gameObject.SetActive(false);
        damageBarAnimator.gameObject.SetActive(false);

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

        CurrentState = BuildingState.IsDamaged;
    }
    void IsDamaged()
    {
    }
    void StartDestroyed()
    {
        audioSource.PlayOneShot(buildingDestroyedSound);
        damageBarAnimator.SetTrigger("Close");
        buildingStanding.SetActive(false);

        destroyedParticles.gameObject.SetActive(true);
        destroyedParticles.Play();
        StartCoroutine(StallDisableStandingBuildingObjects());
        buildingDestroyed.SetActive(true);
        CurrentState = BuildingState.IsDestroyed;
        GameManager.Instance.AddScore(scoreValue);
    }
    void IsDestroyed()
    {
    }

    #endregion
    #region Public Methods

    public void AddDamage(float amount)
    {
        audioSource.PlayOneShot(damagedSound,.1f);
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
        if (currentDamage >= maxDamage)
        {
            CurrentState = BuildingState.StartDestroyed;
        }
    }

    #endregion
    #region Utility Functions

    IEnumerator StallDisableStandingBuildingObjects()
    {
        yield return new WaitForSeconds(1f);
        damagedParticles.gameObject.SetActive(false);
        damageBarAnimator.gameObject.SetActive(false);
    }

    #endregion

}
