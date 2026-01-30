using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ResetZone : MonoBehaviour
{
    [Header("Particle Effects")]
    public GameObject Ball;
    public ParticleSystem BlastEffect;
    public ParticleSystem BlastEffect_2;
    public ParticleSystemRenderer blastRenderer;
    public ParticleSystemRenderer blastRenderer_2;
    public Material BlastMaterial;
    private void OnTriggerEnter2D(Collider2D other)
    {
        blastRenderer.material = BlastMaterial;
        blastRenderer_2.material = BlastMaterial;
        PlayBlastEffect();
        BrickBreakerManager.Instance.OnBallMiss();
    }
    void PlayBlastEffect()
    {
        if (BlastEffect == null) return;
        if (BlastEffect.isPlaying)
        {
            return;
        }
        BlastEffect.transform.position = Ball.transform.position;
        BlastEffect.Play();

        if (BlastEffect_2 == null) return;
        if (BlastEffect_2.isPlaying)
        {
            return;
        }
        BlastEffect_2.transform.position = Ball.transform.position;
        BlastEffect_2.Play();
    }
}
