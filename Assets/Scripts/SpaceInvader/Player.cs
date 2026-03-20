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
    public Vector3 leftEdge = new Vector3(-30f, 0f, 0f);
    public Vector3 rightEdge = new Vector3(30f, 0f, 0f);
    [Header("Particle Effects")]
    public GameObject BoomEffect;
    Camera mainCamera;
    private void Awake()
    {
        mainCamera = Camera.main ?? FindAnyObjectByType<Camera>();
        screenSize = new Vector2(Screen.currentResolution.height, Screen.currentResolution.width);
    }
    void OnEnable()
    {
        if (InputManager.Instance == null)
            return;
        InputManager.Instance.OnVolDownPerformed += ShootLaser;
        InputManager.Instance.OnVolUpPerformed += ShootLaser;
        InputManager.Instance.OnTouchDownPerformed += CheckShoot;
        InputManager.Instance.EnableJoystick();
    }
    void OnDisable()
    {
        if (InputManager.Instance == null)
            return;
        InputManager.Instance.OnVolDownPerformed -= ShootLaser;
        InputManager.Instance.OnVolUpPerformed -= ShootLaser;
        InputManager.Instance.OnTouchDownPerformed -= CheckShoot;
        InputManager.Instance.DisableJoystick();
    }
    private void Update()
    {
        if (!gameObject.activeSelf)
            return;
        Vector3 position = transform.localPosition;
        if (InputManager.Instance == null)
            return;
        position.x += InputManager.Instance.joystick.Horizontal * speed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, leftEdge.x, rightEdge.x);
        transform.localPosition = position;
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
            laser = Instantiate(laserPrefab, transform.position, Quaternion.identity, transform.parent);
            AudioPooler.Instance.Play(SFX.Blast);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Missile") ||
            other.gameObject.layer == LayerMask.NameToLayer("Invader")) {
            Instantiate(BoomEffect, transform.position, Quaternion.identity, transform.parent);
            SpaceInvadersManager.Instance.OnPlayerKilled(this);
        }
    }
}
