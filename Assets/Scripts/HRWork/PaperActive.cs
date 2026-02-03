using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperActive : MonoBehaviour
{   
    public PaperStack paperStack;

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Paper")) return;

        Draggable3D paper = other.GetComponent<Draggable3D>();
        if (paper == null) return;

        // only react if THIS is the active one
        if (paperStack.activePaper != paper) return;

        paperStack.activePaper = null;
        paperStack.SetActivePaper();
    }

}
