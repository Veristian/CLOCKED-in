using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject settingMenu;      

    [Header("Settings")]
    [SerializeField] private bool useTimeScale = true;     


    private bool isPaused = false;

    void Awake()
    {
        // Make sure menu starts hidden
        if (settingMenu != null)
        {
            settingMenu.SetActive(false);
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;

        if (useTimeScale)
        {
            Time.timeScale = 0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale; 
        }

        // Show menu
        if (settingMenu != null)
        {
            settingMenu.SetActive(true);
            pauseButton.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;

        // Hide menu
        if (settingMenu != null)
        {
            settingMenu.SetActive(false);
            pauseButton.SetActive(true);
        }

        // Restore time
        if (useTimeScale)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f; // default value
        }

    }

    public void OnResumeButtonClicked()
    {
        ResumeGame();
    }

}
