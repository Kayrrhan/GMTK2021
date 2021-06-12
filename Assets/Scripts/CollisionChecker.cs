using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionChecker : MonoBehaviour
{
    #region inspector

    public UnityEvent<Collision> onCollisonEnter = null;

    public UnityEvent<Collision> onCollisonStay = null;

    public UnityEvent<Collision> onCollisonExit = null;

    public UnityEvent<Collider> onTriggerEnter = null;

    public UnityEvent<Collider> onTriggerStay = null;

    public UnityEvent<Collider> onTriggerExit = null;
    
    #endregion

    #region Unity messages

    void OnCollisionEnter(Collision other)
    {
        onCollisonEnter?.Invoke(other);
    }

    void OnCollisionStay(Collision other)
    {
        onCollisonStay?.Invoke(other);
    }

    void OnCollisionExit(Collision other)
    {
        onCollisonExit?.Invoke(other);
    }

    void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
    }

    void OnTriggerStay(Collider other)
    {
        onTriggerStay?.Invoke(other);
    }

    void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(other);
    }

    #endregion
}
