using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmissionBox : MonoBehaviour
{
    List<Paper> submittedPapers = new List<Paper>();
    public Paper.PaperLabelColor expectedLabelColor;
    public void ReceivePaper(Paper paper)
    {
        if (!submittedPapers.Contains(paper) && paper.paperLabelColor == expectedLabelColor)
        {
            submittedPapers.Add(paper);
            paper.draggable3D.SetCanDrag(false);
            paper.transform.position = transform.position + Vector3.up * 0.5f * submittedPapers.Count;
            // paper.transform.rotation = Quaternion.Euler(0, 0, 0);
            Debug.Log("SubmissionBox: Received paper with Stamp Color " + paper.paperStampColor + " and Label Color " + paper.paperLabelColor);
        }
        else
        {
            Debug.Log("SubmissionBox: Paper rejected - mismatched stamp and label colors.");
        }
    }
}
