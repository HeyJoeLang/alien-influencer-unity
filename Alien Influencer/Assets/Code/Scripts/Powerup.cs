using System;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public ParticleSystem explosionParticles;
    public GameObject Orb;
    public enum PowerupType
    {
        Missile,
        MegaLaser
    }

    public PowerupType type = PowerupType.Missile;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("UFO"))
        {
            UFOLaser weaponScript = other.GetComponent<UFOLaser>();
            switch (type)
            {
                case PowerupType.Missile:
                    weaponScript.AddMissileCharges(1);
                    Destroy();
                    break;
                case PowerupType.MegaLaser:
                    weaponScript.AddMegaLaserCharge();
                    Destroy();
                    break;
            }
        }
    }

    private void Destroy()
    {
        GetComponent<Collider>().enabled = false;
        explosionParticles.Play();
        Orb.SetActive((false));
        Destroy(gameObject, 1);
    }
}
