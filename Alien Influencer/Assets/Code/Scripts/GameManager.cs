using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public GameObject gameOverMenu;
    public GameObject winMenu;
    public GameObject gameplayHUD;
    public TMP_Text[] scoreText;
    public TMP_Text timeLeftText;
    private float timeRemaining = 241f;
    private int score = 0;
    int minutes = 0;
    int seconds = 0;
    public float UFOHeight = 8;
    public TMP_Text followerCount;
    private bool isCountingDown = false;
    public EventSystem eventSystem;
    public Button PlayAgainButton;

    UFOLaser ufoLaser;
    UfoSuction ufoSuction;
    UfoMovement ufoMovement;


    private void Start()
    {
        ufoLaser = FindObjectOfType<UFOLaser>().GetComponent<UFOLaser>();
        ufoSuction = FindObjectOfType<UfoSuction>().GetComponent<UfoSuction>();
        ufoMovement = FindObjectOfType<UfoMovement>().GetComponent<UfoMovement>();
        timeRemaining = 241f;
        MinionPlacement.Reset();
        Debug.Log("Starting Game...");
        StartGame();
    }

    public void StartGame()
    {
        gameOverMenu.SetActive(false);
        winMenu.SetActive(false);
        score = 0;
        UpdateScore(0);
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        ufoLaser.enabled = false;
        ufoSuction.enabled = false;
        ufoMovement.enabled = false;
        gameplayHUD.SetActive(false);
        gameOverMenu.SetActive(true);
        Time.timeScale = 0f;
        eventSystem.SetSelectedGameObject(PlayAgainButton.gameObject);
    }

    public void WinGame()
    {
        winMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    //Called From Intro Timeline Signal
    public void StartTimer()
    {
        isCountingDown = true;
    }

    private void FixedUpdate()
    {
        followerCount.text = "Followers: " + MinionPlacement.minionCount;
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

    public void AddScore(int points)
    {
        score += points;
        UpdateScore(score);
    }

    private void UpdateScore(int newScore)
    {
        foreach (TMP_Text scoreText in scoreText)
        {
            scoreText.text = "Score: " + newScore;
        }
    }
}
