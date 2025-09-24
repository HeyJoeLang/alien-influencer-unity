using TMPro;
using UnityEngine;

public class WeaponsUI : MonoBehaviour
{
    [Header("References")]
    public UFOLaser weaponsScript;
    
    [Header("UI Elements")]
    public GameObject laserBGIndicator;
    public GameObject megaLaserBGIndicator;
    public GameObject missileBGIndicator;
    public TMP_Text megaLlaserChargesLeftText;
    public TMP_Text missilesChargesLeftText;

    void Start()
    {
        // Initialize UI with current values
        RefreshUI();
        
        // Subscribe to UFOLaser events
        UFOLaser.OnLaserActivated += EnableLaser;
        UFOLaser.OnLaserDeactivated += DisableLaser;
        UFOLaser.OnMegaLaserActivated += EnableMegaLaser;
        UFOLaser.OnMegaLaserDeactivated += DisableMegaLaser;
        UFOLaser.OnMissileLaunched += OnMissileLaunched;
        UFOLaser.OnMissileRechargeComplete += OnMissileRechargeComplete;
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        UFOLaser.OnLaserActivated -= EnableLaser;
        UFOLaser.OnLaserDeactivated -= DisableLaser;
        UFOLaser.OnMegaLaserActivated -= EnableMegaLaser;
        UFOLaser.OnMegaLaserDeactivated -= DisableMegaLaser;
        UFOLaser.OnMissileLaunched -= OnMissileLaunched;
        UFOLaser.OnMissileRechargeComplete -= OnMissileRechargeComplete;
    }
    
    // Laser Events
    void EnableLaser()
    {
        laserBGIndicator.SetActive(true);
    }
    
    void DisableLaser()
    {
        laserBGIndicator.SetActive(false);
    }
    
    // Mega Laser Events
    void EnableMegaLaser()
    {
        megaLaserBGIndicator.SetActive(true);
        UpdateMegaLaserChargesUI();
    }
    
    void DisableMegaLaser()
    {
        megaLaserBGIndicator.SetActive(false);
        UpdateMegaLaserChargesUI();
    }
    
    // Missile Events
    void OnMissileLaunched()
    {
        missileBGIndicator.SetActive(true);
        UpdateMissileChargesUI();
    }
    
    void OnMissileRechargeComplete()
    {
        missileBGIndicator.SetActive(false);
        UpdateMissileChargesUI();
    }
    
    // UI Update Methods
    void UpdateMegaLaserChargesUI()
    {
        if (weaponsScript != null && megaLlaserChargesLeftText != null)
        {
            megaLlaserChargesLeftText.text = $"{weaponsScript.GetMegaLaserCharges()}";
        }
    }
    
    void UpdateMissileChargesUI()
    {
        if (weaponsScript != null && missilesChargesLeftText != null)
        {
            missilesChargesLeftText.text = $"{weaponsScript.GetMissileCharges()}";
        }
    }
    
    // Public methods for manual UI updates (if needed)
    public void RefreshUI()
    {
        UpdateMegaLaserChargesUI();
        UpdateMissileChargesUI();
    }
}