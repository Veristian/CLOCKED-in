using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public float speed = 5f;
    public Projectile laserPrefab;
    private Projectile laser;
    Vector2 screenSize;

    [Header("Particle Effects")]
    public GameObject BoomEffect;


    private void Awake()
    {
        screenSize = new Vector2(Screen.currentResolution.height, Screen.currentResolution.width);
    }

    void OnEnable()
    {
        if (InputManager.Instance == null)
            return;
        InputManager.Instance.OnVolDownPerformed += ShootLaser;
        InputManager.Instance.OnVolUpPerformed += ShootLaser;
        InputManager.Instance.OnTouchDownPerformed += CheckShoot;

    }

    void OnDisable()
    {
        if (InputManager.Instance == null)
            return;
        InputManager.Instance.OnVolDownPerformed -= ShootLaser;
        InputManager.Instance.OnVolUpPerformed -= ShootLaser;
        InputManager.Instance.OnTouchDownPerformed -= CheckShoot;

    }
    private void Update()
    {
        if (!gameObject.activeSelf)
            return;
        Vector3 position = transform.position;
        if (InputManager.Instance == null)
            return;

        // Update the position of the player based on the input
        if (InputManager.Instance.touchPosition.x < screenSize.y/2 && InputManager.Instance.touchPosition.y < screenSize.x/2 && InputManager.Instance.isTouching) {
            position.x -= speed * Time.deltaTime;
        } else if (InputManager.Instance.touchPosition.x > screenSize.y/2 && InputManager.Instance.touchPosition.y < screenSize.x/2 && InputManager.Instance.isTouching) {
            position.x += speed * Time.deltaTime;
        }


        // Clamp the position of the character so they do not go out of bounds
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        position.x = Mathf.Clamp(position.x, leftEdge.x, rightEdge.x);

        // Set the new position
        transform.position = position;

        // Only one laser can be active at a given time so first check that
        // there is not already an active laser
        
    
    }

    public void CheckShoot()
    {
        if (InputManager.Instance.touchPosition.y < screenSize.x/2) 
            return; 
        ShootLaser();
    }
    public void ShootLaser()
    {
        if (laser == null) {
            laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Missile") ||
            other.gameObject.layer == LayerMask.NameToLayer("Invader")) {
            Instantiate(BoomEffect, transform.position, Quaternion.identity);
            SpaceInvadersManager.Instance.OnPlayerKilled(this);
        }
    }

}
