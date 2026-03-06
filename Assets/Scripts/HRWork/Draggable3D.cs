using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider))]
public class Draggable3D : MonoBehaviour
{
    public UnityEvent OnHoverEnter = new UnityEvent();
    public UnityEvent OnHoverExit = new UnityEvent();
    public UnityEvent OnBeginDrag = new UnityEvent();
    public UnityEvent OnDrag = new UnityEvent();
    public UnityEvent OnEndDrag = new UnityEvent();
    public bool IsDragging { get; internal set; }
    public bool CanDrag { get; private set; } = true;
    public BoxCollider boxCollider { get; private set; }
    internal void HoverEnter() => OnHoverEnter?.Invoke();
    internal void HoverExit()  => OnHoverExit?.Invoke();
    internal void BeginDrag()  => OnBeginDrag?.Invoke();
    internal void Drag()       => OnDrag?.Invoke();
    internal void EndDrag()    => OnEndDrag?.Invoke();

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void SetCanDrag(bool canDrag)
    {
        CanDrag = canDrag;
    }
    public void DisableCollider()
    {
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
    }
}
