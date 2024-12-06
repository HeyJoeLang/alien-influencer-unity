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
        if(!Screen.fullScreen)
        {
            Screen.fullScreen = true;
        }
        coinCount = PlayerPrefs.GetInt(CoinKey, 0);
        UpdateCoinDisplay();
    }

    void Update()
    {
        // Check if the '5' key is pressed to simulate inserting a coin
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            InsertCoin();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ResetCredits();
        }
    }

    void InsertCoin()
    {
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
        playButton.interactable = coinCount >= minCoinsForCredit;
        eventSystem.SetSelectedGameObject(playButton.gameObject);
    }
}
