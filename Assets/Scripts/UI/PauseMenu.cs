using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using CallbackCtx = UnityEngine.InputSystem.InputAction.CallbackContext;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    GameObject pausePanel;
    [SerializeField]
    GameObject mainPanel;

    MainControls _controls = null;

    private void Awake()
    {
        _controls = new MainControls();
        _controls.Main.Pause.started += OpenPauseMenu;
        Debug.Log("yolo");
    }

    void OpenPauseMenu(CallbackCtx ctx)
    {
        Debug.Log("start");
        pausePanel.SetActive(pausePanel.activeSelf);
    }

    public void OnResumeClicked()
    {
        Debug.Log("Resume");
        pausePanel.SetActive(false);
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
        SceneManager.LoadScene(0);
    }
}
