using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CallbackCtx = UnityEngine.InputSystem.InputAction.CallbackContext;

public class MonkeyClick : MonoBehaviour
{
    #region inspector

    [SerializeField]
    GameObject _playerManager = null;

    #endregion

    #region private members

    MainControls _controls = null;
    
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake(){
    
        _controls = new MainControls();
        _controls.Main.Select.started += OnMouseClick;
    }

    void OnDestroy(){
    
         _controls.Main.Select.started -= OnMouseClick;
    }

    void OnEnable()
    {
        _controls.Enable();
    }

    void OnDisable()
    {
        _controls.Disable();
    }
    // Update is called once per frame
    void OnMouseClick(CallbackCtx ctx)
    {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(_controls.Main.Mouse.ReadValue<Vector2>());
            if (Physics.Raycast(ray, out hit, 100.0f)){
                    if (hit.transform.gameObject.tag == "Monkey") {
                        // MonkeyMovement MonkeyMovement = _playerManager.GetComponent<MonkeyMovement>();
                        // MonkeyMovement.changeMonkey(hit.transform.gameObject);

                        // Debug.Log(hit.transform.gameObject.name);
                    }
                }
            }
    }
