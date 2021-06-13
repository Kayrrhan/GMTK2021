using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AutoSpawn : MonoBehaviour
{
    public Transform spawn;
    public List<GameObject> monkeys;
    GameObject _lastInstance;
    public List<Button> buttons;
    void Start(){   
        _lastInstance = Instantiate(monkeys[0],spawn.position,Quaternion.identity);       
        CreateButtons(buttons,monkeys);
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject == _lastInstance)
                _lastInstance = Instantiate(other.gameObject,spawn.position,Quaternion.identity);
                
    }

    public void CreateButtons(List<Button> buttons,List<GameObject> monkeys){
        for(var i = 0;i<Math.Min(monkeys.Count,buttons.Count);++i){
            int copy = i; //Important
            buttons[copy].onClick.AddListener(()=>SwitchMonkey(monkeys[copy]));
        }
    }
    void SwitchMonkey(GameObject monkey){
         if (_lastInstance != null)
             Destroy(_lastInstance);
        _lastInstance = Instantiate(monkey,spawn.position,Quaternion.identity);
    }

}
