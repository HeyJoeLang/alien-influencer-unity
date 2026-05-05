using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ForceField : MonoBehaviour
{
    private bool isOn = false;
    public Animator animator;
    public GameObject deflectMissile;
    public GameObject BounceVFX;
    
    [SerializeField] private EventReference eventForceField;
    [SerializeField] private EventReference eventBounce;
    
    private EventInstance forceFieldEventInstance;
    private EventInstance bounceEventInstance;
    
    // Persistent list to track homing missiles in the force field area
    private List<GameObject> homingMissilesInField = new List<GameObject>();

    void Start()
    {
        forceFieldEventInstance = RuntimeManager.CreateInstance(eventForceField);
        
        RuntimeManager.AttachInstanceToGameObject(forceFieldEventInstance, transform);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.LeftAlt)) return;
        if (isOn)
        {
            return;
        }
        isOn = true;
            
        // Immediately destroy all homing missiles in the field
        DestroyHomingMissiles();
            
        StartCoroutine(ForceFieldEffect());
        animator.SetTrigger("On");
        forceFieldEventInstance.start();
    }
    
    IEnumerator ForceFieldEffect()
    {
        yield return new WaitForSeconds(1);
        isOn = false;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.CompareTag("homing_missile")) return;
        Debug.Log("Missile In Force Field");
            
        // Add to the list if not already present
        if (!homingMissilesInField.Contains(collision.gameObject))
        {
            homingMissilesInField.Add(collision.gameObject);
            Debug.Log($"Homing missile count: {homingMissilesInField.Count}");
            //collision.gameObject.GetComponent<HomingMissile.homing_missile>().MissileDestroyed += RemoveMissile;
        }
    }
    
    void OnTriggerExit(Collider collision)
    {
        if (!collision.gameObject.CompareTag("homing_missile")) return;
        Debug.Log("Missile Left Force Field");
        RemoveMissile(collision.gameObject);
    }

    void RemoveMissile(GameObject missile)
    {
        homingMissilesInField.Remove(missile);
    }
    private void DestroyHomingMissiles()
    {
        // Create a copy of the list to avoid modification during iteration
        List<GameObject> missilesToDestroy = new List<GameObject>(homingMissilesInField);
        
        foreach (GameObject missile in missilesToDestroy)
        {
            if (missile == null) continue; // Check if the object still exists
            missile.GetComponent<HomingMissile.homing_missile>().StopFlySound();
            missile.SetActive(false);
            Vector3 direction = (missile.transform.position - transform.position).normalized;
            Instantiate(deflectMissile, missile.transform.position, Quaternion.LookRotation(direction+ new Vector3(0,-.2f,0)) );
            GameObject bounceVFX = Instantiate(BounceVFX, missile.transform.position, Quaternion.LookRotation(direction));
            Destroy(bounceVFX, 1f);
            bounceEventInstance = RuntimeManager.CreateInstance(eventBounce);
            RuntimeManager.AttachInstanceToGameObject(bounceEventInstance, deflectMissile.transform);
            bounceEventInstance.start();
            Destroy(missile);
        }
        
        // Clear the list after destruction
        homingMissilesInField.Clear();
        
        Debug.Log($"Destroyed {missilesToDestroy.Count} homing missiles");
    }
}