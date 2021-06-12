using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

using CallbackCtx = UnityEngine.InputSystem.InputAction.CallbackContext;

public class MonkeyMovement : MonoBehaviour
{
    #region inspector

    [SerializeField]
    float _speed = 8f;

    [SerializeField]
    float _jumpForce = 10f;

    [SerializeField]
    float _swingForce = 1f;

    #endregion

    #region private members

    [Inject]
    EventManager _eventManager = null;

    [Inject]
    PlayerManager _playerManager = null;

    MainControls _controls = null;

    Vector2 _movement;

    bool _shouldJump = false;

    const RigidbodyConstraints REGULAR_CONSTRAINTS = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

    const RigidbodyConstraints GRIP_CONSTRAINTS = RigidbodyConstraints.FreezePositionZ;

    #endregion

    #region Unity messages

    void Awake()
    {
        _controls = new MainControls();
        _controls.Main.Movement.performed += OnMovement;
        _controls.Main.Movement.canceled += OnMovement;
        _controls.Main.Jump.started += OnJump;

        _eventManager.onMonkeyGrip.AddListener(Grip);
        _eventManager.onMonkeySelected.AddListener(OnMonkeySelected);
    }

    void OnDestroy()
    {
        _controls.Main.Movement.performed -= OnMovement;
        _controls.Main.Movement.canceled -= OnMovement;
        _controls.Main.Jump.started -= OnJump;

        _eventManager.onMonkeyGrip.RemoveListener(Grip);
        _eventManager.onMonkeySelected.RemoveListener(OnMonkeySelected);
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
        Monkey monkey = _playerManager.selectedMonkey;
        if (monkey == null)
        {
            return;
        }

        Transform tr = monkey.transform;
        Rigidbody rb = monkey.rigidbody;
        bool isGrounded = IsGrounded();

        if (monkey.gripJoint != null)
        {
            Vector3 anchorPoint = monkey.gripJoint.transform.TransformPoint(monkey.gripJoint.anchor);
            Vector3 dir = Vector3.Cross(anchorPoint - tr.position, Vector3.forward).normalized;
            rb.AddForce(dir * _movement.x * _swingForce, ForceMode.Impulse);
            return;
        }

        Vector3 mvt = _movement.x * Vector3.right * _speed * Time.fixedDeltaTime;
        bool isMoving = mvt.x != 0f;
        if (isMoving)
        {
            monkey.SetSide((int)_movement.x);
        }

        if (isGrounded)
        {
            if (isMoving)
            {
                monkey.SetAnimationState(Monkey.AnimationState.Walk);
            }
            else
            {
                monkey.SetAnimationState(Monkey.AnimationState.Idle);
            }
        }
        else
        {
            monkey.SetAnimationState(Monkey.AnimationState.Jump);
        }

        rb.MovePosition(tr.position + mvt);

        if (_shouldJump)
        {
            _shouldJump = false;
            if (isGrounded)
            {
                rb.AddForce(_jumpForce * rb.mass * Vector3.up, ForceMode.Impulse);
            }
        }
    }

    #endregion

    #region private methods

    void OnMonkeySelected(Monkey oldMonkey, Monkey newMonkey)
    {
        if (newMonkey != null)
        {
            EnableConstraints(newMonkey, newMonkey.gripJoint == null);
        }
        if (oldMonkey != null && oldMonkey.animationState != Monkey.AnimationState.Grip)
        {
            oldMonkey.SetAnimationState(Monkey.AnimationState.Idle);
        }
    }

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

    void EnableConstraints(Monkey monkey, bool on)
    {
        monkey.transform.rotation = Quaternion.identity;
        monkey.rigidbody.constraints = on ? REGULAR_CONSTRAINTS : GRIP_CONSTRAINTS;
    }

    bool IsGrounded()
    {
        Monkey monkey = _playerManager.selectedMonkey;
        if (Mathf.Abs(monkey.rigidbody.velocity.y) > 0.1f)
        {
            return false;
        }

        int layer = LayerMask.NameToLayer("TempLayer");
        int mask = ~(1 << layer | 1 << LayerMask.NameToLayer("Ignore Raycast"));
        Bounds bounds = monkey.collider.bounds;
        // Temporary apply layer to the current monkey.
        int originalLayer = monkey.gameObject.layer;
        monkey.rigidbody.gameObject.layer = layer;
        bool res = Physics.CheckCapsule(bounds.center, bounds.center + (bounds.extents.y + 0.1f) * Vector3.down, 
            bounds.size.x, mask, QueryTriggerInteraction.Ignore);
        // Restore layer
        monkey.gameObject.layer = originalLayer;
        return res;
    }

    void Grip()
    {
        Monkey monkey = _playerManager.selectedMonkey;

        if (monkey.gripJoint != null)
        {
            ReleaseTarget();
            return;
        }
        
        int layer = LayerMask.NameToLayer("TempLayer");
        int mask = ~(1 << layer | 1 << LayerMask.NameToLayer("Ignore Raycast"));
        // Temporary apply layer to the current monkey.
        int originalLayer = monkey.gameObject.layer;
        monkey.rigidbody.gameObject.layer = layer;
        BoxCollider gripCollider = monkey.gripCollider;
        Collider[] colliders = Physics.OverlapBox(gripCollider.bounds.center, gripCollider.size * 0.5f,
            gripCollider.transform.rotation, mask, QueryTriggerInteraction.Ignore);
        // Restore layer
        monkey.gameObject.layer = originalLayer;

        Rigidbody target = null;
        foreach (var coll in colliders)
        {
            if (coll.attachedRigidbody != null)
            {
                target = coll.attachedRigidbody;
                break;
            }
        }
        
        if (target != null)
        {
            AttachToTarget(target);
        }
    }

    void ReleaseTarget()
    {
        Monkey monkey = _playerManager.selectedMonkey;
        Destroy(monkey.gripJoint);
        monkey.gripJoint = null;
        _eventManager.FireMonkeyGripped(monkey, false);
        EnableConstraints(monkey, true);
    }

    void AttachToTarget(Rigidbody target)
    {
        Monkey monkey = _playerManager.selectedMonkey;
        EnableConstraints(monkey, false);
        monkey.gripJoint = target.gameObject.AddComponent<HingeJoint>();
        monkey.gripJoint.connectedBody = monkey.rigidbody;
        monkey.gripJoint.axis = Vector3.forward;
        Vector3 attachPoint = target.ClosestPointOnBounds(monkey.transform.position);
        monkey.gripJoint.anchor = target.transform.InverseTransformPoint(attachPoint);
        monkey.gripJoint.enableCollision = true;
        monkey.gripJoint.autoConfigureConnectedAnchor = false;
        monkey.gripJoint.connectedAnchor = monkey.transform.InverseTransformPoint(monkey.anchor.position);

        monkey.SetAnimationState(Monkey.AnimationState.Grip);

        _eventManager.FireMonkeyGripped(monkey, true);
    }

    #endregion
}
