using UnityEngine;

public class DragController : MonoBehaviour
{
    [SerializeField] Camera cam;

    Draggable3D hovered;
    Draggable3D dragging;

    Plane dragPlane;
    Vector3 grabOffset;
    Draggable3D hitDraggable;

    public LayerMask draggableLayerMask;

    void Awake()
    {
        if (!cam) cam = Camera.main ?? FindAnyObjectByType<Camera>();
    }

    void Update()
    {
        if (!cam) return;

        Ray ray = cam.ScreenPointToRay(InputManager.Instance.touchPosition);

        // Draggable3D hitDraggable = null;

        if (Physics.Raycast(ray, out var hit, 100, draggableLayerMask))
        {
            if (hitDraggable == null)
                hitDraggable = hit.collider.GetComponent<Draggable3D>();
            else
                if (hitDraggable.gameObject != hit.collider.gameObject)
                    hitDraggable = hit.collider.GetComponent<Draggable3D>();
        }
        else
        {
            hitDraggable = null;
        }

        // ---------- Hover handling ----------
        if (hitDraggable != hovered && dragging == null)
        {
            if (hovered != null && dragging == null)
                hovered.HoverExit();

            hovered = hitDraggable;

            if (hovered != null && dragging == null)
                hovered.HoverEnter();
        }

        // ---------- Begin Drag ----------
        if (hovered != null && InputManager.Instance.onTouchDown && dragging == null)
        {
            dragging = hovered;
            dragging.IsDragging = true;

            dragPlane = new Plane(Vector3.up, dragging.transform.position);
            if (dragPlane.Raycast(ray, out float d))
                grabOffset = dragging.transform.position - ray.GetPoint(d);

            dragging.BeginDrag();
        }

        // ---------- Drag ----------
        if (dragging != null && InputManager.Instance.isTouching)
        {
            if (dragPlane.Raycast(ray, out float d))
            {
                dragging.transform.position = ray.GetPoint(d) + grabOffset;
            }

            dragging.Drag();
        }

        // ---------- End Drag ----------
        if (dragging != null && InputManager.Instance.onTouchUp)
        {
            dragging.IsDragging = false;
            dragging.EndDrag();
            dragging = null;
        }
    }
}
