using UnityEngine;
using UnityEngine.SceneManagement;

public class BrickBreakerManager : Singleton<BrickBreakerManager>   
{

    private Ball ball;
    private Paddle paddle;
    private Brick[] bricks;

    public int level { get; private set; } = 1;
    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 3;

    MinigameSelector minigameSelector;

    [SerializeField] private AnimationCurve sanityPerScoreCurve = AnimationCurve.Linear(0f, 1f, 5000f, 50f);

    [Header("UI Elements")]
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI livesText;

    private void Start()
    {
        FindSceneReferences();
    }
    private void Update()
    {
        livesText.text = "Lives: " + lives.ToString();
    }
    private void FindSceneReferences()
    {
        ball = FindAnyObjectByType<Ball>();
        paddle = FindAnyObjectByType<Paddle>();
        bricks = FindObjectsByType<Brick>(FindObjectsSortMode.None);
        minigameSelector = FindObjectOfType<MinigameSelector>();
    }

    private void LoadLevel()
    {
        for (int i = 0; i < bricks.Length; i++)
        {
            bricks[i].ResetBrick();
        }
        // ball.ResetBall();

    }

    // private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     SceneManager.sceneLoaded -= OnLevelLoaded;
    //     FindSceneReferences();
    // }

    public void OnBallMiss()
    {
        lives--;

        if (lives > 0) {
            ResetLevel();
        } else {
            GameOver();
        }
    }

    private void ResetLevel()
    {
        paddle.ResetPaddle();
        ball.ResetBall();
    }

    private void GameOver()
    {

        NewGame();
        Invoke(nameof(ExitMinigame), 0.5f);

    }

    public void NewGame()
    {
        score = 0;
        lives = 3;

        LoadLevel();
    }

    public void OnBrickHit(Brick brick)
    {
        AttributeManager.Instance.IncreaseSanity(sanityPerScoreCurve.Evaluate(brick.points));
        score += brick.points;
        scoreText.text = "Score: " + score.ToString();

        if (Cleared()) {
            LoadLevel();
        }
    }

    private bool Cleared()
    {
        for (int i = 0; i < bricks.Length; i++)
        {
            if (bricks[i].gameObject.activeInHierarchy && !bricks[i].unbreakable) {
                return false;
            }
        }

        return true;
    }

    private void ExitMinigame()
    {
        if (minigameSelector != null)
        {
            minigameSelector.gameExit();
        }
    }
}
