using System.Collections;
HEAD
Updated upstream
Updated upstream
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Invader : MonoBehaviour
{
    public Sprite[] animationSprites = new Sprite[0];
    public float animationTime = 1f;
    public int score = 10;

    private SpriteRenderer spriteRenderer;
    private int animationFrame;

    [Header("Particle Effects")]
    public GameObject BoomEffect;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = animationSprites[0];
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), animationTime, animationTime);
    }

    private void AnimateSprite()
    {
        animationFrame++;

        // Loop back to the start if the animation frame exceeds the length
        if (animationFrame >= animationSprites.Length) {
            animationFrame = 0;
        }

        spriteRenderer.sprite = animationSprites[animationFrame];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser")) {
            //StartCoroutine(PlayBoomEffect());
             Instantiate(BoomEffect, transform.position, Quaternion.identity);
   
            SpaceInvadersManager.Instance.OnInvaderKilled(this);
        } else if (other.gameObject.layer == LayerMask.NameToLayer("Boundary")) {
            SpaceInvadersManager.Instance.OnBoundaryReached();
        }
    }

    //IEnumerator PlayBoomEffect()
    //{
    //    Instantiate(BoomEffect, transform.position, Quaternion.identity);
    //    yield return new WaitForSeconds(0.5f);
    //    Destroy(BoomEffect);

    //}
    

}
