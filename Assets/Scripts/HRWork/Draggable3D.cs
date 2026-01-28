using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Draggable3D : MonoBehaviour
{
    public UnityEvent OnHoverEnter;
    public UnityEvent OnHoverExit;
    public UnityEvent OnBeginDrag;
    public UnityEvent OnDrag;
    public UnityEvent OnEndDrag;

    public bool IsDragging => isDragging;
    public bool IsHovered  => isHovered;

    Camera cam;
    Plane dragPlane;
    Vector3 grabOffset;

    bool isDragging;
    bool isHovered;

    

    void Awake()
    {
        cam = Camera.main ?? FindAnyObjectByType<Camera>();
    }


    void Update()
    {
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(InputManager.Instance.touchPosition);

        bool hitThis = false;

        if (Physics.Raycast(ray, out var hit))
        {
            if (hit.collider == GetComponent<Collider>())
                hitThis = true;
        }

        // ---------- Hover ----------
        if (hitThis && !isHovered && !isDragging)
        {
            isHovered = true;
            OnHoverEnter?.Invoke();
        }
        else if (!hitThis && isHovered && !isDragging)
        {
            isHovered = false;
            OnHoverExit?.Invoke();
        }

        // ---------- Begin Drag ----------
        if (hitThis && InputManager.Instance.onTouchDown && !isDragging)
        {
            isDragging = true;

            dragPlane = new Plane(Vector3.up, transform.position);
            if (dragPlane.Raycast(ray, out float d))
            {
                grabOffset = transform.position - ray.GetPoint(d);
            }

            OnBeginDrag?.Invoke();
        }

        // ---------- Drag ----------
        if (isDragging && InputManager.Instance.isTouching)
        {
            if (dragPlane.Raycast(ray, out float d))
            {
                transform.position = ray.GetPoint(d) + grabOffset;
            }

            OnDrag?.Invoke();
        }

        // ---------- End Drag ----------
        if (isDragging && InputManager.Instance.onTouchUp)
        {
            isDragging = false;
            isHovered = false;

            OnEndDrag?.Invoke();
        }
    }
}
