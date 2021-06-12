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
    #endregion

    #region public methods
    #endregion
}
