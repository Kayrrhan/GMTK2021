using UnityEngine;

public class Monkey : MonoBehaviour
{
    public enum AnimationState
    {
        Idle,
        Walk,
        Jump,
        Grip
    }

    #region inspector

    [SerializeField]
    BoxCollider _gripCollider = null;

    [SerializeField]
    Transform _model = null;

    [SerializeField]
    Animator _animator = null;

    [Header("Grip parameters")]
    [SerializeField]
    Transform _anchor = null;

    [SerializeField]
    Transform _leftHand = null;

    [SerializeField]
    Transform _rightHand = null;

    #endregion

    #region private members

    float _gripColliderX;

    float _anchorX;

    Rigidbody _rigidbody = null;

    Collider _collider = null;

    AnimationState _animationState = AnimationState.Idle;

    int _side = 1;

    #endregion

    #region public properties

    public new Rigidbody rigidbody => _rigidbody;

    public new Collider collider => _collider;

    public BoxCollider gripCollider => _gripCollider;

    public HingeJoint gripJoint { get; set; } = null;

    public AnimationState animationState => _animationState;

    public Transform rightHand => _rightHand;

    public Transform leftHand => _leftHand;

    public Transform anchor => _anchor;

    #endregion

    #region Unity messages

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        // Make sure grip collider is trigger.
        _gripCollider.isTrigger = true;
        _gripColliderX = _gripCollider.transform.localPosition.x;
        _anchorX = _anchor.transform.localPosition.x;
    }

    void LateUpdate()
    {
        if (_animationState == AnimationState.Grip && gripJoint != null)
        {
            // It's always the left hand because we invert the horizontal scale of the model.
            _leftHand.position = gripJoint.transform.TransformPoint(gripJoint.anchor);
        }
    }

    #endregion

    #region public methods

    public void SetSide(int i)
    {
        _side = i >= 0 ? 1 : -1;
        _model.localScale = new Vector3(_side, 1, 1);
        Vector3 localPos = _gripCollider.transform.localPosition;
        _gripCollider.transform.localPosition = new Vector3(_gripColliderX * _side, localPos.y, localPos.z);
        localPos = _anchor.transform.localPosition;
        _anchor.transform.localPosition = new Vector3(_anchorX * _side, localPos.y, localPos.z);
    }

    public void SetAnimationState(AnimationState state)
    {
        if (_animationState == state)
        {
            return;
        }

        _animationState = state;
        _animator.Play(_animationState.ToString());
    }

    #endregion
}