using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Zenject;


public class AutoSpawn : MonoBehaviour
{
    public Transform spawn;
    
    [System.NonSerialized]
    public GameObject lastInstance;

    public List<GameObject> monkeys;
    public List<Button> buttons;

    public List<Monkey.TestType> types;

    [Inject]
    PlayerManager _playerManager = null;

    public LevelBehaviour _levelBehaviour = null;


    int _count = 0;

    void Start()
    {       
        CreateButtons(buttons,monkeys);
        spawnMonkey(0, true);
    }
    void spawnMonkey(int index, bool selectAuto){
        if(_levelBehaviour.SpawnMonkey())
        {
            GameObject currentMonkey  =  monkeys[index];
            lastInstance = Instantiate(currentMonkey,spawn.position,Quaternion.identity);
            lastInstance.name = currentMonkey.name + $" {++_count} ";   
            Monkey monkey = lastInstance.GetComponent<Monkey>();

            monkey.typemonkey = types[index];
            if (selectAuto){   
                _playerManager.selectedMonkey = monkey;
            }
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
