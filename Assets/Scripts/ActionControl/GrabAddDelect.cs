using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAddDelect : MonoBehaviour
{
    public SceneSDFArea scenesdfArea;
    public void SetCollidingObject(Collider col, bool rightOrLeft)
    {
        if (rightOrLeft)
        {
            if (scenesdfArea.colliderLeft || !col.GetComponent<Rigidbody>())
            {
                return;
            }
            scenesdfArea.colliderLeft = col.GetComponent<MeshFilter>();
        }
        else
        {
            if (scenesdfArea.colliderRight || !col.GetComponent<Rigidbody>())
            {
                return;
            }
            scenesdfArea.colliderRight = col.GetComponent<MeshFilter>();
        }
    }

    public void ReleaseCollidingObject(Collider col, bool rightOrLeft)
    {
        if (rightOrLeft)
        {
            if (!scenesdfArea.colliderLeft) return;
            scenesdfArea.colliderLeft = null;
        }
        else
        {
            if (!scenesdfArea.colliderRight) return;
            scenesdfArea.colliderRight = null;
        }
    }

    public bool GrabObject(bool rightOrLeft)
    {
        return scenesdfArea.GrabObject(rightOrLeft);
    }

    public void ReleaseObject(bool rightOrLeft)
    {
        scenesdfArea.ReleaseObject(rightOrLeft);
    }

    public bool AddOperation(BooleanType type, bool rightOrLeft)
    {
        return scenesdfArea.AddOperation(type, rightOrLeft);
    }

    public bool DelectOperation(bool rightOrLeft)
    {
        return scenesdfArea.DelectOperation(rightOrLeft);
    }

}
