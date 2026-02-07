using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AttributeManager : Singleton<AttributeManager>
{
    [Header("Attributes")]
    public float Sanity;
    public float sanityLossPerSecond = 2f;
    public float sanityLossDuringGrace = 0.1f;
    public float sanityLossPerWorkDone = 10f;
    public float WorkProgress;
    public Material sanityEffectMaterial;


    bool countdownActive = false;
    private void Start()
    {
        Sanity = 100f;
        WorkProgress = 0f;
    }

    void Update()
    {

        if (Sanity <= 0 && !countdownActive)
        {
            StartSanityCountdown();
        }
        else
        {

            if (Sanity <= 0)
            {
                Sanity -= Time.deltaTime * sanityLossDuringGrace;
            }
            else
            {
                Sanity -= Time.deltaTime * sanityLossPerSecond;
            }
        }

        if (Sanity <= 40f)
        {
            float intensity =2f * Mathf.InverseLerp(0f, 40f, Sanity);
            sanityEffectMaterial.SetFloat("_BreathSpeed", 2f - intensity);
        }
        else
        {
            sanityEffectMaterial.SetFloat("_BreathSpeed", 0f);
        }
    }

    public void StartSanityCountdown() //call this when sanity reaches 0
    {
        if (!countdownActive)
        {
            _ = SanityCountDown(5f);
        }
    }

    public async Task SanityCountDown(float delay)
    {
        countdownActive = true;
        Debug.Log("Sanity is 0");
        await Task.Delay(Mathf.RoundToInt(delay * 1000));
        if (Sanity <= 0.01f)
        {
            Debug.Log("Game Over due to sanity");
            countdownActive = false;
            // GameManager.Instance.GameOver();
        }
        else
        {
            countdownActive = false;
            Debug.Log("Sanity restored above 0, cancelling Game Over");
        }
    }

    public void DecreaseSanity(float amount)
    {
        Sanity -= amount;
        Sanity = Mathf.Clamp(Sanity, 0f, 100f);
    }

    public void IncreaseSanity(float amount)
    {
        Sanity += amount;
        Sanity = Mathf.Clamp(Sanity, 0f, 100f);
    }

    public void SetWorkProgress(float amount)
    {
        WorkProgress = amount;
        WorkProgress = Mathf.Clamp(WorkProgress, 0f, 100f);
    }
}
