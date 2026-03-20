using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 10f;

    [Header("Particle Effects")]
    public ParticleSystem BlastEffect;
    public ParticleSystem BlastEffect_2;
    public ParticleSystemRenderer blastRenderer;
    public ParticleSystemRenderer blastRenderer_2;
    public Material[] BlastMaterial = new Material[0];

    Vector3 initialPosition;
    Vector3 currentVelocity;
    private void Awake()
    {
        initialPosition = transform.position;
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    public void ResetBall()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ResetBallCoroutine());
        }
    }
    public IEnumerator ResetBallCoroutine()
    {
        rb.velocity = Vector3.zero;
        transform.position = initialPosition;
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(OnPlayerReady);
        Debug.Log("Player is ready, launching ball...");
        Vector3 force = new Vector3(Random.Range(-1f, 1f), -1f, 0f);
        rb.AddForce(force.normalized * speed, ForceMode2D.Impulse);
    }


    private bool OnPlayerReady()
    {
        if (InputManager.Instance == null) return false;
        if (!InputManager.Instance.isTouching) return false;
        return true;
    }

    void OnEnable()
    {
        rb.velocity = currentVelocity;

        if (rb.velocity == Vector2.zero)
        {
            ResetBall();
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = rb.velocity.normalized * speed;
        currentVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioPooler.Instance.Play(SFX.BallBounce);
        if (collision.gameObject.name == "Brick")
        {
            Brick brick = collision.gameObject.GetComponent<Brick>();
            if (brick != null)
            {
                int brickHealth = brick.GetHealth();
                if (brickHealth > 0)
                {
                    blastRenderer.material = BlastMaterial[brickHealth];
                    blastRenderer_2.material = BlastMaterial[brickHealth];
                }

            }
            PlayBlastEffect();
        }
    }

    void PlayBlastEffect()
    {
        if (BlastEffect == null) return;

        BlastEffect.transform.position = transform.position;
        BlastEffect.Play();
        
        if (BlastEffect_2 == null) return;
        BlastEffect_2.transform.position = transform.position;
        BlastEffect_2.Play();
    }

}
