using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using CallbackCtx = UnityEngine.InputSystem.InputAction.CallbackContext;

public class MonkeyClick : MonoBehaviour
{
    #region inspector

    #endregion

    #region private members

    [Inject]
    PlayerManager _playerManager = null;

    [Inject]
    EventManager _eventManager = null;
    
    MainControls _controls = null;

    AutoSpawn _autospawn;
    
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake(){
    
        _controls = new MainControls();
        _controls.Main.Select.started += OnMouseClickLeft;
        _controls.Main.ExitSelection.started += OnSelectNewSpawned;
        _controls.Main.Unselect.started += OnMouseClickRight;
    }

    void OnDestroy(){
    
        _controls.Main.Select.started -= OnMouseClickLeft;
        _controls.Main.Unselect.started -= OnMouseClickRight;
    }

    void OnEnable()
    {
        _autospawn = GameObject.Find("SpawnArea").GetComponent<AutoSpawn>();
        _controls.Enable();
    }

    void OnDisable()
    {
        _controls.Disable();
    }
    // Update is called once per frame
    void OnMouseClickLeft(CallbackCtx ctx)
    {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(_controls.Main.Mouse.ReadValue<Vector2>());
            
            if (Physics.Raycast(ray, out hit, 100.0f)){
                    if (hit.transform.gameObject.tag == "Monkey") {
                        _playerManager.selectedMonkey = hit.transform.gameObject.GetComponent<Monkey>();
                        _eventManager.FireMonkeySelection(_playerManager.selectedMonkey);
                    }
            }
    }
    
    void OnSelectNewSpawned(CallbackCtx ctx){
        Monkey monkey = _autospawn.lastInstance.GetComponent<Monkey>();
        _playerManager.selectedMonkey = monkey;
        _eventManager.FireMonkeySelection(monkey);
    }

    void OnMouseClickRight(CallbackCtx ctx)
    {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(_controls.Main.Mouse.ReadValue<Vector2>());
            if (Physics.Raycast(ray, out hit, 100.0f)){
                    if (hit.transform.gameObject.tag == "Monkey") {
                        if (_autospawn.lastInstance != hit.transform.gameObject) { 
                            Destroy(hit.transform.gameObject);
                        }
                    }
            }
    }
}
