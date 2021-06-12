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

    MainControls _controls = null;
    
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake(){
    
        _controls = new MainControls();
        _controls.Main.Select.started += OnMouseClickLeft;
        _controls.Main.Unselect.started += OnMouseClickRight;
    }

    void OnDestroy(){
    
        _controls.Main.Select.started -= OnMouseClickLeft;
        _controls.Main.Unselect.started -= OnMouseClickRight;
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
    void OnMouseClickLeft(CallbackCtx ctx)
    {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(_controls.Main.Mouse.ReadValue<Vector2>());
            
            if (Physics.Raycast(ray, out hit, 100.0f)){
                    if (hit.transform.gameObject.tag == "Monkey") {
                        _playerManager.selectedMonkey = hit.transform.gameObject.GetComponent<Monkey>();
                    }
            }
    }
    
    void OnMouseClickRight(CallbackCtx ctx)
    {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(_controls.Main.Mouse.ReadValue<Vector2>());
            if (Physics.Raycast(ray, out hit, 100.0f)){
                    if (hit.transform.gameObject.tag == "Monkey") {
                        Destroy(hit.transform.gameObject);
                    }
            }
    }
}
