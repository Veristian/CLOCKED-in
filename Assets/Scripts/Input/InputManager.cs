using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public TextMeshProUGUI attitudeText;

    public static PlayerInput playerInput;
    private static UnityEngine.InputSystem.Gyroscope gyro;
    public static Vector3 deviceRotation;

    public static InputAction _gyroAction;



    private void OnEnable()
    {
        if (_gyroAction != null)
            _gyroAction.Enable();
    }

    private void OnDisable()
    {
        if (_gyroAction != null)
            _gyroAction.Disable();
    }
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        _gyroAction = playerInput.actions["Gyro"];

        gyro = UnityEngine.InputSystem.Gyroscope.current;
        if (UnityEngine.InputSystem.Gyroscope.current != null)
        {
            InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
        }
        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
        }
    }

    void Update()
    {
        if (gyro == null)
        {
            attitudeText.text = "Gyroscope not available.";
            return;
        }
        Vector3 angularVelocity = UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue();
        Quaternion attitude = AttitudeSensor.current.attitude.ReadValue();

        // Quaternion attitude = _gyroAction.ReadValue<Quaternion>();

        // Device → Unity coordinate conversion
        Quaternion unityAttitude = new Quaternion(
            attitude.x,
            attitude.y,
            -attitude.z,
            -attitude.w
        );

        Vector3 euler = unityAttitude.eulerAngles;
        deviceRotation = euler;

        attitudeText.text =
            $"Device Rotation\nX: {euler.x:F1}\nY: {euler.y:F1}\nZ: {euler.z:F1} and {unityAttitude.x:F1},{unityAttitude.y:F1},{unityAttitude.z:F1},{unityAttitude.w:F1}";
    }
}
