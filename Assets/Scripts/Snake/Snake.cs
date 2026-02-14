using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right;
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public float speedIncreasePerFood = 0.1f;
    public int initialSize = 4;    
    public bool moveThroughWalls = false;
    [SerializeField] private AnimationCurve sanityPerFoodCurve = AnimationCurve.Linear(0f, 5f, 100f, 20f);


    private readonly List<Transform> segments = new List<Transform>();
    private List<Vector2Int> input = new List<Vector2Int>();
    private float nextUpdate;
    private System.Action up, down, left, right;

    [Header("Effects & UI")]
    public ParticleSystem eatEffect;
    public ParticleSystem hitEffect;
    private int score = 0;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI speedText;
    Vector3 initialPosition;

    MinigameSelector minigameSelector;
    private void Start()
    {
        minigameSelector = FindObjectOfType<MinigameSelector>();
        initialPosition = Vector3Int.RoundToInt(this.transform.position);
        if (input == null)
            input = new List<Vector2Int>();
        ResetState();
    }


    private void OnEnable()
    {
        up = () => OnHandleInput(Vector2Int.up);
        down = () => OnHandleInput(Vector2Int.down);
        left = () => OnHandleInput(Vector2Int.right);
        right = () => OnHandleInput(Vector2Int.left);

        InputManager.Instance.OnSwipeUp += up;
        InputManager.Instance.OnSwipeDown += down;
        InputManager.Instance.OnSwipeLeft += left;
        InputManager.Instance.OnSwipeRight += right;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnSwipeUp -= up;
        InputManager.Instance.OnSwipeDown -= down;
        InputManager.Instance.OnSwipeLeft -= left;
        InputManager.Instance.OnSwipeRight -= right;
    }


    // private void OnHandleInput(Vector2Int dir)
    // {
    //     // Only allow turning up or down while moving in the x-axis
    //     if (direction.x != 0f || (input[0] != Vector2Int.up && input[0] != Vector2Int.down))
    //     {
    //         if (dir == Vector2Int.up)
    //         {
    //             input.Add(Vector2Int.up);
    //         }
    //         else if (dir == Vector2Int.down)
    //         {
    //             input.Add(Vector2Int.down);
    //         }
    //     }
    //     // Only allow turning left or right while moving in the y-axis
    //     else if (direction.y != 0f || (input[0] != Vector2Int.left && input[0] != Vector2Int.right))
    //     {
    //         if (dir == Vector2Int.right)
    //         {
    //             input.Add(Vector2Int.right);
    //         }
    //         else if (dir == Vector2Int.left)
    //         {
    //             input.Add(Vector2Int.left);
    //         }
    //     }
    // }
    private void OnHandleInput(Vector2Int dir)
    {
        Vector2Int last = input.Count > 0 ? input[input.Count - 1] : direction;

        if (dir + last == Vector2Int.zero) return; // prevent reversing

        input.Add(dir);
    }


    private void Update()
    {
        // Wait until the next update before proceeding
        if (Time.time < nextUpdate)
        {
            return;
        }

        // Set the new direction based on the input
        if (!(input == null || input.Count == 0))
        {
            if (input[0] != Vector2Int.zero)
            {
                direction = input[0];
                input.RemoveAt(0);
            }
        }


        // Set each segment's position to be the same as the one it follows. We
        // must do this in reverse order so the position is set to the previous
        // position, otherwise they will all be stacked on top of each other.
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector3(x, y, transform.position.z);

        // Set the next update time based on the speed
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    public void Grow()
    {
        speedMultiplier += speedIncreasePerFood;
        Transform segment = Instantiate(segmentPrefab, segments[segments.Count - 1].position, Quaternion.identity, this.transform.parent);
        segment.transform.position = new Vector3(segment.transform.localPosition.x, segment.transform.localPosition.y, 0f);
        segments.Add(segment);
    }

    public void ResetState()
    {
        
        direction = Vector2Int.right;
        transform.position = initialPosition;
        speedMultiplier = 1f;
        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        segments.Clear();
        segments.Add(transform);

        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++)
        {
            Grow();
        }

    }

    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            PlayEatEffect();
            score += 1;
            scoreText.text = "Score: " + score.ToString();
            speedText.text = "Speed: " + speedMultiplier.ToString("F1");
            Grow();
            AttributeManager.Instance.IncreaseSanity(sanityPerFoodCurve.Evaluate(score));
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            score = 0;
            Debug.Log("Hit Obstacle" + other.gameObject.name);
            scoreText.text = "Score: " + score.ToString();
            speedText.text = "Speed: " + speedMultiplier.ToString("F1");
            PlayCrashEffect();
            ResetState();

        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            if (moveThroughWalls)
            {
                Traverse(other.transform);
            }
            else
            {
                score = 0;
                Debug.Log("Hit Wall");
                scoreText.text ="Score: " + score.ToString();
                speedText.text = "Speed: " + speedMultiplier.ToString("F1");
                PlayCrashEffect();
                ResetState();
                minigameSelector.gameExit();

            }
        }
    }

    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (direction.x != 0f)
        {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        }
        else if (direction.y != 0f)
        {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }

        transform.position = position;
    }

    void PlayCrashEffect()
    {
        if (hitEffect == null) return;
        if (hitEffect.isPlaying)
        {
            return;
        }
        hitEffect.transform.position = transform.position;
        hitEffect.Play();
    }

    void PlayEatEffect()
    {
        if (eatEffect == null) return;
        eatEffect.transform.position = transform.position;
        eatEffect.Play();
    }

}
