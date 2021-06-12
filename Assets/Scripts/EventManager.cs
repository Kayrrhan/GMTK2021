using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    #region inspector

    public UnityEvent onMonkeyGrip = null;

    public UnityEvent<Monkey, bool> onMonkeyGripped = null;
    public UnityEvent<Monkey> onMonkeySelection = null;

    #endregion

    #region public methods

    public void FireMonkeyGrip()
    {
        onMonkeyGrip?.Invoke();
    }

    public void FireMonkeyGripped(Monkey monkey, bool gripped)
    {
        onMonkeyGripped?.Invoke(monkey, gripped);
    }

    public void FireMonkeySelection(Monkey monkey)
    {
        onMonkeySelection?.Invoke(monkey);
    }
    
    #endregion
}
