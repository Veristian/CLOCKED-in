using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class StampHandler : MonoBehaviour
{
    public Paper paper;

    void Awake()
    {
        if (paper == null)
        paper = GetComponentInParent<Paper>();
    }

    public void ApplyStamp(Paper.PaperStampColor stampColor)
    {
        paper.ActivateMark(stampColor);
    }
}
