using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AnimationState = Monkey.AnimationState;

public class AutoMonkey : MonoBehaviour
{
    #region inspector

    [SerializeField]
    float _speed = 8f;

    [SerializeField]
    [Range(0f, 1f)]
    float _speedModifierInAir = 0.5f;

    [SerializeField]
    Animator _animator = null;

    [SerializeField]
    Transform _model = null;

    #endregion

    #region private members

    Vector3 _direction;

    #endregion

    #region public properties

    public new Rigidbody rigidbody { get; private set; }

    public new Collider collider { get; private set; }

    public AnimationState animationState { get; private set; } = AnimationState.Idle;

    public int side { get; private set; } = 1;

    public bool hasStartedRunning { get; private set; } = false;

    #endregion

    #region Unity messages

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

    }

    void Update() {
        if (UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Run(Vector3.right);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        OnCollide(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        OnCollide(collision.collider);
    }
    
    #endregion

    #region private methods

    void OnCollide(Collider other)
    {
        Monkey monkey = other.GetComponent<Monkey>();
        if (monkey != null && monkey.typemonkey == Monkey.TestType.PATH)
        {
            _direction = new Vector3(_direction.x * -1, _direction.y, _direction.z);
        }
    }

    void SetSide(int i)
    {
        side = i >= 0 ? 1 : -1;
        _model.localScale = new Vector3(side, 1, 1);
    }

    void SetAnimationState(AnimationState state)
    {
        if (animationState == state)
        {
            return;
        }

        animationState = state;
        _animator.Play(state.ToString());
    }

    public bool IsGrounded()
    {
        return Utils.IsGrounded(rigidbody, collider);
    }

    IEnumerator RunCoroutine()
    {
        bool isRunning = true;
        while (isRunning)
        {
            yield return new WaitForFixedUpdate();
            bool isGrounded = IsGrounded();
            Vector3 mvt = _direction.x * Vector3.right * _speed * Time.fixedDeltaTime;
            if (!isGrounded)
            {
                mvt *= _speedModifierInAir;
            }
            bool isMoving = mvt.x != 0f;
            if (isMoving)
            {
                SetSide((int)_direction.x);
            }

            if (isGrounded)
            {
                if (isMoving)
                {
                    SetAnimationState(AnimationState.Walk);
                }
                else
                {
                    SetAnimationState(AnimationState.Idle);
                }
            }
            else
            {
                SetAnimationState(AnimationState.Jump);
            }

            rigidbody.MovePosition(transform.position + mvt);
        }
    }

    #endregion

    #region public methods

    public void Run(Vector3 direction)
    {
        if (hasStartedRunning)
        {
            return;
        }

        hasStartedRunning = true;
        _direction = direction;
        StartCoroutine(RunCoroutine());
    }

    #endregion
}
