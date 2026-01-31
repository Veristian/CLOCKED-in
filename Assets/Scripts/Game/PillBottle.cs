using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillBottle : MonoBehaviour
{
    [SerializeField] private float localYMaxSanity = 0.5f;
    [SerializeField] private float localYMinSanity = -0.5f;
    [SerializeField] private GameObject[] sanityIndicators;
    [SerializeField] private float timeToUpdate = 3f;

    float pillBottleHeight;
    void UpdateSanity()
    {
        foreach (GameObject indicator in sanityIndicators)
        {
            if (indicator != null)
            {
                float localY = indicator.transform.localPosition.y;

                float currentSanityHeight = (AttributeManager.Instance.Sanity / 100f) * pillBottleHeight;
                if (localY <= localYMinSanity + currentSanityHeight)
                {
                    indicator.SetActive(true);
                }
                else
                {
                    indicator.SetActive(false);
                }
            }
        }
    }

    private void Start()
    {
        pillBottleHeight = localYMaxSanity - localYMinSanity;
        StartCoroutine(SanityUpdater());
    }

    IEnumerator SanityUpdater()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToUpdate);
            UpdateSanity();
        }
    }
}
