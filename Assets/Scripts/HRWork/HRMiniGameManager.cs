using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRMiniGameManager : Singleton<HRMiniGameManager>
{
    
    //Spawn points
    // public Transform paperSpawnPoint;
    public GameObject paperPrefab;
    public PaperStack paperStack;
    public List<Paper> allPapers = new List<Paper>();
    // List<SubmissionBox> allSubmissionBoxes = new List<SubmissionBox>();

    protected override void Awake()
    {
        base.Awake();
        // allSubmissionBoxes.AddRange(FindObjectsByType<SubmissionBox>(FindObjectsSortMode.None));
        if (paperStack == null)
        {
            paperStack = FindAnyObjectByType<PaperStack>();
        }
    }
    //Spawn methods
    public void SpawnPaper(int amounts)
    {
        for (int i = 0; i < amounts; i++)
        {
            Paper paper = Instantiate(paperPrefab, transform.position, Quaternion.Euler(0, 90, 0)).GetComponent<Paper>();
            allPapers.Add(paper);
            paperStack.EnterPaperToStack(paper.GetComponent<Draggable3D>());
        }
        GameManager.Instance.UpdateTaskDisplay();
    }

    //Submission Checks
    public bool CheckAllSubmissions()
    {
        foreach (Paper paper in allPapers)
        {
            if (!paper.isSubmitted)
            {
                Debug.Log("HRMiniGameManager: Not all papers have been submitted.");
                return false;
            }
        }
        Debug.Log("HRMiniGameManager: All papers have been successfully submitted!");

        return true;
    }

    public int CheckSubmittedCount()
    {
        int submittedCount = 0;
        foreach (Paper paper in allPapers)
        {
            if (paper.isSubmitted)
            {
                submittedCount++;
            }
        }
        return submittedCount;
    }
    
}
