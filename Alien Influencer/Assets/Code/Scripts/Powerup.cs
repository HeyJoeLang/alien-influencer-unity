using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class Powerup : MonoBehaviour
{
    public enum PowerupType
    {
        Missile,
        MegaLaser
    }

    [Header("Powerup Settings")]
    [SerializeField] private PowerupType type = PowerupType.Missile;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private GameObject orb;

    [Header("Audio Events")]
    [SerializeField] private EventReference eventMegaLaserPickup;
    [SerializeField] private EventReference eventMissilePickup;

    private EventInstance megaLaserPickupEventInstance;
    private EventInstance missilePickupEventInstance;

    private void Start()
    {
        switch (type)
        {
            case PowerupType.Missile:
                missilePickupEventInstance = RuntimeManager.CreateInstance(eventMissilePickup);
                RuntimeManager.AttachInstanceToGameObject(missilePickupEventInstance, transform);
                break;
            case PowerupType.MegaLaser:
                megaLaserPickupEventInstance = RuntimeManager.CreateInstance(eventMegaLaserPickup);
                RuntimeManager.AttachInstanceToGameObject(megaLaserPickupEventInstance, transform);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("UFO"))
            return;

        UFOLaser weaponScript = other.GetComponent<UFOLaser>();
        if (weaponScript == null)
        {
            Debug.LogWarning("[Powerup] UFO does not have UFOLaser component!");
            return;
        }

        switch (type)
        {
            case PowerupType.Missile:
                weaponScript.AddMissileCharges(1);
                missilePickupEventInstance.start();
                break;
            case PowerupType.MegaLaser:
                weaponScript.AddMegaLaserCharge();
                megaLaserPickupEventInstance.start();
                break;
        }

        DestroyPowerup();
    }

    private void DestroyPowerup()
    {
        GetComponent<Collider>().enabled = false;
        explosionParticles.Play();
        orb.SetActive(false);
        Destroy(gameObject, 1f);
    }

    private void OnDestroy()
    {
        if (missilePickupEventInstance.isValid())
        {
            missilePickupEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            missilePickupEventInstance.release();
        }

        if (megaLaserPickupEventInstance.isValid())
        {
            megaLaserPickupEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            megaLaserPickupEventInstance.release();
        }
    }
}
