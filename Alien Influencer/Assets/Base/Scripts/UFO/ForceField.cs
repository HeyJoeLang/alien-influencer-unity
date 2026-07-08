using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    private bool isOn = false;
    public Animator animator;
    public GameObject deflectMissile;
    public GameObject BounceVFX;
    
    private List<GameObject> homingMissilesInField = new List<GameObject>();

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.LeftAlt) && !Input.GetKeyDown(KeyCode.K)) return;
        if (isOn)
        {
            return;
        }
        isOn = true;
            
        DestroyHomingMissiles();
            
        StartCoroutine(ForceFieldEffect());
        animator.SetTrigger("On");
        AudioManager.Instance.PlayUfoForceFieldActivation();
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
            
        if (!homingMissilesInField.Contains(collision.gameObject))
        {
            homingMissilesInField.Add(collision.gameObject);
            Debug.Log($"Homing missile count: {homingMissilesInField.Count}");
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
        List<GameObject> missilesToDestroy = new List<GameObject>(homingMissilesInField);
        
        foreach (GameObject missile in missilesToDestroy)
        {
            if (missile == null) continue;
            missile.GetComponent<HomingMissile.homing_missile>().StopFlySound();
            missile.SetActive(false);
            Vector3 direction = (missile.transform.position - transform.position).normalized;
            Instantiate(deflectMissile, missile.transform.position, Quaternion.LookRotation(direction+ new Vector3(0,-.2f,0)) );
            GameObject bounceVFX = Instantiate(BounceVFX, missile.transform.position, Quaternion.LookRotation(direction));
            Destroy(bounceVFX, 1f);
            AudioManager.Instance.PlayUfoForceFieldBounce();
            Destroy(missile);
        }
        
        homingMissilesInField.Clear();
        
        Debug.Log($"Destroyed {missilesToDestroy.Count} homing missiles");
    }
}
