using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CallbackCtx = UnityEngine.InputSystem.InputAction.CallbackContext;

public class MonkeyMovement : MonoBehaviour
{
    #region inspector

    [SerializeField]
    Rigidbody _monkeyRb = null;

    [SerializeField]
    float _speed = 8f;

    [SerializeField]
    float _jumpForce = 10f;

    #endregion

    #region private members

    MainControls _controls = null;

    Vector2 _movement;

    bool _shouldJump = false;

    Collider _monkeyCollider = null;

    #endregion

    #region Unity messages

    void Awake()
    {
        _controls = new MainControls();
        _controls.Main.Movement.performed += OnMovement;
        _controls.Main.Movement.canceled += OnMovement;
        _controls.Main.Jump.started += OnJump;

        _monkeyCollider = _monkeyRb.GetComponent<Collider>();
    }

    void OnDestroy()
    {
        _controls.Main.Movement.performed -= OnMovement;
        _controls.Main.Movement.canceled -= OnMovement;
        _controls.Main.Jump.started -= OnJump;
    }

    void OnEnable()
    {
        _controls.Enable();
    }

    void OnDisable()
    {
        _controls.Disable();
    }

    void FixedUpdate()
    {
        if (_monkeyRb == null)
        {
            return;
        }

        Transform tr = _monkeyRb.transform;
        Vector3 mvt = _movement.x * Vector3.right * _speed * Time.fixedDeltaTime;
        _monkeyRb.MovePosition(tr.position + mvt);

        if (_shouldJump)
        {
            _shouldJump = false;
            if (IsGrounded())
            {
                _monkeyRb.AddForce(_jumpForce * _monkeyRb.mass * Vector3.up, ForceMode.Impulse);
            }
        }
    }

    #endregion

    #region private methods

    void OnMovement(CallbackCtx ctx)
    {
        if (ctx.performed)
        {
            _movement = ctx.ReadValue<Vector2>();
        }
        else
        {
            _movement = Vector2.zero;
        }
    }

    void OnJump(CallbackCtx ctx)
    {
        _shouldJump = true;
    }

    bool IsGrounded()
    {
        if (Mathf.Abs(_monkeyRb.velocity.y) > 0.1f)
        {
            return false;
        }

        int layer = LayerMask.NameToLayer("TempLayer");
        int mask = ~(1 << layer);
        Bounds bounds = _monkeyCollider.bounds;
        // Temporary apply layer to the current monkey.
        int originalLayer = _monkeyRb.gameObject.layer;
        _monkeyRb.gameObject.layer = layer;
        bool res = Physics.CheckCapsule(bounds.center, bounds.center + (bounds.extents.y + 0.1f) * Vector3.down, 
            bounds.size.x, mask);
        // Restore layer
        _monkeyRb.gameObject.layer = originalLayer;
        return res;
    }

    #endregion
}
