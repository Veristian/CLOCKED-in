using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Paddle : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public float speed = 30f;
    public float maxBounceAngle = 75f;

    Vector3 initialPosition;

    public BoxCollider2D paddleCollider;
    public BoxCollider2D leftLimit;
    public BoxCollider2D rightLimit;

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
        transform.position = initialPosition;
    }


    private void Update()
    {
        if (InputManager.Instance == null) return;
        if (!InputManager.Instance.isTouching) return;

        Vector3 screenPos = InputManager.Instance.touchPosition;

        float zDistance = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
        screenPos.z = zDistance;
        
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        Vector3 targetPos = Vector3.Lerp(
            transform.position,
            new Vector3(worldPos.x, transform.position.y, transform.position.z),
            speed * Time.deltaTime
        );

        float leftX = leftLimit.transform.position.x + leftLimit.bounds.size.x / 2f + paddleCollider.bounds.size.x / 2;
        float rightX = rightLimit.transform.position.x - rightLimit.bounds.size.x / 2f - paddleCollider.bounds.size.x / 2;

        targetPos.x = Mathf.Clamp(targetPos.x, leftX, rightX);

        transform.position = targetPos;
    }


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
