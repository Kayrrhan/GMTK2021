using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MainMenu : MonoBehaviour
{
    public void OnContinueClicked()
    {
        Debug.Log("Continue");
    }

    public void OnNewGameClicked()
    {
        Debug.Log("New Game");
        SceneManager.LoadScene(1);
    }
    
    public void OnLoadGameClicked()
    {
        Debug.Log("Load Game");
    }
    
    public void OnSettingsClicked()
    {
        Debug.Log("Settings");
    }

    public void OnExitClicked()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
