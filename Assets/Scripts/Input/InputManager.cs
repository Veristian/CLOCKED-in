using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : Singleton<InputManager>
{
    public TextMeshProUGUI attitudeText;

    public static PlayerInput playerInput;
    private static UnityEngine.InputSystem.Gyroscope gyro;
    public static Vector3 deviceRotation;

    public static InputAction _gyroAction;


    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public Vector3 newPos = new Vector3(0, 0, 0);

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

        Vector3 delta = angularVelocity * Time.deltaTime * 180/ Mathf.PI; // convert from radian to degree
        newPos += delta;

        newPos.x = Mathf.Repeat(newPos.x + 180, 360) - 180;
        newPos.y = Mathf.Repeat(newPos.y + 180, 360) - 180;
        newPos.z = Mathf.Repeat(newPos.z + 180, 360) - 180;

        float angle1 = newPos.y * Mathf.Deg2Rad;
        float angle2 = newPos.z * Mathf.Deg2Rad;

        float x = Mathf.Cos(angle1) + Mathf.Cos(angle2);
        float y = Mathf.Sin(angle1) + Mathf.Sin(angle2);

        float finalDirection = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        int sector;

        if (finalDirection < -30f && finalDirection >= -90f)
            sector = 1;
        else if (finalDirection < 30f && finalDirection >= -30f)
            sector = 2;
        else if (finalDirection < 90f && finalDirection >= 30f)
            sector = 3;
        else
            sector = 0;

        attitudeText.text =
            $"Device Rotation\nX: {euler.x:F1} Y: {euler.y:F1} Z: {euler.z:F1} and {unityAttitude.x:F1},{unityAttitude.y:F1},{unityAttitude.z:F1},{unityAttitude.w:F1}\nPosition+offset\nX: {(euler.x + rotationOffset.x):F1} Y: {(euler.y + rotationOffset.y):F1} Z: {(euler.z + rotationOffset.z):F1}\nangularVelocity\nX: {angularVelocity.x:F1} Y: {angularVelocity.y:F1} Z: {angularVelocity.z:F1} \n new position\nX: {newPos.x:F1} Y: {newPos.y:F1} Z: {newPos.z:F1}\n Final Direction: {finalDirection:F1} Sector: {sector}";

        CanvasManager.Instance.MoveToSector(sector);

    }

    public void OnSetPosition()
    {
        rotationOffset = new Vector3(-deviceRotation.x, -deviceRotation.y, -deviceRotation.z);
        newPos = new Vector3(0, 0, 0);
    }
}
