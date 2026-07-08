using System.Collections;
using HomingMissile;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public GameObject gameOverMenu;
    public GameObject winMenu;
    public GameObject gameplayHUD;
    public TMP_Text[] scoreText;
    public TMP_Text timeLeftText;
    public float timeRemaining = 181f;
    private int score = 0;
    private int displayedScore = 0;
    private bool isCountingDown = false;
    public EventSystem eventSystem;
    public Button PlayAgainButton;
    public GameObject fadeInOut;
    public AlienAnimationManager alienAnimation;
    private int scoreMultiplier = 1;
    public MissileSpawner missileSpawner;
    public BuildingManager buildingManager;
    public UFOLaser ufoLaser;
    public UfoMovement ufoMovement;
    public ParticleSystem scoreMultiplierParticle;
    public TMP_Text scoreMultiplierText;
    public Animator scoreMultiplierAnimator;

    [Header("Score Animation")]
    public float scoreLerpDuration = 0.5f;

    private void Start()
    {
        fadeInOut.SetActive(true);
        timeRemaining = 181f;
        PositionDeltaManager.Reset();
        Debug.Log("Starting Game...");
        StartGame();
        StartTimer();
    }

    public void StartGame()
    {
        gameOverMenu.SetActive(false);
        gameplayHUD.SetActive(false);
        winMenu.SetActive(false);
        score = 0;
        displayedScore = 0;
        UpdateScore(0);
        Time.timeScale = 1f;
        ufoMovement.enabled = true;
        ufoLaser.enabled = true;
        scoreMultiplierText.text = "1";
    }

    public void GameOver()
    {
        ufoMovement.enabled = false;
        ufoLaser.enabled = false;
        gameplayHUD.SetActive(false);
        gameOverMenu.SetActive(true);
        isCountingDown = false;
        eventSystem.SetSelectedGameObject(PlayAgainButton.gameObject);
        StartCoroutine(GameOverDelay());
    }
    IEnumerator GameOverDelay()
    {
        yield return new WaitForSeconds(10);
        GetComponent<SceneLoader>().MainMenu();
    }
    public void WinGame()
    {
        winMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    public void StartTimer()
    {
        isCountingDown = true;
        ufoMovement.enabled = true;
        ufoLaser.enabled = true;
        gameplayHUD.SetActive(true);
    }

    private void FixedUpdate()
    {
        UpdateTimer();
    }
    private void UpdateTimer()
    {
        if (isCountingDown && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timeLeftText.text = string.Format("{0}:{1:00}", minutes, seconds);
        }
        else if (timeRemaining <= 0 && isCountingDown)
        {
            timeRemaining = 0;
            isCountingDown = false;
            GameOver();
        }
    }
    public void IncreaseScoreMultiplier()
    {
        StartCoroutine(ScoreMultiplyIncrease());
    }

    IEnumerator ScoreMultiplyIncrease()
    {
        scoreMultiplier *= 2;
        missileSpawner.PauseFiring();
        foreach (homing_missile missile in FindObjectsByType<homing_missile>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            missile.DestroyMe();
        }
        
        scoreMultiplierAnimator.SetTrigger("TriggerScoreMultiplierIncrease");

        var emission = scoreMultiplierParticle.emission;
        ParticleSystem.Burst burst = new ParticleSystem.Burst(0.0f, scoreMultiplier);
        emission.SetBurst(0, burst);
        yield return new WaitForSeconds(.75f);
        scoreMultiplierParticle.Play();
        AudioManager.Instance.PlayResetScore();
 //       AudioManager.Instance.Play2D(AudioSoundIds.SoundDesign.UI.ResetScore);
        yield return new WaitForSeconds(1f);
        scoreMultiplierText.text = $"{scoreMultiplier}";
        yield return new WaitForSeconds(1f);
        buildingManager.Reset();
        yield return new WaitForSeconds(.5f);
        missileSpawner.ResumeFiring();
    }

    public int GetScoreMultiplier()
    {
        return scoreMultiplier;
    }
    public void AddScore(int points)
    {
        int earnedPoints = points * scoreMultiplier;
        score += earnedPoints;
        UpdateScore(score);
        alienAnimation.AlienCelebrate();
    }

    private void UpdateScore(int newScore)
    {
        StartCoroutine(LerpScore(newScore));
    }

    private IEnumerator LerpScore(int newScore)
    {
        int startScore = displayedScore;
        float elapsedTime = 0f;

        while (elapsedTime < scoreLerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scoreLerpDuration;
            t = Mathf.SmoothStep(0f, 1f, t);
            displayedScore = Mathf.RoundToInt(Mathf.Lerp(startScore, newScore, t));

            foreach (TMP_Text scoreTextElement in scoreText)
            {
                scoreTextElement.text = "Score: " + displayedScore;
            }

            yield return null;
        }

        displayedScore = newScore;
        foreach (TMP_Text scoreTextElement in scoreText)
        {
            scoreTextElement.text = "Score: " + displayedScore;
        }
    }
}
