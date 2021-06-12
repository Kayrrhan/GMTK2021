using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainUI : MonoBehaviour
{
    #region inspector

    [SerializeField]
    Text _gripButtonText = null;

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

    #endregion
}