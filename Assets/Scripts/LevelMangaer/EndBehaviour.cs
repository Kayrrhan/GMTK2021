using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBehaviour : MonoBehaviour
{
    [SerializeField]
    LevelBehaviour _levelBehaviour;

    private void OnCollisionEnter(Collision collision)
    {
        _levelBehaviour.MonkeyFinish();
        Debug.Log(collision.gameObject.name);
        Destroy(collision.gameObject);
    }
}
