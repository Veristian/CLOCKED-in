using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class UIDraggable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public UnityEvent OnHoverEnter = new UnityEvent();
    public UnityEvent OnHoverExit = new UnityEvent();


    public UnityEvent onBeginDrag = new UnityEvent();
    public UnityEvent onDrag = new UnityEvent();
    public UnityEvent onEndDrag = new UnityEvent();

    public bool IsDragging { get; private set; }
    public bool CanDrag { get; private set; } = true;

    [SerializeField] private List<RectTransform> blockers = new List<RectTransform>(); 

    private Canvas canvas;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverExit();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanDrag || CanvasManager.Instance.activeSector != 2)
        {
            eventData.pointerDrag = null;
            return;
        }

        IsDragging = true;
        BeginDrag();
        //transform.SetAsLastSibling(); // Bring to front
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDragging) return;

        Rect currentScreenRect = GetScreenRect(rectTransform);

        Vector2 screenDelta = eventData.delta;
        Vector2 appliedScreenDelta = Vector2.zero;

        if (screenDelta.sqrMagnitude > 0.01f)
        {
            // Try full movement
            if (CanApplyDelta(screenDelta, currentScreenRect))
            {
                appliedScreenDelta = screenDelta;
            }
            else
            {
                // Try horizontal only
                Vector2 deltaX = new Vector2(screenDelta.x, 0);
                if (deltaX.sqrMagnitude > 0.01f && CanApplyDelta(deltaX, currentScreenRect))
                {
                    appliedScreenDelta.x = deltaX.x;
                }

                // Try vertical only
                Vector2 deltaY = new Vector2(0, screenDelta.y);
                if (deltaY.sqrMagnitude > 0.01f && CanApplyDelta(deltaY, currentScreenRect))
                {
                    appliedScreenDelta.y = deltaY.y;
                }
            }
        }

        if (appliedScreenDelta != Vector2.zero)
        {
            rectTransform.anchoredPosition += appliedScreenDelta / canvas.scaleFactor;
        }

        Drag();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsDragging)
        {
            IsDragging = false;
            EndDrag();
        }
    }


    internal void HoverEnter() => OnHoverEnter?.Invoke();
    internal void HoverExit() => OnHoverExit?.Invoke();
    internal void BeginDrag() => onBeginDrag?.Invoke();
    internal void Drag() => onDrag?.Invoke();
    internal void EndDrag() => onEndDrag?.Invoke();

    public void SetCanDrag(bool canDrag)
    {
        CanDrag = canDrag;
    }

    // --------------------- Collision Helpers ---------------------


    private bool CanApplyDelta(Vector2 screenDelta, Rect currentRect)
    {
        if (screenDelta == Vector2.zero) return true;

        Rect testRect = currentRect;
        testRect.x += screenDelta.x;
        testRect.y += screenDelta.y;

        foreach (RectTransform blocker in blockers)
        {
            if (blocker == null) continue;

            Rect blockerRect = GetScreenRect(blocker);
            if (blockerRect.width <= 0 || blockerRect.height <= 0) continue;

            // Small margin so it stops just before visually touching
            float margin = 5f;
            Rect expandedBlocker = new Rect(
                blockerRect.x - margin,
                blockerRect.y - margin,
                blockerRect.width + 2 * margin,
                blockerRect.height + 2 * margin
            );

            if (testRect.Overlaps(expandedBlocker, true))
            {
                return false;
            }
        }

        return true;
    }

    private Rect GetScreenRect(RectTransform rt)
    {
        if (rt == null) return new Rect();

        Vector3[] worldCorners = new Vector3[4];
        rt.GetWorldCorners(worldCorners);

        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        foreach (Vector3 corner in worldCorners)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, corner);
            minX = Mathf.Min(minX, screenPoint.x);
            maxX = Mathf.Max(maxX, screenPoint.x);
            minY = Mathf.Min(minY, screenPoint.y);
            maxY = Mathf.Max(maxY, screenPoint.y);
        }

        if (minX > maxX || minY > maxY) return new Rect();

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
}