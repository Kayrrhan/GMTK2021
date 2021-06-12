using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainBehaviour : MonoBehaviour
{
    #region public properties
    #endregion

    #region private methods

    private bool firstCollision = true;

    void TempChain(Collision collision)
    {
        // TODO : Rewrite when monkey behaviour finished
        if (firstCollision)
        {
            // Find the collision parent
            GameObject connectedGO = collision.gameObject;
            // Add the joint
            connectedGO.AddComponent<HingeJoint>();
            // Configure the joint
            HingeJoint hingeJoint = connectedGO.GetComponent<HingeJoint>();
            hingeJoint.autoConfigureConnectedAnchor = false;
            hingeJoint.axis = new Vector3(0,0,1);
            hingeJoint.connectedBody = gameObject.GetComponent<Rigidbody>();
            hingeJoint.connectedAnchor = new Vector3(0, 3.5f, 0);
            JointSpring spring = hingeJoint.spring;
            spring.damper = 5.0f;
            hingeJoint.spring = spring;
            hingeJoint.useSpring = true;
            gameObject.GetComponent<Rigidbody>().useGravity = true;

            firstCollision = false;
        }
    }

    #endregion
}
