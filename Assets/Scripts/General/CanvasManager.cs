using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Singleton<CanvasManager>
{
    [Header("References")]
    [SerializeField] private Cinemachine.CinemachineVirtualCamera camera1;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera camera2;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera camera3;

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
    // void Update()
    // {
    //     if (Vector3.Distance(PlayAreaTransform.localPosition, targetPosition) <= snapDistance)
    //     {
    //         PlayAreaTransform.localPosition = targetPosition;
    //     }
    //     else
    //     {
    //         PlayAreaTransform.localPosition = Vector3.Lerp(PlayAreaTransform.localPosition, targetPosition, 0.1f);
    //     }

    // }
    public void MoveToSector(int sector)
    {
        switch (sector)
        {
            case 1:
                ActivateCamera(1);
                break;
            case 2:
                ActivateCamera(2);
                break;
            case 3:
                ActivateCamera(3);
                break;
            default:
                Debug.LogError("Invalid sector number");
                return;
        }

    }

    public void ActivateCamera(int cameraNumber)
    {
        camera1.gameObject.SetActive(cameraNumber == 1);
        camera2.gameObject.SetActive(cameraNumber == 2);
        camera3.gameObject.SetActive(cameraNumber == 3);
    }
}
