using UnityEngine;
using Zenject;

public class Monkey : MonoBehaviour
{
    public enum AnimationState
    {
        Idle,
        Walk,
        Jump,
        Grip,
        Stop
    }

    public enum TestType 
    {
        NORMAL,
        COPTERE,

        PATH
    }

    #region inspector

    [SerializeField]
    BoxCollider _gripCollider = null;

    [SerializeField]
    BoxCollider _otherGripCollider = null;

    [SerializeField]
    BoxCollider _monkeyGripCollider = null;

    [SerializeField]
    Transform _model = null;

    [SerializeField]
    Animator _animator = null;

    [Header("Grip parameters")]
    [SerializeField]
    Transform _anchor = null;

    [SerializeField]
    Transform _otherAnchor = null;

    [SerializeField]
    Transform _leftHand = null;

    [SerializeField]
    Transform _rightHand = null;

    [Header("Animations")]
    [SerializeField]
    GameObject _stopSign = null;

    #endregion

    #region private members

    HingeJoint _jointOnMonkey = null;

    Rigidbody _rigidbody = null;

    Collider _collider = null;

    AnimationState _animationState = AnimationState.Idle;

    int _side = 1;


    #endregion

    #region public properties

    public new Rigidbody rigidbody => _rigidbody;

    public new Collider collider => _collider;

    public BoxCollider gripCollider => _gripCollider;

    public BoxCollider otherGripCollider => _otherGripCollider;

    public BoxCollider monkeyGripCollider => _monkeyGripCollider;

    public HingeJoint gripJointOnWall { get; set; } = null;

    /// <summary>
    /// Cached version of this.GetComponent(typeof(HingeJoint)). != attachedGripJoint.
    /// </summary>
    /// <value></value>
    public HingeJoint jointOnMonkey
    {
        get
        {
            if (_jointOnMonkey == null)
            {
                _jointOnMonkey = GetComponent<HingeJoint>();
            }
            return _jointOnMonkey;
        }
    }

    /// <summary>
    /// The joint attached to the monkey. Not equal to this.GetComponent(typeof(HingeJoint))
    /// </summary>
    public HingeJoint attachedGripJoint
    {
        get
        {
            var joint = rightGripJoint;
            if (joint != null && joint.connectedBody == rigidbody)
            {
                return joint;
            }
            joint = leftGripJoint;
            if (joint != null && joint.connectedBody == rigidbody)
            {
                return joint;
            }
            return null;
        }
    }

    public HingeJoint rightGripJoint 
    {
        get
        {
            if (rightMonkey != null)
            {
                return rightMonkey.jointOnMonkey;
            }

            if (leftMonkey == null)
            {
                return side >= 0 ? gripJointOnWall : null;
            }
            return gripJointOnWall;
        }
    }

    public HingeJoint leftGripJoint
    {
        get
        {
            if (leftMonkey != null)
            {
                return leftMonkey.jointOnMonkey;
            }

            if (rightMonkey == null)
            {
                return side < 0 ? gripJointOnWall : null;
            }
            return gripJointOnWall;
        }
    }

    public AnimationState animationState => _animationState;

    public Transform rightHand => _rightHand;

    public Transform leftHand => _leftHand;

    public Transform anchor => _anchor;


    [SerializeField]
    public TestType typemonkey { get; set; } = TestType.NORMAL;

    public Transform otherAnchor => _otherAnchor;

    public bool isRightSideGrip => rightMonkey != null || rightGripJoint != null;

    public bool isLeftSideGrip => leftMonkey != null || leftGripJoint != null;

    public bool isFreeToGrip => !isRightSideGrip || !isLeftSideGrip;

    public int side => _side;

    public Monkey rightMonkey { get; set; } = null;

    public Monkey leftMonkey { get; set; } = null;

    public bool isInChain => gripJointOnWall != null || rightMonkey != null || leftMonkey != null;

    #endregion

    #region Unity messages

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        // Make sure grip collider is trigger.
        _gripCollider.isTrigger = true;
        _otherGripCollider.isTrigger = true;
        _monkeyGripCollider.isTrigger = true;
        if (_stopSign != null)
        {
            _stopSign.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (rightMonkey)
        {
            MonkeyMovement.DetachMonkey(rightMonkey, this, GripSide.Left);
        }
        if (leftMonkey)
        {
            MonkeyMovement.DetachMonkey(leftMonkey, this, GripSide.Right);
        }
        if (PlayerManager.instance.selectedMonkey == this){
            PlayerManager.instance.selectedMonkey = AutoSpawn.instance.lastInstance.GetComponent<Monkey>();
        }
    }

    void Update()
    {
        if (PlayerManager.instance.selectedMonkey != this && animationState != AnimationState.Grip)
        {
            if (IsGrounded())
            {
                SetAnimationState(AnimationState.Idle);
            }
            else
            {
                SetAnimationState(AnimationState.Jump);
            }
        }
    }

    void LateUpdate()
    {
        var joint = attachedGripJoint;
        if (_animationState == AnimationState.Grip && joint != null)
        {
            // It's always the left hand because we invert the horizontal scale of the model.
            _leftHand.position = joint.transform.TransformPoint(joint.anchor);

            if (IsEndOfFullChain())
            {
                _rightHand.position = gripJointOnWall.transform.TransformPoint(gripJointOnWall.anchor);
            }
        }
    }

    #endregion

    #region public methods

    public HingeJoint GetJoint(GripSide side) => side == GripSide.Left ? leftGripJoint : rightGripJoint;

    public Monkey GetNeighbour(GripSide side) => side == GripSide.Left ? leftMonkey : rightMonkey;

    public bool IsEndOfFullChain()
    {
        if (gripJointOnWall == null)
        {
            return false;
        }

        return (leftMonkey && leftMonkey.jointOnMonkey != null && leftMonkey.jointOnMonkey.connectedBody == rigidbody) ||
            (rightMonkey && rightMonkey.jointOnMonkey != null && rightMonkey.jointOnMonkey.connectedBody == rigidbody);
    }

    public bool IsGrounded()
    {
        return Utils.IsGrounded(rigidbody, collider);
    }

    public void SetSide(int i)
    {
        _side = i >= 0 ? 1 : -1;
        _model.localScale = new Vector3(_side, 1, 1);
        Vector3 localPos = _gripCollider.transform.localPosition;
        _gripCollider.transform.localPosition = new Vector3(Mathf.Abs(localPos.x) * _side, localPos.y, localPos.z);
        localPos = _otherGripCollider.transform.localPosition;
        _otherGripCollider.transform.localPosition = new Vector3(Mathf.Abs(localPos.x) * -_side, localPos.y, localPos.z);
        localPos = _anchor.transform.localPosition;
        _anchor.transform.localPosition = new Vector3(Mathf.Abs(localPos.x) * _side, localPos.y, localPos.z);
    }

    public void SetAnimationState(AnimationState state)
    {
        if (_animationState == state)
        {
            return;
        }

        if (state == AnimationState.Stop && _stopSign != null)
        {
            _stopSign.SetActive(true);
        }
        if (state != AnimationState.Stop && state == AnimationState.Stop && _stopSign != null)
        {
            _stopSign.SetActive(false);
        }

        _animationState = state;
        _animator.Play(_animationState.ToString());
    }

    #endregion
}