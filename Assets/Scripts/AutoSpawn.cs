using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


public class AutoSpawn : MonoBehaviour
{
    public Transform spawn;
    public GameObject monkeyPrefab;
    public GameObject lastInstance;

    private bool special;
    
    [Inject]
    PlayerManager _playerManager = null;

    void Start()
    {
        spawnMonkey();
        _playerManager.selectedMonkey = lastInstance.GetComponent<Monkey>();
    }
    void OnTriggerExit(Collider other){
        if (other.gameObject == lastInstance)
            spawnMonkey();
    }
    
    private void spawnMonkey(){
        special = !special;
        lastInstance = Instantiate(monkeyPrefab,spawn.position,Quaternion.identity);
        if (special){
            lastInstance.GetComponent<Monkey>().typemonkey = Monkey.TestType.COPTERE;
        }
    }
}
