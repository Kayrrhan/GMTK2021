using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Zenject;


public class AutoSpawn : MonoBehaviour
{
    public Transform spawn;
    public List<GameObject> monkeys;
    public List<Button> buttons;
    public GameObject lastInstance;
    
    [Inject]
    PlayerManager _playerManager = null;

    void Start()
    {
        lastInstance = Instantiate(monkeys[0],spawn.position,Quaternion.identity);       
        CreateButtons(buttons,monkeys);
        _playerManager.selectedMonkey = lastInstance.GetComponent<Monkey>();
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject == lastInstance)
                lastInstance = Instantiate(other.gameObject,spawn.position,Quaternion.identity);
                
    }

    public void CreateButtons(List<Button> buttons,List<GameObject> monkeys){
        for(var i = 0;i<Math.Min(monkeys.Count,buttons.Count);++i){
            int copy = i; //Important
            buttons[copy].onClick.AddListener(()=>SwitchMonkey(monkeys[copy]));
        }
    }
    void SwitchMonkey(GameObject monkey){
         if (lastInstance != null)
            Destroy(lastInstance);
        lastInstance = Instantiate(monkey,spawn.position,Quaternion.identity);
    }

}
