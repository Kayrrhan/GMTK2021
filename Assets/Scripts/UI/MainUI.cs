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
    Button _gripButtonRight = null;

    [SerializeField]
    Text _gripButtonRightText = null;

    [SerializeField]
    Button _gripButtonLeft = null;


    [SerializeField]
    Text _gripButtonLeftText = null;

    [SerializeField]
    Button _autoRunButton = null;

    #endregion

    #region private members

    [Inject]
    EventManager _eventManager;

    [Inject]
    PlayerManager _playerManager;

    #endregion

    #region Unity messages

    void Awake()
    {
        _eventManager.onMonkeyGripped.AddListener(OnMonkeyGripped);
        _eventManager.onMonkeySelected.AddListener(OnMonkeySelection);
        _eventManager.onAutoRunStarted.AddListener(OnAutoRunStarted);
    }

    void OnDestroy()
    {
        _eventManager.onMonkeyGripped.RemoveListener(OnMonkeyGripped);
        _eventManager.onMonkeySelected.RemoveListener(OnMonkeySelection);
        _eventManager.onAutoRunStarted.RemoveListener(OnAutoRunStarted);
    }

    #endregion

    #region private methods

    void OnAutoRunStarted()
    {
        _autoRunButton.interactable = false;
        _gripButtonLeft.interactable = false;
        _gripButtonRight.interactable = false;
    }

    void OnMonkeyGripped(Monkey monkey, bool gripped, GripSide side)
    {
        UpdateGripButtons();
    }


    void OnMonkeySelection(Monkey oldMonkey, Monkey newMonkey)
    {
        UpdateGripButtons();
    }

    void UpdateGripButtons()
    {
        Monkey monkey = _playerManager.selectedMonkey;
        if (monkey == null)
        {
            _gripButtonLeft.interactable = false;
            _gripButtonRight.interactable = false;
        }
        else
        {
            _gripButtonLeft.interactable = true;
            _gripButtonRight.interactable = true;

            _gripButtonLeftText.text = monkey.isLeftSideGrip ? "Drop Left" : "Grip Left";
            _gripButtonRightText.text = monkey.isRightSideGrip ? "Drop Right" : "Grip Right";
        }
    }

    #endregion

    #region public methods

    public void OnGripButtonClicked(int side)
    {
        OnGripButtonClicked(side >= 0 ? GripSide.Right : GripSide.Left);
    }

    public void OnGripButtonClicked(GripSide side)
    {
        _eventManager.FireMonkeyGrip(side);
    }


    public void OnResetButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnAutoRunClicked()
    {
        _eventManager.FireAutoRunTriggered();
    }

    #endregion
}