using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SelectMenuUI;
using UnityEngine.EventSystems;

public class GetModelHandler : MonoBehaviour
{
    public LaserPointer laserPointer;
    public Transform candidateModels;
    public Transform parentModel;

    void Awake()
    {
        laserPointer.PointerClick += PointerClick;

        laserPointer.PointerDragDown += ProcessPressDown;
        laserPointer.PointerDrag += ProcessDrag;
        laserPointer.PointerDragUp += ProcessPressUp;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        GameObject model = candidateModels.Find(e.target.name).Find(e.target.name).gameObject;
        Instantiate(model, e.hit.transform.forward / 8 + transform.position, Quaternion.identity , parentModel);
        //Instantiate(model, transform.position, transform.rotation, parentModel);


        //Rigidbody newModelRig = newModel.AddComponent<Rigidbody>();
        //newModelRig.useGravity = false;
        //MeshCollider newModelCol = newModel.AddComponent<MeshCollider>();
        //newModelCol.convex = true;
        //newModelCol.isTrigger = true;
    }


    protected void ProcessDrag(object sender, Pointer3DEventData eventData)
    {
        //Debug.Log(eventData.pointerDrag);
        if (eventData.pointerDrag != null && !eventData.dragging)// && ShouldStartDrag(eventData)
        {
            //Debug.Log(eventData.pointerDrag);
            ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.beginDragHandler);//IBeginDragHandler
            eventData.dragging = true;
        }

        // Drag notification
        if (eventData.dragging && eventData.pointerDrag != null)
        {
            //Debug.Log(eventData.pointerDrag);
            ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.dragHandler);//IDragHandler
        }

    }

    protected void ProcessPressDown(object sender, Pointer3DEventData eventData)
    {
        //Debug.Log(eventData.enterEventCamera);
        //Debug.Log(eventData.pressEventCamera);

        var currentOverGo = eventData.pointerCurrentRaycast.gameObject;
        //Debug.Log(currentOverGo);
        //eventData.pressPrecessed = true;
        eventData.eligibleForClick = true;
        eventData.delta = Vector2.zero;
        eventData.dragging = false;
        eventData.useDragThreshold = true;
        eventData.pressPosition = eventData.position;
        eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;

        // Save the drag handler as well
        eventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

        if (eventData.pointerDrag != null)
        {
            ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.initializePotentialDrag);//IInitializePotentialDragHandler
        }

    }

    protected void ProcessPressUp(object sender, Pointer3DEventData eventData)
    {
        var currentOverGo = eventData.pointerCurrentRaycast.gameObject;
        //Debug.Log(currentOverGo);
        ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

        // see if we mouse up on the same element that we clicked on...
        var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

        // PointerClick and Drop events
        if (eventData.pointerPress == pointerUpHandler && eventData.eligibleForClick)
        {
            ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerClickHandler);
        }
        else if (eventData.pointerDrag != null && eventData.dragging)
        {
            ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.dropHandler);
        }

        //eventData.pressPrecessed = false;
        eventData.eligibleForClick = false;
        eventData.pointerPress = null;
        eventData.rawPointerPress = null;

        if (eventData.pointerDrag != null && eventData.dragging)
        {
            ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);//IEndDragHandler
        }

        eventData.dragging = false;
        eventData.pointerDrag = null;
    }
    
}
