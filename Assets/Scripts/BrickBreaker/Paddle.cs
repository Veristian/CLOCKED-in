using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Paddle : MonoBehaviour
{
    // private Rigidbody2D rb;
    // private Vector2 direction;
    [SerializeField] private Camera mainCamera;

    public float speed = 30f;
    public float maxBounceAngle = 75f;

    Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = transform.position;
        if (mainCamera == null)
            mainCamera = FindAnyObjectByType<Camera>();
    }

    private void Start()
    {
        ResetPaddle();
    }

    public void ResetPaddle()
    {
        // rb.velocity = Vector2.zero;
        transform.position = initialPosition;
    }

    // private void Update()
    // {
    //     // if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
    //     //     direction = Vector2.left;
    //     // } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
    //     //     direction = Vector2.right;
    //     // } else {
    //     //     direction = Vector2.zero;
    //     // }
    //     if (InputManager.Instance == null) return;
    //     if (!InputManager.Instance.isTouching) return;
    //     transform.position = Vector2.Lerp(transform.position, new Vector3(mainCamera.ScreenToWorldPoint(InputManager.Instance.touchPosition).x, transform.position.y, Mathf.Infinity), speed * Time.deltaTime);
    // }
    private void Update()
    {
        if (InputManager.Instance == null) return;
        if (!InputManager.Instance.isTouching) return;

        Vector3 screenPos = InputManager.Instance.touchPosition;

        float zDistance = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
        screenPos.z = zDistance;
        
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(worldPos.x, transform.position.y, transform.position.z),
            speed * Time.deltaTime
        );
    }

    // private void FixedUpdate()
    // {
    //     if (direction != Vector2.zero) {
    //         rb.AddForce(direction * speed);
    //     }
    // }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) {
            return;
        }

        Rigidbody2D ball = collision.rigidbody;
        Collider2D paddle = collision.otherCollider;

        // Gather information about the collision
        Vector2 ballDirection = ball.velocity.normalized;
        Vector2 contactDistance = paddle.bounds.center - ball.transform.position;

        // Rotate the direction of the ball based on the contact distance
        // to make the gameplay more dynamic and interesting
        float bounceAngle = (contactDistance.x / paddle.bounds.size.x) * maxBounceAngle;
        ballDirection = Quaternion.AngleAxis(bounceAngle, Vector3.forward) * ballDirection;

        // Re-apply the new direction to the ball
        ball.velocity = ballDirection * ball.velocity.magnitude;
    }

}
