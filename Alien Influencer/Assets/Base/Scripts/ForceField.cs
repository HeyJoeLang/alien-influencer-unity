using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    private bool isOn = false;
    public Animator animator;
    
    // Persistent list to track homing missiles in the force field area
    private List<GameObject> homingMissilesInField = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = true;
            
            // Immediately destroy all homing missiles in the field
            DestroyHomingMissiles();
            
            StartCoroutine(ForceFieldEffect());
            animator.SetTrigger("On");
        }
    }
    
    IEnumerator ForceFieldEffect()
    {
        yield return new WaitForSeconds(1);
        isOn = false;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "homing_missile")
        {
            Debug.Log("Missile In Force Field");
            
            // Add to the list if not already present
            if (!homingMissilesInField.Contains(collision.gameObject))
            {
                homingMissilesInField.Add(collision.gameObject);
                Debug.Log($"Homing missile count: {homingMissilesInField.Count}");
                //collision.gameObject.GetComponent<HomingMissile.homing_missile>().MissileDestroyed += RemoveMissile;
            }
        }
    }
    
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "homing_missile")
        {
            Debug.Log("Missile Left Force Field");
            RemoveMissile(collision.gameObject);
        }
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
            if (missile != null) // Check if the object still exists
            {
                missile.SetActive(false);
                Destroy(missile);
            }
        }
        
        // Clear the list after destruction
        homingMissilesInField.Clear();
        
        Debug.Log($"Destroyed {missilesToDestroy.Count} homing missiles");
    }
}