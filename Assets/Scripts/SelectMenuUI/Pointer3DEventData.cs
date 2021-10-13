using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer3DEventData : PointerEventData
{

    public Vector3 position3D;
    public Quaternion rotation;

    public Vector3 position3DDelta;
    public Quaternion rotationDelta;

    public Vector3 pressPosition3D;
    public Quaternion pressRotation;

    public float pressDistance;
    public GameObject pressEnter;
    public bool pressPrecessed;

    public Pointer3DEventData(EventSystem eventSystem) : base(eventSystem)
    {
        //Pointer3DInputModule.AssignPointerId(this);
    }



}
