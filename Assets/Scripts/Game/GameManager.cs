using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public class GameLevel
    {
        public readonly int levelNumber;
        public int papersToSpawn;
        // public float bossAppearanceRate; //percentage chance of boss appearing this level for every minute

        public GameLevel(int levelNumber, int papersToSpawn)
        {
            this.levelNumber = levelNumber;
            this.papersToSpawn = papersToSpawn;

        }
        
    }

    [Header("Settings")]

    public List<GameLevel> gameLevels = new List<GameLevel>()
    {
        new GameLevel(1, 10),
        new GameLevel(2, 12),
        new GameLevel(3, 14),
        new GameLevel(4, 16),
        new GameLevel(5, 20)
    };
    public float timePerLevelInSeconds = 600f; // 10 minutes per level
    float currentLevelTimeRemaining;
    bool isGameOngoing = false;
    public int currentLevelIndex = 0; //index starts at 0

    [Header("Scene Names")]
    public string winSceneName = "WinScreen";
    public string loseSceneName = "LoseScreen";
    public string gameSceneName = "SampleScene";
    public string intermissionSceneName = "IntermissionScene";

    [Header("References")]
    public TMPro.TextMeshPro timeDisplayText;
    public TMPro.TextMeshPro taskDisplayText;

    protected override void Awake()
    {
        base.Awake();
        GetLevelIndex();
    }
    private void Start()
    {
        SetupGame();
        ResetLevelTimer();
        DisplayTimeRemaining();
        UpdateTaskDisplay();
    }
    void Update()
    {
        CountdownLevelTimer();
        DisplayTimeRemaining();
    }
    #region PlayerPrefs
    //Get current level from player prefs
    public void GetLevelIndex()
    {
        int level = PlayerPrefs.GetInt("CurrentLevel", 1);
        currentLevelIndex = Mathf.Clamp(level - 1, 0, gameLevels.Count - 1);
        
    }
    public void SetLevelIndex(int level)
    {
        currentLevelIndex = Mathf.Clamp(level - 1, 0, gameLevels.Count - 1);
        PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex + 1);
    }

    #endregion


    #region Level Setup

    void SetupGame()
    {
        HRMiniGameManager.Instance.SpawnPaper(gameLevels[currentLevelIndex].papersToSpawn);
        isGameOngoing = true;
    }

    public void CheckWinGame() //checks in submission box when paper is submitted
    {
        if (HRMiniGameManager.Instance.CheckAllSubmissions())
        {
            Debug.Log("GameManager: Player has won Level " + (currentLevelIndex + 1));
            WinCurrentLevel();
        }
        else
        {
            Debug.Log("GameManager: Player has not yet completed all submissions for Level " + (currentLevelIndex + 1));
        }
    }

    void WinCurrentLevel()
    {
        //Play win animation or something here before going to next level
        ProceedToNextScene();
    }

    void ProceedToNextScene()
    {
        Debug.Log("GameManager: Winning Level " + (currentLevelIndex + 1));
        //go to finish screen or next level
        if (currentLevelIndex + 1 >= gameLevels.Count)
        {
            Debug.Log("GameManager: Player has completed all levels. Go to win screen.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(winSceneName);
            return;
        }
        else
        {
            SetLevelIndex(currentLevelIndex + 2); //advance to next level
            UnityEngine.SceneManagement.SceneManager.LoadScene(intermissionSceneName);
        }

    }

    public void LoseGame()
    {
        // SetLevelIndex(1); //reset to one
        Debug.Log("GameManager: Player has lost Level " + (currentLevelIndex + 1));
        UnityEngine.SceneManagement.SceneManager.LoadScene(loseSceneName);
    }
    #endregion

    #region Time
    void ResetLevelTimer()
    {
        currentLevelTimeRemaining = timePerLevelInSeconds;
    }
    void CountdownLevelTimer()
    {
        if (!isGameOngoing) return;
        currentLevelTimeRemaining -= Time.deltaTime;
        if (currentLevelTimeRemaining <= 0)
        {
            LoseGame();
        }
    }
    public string GetFormattedTimeRemaining()
    {
        //returns the string in HH:MM format of time between 9:00 AM to 5:00 PM
        int totalSeconds = Mathf.CeilToInt(timePerLevelInSeconds - currentLevelTimeRemaining) * 8 * 60 * 60 / Mathf.CeilToInt(timePerLevelInSeconds);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        return string.Format("{0:00}:{1:00}", hours + 9, minutes); 
    }

    void DisplayTimeRemaining()
    {
        if (timeDisplayText != null)
        {
            timeDisplayText.text = GetFormattedTimeRemaining();
        }
    }
    #endregion

    #region Task
    public void UpdateTaskDisplay()
    {
        //Update task display UI here
        if (taskDisplayText != null)
        {
            string completedTasks = HRMiniGameManager.Instance.CheckSubmittedCount().ToString();
            string totalTasks = HRMiniGameManager.Instance.allPapers.Count.ToString();
            taskDisplayText.text = completedTasks + "/" + totalTasks;
        }
    }
    #endregion

}
