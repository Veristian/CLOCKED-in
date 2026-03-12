using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MinigameSelector : Singleton<MinigameSelector>
{
    public GameObject minigameSelectorUI;
    public GameObject workImage;
    public GameObject snake;
    public GameObject brickBreaker;
    public GameObject galaga;
    public GameObject[] button;
    public GameObject[] blackButton;

    private List<int> buttonOrder;
    private int currentIndex = 0;
    Ball Ball;
    // Start is called before the first frame update

    private void Start()
    {
        gameExit();
        Ball = FindObjectOfType<Ball>();
    }
    public void OpenMinigameSelector()
    {
        if (minigameSelectorUI != null && workImage != null)
        {
            workImage.SetActive(false);

            foreach (GameObject btn in blackButton)
                btn.SetActive(true);

            // Initialize the shuffled order only once, on first run
            if (buttonOrder == null)
            {
                buttonOrder = new List<int> { 0, 1, 2 };
                buttonOrder = buttonOrder.OrderBy(x => UnityEngine.Random.value).ToList();
            }

            int buttonIndex = buttonOrder[currentIndex];
            currentIndex = (currentIndex + 1) % 3;

            if (button != null)
            {
                button[buttonIndex].SetActive(true);
            }
        }

    }
    public void CloseMinigameSelector()
    {
        if (minigameSelectorUI != null && workImage != null)
        {
            foreach (GameObject btn in button)
                btn.SetActive(false);
            foreach (GameObject btn in blackButton)
                btn.SetActive(false);
            workImage.SetActive(true);
        }
    }
    public void OpenSnake()
    {
        AudioPooler.Instance.Play(SFX.Select);
        if (snake != null)
        {
            snake.SetActive(true);

            foreach (GameObject btn in button)
                btn.SetActive(false);
            foreach (GameObject btn in blackButton)
                btn.SetActive(false);
        }
    }

    public void OpenBrickBreaker()
    {
        AudioPooler.Instance.Play(SFX.Select);
        if (brickBreaker != null)
        {
            brickBreaker.SetActive(true);

            foreach (GameObject btn in button)
                btn.SetActive(false);
            foreach (GameObject btn in blackButton)
                btn.SetActive(false);
            Invoke(nameof(BrickBreakerNewGame), 0.5f);
        }
    }

    public void OpenGalaga()
    {
        AudioPooler.Instance.Play(SFX.Select);
        if (galaga != null)
        {
            galaga.SetActive(true);

            foreach (GameObject btn in button)
                btn.SetActive(false);
            foreach (GameObject btn in blackButton)
                btn.SetActive(false);
        }
    }

    public void gameExit()
    {
        foreach (GameObject btn in button)
            btn.SetActive(false);
        foreach (GameObject btn in blackButton)
            btn.SetActive(false);
        if (snake != null)
            snake.SetActive(false);
        if (brickBreaker != null)
            brickBreaker.SetActive(false);
        if (galaga != null)
            galaga.SetActive(false);
    }

    private void BrickBreakerNewGame()
    {
        BrickBreakerManager.Instance.NewGame();
    }
}