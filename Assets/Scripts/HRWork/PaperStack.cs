using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PaperStack : MonoBehaviour
{
    BoxCollider paperStackCollider;
    BoxCollider availablePaperCollider;
    PaperActive paperActive;
    List<Draggable3D> paperList = new List<Draggable3D>();
    public Draggable3D activePaper;
    Draggable3D enteringPaper;

    [Header("Settings")]
    public Vector3 paperOffset = new Vector3(5, -0.1f, 0);
    private void Awake()
    {
        if (paperStackCollider == null)
            paperStackCollider = GetComponent<BoxCollider>();
        if (availablePaperCollider == null)
            availablePaperCollider = transform.GetChild(0).GetComponent<BoxCollider>();
        if (paperActive == null)
        {
            paperActive = availablePaperCollider.GetComponent<PaperActive>();
            paperActive.paperStack = this;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Paper") && !paperList.Contains(other.GetComponent<Draggable3D>()))
        {
            enteringPaper = other.GetComponent<Draggable3D>();
            enteringPaper.OnEndDrag.AddListener(() => EnterPaperToStack(enteringPaper));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        enteringPaper?.OnEndDrag.RemoveAllListeners();
        enteringPaper = null;
    }

    public void EnterPaperToStack(Draggable3D paper)
    {
        if (paper.CompareTag("Paper") && !paperList.Contains(paper))
        {
            // Debug.Log("PaperStack: EnterPaperToStack with " + paper.name);
            paperList.Add(paper);
            paper.SetCanDrag(false);
        }
        enteringPaper?.OnEndDrag.RemoveListener(() => EnterPaperToStack(enteringPaper));
        TidyPapers();
    }
    public void TidyPapers()
    {
        // Debug.Log("TidyPapers: Total papers in stack: " + paperList.Count);
        for (int i = 0; i < paperList.Count; i++)
        {
            Vector3 targetPosition = new Vector3(paperStackCollider.bounds.center.x + paperOffset.x * i, paperStackCollider.bounds.center.y + paperOffset.y * i, paperStackCollider.bounds.min.z + paperOffset.z * i);
            paperList[i].transform.position = targetPosition;
        }

    }

    void Update()
    {
        if (activePaper == null)
        {
            SetActivePaper();
            return;
        }
        
        // if (activePaper.IsDragging)
        // {
        //     paperList.Remove(activePaper);
        //     activePaper = null;
        //     SetActivePaper();
        // }
    }

    public void SetActivePaper()
    {
        if (paperList.Count > 0)
        {
            activePaper = paperList[paperList.Count - 1];
            activePaper.SetCanDrag(true);
            paperList.Remove(activePaper);
            MoveActivePaperToAvailableArea();
            TidyPapers();

            // activePaper?.OnBeginDrag.AddListener(() =>
            // {
            //     activePaper?.OnBeginDrag.RemoveAllListeners();
            //     activePaper = null;
            //     SetActivePaper();

            // });
        }
        else
        {
            activePaper = null;
        }
    }

    public void MoveActivePaperToAvailableArea()
    {
        if (activePaper != null)
        {
            activePaper.OnBeginDrag.RemoveAllListeners();
            Vector3 targetPosition = availablePaperCollider.bounds.center;
            activePaper.transform.position = targetPosition;
        }
    }


    
}
