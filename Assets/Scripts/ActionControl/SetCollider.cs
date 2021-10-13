using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCollider : MonoBehaviour
{

    public GrabAddDelect grabAddDelect;
    public bool flag;

    //1 When the trigger collider enters another, this sets up the other collider as a potential grab target.
    public void OnTriggerEnter(Collider other)
    {
        grabAddDelect.SetCollidingObject(other, flag);
    }

    //2 Similar to section one (// 1), but different because it ensures that the target is set when the player holds a controller over an object for a while. Without this, the collision may fail or become buggy.???
    public void OnTriggerStay(Collider other)
    {
        grabAddDelect.SetCollidingObject(other, flag);
    }

    //3 When the collider exits an object, abandoning an ungrabbed target, this code removes its target by setting it to null.
    public void OnTriggerExit(Collider other)
    {
        grabAddDelect.ReleaseCollidingObject(other, flag);
    }

    public FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }
}
