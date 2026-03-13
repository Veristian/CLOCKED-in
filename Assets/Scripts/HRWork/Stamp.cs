using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamp : MonoBehaviour
{
    public Paper.PaperStampColor activeStampColor;
    public LayerMask interactableLayer;
    public BoxCollider stampCollider;

    [Header("Stamp material")]
    public Renderer stampRenderer;
    public Material defaultMaterial;
    public Material purpleMaterial;
    public Material orangeMaterial;
    public Material greenMaterial;

    public Animator stamping;

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
        Collider other = null;
        float closestDistance = float.MaxValue;

        foreach (Collider col in overlap)
        {
            float dist = (col.ClosestPoint(transform.position) - transform.position).sqrMagnitude;

            if (dist < closestDistance)
            {
                closestDistance = dist;
                other = col;
            }
        }
        if (other == null) return;
        if (other.CompareTag("StampArea"))
        {
            Debug.Log("StampHandler: OnTriggerEnter with " + other.name);
            StampHandler stampArea = other.GetComponent<StampHandler>();
            if (stampArea != null)
            {
                stampArea.ApplyStamp(activeStampColor);
                stamping.SetTrigger("Stamp");
                AudioPooler.Instance.Play(SFX.Stamp);
            }
        }

        if (other.CompareTag("InkArea"))
        {
            Debug.Log("StampHandler: OnTriggerEnter with " + other.name);
            Ink inkArea = other.GetComponent<Ink>();
            if (inkArea != null)
            {
                activeStampColor = inkArea.inkColor;

                if (activeStampColor == Paper.PaperStampColor.Purple)
                {
                    stampRenderer.material = purpleMaterial;
                }
                else if (activeStampColor == Paper.PaperStampColor.Orange)
                {
                    stampRenderer.material = orangeMaterial;
                }
                else if (activeStampColor == Paper.PaperStampColor.Green)
                {
                    stampRenderer.material = greenMaterial;
                }
                else 
                {
                    stampRenderer.material = defaultMaterial;
                }
                stamping.SetTrigger("Stamp");
                AudioPooler.Instance.Play(SFX.Stamp);
            }
        }
    }

}
