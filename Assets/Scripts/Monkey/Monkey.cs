using UnityEngine;

public class Monkey : MonoBehaviour
{
    #region inspector

    [SerializeField]
    BoxCollider _gripCollider = null;

    #endregion

    #region private members

    Rigidbody _rigidbody = null;

    Collider _collider = null;

    #endregion

    #region public properties

    public new Rigidbody rigidbody => _rigidbody;

    public new Collider collider => _collider;

    public BoxCollider gripCollider => _gripCollider;

    public HingeJoint gripJoint { get; set; } = null;

    #endregion

    #region Unity messages

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        // Make sure grip collider is trigger.
        _gripCollider.isTrigger = true;
    }
    #endregion
}