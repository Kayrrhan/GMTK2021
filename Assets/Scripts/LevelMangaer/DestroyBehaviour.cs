using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBehaviour : MonoBehaviour
{
    public LevelBehaviour _levelBehaviour = null;

    private void OnCollisionEnter(Collision collision)
    {
        _levelBehaviour.MonkeyDied();
        Debug.Log(collision.gameObject.name);
        Destroy(collision.gameObject);
    }
}
