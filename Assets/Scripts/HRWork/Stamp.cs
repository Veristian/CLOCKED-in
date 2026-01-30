using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamp : MonoBehaviour
{
    public Paper.PaperStampColor activeStampColor;
    public LayerMask interactableLayer;
    public BoxCollider stampCollider; 

    Draggable3D draggable3D;
    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("StampArea"))
    //     {
    //         Debug.Log("StampHandler: OnTriggerEnter with " + other.name);
    //         StampHandler stampArea = other.GetComponent<StampHandler>();
    //         if (stampArea != null)
    //         {
    //             stampArea.ApplyStamp(activeStampColor);
    //         }
    //     }

    //     if (other.CompareTag("InkArea"))
    //     {
    //         Debug.Log("StampHandler: OnTriggerEnter with " + other.name);
    //         Ink inkArea = other.GetComponent<Ink>();
    //         if (inkArea != null)
    //         {
    //             activeStampColor = inkArea.inkColor;
    //         }
    //     }
    // }

    void Start()
    {
        if (stampCollider == null)
            stampCollider = GetComponent<BoxCollider>();
        if(draggable3D == null)
            draggable3D = GetComponent<Draggable3D>();
        draggable3D.OnEndDrag.AddListener(OnStartStamp);
    }

    void OnStartStamp()
    {
        Collider[] overlap = Physics.OverlapBox(transform.position, stampCollider.size*transform.localScale.x, Quaternion.identity, interactableLayer);
        Collider other = overlap.Length > 0 ? overlap[0] : null;
        if (other == null) return;
        if (other.CompareTag("StampArea"))
        {
            Debug.Log("StampHandler: OnTriggerEnter with " + other.name);
            StampHandler stampArea = other.GetComponent<StampHandler>();
            if (stampArea != null)
            {
                stampArea.ApplyStamp(activeStampColor);
            }
        }

        if (other.CompareTag("InkArea"))
        {
            Debug.Log("StampHandler: OnTriggerEnter with " + other.name);
            Ink inkArea = other.GetComponent<Ink>();
            if (inkArea != null)
            {
                activeStampColor = inkArea.inkColor;
            }
        }
    }

}
