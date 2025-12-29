using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI attitudeText;


    public static PlayerInput playerInput;
    private static UnityEngine.InputSystem.Gyroscope gyro;
    public static Vector3 deviceRotation;
        
    public static InputAction _gyroAction;
    // private UnityEngine.InputSystem.Gyroscope gyro;

    private void OnValidate()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
    }
    private void Awake()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
        playerInput = GetComponent<PlayerInput>();
        _gyroAction = playerInput.actions["Gyro"];
    }

    void Start()
    {
        // Enable the gyroscope if available
        gyro = UnityEngine.InputSystem.Gyroscope.current;
        if (gyro != null)
        {
            InputSystem.EnableDevice(gyro);
        }
        else
        {
            Debug.LogWarning("No gyroscope found on this device.");
        }
    }

    void Update()
    {
        if (gyro == null)
        {
            attitudeText.text = "Gyroscope not available.";
            return;
        }
        

        // Read the device rotation
        Quaternion attitude = _gyroAction.ReadValue<Quaternion>();

        // Convert from right-handed device space to Unity's left-handed coordinates
        Quaternion unityAttitude = new Quaternion(attitude.x, attitude.y, -attitude.z, -attitude.w);

        // Convert to Euler angles
        Vector3 euler = unityAttitude.eulerAngles;
        deviceRotation = euler;
        // Display in TextMeshPro
        if (attitudeText != null)
        {
            attitudeText.text = $"Device Rotation:\nX: {euler.x:F1}\nY: {euler.y:F1}\nZ: {euler.z:F1}";
        }
    }
}
