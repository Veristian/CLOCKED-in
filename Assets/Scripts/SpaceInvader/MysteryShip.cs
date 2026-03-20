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
