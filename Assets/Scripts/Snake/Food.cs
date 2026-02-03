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

        int minX = Mathf.FloorToInt(bounds.min.x);
        int maxX = Mathf.CeilToInt(bounds.max.x) - 1;

        int minY = Mathf.FloorToInt(bounds.min.y);
        int maxY = Mathf.CeilToInt(bounds.max.y) - 1;

        int x, y;

        int counter = 0;

        do
        {
            x = Random.Range(minX, maxX + 1); // int version (safe)
            y = Random.Range(minY, maxY + 1);

            counter++;

            if (counter > 50)
            {
                Debug.LogWarning("No valid food position found.");
                break;
            }

        } while (
            snake.Occupies(x, y) ||
            Physics2D.OverlapCircle(new Vector3(x, y, transform.position.z), 0.4f, foodLayer) != null
        );

        transform.position = new Vector3(x, y, transform.position.z);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        RandomizePosition();
    }
    

}
