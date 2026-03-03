using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    public GameObject settingMenu;
    public void BackButton()
    {
        settingMenu.SetActive(false);

    }

    public void HomeButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TestMenu");
    }
}
