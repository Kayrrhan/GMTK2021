using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class AutoSpawn : MonoBehaviour
{
    public Transform spawn;
    public GameObject monkeyPrefab;
    public GameObject lastInstance;

    
    [Inject]
    PlayerManager _playerManager = null;

    void Start()
    {
        lastInstance = Instantiate(monkeyPrefab,spawn.position,Quaternion.identity);
        _playerManager.selectedMonkey = lastInstance.GetComponent<Monkey>();
    }
    void OnTriggerExit(Collider other){
        if (other.gameObject == lastInstance)
            lastInstance = Instantiate(monkeyPrefab,spawn.position,Quaternion.identity);
    }
    
    
}
