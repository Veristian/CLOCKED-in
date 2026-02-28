using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinSummary : MonoBehaviour
{
    public void BackToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TestMenu");
    }
}
