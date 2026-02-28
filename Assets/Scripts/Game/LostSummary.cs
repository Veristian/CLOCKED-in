using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostSummary : MonoBehaviour
{
    public TMPro.TextMeshProUGUI lostSummaryText;

    void Start()
    {
        LostSummaryScreen();
    }
    public void LostSummaryScreen()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        lostSummaryText.text = currentLevel.ToString();
    }

    public void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TestMenu");
    }
}
