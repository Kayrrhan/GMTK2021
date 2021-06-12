using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpawn : MonoBehaviour
{
    public Transform spawn;
    public GameObject monkeyPrefab;
    [SerializeField]
    GameObject lastInstance;

    void Start()
    {
        lastInstance = monkeyPrefab;
    }
    void OnTriggerExit(Collider other){
        if (other.gameObject == lastInstance)
            lastInstance = Instantiate(monkeyPrefab,new Vector3(0,0,0),Quaternion.identity);
    }
    
    
}
