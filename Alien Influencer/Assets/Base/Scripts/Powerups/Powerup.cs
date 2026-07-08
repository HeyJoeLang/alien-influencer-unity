using UnityEngine;

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
    //            AudioManager.Instance.Play(AudioSoundIds.SoundDesign.PowerUps.CollectUfoMissileCharge, transform);
                break;
            case PowerupType.MegaLaser:
                weaponScript.AddMegaLaserCharge();
   //             AudioManager.Instance.Play(AudioSoundIds.SoundDesign.PowerUps.CollectMegaLaserCharge, transform);
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
}
