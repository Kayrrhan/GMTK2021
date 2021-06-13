using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void OnResumeClicked()
    {
        Debug.Log("Resume");
    }

    public void OnSaveClicked()
    {
        Debug.Log("Save");
    }

    public void OnSettingsClicked()
    {
        Debug.Log("Settings");
    }

    public void OnReturnClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
