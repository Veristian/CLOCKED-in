using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intermission : MonoBehaviour
{
    public Image[] intermissionImages; 
    public TMPro.TextMeshProUGUI intermissionText;
    public Sprite finishSprite;
    public Sprite currentDaySprite;

    private void Start()
    {
        UpdateIntermissionDisplay();
        StartCoroutine(SwicthSceneAfterDelay(5f)); // Change scene after 5 seconds
    }

    private void UpdateIntermissionDisplay()
    {
        int level = PlayerPrefs.GetInt("CurrentLevel", 1);
        int currentDay = Mathf.Clamp(level, 0, 6);
        intermissionText.text = $"Day {currentDay + 24}";
        for (int i = 0; i < intermissionImages.Length; i++)
        {
            if (i < currentDay - 1)
            {
                intermissionImages[i].enabled = true;
                intermissionImages[i].sprite = finishSprite;
            }
            else if (i == currentDay - 1)
            {
                intermissionImages[i].enabled = true;
                intermissionImages[i].sprite = currentDaySprite;
            }
            else
            {
                intermissionImages[i].enabled = false;
                intermissionImages[i].sprite = null; // or a default sprite for future days
            }
        }
    }

    IEnumerator SwicthSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("SampleScene");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("TestMenu");
    }
}
