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

    const RigidbodyConstraints FLY_CONSTRAINTS = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

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

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            Monkey monkey = _playerManager.selectedMonkey;
            MonkeyChain chain = new MonkeyChain(monkey);
            chain.GoToRight();
            Debug.Log("Chain starting with : " + chain.current.name);
            foreach (var m in chain.RightToLeft())
            {
                Debug.Log(m.name);
            }
        }
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

        HingeJoint gripJoint = monkey.attachedGripJoint;
        if (gripJoint != null)
        {
            Vector3 anchorPoint = gripJoint.transform.TransformPoint(gripJoint.anchor);
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
            EnableConstraints(newMonkey, !newMonkey.isInChain);
        }
        if (oldMonkey != null && oldMonkey != newMonkey && oldMonkey.typemonkey == Monkey.TestType.COPTERE){
            Rigidbody rb = oldMonkey.rigidbody;
            rb.constraints = FLY_CONSTRAINTS;
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

    static void EnableConstraints(Monkey monkey, bool on)
    {
        monkey.transform.rotation = Quaternion.identity;
        monkey.rigidbody.constraints = on ? REGULAR_CONSTRAINTS : GRIP_CONSTRAINTS;
    }

    bool IsGrounded()
    {
        Monkey monkey = _playerManager.selectedMonkey;
        return monkey.IsGrounded();
    }

    void Grip(GripSide side)
    {
        Monkey monkey = _playerManager.selectedMonkey;

        Monkey neighbour = monkey.GetNeighbour(side);
        if (neighbour != null)
        {
            DetachMonkey(monkey, neighbour, side);
            // Fire event
            _eventManager.FireMonkeyGripped(monkey, false, side);
            return;
        }

        HingeJoint sideJoint =  monkey.GetJoint(side);
        if (monkey.gripJointOnWall != null && monkey.gripJointOnWall == sideJoint)
        {
            ReleaseTarget(side);
            return;
        }

        if (monkey.gripJointOnWall != null)
        {
            return;
        }
        
        BoxCollider gripCollider = monkey.isInChain ? monkey.otherGripCollider : monkey.gripCollider;
        
        int layer = LayerMask.NameToLayer("TempLayer");
        int mask = ~(1 << layer | 1 << LayerMask.NameToLayer("Ignore Raycast") | 1 << LayerMask.NameToLayer("Monkey"));
        // Temporary apply layer to the current monkey.
        int originalLayer = monkey.gameObject.layer;
        monkey.rigidbody.gameObject.layer = layer;
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
        else if (!monkey.isInChain)
        {
            GripMonkey();
        }
    }

    void GripMonkey()
    {
        Monkey monkey = _playerManager.selectedMonkey;
        int layer = LayerMask.NameToLayer("TempLayer");
        int mask = 1 << LayerMask.NameToLayer("Monkey");
        // Temporary apply layer to the current monkey.
        int originalLayer = monkey.gameObject.layer;
        monkey.rigidbody.gameObject.layer = layer;
        BoxCollider gripCollider = monkey.monkeyGripCollider;
        Collider[] colliders = Physics.OverlapBox(gripCollider.bounds.center, gripCollider.size * 0.5f,
            gripCollider.transform.rotation, mask, QueryTriggerInteraction.Ignore);
        // Restore layer
        monkey.gameObject.layer = originalLayer;
        
        foreach (var coll in colliders)
        {
            Monkey target = coll.GetComponent<Monkey>();
            if (target != null && target != monkey)
            {
                AttachMonkey(monkey, target);
                break;
            }
        }
    }

    void AttachMonkey(Monkey monkeyToAttach, Monkey monkeyTarget)
    {
        // Get monkey at end of the chain
        MonkeyChain chain = new MonkeyChain(monkeyTarget);
        var endMonkey = chain.MonkeyAtRight();
        if (!endMonkey.isInChain)
        {
            return;
        }
        if (!endMonkey.isFreeToGrip)
        {
            endMonkey = chain.MonkeyAtLeft();
            if (!endMonkey.isFreeToGrip)
            {
                Debug.Log("Chain is full");
                // The chain is full.
                return;
            }
        }

        if (endMonkey == monkeyToAttach)
        {
            Debug.LogError("WTF ????");
            return;
        }

        // Move to the end of the chain
        monkeyToAttach.transform.position = endMonkey.transform.position;
        // Create joint
        EnableConstraints(monkeyToAttach, false);
        HingeJoint hingeJoint = endMonkey.gameObject.AddComponent<HingeJoint>();
        hingeJoint.connectedBody = monkeyToAttach.rigidbody;
        hingeJoint.axis = Vector3.forward;
        Vector3 attachPoint = endMonkey.rightHand.position;
        hingeJoint.anchor = endMonkey.transform.InverseTransformPoint(attachPoint);
        hingeJoint.enableCollision = true;
        hingeJoint.autoConfigureConnectedAnchor = false;
        hingeJoint.connectedAnchor = monkeyToAttach.transform.InverseTransformPoint(monkeyToAttach.anchor.position);
        JointSpring spring = hingeJoint.spring;
        spring.damper = 5.0f;
        hingeJoint.spring = spring;
        hingeJoint.useSpring = true;

        monkeyToAttach.SetAnimationState(Monkey.AnimationState.Grip);

        GripSide side;
        // Chain logic
        if (endMonkey.isLeftSideGrip)
        {
            endMonkey.rightMonkey = monkeyToAttach;
            monkeyToAttach.leftMonkey = endMonkey;
            side = GripSide.Right;
        }
        else
        {
            endMonkey.leftMonkey = monkeyToAttach;
            monkeyToAttach.rightMonkey = endMonkey;
            side = GripSide.Left;
        }

        // Fire event
        _eventManager.FireMonkeyGripped(monkeyToAttach, true, side);
    }

    static void CheckChainDestruction(Monkey monkey)
    {
        MonkeyChain chain = new MonkeyChain(monkey);
        Monkey left = chain.MonkeyAtLeft();
        Monkey right = chain.MonkeyAtRight();

        // If neither tip of the chain is gripped to something then the chain explodes.
        if (!left.isLeftSideGrip && !right.isRightSideGrip)
        {
            // Explode chain
            chain.GoToLeft();
            List<Monkey> monkeysInChain = new List<Monkey>(chain.LeftToRight());
            foreach (var m in monkeysInChain)
            {
                if (m.jointOnMonkey != null)
                {
                    Destroy(m.jointOnMonkey);
                }
                m.rightMonkey = null;
                m.leftMonkey = null;

                Debug.Log(m.name + " explode chain");
                m.SetAnimationState(Monkey.AnimationState.Idle);
                EnableConstraints(m, true);
            }
        }
    }

    void ReleaseTarget(GripSide side)
    {
        Monkey monkey = _playerManager.selectedMonkey;
        Destroy(monkey.gripJointOnWall);
        monkey.gripJointOnWall = null;
        if (monkey.isInChain)
        {
            CheckChainDestruction(monkey);
        }
        if (!monkey.isInChain)
        {
            EnableConstraints(monkey, true);
        }
        _eventManager.FireMonkeyGripped(monkey, false, side);
    }

    void AttachToTarget(Rigidbody target)
    {
        Monkey monkey = _playerManager.selectedMonkey;
        Vector3 anchorPos = monkey.isInChain ? monkey.otherAnchor.position : monkey.anchor.position;

        EnableConstraints(monkey, false);
        monkey.gripJointOnWall = target.gameObject.AddComponent<HingeJoint>();
        monkey.gripJointOnWall.connectedBody = monkey.rigidbody;
        monkey.gripJointOnWall.axis = Vector3.forward;
        Vector3 attachPoint = target.ClosestPointOnBounds(monkey.transform.position);
        monkey.gripJointOnWall.anchor = target.transform.InverseTransformPoint(attachPoint);
        monkey.gripJointOnWall.enableCollision = true;
        monkey.gripJointOnWall.autoConfigureConnectedAnchor = false;
        monkey.gripJointOnWall.connectedAnchor = monkey.transform.InverseTransformPoint(anchorPos);

        monkey.SetAnimationState(Monkey.AnimationState.Grip);

        GripSide side = monkey.side >= 0 ? GripSide.Right : GripSide.Left;
        _eventManager.FireMonkeyGripped(monkey, true, side);
    }

    #endregion

    #region public methods

    public static void DetachMonkey(Monkey monkeyToDetach, Monkey monkeyTarget, GripSide side)
    {
        // Get correct joint and destroy it
        HingeJoint jointToDestroy = null;
        if (monkeyTarget.jointOnMonkey != null && 
            monkeyTarget.jointOnMonkey.connectedBody == monkeyToDetach.rigidbody)
        {
            jointToDestroy = monkeyTarget.jointOnMonkey;
        }
        else if (monkeyToDetach.jointOnMonkey != null && 
            monkeyToDetach.jointOnMonkey.connectedBody == monkeyTarget.rigidbody)
        {
            jointToDestroy = monkeyToDetach.jointOnMonkey;
        }

        if (jointToDestroy != null)
        {
            Destroy(jointToDestroy);
        }

        // Chain logic
        if (side == GripSide.Right)
        {
            monkeyToDetach.rightMonkey = null;
            monkeyTarget.leftMonkey = null;
        }
        else
        {
            monkeyToDetach.leftMonkey = null;
            monkeyTarget.rightMonkey = null;
        }

        CheckChainDestruction(monkeyToDetach);
        CheckChainDestruction(monkeyTarget);

        if (!monkeyToDetach.isInChain)
        {
            EnableConstraints(monkeyToDetach, true);
        }
        if (!monkeyTarget.isInChain)
        {
            EnableConstraints(monkeyTarget, true);
        }
    }

    #endregion
}
