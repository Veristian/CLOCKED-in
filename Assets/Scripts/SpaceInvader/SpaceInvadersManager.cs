using System;
using UnityEngine;
using UnityEngine.UI;

public class SpaceInvadersManager : Singleton<SpaceInvadersManager>
{

    [SerializeField] private GameObject gameOverUI;
    // [SerializeField] private GameObject shootButton;

    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;

    private Player player;
    private Invaders invaders;
    private MysteryShip mysteryShip;
    private Bunker[] bunkers;

    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 3;

    Transform initialPlayerPosition;

    MinigameSelector minigameSelector;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        invaders = FindObjectOfType<Invaders>();
        mysteryShip = FindObjectOfType<MysteryShip>();
        bunkers = FindObjectsOfType<Bunker>();
        minigameSelector = FindObjectOfType<MinigameSelector>();
        initialPlayerPosition = player.transform;

        NewGame();
    }

    void OnEnable()
    {
        if (InputManager.Instance == null)
            return;
        InputManager.Instance.OnTouchDownPerformed += RestartGame;
    }
    void OnDisable()
    {
        if (InputManager.Instance == null)
            return;
        InputManager.Instance.OnTouchDownPerformed -= RestartGame;
    }


    void RestartGame()
    {
        if (lives <= 0) {
            minigameSelector.gameExit();
            NewGame();

        }
    }

    private void NewGame()
    {
        gameOverUI.SetActive(false);
        // shootButton.SetActive(true);
        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        invaders.ResetInvaders();
        invaders.gameObject.SetActive(true);

        for (int i = 0; i < bunkers.Length; i++) {
            bunkers[i].ResetBunker();
        }

        Respawn();
    }

    private void Respawn()
    {
        Vector3 position = player.transform.position;
        position.x = initialPlayerPosition.position.x;
        player.transform.position = position;
        player.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        // shootButton.SetActive(false);
        gameOverUI.SetActive(true);
        invaders.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(4, '0');
    }

    private void SetLives(int lives)
    {
        this.lives = Mathf.Max(lives, 0);
        livesText.text = this.lives.ToString();
    }

    public void OnPlayerKilled(Player player)
    {
        SetLives(lives - 1);

        player.gameObject.SetActive(false);

        if (lives > 0) {
            Invoke(nameof(NewRound), 1f);
        } else {
            GameOver();
        }
    }

    public void OnInvaderKilled(Invader invader)
    {
        invader.gameObject.SetActive(false);

        SetScore(score + invader.score);

        if (invaders.GetAliveCount() == 0) {
            NewRound();
        }
    }

    public void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        SetScore(score + mysteryShip.score);
    }

    public void OnBoundaryReached()
    {
        if (invaders.gameObject.activeSelf)
        {
            invaders.gameObject.SetActive(false);
            OnPlayerKilled(player);
        }
    }

}
