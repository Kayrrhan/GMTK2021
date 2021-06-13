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
    public List<Monkey.TestType> types;
    
    [Inject]
    PlayerManager _playerManager = null;

    void Start()
    {       
        CreateButtons(buttons,monkeys);
        spawnMonkey(0, true);
    }
    void spawnMonkey(int index, bool selectAuto){
        lastInstance = Instantiate(monkeys[index],spawn.position,Quaternion.identity);
        
        Monkey monkey = lastInstance.GetComponent<Monkey>();

        monkey.typemonkey = types[index];
        if (selectAuto){   
            _playerManager.selectedMonkey = monkey;
        }   
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject == lastInstance)
                spawnMonkey(0, false);
                
    }

    public void CreateButtons(List<Button> buttons,List<GameObject> monkeys){
        for(var i = 0;i<Math.Min(monkeys.Count,buttons.Count);++i){
            int copy = i; //Important
            buttons[copy].onClick.AddListener(()=>SwitchMonkey(copy));
        }
    }
    void SwitchMonkey(int index){
         if (lastInstance != null)
            Destroy(lastInstance);
        spawnMonkey(index, true);
    }

}
