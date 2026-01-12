using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Food : MonoBehaviour
{
    public Collider2D gridArea;
    private Snake snake;

    [SerializeField] private LayerMask foodLayer = new LayerMask();

    private void Awake()
    {
        snake = FindAnyObjectByType<Snake>();
    }

    private void Start()
    {
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        Bounds bounds = gridArea.bounds;

        // Pick a random position inside the bounds
        // Round the values to ensure it aligns with the grid
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));
        int counter = 0;
        // Prevent the food from spawning on the snake and near other food
        while (snake.Occupies(x, y) || Physics2D.OverlapCircle(new Vector2(x, y), 0.4f, foodLayer) != null)
        {
            counter++;
            if (counter > 10)
            {
                Debug.LogWarning("Could not find a valid position for the food after 10 attempts. Placing it at the last tried position.");
                break;
            }
            x++;

            if (x > bounds.max.x)
            {
                x = Mathf.RoundToInt(bounds.min.x);
                y++;

                if (y > bounds.max.y) {
                    y = Mathf.RoundToInt(bounds.min.y);
                }
            }
            if (counter > 1)
                Debug.Log("Trying new position for food: " + x + ", " + y);
        }

        transform.position = new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RandomizePosition();
    }
    

}
