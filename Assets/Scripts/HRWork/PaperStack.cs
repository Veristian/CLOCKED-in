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
    Dictionary<Draggable3D, UnityEngine.Events.UnityAction> endDragActions
    = new Dictionary<Draggable3D, UnityEngine.Events.UnityAction>();


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



    public void EnterPaperToStack(Draggable3D paper)
    {
        if (!paperList.Contains(paper))
        {
            paperList.Add(paper);
            paper.SetCanDrag(false);
        }

        if (endDragActions.TryGetValue(paper, out var action))
        {
            paper.OnEndDrag.RemoveListener(action);
            endDragActions.Remove(paper);
        }

        TidyPapers();
    }

    public void TidyPapers()
    {
        int count = paperList.Count;

        for (int i = 0; i < count; i++)
        {
            int reverseIndex = count - 1 - i;

            Vector3 targetPosition = new Vector3(
                paperStackCollider.bounds.center.x + paperOffset.x * reverseIndex,
                paperStackCollider.bounds.center.y + paperOffset.y * reverseIndex,
                paperStackCollider.bounds.min.z + paperOffset.z * reverseIndex
            );

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
