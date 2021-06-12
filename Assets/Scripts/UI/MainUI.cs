using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Zenject;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour
{
    #region inspector

    [SerializeField]
    Text _gripButtonText = null;

    
    [SerializeField]
    Text _resetButtonText = null;

    #endregion

    #region private members

    [Inject]
    EventManager _eventManager;

    #endregion

    #region Unity messages

    void Awake()
    {
        _eventManager.onMonkeyGripped.AddListener(OnMonkeyGripped);
        _eventManager.onMonkeySelection.AddListener(OnMonkeySelection);
    }

    #endregion

    #region private methods

    void OnMonkeyGripped(Monkey monkey, bool gripped)
    {
        _gripButtonText.text = gripped ? "Drop" : "Grip";
    }


    void OnMonkeySelection(Monkey monkey)
    {
        _gripButtonText.text = monkey.gripJoint != null ? "Drop" : "Grip";
    }

    #endregion

    #region public methods

    public void OnGripButtonClicked()
    {
        _eventManager.FireMonkeyGrip();
    }


    public void OnResetButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}