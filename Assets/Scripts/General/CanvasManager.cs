using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Singleton<CanvasManager>
{
    [Header("References")]
    [SerializeField] private RectTransform PlayAreaTransform;
    [Header("Sector Transforms")]
    [SerializeField] private Vector3 Sector1Transform;
    [SerializeField] private Vector3 Sector2Transform;
    [SerializeField] private Vector3 Sector3Transform;

    [Header("Settings")]
    [SerializeField] private float snapDistance = 0.1f;
    private Vector3 targetPosition = Vector3.zero;

    void Start()
    {
        MoveToSector(2); // Start at sector 2
    }
    void Update()
    {
        if (Vector3.Distance(PlayAreaTransform.localPosition, targetPosition) <= snapDistance)
        {
            PlayAreaTransform.localPosition = targetPosition;
        }
        else
        {
            PlayAreaTransform.localPosition = Vector3.Lerp(PlayAreaTransform.localPosition, targetPosition, 0.1f);
        }

    }
    public void MoveToSector(int sector)
    {
        switch (sector)
        {
            case 1:
                targetPosition = Sector1Transform;
                break;
            case 2:
                targetPosition = Sector2Transform;
                break;
            case 3:
                targetPosition = Sector3Transform;
                break;
            default:
                Debug.LogError("Invalid sector number");
                return;
        }

    }
}
