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

    [Header("Auto Monkeys")]
    [SerializeField]
    GameObject _autoMonkeyPrefab = null;

    [SerializeField]
    Vector3 _autoMonkeyDirection;

    [SerializeField]
    float _delayBetweenAutoMonkeys = 1f;

    [Inject]
    PlayerManager _playerManager = null;

    [Inject]
    EventManager _eventManager = null;

    public LevelBehaviour _levelBehaviour = null;


    int _count = 0;

    bool _autoRun = false;

    #region public properties

    public static AutoSpawn instance { get; private set; } = null; // je crée un singleton parce qu'apparemment

    #endregion

    void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        _eventManager.onAutoRunTriggered.AddListener(AutoRun);
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        _eventManager.onAutoRunTriggered.RemoveListener(AutoRun);
    }

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

    IEnumerator AutoRunSpawnCoroutine()
    {
        int count = LevelBehaviour.instance.LeftMonkeys;
        Debug.Log("count = " + count);
        for (int i=0; i<count; ++i)
        {
            var go = Instantiate(_autoMonkeyPrefab, spawn.position, Quaternion.identity);
            var monkey = go.GetComponent<AutoMonkey>();
            monkey.Run(_autoMonkeyDirection);
            yield return new WaitForSeconds(_delayBetweenAutoMonkeys);
        }
    }

    IEnumerator CheckOnGroundCoroutine(Monkey monkey)
    {
        while (!monkey.IsGrounded())
        {
            yield return new WaitForFixedUpdate();
        }
        // kinematic ...
    }

    public void AutoRun()
    {
        if (_autoRun)
        {
            return;
        }

        _autoRun = true;
        _eventManager.FireAutoRunStarted();
        Debug.Log("AUTORUN STARTED");
        StartCoroutine(AutoRunSpawnCoroutine());
    }

}
