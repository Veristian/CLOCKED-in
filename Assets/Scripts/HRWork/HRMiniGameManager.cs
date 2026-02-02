using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRMiniGameManager : Singleton<HRMiniGameManager>
{
    
    //Spawn points
    public Transform paperSpawnPoint;
    public GameObject paperPrefab;

    List<Paper> allPapers = new List<Paper>();
    // List<SubmissionBox> allSubmissionBoxes = new List<SubmissionBox>();

    void Start()
    {
        // allSubmissionBoxes.AddRange(FindObjectsByType<SubmissionBox>(FindObjectsSortMode.None));
    }
    //Spawn methods
    public void SpawnPaper(int amounts)
    {
        for (int i = 0; i < amounts; i++)
        {
            Paper paper = Instantiate(paperPrefab, paperSpawnPoint.position, Quaternion.identity).GetComponent<Paper>();
            allPapers.Add(paper);
        }
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
    
}
