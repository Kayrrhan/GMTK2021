using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpawn : MonoBehaviour
{
    public Transform spawn;
    public GameObject monkeyPrefab;
    GameObject lastInstance;

    void Start()
    {
        lastInstance = Instantiate(monkeyPrefab,spawn.position,Quaternion.identity);
    }
    void OnTriggerExit(Collider other){
        if (other.gameObject == lastInstance)
            lastInstance = Instantiate(monkeyPrefab,spawn.position,Quaternion.identity);
    }
    
    
}
