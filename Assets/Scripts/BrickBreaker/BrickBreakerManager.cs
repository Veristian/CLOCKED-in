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

    private void Start()
    {
        FindSceneReferences();
    }
    private void FindSceneReferences()
    {
        ball = FindAnyObjectByType<Ball>();
        paddle = FindAnyObjectByType<Paddle>();
        bricks = FindObjectsByType<Brick>(FindObjectsSortMode.None);
    }

    private void LoadLevel()
    {
        for (int i = 0; i < bricks.Length; i++)
        {
            bricks[i].ResetBrick();
        }
        ball.ResetBall();
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
    }

    private void NewGame()
    {
        score = 0;
        lives = 3;

        LoadLevel();
    }

    public void OnBrickHit(Brick brick)
    {
        score += brick.points;

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

}
