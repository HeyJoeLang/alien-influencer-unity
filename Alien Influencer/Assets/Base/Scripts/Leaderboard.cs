using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public TMP_Text[] scoreTexts; // UI Text elements to display high scores
    public TMP_Text[] initialTexts; // UI Text elements to display initials
    public GameObject[] highlighters;
    public TMP_Text newHighScoreText; // Text object to display "New High Score: Xth Place"

    public int playerScore;

    private string[] highScoreInitials = new string[10];
    private int[] highScores = new int[10];
    private char[] currentInitials = new char[3] { 'A', 'A', 'A' }; // Default 'AAA'
    private int currentInitialIndex = 0; // Which initial is in focus (0, 1, 2)
    private bool canSubmit = false; // Whether the player is eligible to submit their score

    private int alphabetIndex = 0;
    private char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    void Start()
    {
        LoadScores();
        UpdateUI();
        CheckForNewHighScore();
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Q))
        {
            ResetScores();
        }
        HandleInitialInput();
        if (canSubmit)
        {
            HandleSubmit();
        }
    }

    void HandleInitialInput()
    {
        // Move between initials (left/right)
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            highlighters[currentInitialIndex].SetActive(false);
            currentInitialIndex = (currentInitialIndex + 1) % 3;
            highlighters[currentInitialIndex].SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            highlighters[currentInitialIndex].SetActive(false);
            currentInitialIndex = (currentInitialIndex - 1 + 3) % 3;
            highlighters[currentInitialIndex].SetActive(true);
        }

        // Change the current initial (up/down)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            alphabetIndex = (alphabetIndex + 1) % alphabet.Length;
            currentInitials[currentInitialIndex] = alphabet[alphabetIndex];
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            alphabetIndex = (alphabetIndex - 1 + alphabet.Length) % alphabet.Length;
            currentInitials[currentInitialIndex] = alphabet[alphabetIndex];
        }

        // Update initials UI
        for (int i = 0; i < 3; i++)
        {
            initialTexts[i].text = currentInitials[i].ToString();
        }
    }

    void HandleSubmit()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Simulate joystick button press for submission
        {
            SaveNewScore();
            UpdateUI();
            CheckForNewHighScore();
        }
    }

    void SaveNewScore()
    {
        // Find where to insert the new score
        for (int i = 0; i < highScores.Length; i++)
        {
            if (playerScore > highScores[i])
            {
                // Shift lower scores down
                for (int j = highScores.Length - 1; j > i; j--)
                {
                    highScores[j] = highScores[j - 1];
                    highScoreInitials[j] = highScoreInitials[j - 1];
                }

                // Insert new score
                highScores[i] = playerScore;
                highScoreInitials[i] = new string(currentInitials);
                break;
            }
        }

        // Save to PlayerPrefs
        SaveScores();
        LoadScores();
        UpdateUI();
    }

    void SaveScores()
    {
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetInt("HighScore" + i, highScores[i]);
            PlayerPrefs.SetString("HighScoreInitials" + i, highScoreInitials[i]);
        }
    }

    void LoadScores()
    {
        for (int i = 0; i < 10; i++)
        {
            highScores[i] = PlayerPrefs.GetInt("HighScore" + i, 10 * (i + 1)); // Default scores from 10 to 100
            highScoreInitials[i] = PlayerPrefs.GetString("HighScoreInitials" + i, "AAA");
        }
    }

    void UpdateUI()
    {
        // Update high scores display
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            scoreTexts[i].text = highScoreInitials[i] + " - " + highScores[i];
        }
    }
    void ResetScores()
    {
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetInt("HighScore" + (9 - i), 10 * (i + 1)); // Default scores from 10 to 100
            PlayerPrefs.SetString("HighScoreInitials" + (9 - i), "AAA");
        }
        LoadScores();
        UpdateUI();
    }
        void CheckForNewHighScore()
    {
        // Check where the player score fits in and update the newHighScoreText accordingly
        for (int i = 0; i < highScores.Length; i++)
        {
            if (playerScore > highScores[i])
            {
                newHighScoreText.text = $"New High Score: {Ordinal(i + 1)} Place";
                return;
            }
        }
        newHighScoreText.text = ""; // Clear the message if no new high score
    }

    // Helper method to convert number to ordinal (1st, 2nd, 3rd, etc.)
    string Ordinal(int number)
    {
        if (number <= 0) return number.ToString();

        int lastDigit = number % 10;
        int lastTwoDigits = number % 100;

        if (lastTwoDigits >= 11 && lastTwoDigits <= 13)
        {
            return number + "th";
        }

        switch (lastDigit)
        {
            case 1: return number + "st";
            case 2: return number + "nd";
            case 3: return number + "rd";
            default: return number + "th";
        }
    }
}
