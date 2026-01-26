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
    // bool IsReady = false;
    // TaskCompletionSource<bool> resetCompletionSource;

    private void Awake()
    {
        initialPosition = transform.position;
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    // void OnEnable()
    // {
    //     if (InputManager.Instance == null) return;
    //     InputManager.Instance.OnTouchDownPerformed += ReadyReset;
    //     InputManager.Instance.OnTouchUpPerformed += CancelReset;
    // }
    // void OnDisable()
    // {
    //     if (InputManager.Instance == null) return;
    //     InputManager.Instance.OnTouchDownPerformed -= ReadyReset;
    //     InputManager.Instance.OnTouchUpPerformed -= CancelReset;
    // }

    private void Start()
    {
        ResetBall();
    }

    public void ResetBall()
    {
        StartCoroutine(ResetBallCoroutine());
    }
    public IEnumerator ResetBallCoroutine()
    {
        rb.velocity = Vector3.zero;
        transform.position = initialPosition;
        yield return new WaitForSeconds(0.5f);
        // await Task.Delay(1000); // small delay to avoid immediate launch
        yield return new WaitUntil(OnPlayerReady);
        // create a new waiter each reset
        // IsReady = true;
        // resetCompletionSource = new TaskCompletionSource<bool>();
        // await resetCompletionSource.Task;
        Vector3 force = new Vector3(Random.Range(-1f, 1f), -1f, 0f);
        rb.AddForce(force.normalized * speed, ForceMode2D.Impulse);
    }

    

    // called by InputManager or UI
    // public void ReadyReset()
    // {
    //     if (resetCompletionSource != null && !resetCompletionSource.Task.IsCompleted)
    //         resetCompletionSource.SetResult(true);
    // }

    // // optional: cancel instead of "not ready"
    // public void CancelReset()
    // {
    //     if (resetCompletionSource != null && !resetCompletionSource.Task.IsCompleted)
    //         resetCompletionSource.SetCanceled();
    // }

    private bool OnPlayerReady()
    {
        if (InputManager.Instance == null) return false;
        if (!InputManager.Instance.isTouching) return false;
        return true;
    }

    private void FixedUpdate()
    {
        rb.velocity = rb.velocity.normalized * speed;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Brick")
        {
            Brick brick = collision.gameObject.GetComponent<Brick>();
            if (brick != null)
            {
                int brickHealth = brick.GetHealth();
                blastRenderer.material = BlastMaterial[brickHealth ];
                blastRenderer_2.material = BlastMaterial[brickHealth ];

            }
            PlayBlastEffect();
        }
    }

    void PlayBlastEffect()
    {
        if (BlastEffect == null) return;
      //  if (BlastEffect.isPlaying)
       // {
      //      return;
        //}
        BlastEffect.transform.position = transform.position;
        BlastEffect.Play();

        if (BlastEffect_2 == null) return;
        //if (BlastEffect_2.isPlaying)
       // {
        //    return;
        //}
        BlastEffect_2.transform.position = transform.position;
        BlastEffect_2.Play();
    }

}
