using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AttributeManager : Singleton<AttributeManager>
{
    [Header("Attributes")]
    public float Sanity;
    public float sanityLossPerSecond = 2f;
    public float sanityLossDuringGrace = 0.1f;
    public float sanityLossPerWorkDone = 10f;
    public float WorkProgress;
    public Material sanityEffectMaterial;
    public Slider sanitySlider;


    bool countdownActive = false;
    private void Start()
    {
        Sanity = 100f;
        WorkProgress = 0f;
    }

    void Update()
    {
        UpdateSanityUI();

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
            float intensity = Mathf.InverseLerp(40f, 0f, Sanity);  // 0 at Sanity=40, 1 at Sanity=0
            sanityEffectMaterial.SetFloat("_VignetteIntensity", 1f + intensity);
            sanityEffectMaterial.SetFloat("_BreathSpeed", 2f);
        }
        else
        {
            sanityEffectMaterial.SetFloat("_VignetteIntensity", 1f);
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


    public void UpdateSanityUI()
    {
        if (sanitySlider != null)
        {
            sanitySlider.value = Sanity;
        }
    }
}