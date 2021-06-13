using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBehaviour : MonoBehaviour
{
    [SerializeField]
    LevelBehaviour currentLevelBehaviour;

    private void OnCollisionEnter(Collision collision)
    {
        currentLevelBehaviour.MonkeyFinish();
        Debug.Log(collision.gameObject.name);
        Destroy(collision.gameObject);
    }
}
