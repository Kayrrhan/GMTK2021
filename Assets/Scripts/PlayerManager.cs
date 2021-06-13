using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerManager : MonoBehaviour
{
    #region inspector

    #endregion

    #region private members

    [Inject]
    EventManager _eventManager;

    Monkey _selectedMonkey = null;

    #endregion

    #region public properties
    
    public static PlayerManager instance { get; private set; } = null; // je crée un singleton parce qu'apparemment
    // Zenject ne gère pas les objets instanciés à la volée ...

    [SerializeField]
    public Monkey selectedMonkey
    {
        get => _selectedMonkey;
        set
        {
            Monkey oldMonkey = _selectedMonkey;
            _selectedMonkey = value;
            _eventManager.FireMonkeySelected(oldMonkey, _selectedMonkey);
        }
    }

    #endregion

    #region private methods

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            _eventManager.onAutoRunStarted.AddListener(OnAutoRunStarted);
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            _eventManager.onAutoRunStarted.RemoveListener(OnAutoRunStarted);
        }
    }
    #endregion

    #region private methods

    void OnAutoRunStarted()
    {
        selectedMonkey = null;
    }
    #endregion
}
