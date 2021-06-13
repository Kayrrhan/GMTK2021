using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBehaviour : MonoBehaviour
{
    [SerializeField]
    LevelBehaviour currentLevelBehaviour;

    private void OnTriggerEnter(Collider collider)
    {
        AutoMonkey monkey = collider.gameObject.GetComponent<AutoMonkey>();
        if (monkey != null)
        {
            currentLevelBehaviour.MonkeyFinish();
            Debug.Log(collider.gameObject.name);
            Destroy(collider.gameObject);
        }
    }
}
