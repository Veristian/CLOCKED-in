using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : MonoBehaviour
{
    [Header("References")]
    public GameObject purpleStamps;
    public GameObject orangeStamps;
    public GameObject greenStamps;

    public GameObject yellowLabel;
    public GameObject blueLabel;
    public GameObject redLabel;

    public GameObject purpleMarkings;
    public GameObject orangeMarkings;
    public GameObject greenMarkings;


    public enum PaperStampColor
    {
        Purple,
        Orange,
        Green,
        None
    }
    public enum PaperLabelColor
    {
        Yellow,
        Blue,
        Red
    }

    // public enum PaperMarkingsColor
    // {
    //     Purple,
    //     Orange,
    //     Green
    // }

    [Header("Property")]
    public PaperStampColor paperStampColor;
    public PaperLabelColor paperLabelColor;
    public PaperStampColor paperMarkingsColor;
    public LayerMask submissionBoxLayerMask;

    public Draggable3D draggable3D;
    private void Start()
    {
        paperMarkingsColor = PaperStampColor.None;
        SetPaperAppearance();
        if (draggable3D == null)
            draggable3D = GetComponent<Draggable3D>();
        draggable3D.OnEndDrag.AddListener(CheckPlaceInSubmissionBox);
    }

    void SetPaperAppearance()
    {
        // Set stamp color
        purpleStamps.SetActive(paperStampColor == PaperStampColor.Purple);
        orangeStamps.SetActive(paperStampColor == PaperStampColor.Orange);
        greenStamps.SetActive(paperStampColor == PaperStampColor.Green);

        // Set label color
        yellowLabel.SetActive(paperLabelColor == PaperLabelColor.Yellow);
        blueLabel.SetActive(paperLabelColor == PaperLabelColor.Blue);
        redLabel.SetActive(paperLabelColor == PaperLabelColor.Red);

        // Set markings color
        purpleMarkings.SetActive(false);
        orangeMarkings.SetActive(false);
        greenMarkings.SetActive(false);
    }

    public void ActivateMark(PaperStampColor stampedMarkingsColor)
    {
        switch (stampedMarkingsColor)
        {
            case PaperStampColor.Purple:
                purpleMarkings.SetActive(true);
                orangeMarkings.SetActive(false);
                greenMarkings.SetActive(false);
                paperMarkingsColor = PaperStampColor.Purple;
                break;
            case PaperStampColor.Orange:
                orangeMarkings.SetActive(true);
                purpleMarkings.SetActive(false);
                greenMarkings.SetActive(false);
                paperMarkingsColor = PaperStampColor.Orange;
                break;
            case PaperStampColor.Green:
                greenMarkings.SetActive(true);
                purpleMarkings.SetActive(false);
                orangeMarkings.SetActive(false);
                paperMarkingsColor = PaperStampColor.Green;
                break;
            case PaperStampColor.None:
                purpleMarkings.SetActive(false);
                orangeMarkings.SetActive(false);
                greenMarkings.SetActive(false);
                paperMarkingsColor = PaperStampColor.None;
                break;
        }
    }




    void CheckPlaceInSubmissionBox()
    {
        Collider[] overlap = Physics.OverlapBox(transform.position, draggable3D.boxCollider.size*transform.localScale.x, Quaternion.identity, submissionBoxLayerMask);
        Collider other = overlap.Length > 0 ? overlap[0] : null;
        if (other == null) return;
        if (paperStampColor != paperMarkingsColor)
        {
            Debug.Log("Paper: CheckPlaceInSubmissionBox - Stamp color does not match Markings color.");
            return;
        }
        if (other.CompareTag("SubmissionArea"))
        {
            Debug.Log("Paper: CheckPlaceInSubmissionBox with " + other.name);
            SubmissionBox submissionBox = other.GetComponent<SubmissionBox>();
            if (submissionBox != null)
            {
                submissionBox.ReceivePaper(this);
            }
        }
    }

}
