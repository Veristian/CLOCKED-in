using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using System.Threading.Tasks;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : Singleton<InputManager>
{
    public static PlayerInput playerInput;
    public Vector2 touchPosition;
    public event System.Action<Vector2> OnSwipe;
    public event System.Action OnSwipeUp;
    public event System.Action OnSwipeDown;
    public event System.Action OnSwipeLeft;
    public event System.Action OnSwipeRight;
    public event System.Action OnTouchDownPerformed;
    public event System.Action OnTouchUpPerformed;
    public event System.Action OnVolUpPerformed;
    public event System.Action OnVolDownPerformed;
    public event System.Action OnQuickTap;

    public event System.Action<Vector2> OnPan;

    [Header("Swipe Settings")]
    public float swipeMinDistance = 100f;
    public float swipeMaxTime = 0.5f;

    [Header("QuickTap Settings")]
    public float quickTapTime = 0.3f;
    [Header("JoyStick")]
    public FloatingJoystick joystick;
    public float speed;
    private Vector2 startPos;
    private Vector2 endPos;
    private float startTime;
    private float endTime;
    public bool isPanning;
    public bool isTouching;
    public bool onTouchDown;
    public bool onTouchUp;

    public bool FakeGyroLeft;
    public bool FakeGyroRight;

    [Header("Gyro Settings")]
    [SerializeField] private float middleSectorArea = 30f;
    [SerializeField] private float extraSectorArea = 10f;

    private Quaternion referenceRotation;
    private bool isCalibrated = false;

    protected override void Awake()
    {

        base.Awake();
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions["TouchContact"].performed += OnTouchDown;
        playerInput.actions["TouchContact"].canceled += OnTouchUp;
        playerInput.actions["TouchPos"].performed += OnTouchPosition;
        playerInput.actions["W"].performed += ctx => OnSwipeUp?.Invoke();
        playerInput.actions["S"].performed += ctx => OnSwipeDown?.Invoke();
        playerInput.actions["A"].performed += ctx => OnSwipeLeft?.Invoke();
        playerInput.actions["D"].performed += ctx => OnSwipeRight?.Invoke();
        playerInput.actions["Space"].performed += ctx => OnVolUpPerformed?.Invoke();
        playerInput.actions["Space"].performed += ctx => OnVolDownPerformed?.Invoke();

        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
        }
    }

    public void OnTouchDown(InputAction.CallbackContext context)
    {

        isTouching = true;
        startPos = touchPosition;
        startTime = Time.time;

        _ = EndSwipe();
        OnTouchDownPerformed?.Invoke();
    }

    private async Task EndSwipe()
    {
        await Task.Delay(100);
        if (!isTouching)
            return;
        endPos = touchPosition;
        Vector2 distance = endPos - startPos;
        if (distance.magnitude >= swipeMinDistance)
        {
            SwipeHandler(distance.normalized);
        }

    }
    public void OnTouchUp(InputAction.CallbackContext context)
    {
        isTouching = false;
        endPos = touchPosition;
        Vector2 distance = endPos - startPos;
        endTime = Time.time;
        float time = endTime - startTime;
        if (time <= quickTapTime)
        {
            OnQuickTap?.Invoke();
        }
        if (distance.magnitude >= swipeMinDistance && time <= swipeMaxTime)
        {
            SwipeHandler(distance.normalized);
        }
        OnTouchUpPerformed?.Invoke();
    }
    public void OnTouchPosition(InputAction.CallbackContext context)
    {
        Vector2 position = context.ReadValue<Vector2>();
        touchPosition = position;
    }

    public void SwipeHandler(Vector2 direction)
    {

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                OnSwipeRight?.Invoke();
            }
            else
            {
                OnSwipeLeft?.Invoke();
            }
        }
        else
        {
            if (direction.y > 0)
            {
                OnSwipeUp?.Invoke();
            }
            else
            {
                OnSwipeDown?.Invoke();
            }
        }
    }

    void Update()
    {
        GyroInputUpdate();
        onTouchDown = playerInput.actions["TouchContact"].WasPressedThisFrame();
        onTouchUp = playerInput.actions["TouchContact"].WasReleasedThisFrame();
        FakeGyroLeft = playerInput.actions["K"].WasPressedThisFrame();
        FakeGyroRight = playerInput.actions["J"].WasPressedThisFrame();

    }

    private void GyroInputUpdate()
    {

        if (AttitudeSensor.current == null)
        {
            Debug.LogWarning("Attitude Sensor not available.");
            return;
        }

        Quaternion attitude = AttitudeSensor.current.attitude.ReadValue();

        Quaternion unityAttitude = new Quaternion(
            attitude.x,
            attitude.y,
            -attitude.z,
            -attitude.w
        );

        if (!isCalibrated)
        {
            referenceRotation = unityAttitude;
            isCalibrated = true;
        }

        Quaternion relativeRotation = Quaternion.Inverse(referenceRotation) * unityAttitude;

        Vector3 euler = relativeRotation.eulerAngles;

        float yaw = Mathf.DeltaAngle(0, euler.y);
        float roll = Mathf.DeltaAngle(0, euler.z);

        float x = Mathf.Cos(yaw * Mathf.Deg2Rad) + Mathf.Cos(roll * Mathf.Deg2Rad);
        float y = Mathf.Sin(yaw * Mathf.Deg2Rad) + Mathf.Sin(roll * Mathf.Deg2Rad);

        float finalDirection = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        int sector;

        if (finalDirection < (-middleSectorArea / 2 - extraSectorArea) && finalDirection >= -90f)
            sector = 3;
        else if (finalDirection >= (middleSectorArea / 2 + extraSectorArea) && finalDirection < 90f)
            sector = 1;
        else if (
            ((finalDirection >= -middleSectorArea / 2 && finalDirection < (middleSectorArea / 2 - extraSectorArea))
                && CanvasManager.Instance.activeSector == 1)
            ||
            ((finalDirection >= (-middleSectorArea / 2 + extraSectorArea) && finalDirection < middleSectorArea / 2)
                && CanvasManager.Instance.activeSector == 3)
        )
            sector = 2;
        else
            return;

        CanvasManager.Instance.MoveToSector(sector);
    }

    public void OnSetPosition()
    {
        isCalibrated = false;
    }

    public void OnVolumeUp(string msg)
    {
        Debug.Log("Volume Up pressed");
        OnVolUpPerformed?.Invoke();
    }

    public void OnVolumeDown(string msg)
    {
        Debug.Log("Volume Down pressed");
        OnVolDownPerformed?.Invoke();
    }

    public void DisableJoystick()
    {
        joystick.gameObject.SetActive(false);
    }

    public void EnableJoystick()
    {
        joystick.gameObject.SetActive(true);
    }
}
