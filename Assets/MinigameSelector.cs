using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameSelector : MonoBehaviour
{
    public GameObject snake;
    public GameObject brickBreaker;
    public GameObject galaga;
    public  GameObject[] button;
    Ball Ball;
    // Start is called before the first frame update

    private void Start()
    {
        gameExit();
        Ball = FindObjectOfType<Ball>();
    }
    public void OpenSnake ()
    {
        if (snake != null)
        {
            snake.SetActive(true);

            foreach (GameObject btn in button)
                btn.SetActive(false);
        }
    }

    public void OpenBrickBreaker()
    {
        if (brickBreaker != null)
        {
            brickBreaker.SetActive(true);

            foreach (GameObject btn in button)
                btn.SetActive(false);
            Invoke(nameof(BrickBreakerNewGame), 0.5f);
        }
    }

    public void OpenGalaga()
    {
        if (galaga != null)
        {
            galaga.SetActive(true);

            foreach (GameObject btn in button)
                btn.SetActive(false);
        }
    }

    public void gameExit()
    {
        foreach (GameObject btn in button)
            btn.SetActive(true);
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
