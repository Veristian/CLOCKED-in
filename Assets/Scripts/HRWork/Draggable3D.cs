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

    public bool IsDragging { get; internal set; }

    internal void HoverEnter() => OnHoverEnter?.Invoke();
    internal void HoverExit()  => OnHoverExit?.Invoke();
    internal void BeginDrag()  => OnBeginDrag?.Invoke();
    internal void Drag()       => OnDrag?.Invoke();
    internal void EndDrag()    => OnEndDrag?.Invoke();
}
