using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
public class MysteryShip : MonoBehaviour
{
    public float speed = 5f;
    public float cycleTime = 30f;
    public int score = 300;
    
    public Vector3 leftDestination;
    public Vector3 rightDestination;
    private int direction = -1;
    private bool spawned;

    [Header("Particle Effects")]
    public GameObject BoomEffect;
    Camera mainCamera;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    private void Start()
    {
        // mainCamera = Camera.main ?? FindAnyObjectByType<Camera>();

        // // Transform the viewport to world coordinates so we can set the mystery
        // // ship's destination points
        // Vector3 leftEdge = mainCamera.ViewportToWorldPoint(Vector3.zero);
        // Vector3 rightEdge = mainCamera.ViewportToWorldPoint(Vector3.right);

        // // Offset each destination by 1 unit so the ship is fully out of sight
        // leftDestination = new Vector3(leftEdge.x - 1f, transform.position.y, transform.position.z);
        // rightDestination = new Vector3(rightEdge.x + 1f, transform.position.y, transform.position.z);
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        Despawn();
    }

    private void Update()
    {
        if (!spawned) return;

        if (direction == 1) {
            MoveRight();
        } else {
            MoveLeft();
        }
    }

    private void MoveRight()
    {
        transform.position += speed * Time.deltaTime * Vector3.right;

        if (transform.position.x >= rightDestination.x) {
            Despawn();
        }
    }

    private void MoveLeft()
    {
        transform.position += speed * Time.deltaTime * Vector3.left;

        if (transform.position.x <= leftDestination.x) {
            Despawn();
        }
    }

    private void Spawn()
    {
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;
        direction *= -1;

        if (direction == 1) {
            transform.position = leftDestination;
        } else {
            transform.position = rightDestination;
        }
        
        spawned = true;
    }

    private void Despawn()
    {
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;
        spawned = false;

        if (direction == 1) {
            transform.position = rightDestination;
        } else {
            transform.position = leftDestination;
        }

        Invoke(nameof(Spawn), cycleTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            Instantiate(BoomEffect, transform.position, Quaternion.identity, transform.parent);
            Despawn();
            SpaceInvadersManager.Instance.OnMysteryShipKilled(this);

        }
    }

}
