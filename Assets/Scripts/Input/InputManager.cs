using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using System.Threading.Tasks;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : Singleton<InputManager>
{
    public TextMeshProUGUI attitudeText;

    public static PlayerInput playerInput;
    private static UnityEngine.InputSystem.Gyroscope gyro;
    public static Vector3 deviceRotation;


    public Vector3 rotationOffset = new Vector3(0, 0, 0);
    public Vector3 newPos = new Vector3(0, 0, 0);

    public Vector2 touchPosition;
    public event System.Action<Vector2> OnSwipe;
    public event System.Action OnSwipeUp;
    public event System.Action OnSwipeDown;
    public event System.Action OnSwipeLeft;
    public event System.Action OnSwipeRight;
    public event System.Action OnTouchDownPerformed;
    public event System.Action OnTouchUpPerformed;


    public event System.Action<Vector2> OnPan;

    [Header("Swipe Settings")]
    public float swipeMinDistance = 100f;
    public float swipeMaxTime = 0.5f;

    private Vector2 startPos;
    private Vector2 endPos;
    private float startTime;
    private float endTime;
    public bool isPanning;
    public bool isTouching;

    

    protected override void Awake()
    {
        base.Awake();
        playerInput = GetComponent<PlayerInput>();

        // playerInput.actions["TouchDelta"].performed += OnTouchDelta;
        playerInput.actions["TouchContact"].performed += OnTouchDown;
        playerInput.actions["TouchContact"].canceled += OnTouchUp;
        playerInput.actions["TouchPos"].performed += OnTouchPosition;
        playerInput.actions["W"].performed += ctx => OnSwipeUp?.Invoke();
        playerInput.actions["S"].performed += ctx => OnSwipeDown?.Invoke();
        playerInput.actions["A"].performed += ctx => OnSwipeLeft?.Invoke();
        playerInput.actions["D"].performed += ctx => OnSwipeRight?.Invoke();
        // playerInput.actions["TouchStartTime"].performed += ctx => OnTouchStartTime(ctx.ReadValue<float>());
        // playerInput.actions["TouchStartPosition"]. += OnTouchStartPosition;
        
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

    // private void OnEnable()
    // {
    //     EnhancedTouchSupport.Enable();
    //     Touch.onFingerDown += FingerDown;
    //     Touch.onFingerMove += FingerMove;
    //     Touch.onFingerUp += FingerUp;
    // }

    // private void OnDisable()
    // {
    //     Touch.onFingerDown -= FingerDown;
    //     Touch.onFingerMove -= FingerMove;
    //     Touch.onFingerUp -= FingerUp;
    //     EnhancedTouchSupport.Disable();
    // }

    // private void FingerDown(Finger finger)
    // {
    //     startPos = finger.screenPosition;
    //     startTime = Time.time;
    //     isPanning = true;
    // }

    // private void FingerMove(Finger finger)
    // {
    //     if (!isPanning) return;

    //     Vector2 delta = finger.delta;
    //     OnPan?.Invoke(delta);
    // }

    // private void FingerUp(Finger finger)
    // {
    //     float time = Time.time - startTime;
    //     Vector2 endPos = finger.screenPosition;
    //     Vector2 distance = endPos - startPos;

    //     isPanning = false;

    //     if (distance.magnitude >= swipeMinDistance && time <= swipeMaxTime)
    //     {
    //         OnSwipe?.Invoke(distance.normalized);
    //     }
    // }
    // public void OnTouchDelta(InputAction.CallbackContext context)
    // {
    //     Vector2 delta = context.ReadValue<Vector2>();
    //     Debug.Log("Touch Delta: " + delta);
    //     if (delta.magnitude >= swipeMinDistance)
    //     {
    //         isPanning = false;
    //         OnSwipe?.Invoke(delta);
    //     }
    //     else
    //     {
    //         isPanning = true;
    //         OnPan?.Invoke(delta);
    //     }
    // }
    public void OnTouchDown(InputAction.CallbackContext context)
    {
        // This can be used to detect touch start or end if needed
        isTouching = true;
        startPos = touchPosition;
        startTime = Time.time;
        // Debug.Log("Touch Contact: " + isTouching + touchPosition);
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
        if (distance.magnitude >= swipeMinDistance && time <= swipeMaxTime)
        {
            SwipeHandler(distance.normalized);
        }
        OnTouchUpPerformed?.Invoke();
        // Debug.Log("Touch Contact: " + isTouching + touchPosition);
    }
    public void OnTouchPosition(InputAction.CallbackContext context)
    {
        Vector2 position = context.ReadValue<Vector2>();
        touchPosition = position;
        // Debug.Log("Touch Position: " + position);
    }

    // public void OnTouchStartTime(InputAction.CallbackContext context)
    // {
    //     if (!context.performed)
    //         return;

    //     double touchStartTime = context.ReadValue<double>();
    //     Debug.Log($"Touch start time: {touchStartTime}");
    // }


    // public void OnTouchStartPosition(Vector2 position)
    // {
    //     startPos = position;
    //     Debug.Log("Touch Start Position: " + startPos);
    // }

    public void SwipeHandler(Vector2 direction)
    {
        Debug.Log("Swipe Detected in direction: " + direction);
        //detect swipe for all 4 directions
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                Debug.Log("Swipe Right");
                OnSwipeRight?.Invoke();
            }
            else
            {
                Debug.Log("Swipe Left");
                OnSwipeLeft?.Invoke();
            }
        }
        else
        {
            if (direction.y > 0)
            {
                Debug.Log("Swipe Up");
                OnSwipeUp?.Invoke();
            }
            else
            {
                Debug.Log("Swipe Down");
                OnSwipeDown?.Invoke();
            }
        }
    }

    

    // public void OnSwipeInput(InputAction.CallbackContext context)
    // {
    //     Vector2 swipeDirection = context.ReadValue<Vector2>();
    //     OnSwipe?.Invoke();
    // }

    // public void OnPanInput(InputAction.CallbackContext context)
    // {
    //     Vector2 panDelta = context.ReadValue<Vector2>();
    //     OnPan?.Invoke(panDelta);
    // }




    void Update()
    {
        GyroInputUpdate();

    }

    // private void InputEventsRecognizer()
    // {
        
    // }

    private void GyroInputUpdate()
    {
        
        if (gyro == null)
        {
            if (attitudeText != null)
                attitudeText.text = "Gyroscope not available.";
            return;
        }
        Vector3 angularVelocity = UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue();
        Quaternion attitude = AttitudeSensor.current.attitude.ReadValue();

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
