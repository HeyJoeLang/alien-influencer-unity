using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;
using FMOD.Studio;

public class CoinManager : MonoBehaviour
{
    public TMP_Text coinDisplay;
    public int minCoinsForCredit = 1;
    private int coinCount = 0;
    private const string CoinKey = "CoinCount";
    public Button playButton;
    public EventSystem eventSystem;
    
    [SerializeField] private EventReference eventCoinInsert;
    private EventInstance coinInsertEventInstance;

    void Start()
    {
        coinInsertEventInstance = RuntimeManager.CreateInstance(eventCoinInsert);
        RuntimeManager.AttachInstanceToGameObject(coinInsertEventInstance, transform);
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
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Alpha6))
        {
            InsertCoin();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ResetCredits();
        }
        if (!Input.GetKeyDown(KeyCode.LeftAlt) && !Input.GetKeyDown(KeyCode.LeftControl)
                                               && !Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.X) &&
                                               !Input.GetKeyDown(KeyCode.C) && !Input.GetKeyDown(KeyCode.V)) return;
        if (playButton.interactable)
        {
            playButton.onClick.Invoke();
        }
    }

    void InsertCoin()
    {
        coinInsertEventInstance.getPlaybackState(out var playbackState);
        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            coinInsertEventInstance.start();
        }
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
        playButton.interactable = coinCount >= minCoinsForCredit;
        eventSystem.SetSelectedGameObject(playButton.gameObject);
    }
}
