using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AttributeManager : Singleton<AttributeManager>
{
    [Header("Attributes")]
    public float Sanity;
    public float WorkProgress;


    bool countdownActive = false;
    private void Start()
    {
        Sanity = 100f;
        WorkProgress = 0f;
    }

    void Update()
    {
        // Sanity -= Time.deltaTime * 2f; //temp
        //if (!GameManager.Instance.gameActive) return;
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
