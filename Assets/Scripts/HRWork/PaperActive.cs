using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperActive : MonoBehaviour
{   
    public PaperStack paperStack;

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Paper"))
        {
            paperStack.activePaper = null;
            paperStack.SetActivePaper();
        }
    }
}
