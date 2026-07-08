using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CoinManager : MonoBehaviour
{
    public TMP_Text coinDisplay;
    public int minCoinsForCredit = 1;
    private int coinCount = 0;
    private const string CoinKey = "CoinCount";
    public Button playButton;
    public EventSystem eventSystem;

    void Start()
    {
        coinCount = PlayerPrefs.GetInt(CoinKey, 0);
        UpdateCoinDisplay();
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.LeftAlt) && !Input.GetKeyDown(KeyCode.LeftControl)
                                               && !Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.X) &&
                                               !Input.GetKeyDown(KeyCode.LeftAlt) && !Input.GetKeyDown(KeyCode.Z)) return;
        if (playButton.interactable)
        {
            playButton.onClick.Invoke();
        }
    }

    void InsertCoin()
    {
        // TODO: add Non-FMOD export for Sound Design/UI/InsertCoin
   //    AudioManager.Instance.Play2D(AudioSoundIds.SoundDesign.UI.InsertCoin);
        coinCount++;
        PlayerPrefs.SetInt(CoinKey, coinCount);
        UpdateCoinDisplay();

        if (coinCount >= minCoinsForCredit)
        {
            UpdateCoinDisplay();
        }
    }
    public void RemoveCredit()
    {
        coinCount -= minCoinsForCredit;
        PlayerPrefs.SetInt(CoinKey, coinCount);
        UpdateCoinDisplay();
        InsertCoin(); //Demo: unlimited coins
    }
    void ResetCredits()
    {
        coinCount = 0;
        PlayerPrefs.SetInt(CoinKey, coinCount);
        UpdateCoinDisplay();
    }

    void UpdateCoinDisplay()
    {
        coinDisplay.text = string.Format("Coins: {0} / {1}", coinCount, minCoinsForCredit);
        playButton.interactable = true;
        eventSystem.SetSelectedGameObject(playButton.gameObject);
    }
}
