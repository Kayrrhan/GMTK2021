using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    #region inspector

    public UnityEvent<GripSide> onMonkeyGrip = null;

    public UnityEvent<Monkey, bool, GripSide> onMonkeyGripped = null;

    public UnityEvent<Monkey, Monkey> onMonkeySelected = null;

    #endregion

    #region public methods

    public void FireMonkeyGrip(GripSide side)
    {
        onMonkeyGrip?.Invoke(side);
    }

    public void FireMonkeyGripped(Monkey monkey, bool gripped, GripSide side)
    {
        onMonkeyGripped?.Invoke(monkey, gripped, side);
    }

    public void FireMonkeySelected(Monkey oldMonkey, Monkey newMonkey)
    {
        onMonkeySelected?.Invoke(oldMonkey, newMonkey);
    }
    
    #endregion
}
