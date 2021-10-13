using System.Collections;
using Valve.VR;
using UnityEngine;
using Unity.Profiling;

public class InputController : MonoBehaviour
{
    // HANDS
    [Header("SteamVR Controllers")]
    public SteamVR_Behaviour_Pose primaryHandObject;
    public SteamVR_Behaviour_Pose secondaryHandObject;

    // INPUTS
    [Header("SteamVR Actions")]
    public SteamVR_Action_Boolean unionAddAction;
    public SteamVR_Action_Boolean intersectionAddAction;
    public SteamVR_Action_Boolean subtractAddAction;
    public SteamVR_Action_Boolean delectAction;

    public SteamVR_Action_Boolean grabModelAction;
    public SteamVR_Action_Boolean interactUIAction;

    public SteamVR_Action_Boolean editorAction;//leftHand
    public SteamVR_Action_Boolean updateAction;//rightHand

    public SteamVR_Action_Boolean selectMenuAction;//select model by penal , menu
    public SteamVR_Action_Boolean saveMeshAction;//
    public SteamVR_Action_Boolean destroyModelAction;

    //zoom
    //public SteamVR_Action_Boolean touchAction;
    public SteamVR_Action_Vector2 touchPos;

    public SteamVR_Action_Pose pose;
    public SteamVR_Input_Sources primarySource = SteamVR_Input_Sources.RightHand;
    public SteamVR_Input_Sources secondarySource = SteamVR_Input_Sources.LeftHand;
    public Transform headTransform;

    public SteamVR_Action_Vibration hapticAction;

    // ACTION CONTROLLERS
    [Header("App controllers")]
    public GrabAddDelect grabAddDelectController;
    public SaveMeshController saveMeshController;
    public UpdateMeshController updateMeshController;
    public SelectMenuUIController selectMenuUIController;
    public ZoomModelController zoomModelController;
    public DestoryModelController destoryModelController;

    public AvoidStickingController avoidStickingController;

    [Header("App Functions Tips")]
    public ZoomTips zoomTips;

    private Action currentActionLeft = Action.Idle;
    private Action currentActionRight = Action.Idle;

    private bool waitingForConfirm = false;

    static ProfilerMarker InputControllerMarker = new ProfilerMarker("SceneSDFArea");


    private enum Action
    {
        Idle,
        Grab,
        Zoom
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingForConfirm) return;

        InputControllerMarker.Begin();

        if (currentActionRight.Equals(Action.Idle))
        {//right false; left true
            //right hand
            avoidStickingController.AvoidStickingRight();

            if (grabModelAction.GetLastStateDown(primarySource))
            {
                //currentActionRight = Action.Grab;
                //grabAddDelectController.GrabObject(false);

                if (grabAddDelectController.GrabObject(false))
                {
                    if (currentActionLeft.Equals(Action.Grab))
                    {
                        if (zoomModelController.SameObjectInhand())
                        {
                            currentActionLeft = Action.Zoom;
                            currentActionRight = Action.Zoom;


                            zoomTips.StartZoom();
                            float handsDistance = Vector3.Distance(Pos(primarySource), Pos(secondarySource));
                            zoomModelController.StartZoom(handsDistance);
                        }
                    }
                    else
                    {
                        currentActionRight = Action.Grab;
                    }
                }

            }
            else if (unionAddAction.GetStateDown(primarySource))
            {
                if (grabAddDelectController.AddOperation(BooleanType.Union, false))
                {
                    hapticAction.Execute(0f, 0.1f, 25, 5, primarySource);
                }
            }
            else if (intersectionAddAction.GetStateDown(primarySource))
            {
                if (grabAddDelectController.AddOperation(BooleanType.Intersection, false))
                {
                    hapticAction.Execute(0f, 0.1f, 25, 5, primarySource);
                }
            }
            else if (subtractAddAction.GetStateDown(primarySource))
            {
                if (grabAddDelectController.AddOperation(BooleanType.Subtract, false))
                {
                    hapticAction.Execute(0f, 0.1f, 25, 5, primarySource);
                }
            }
            else if (delectAction.GetStateDown(primarySource))
            {
                if (grabAddDelectController.DelectOperation(false))
                {
                    hapticAction.Execute(0f, 0.1f, 25, 5, primarySource);
                }
            }
            else if (updateAction.GetStateDown(primarySource))
            {
                updateMeshController.UpdateSDFMeshOnce();
            }
            else if (selectMenuAction.GetStateDown(primarySource))
            {
                selectMenuUIController.SwitchToMenuMode();
            }
            else if (destroyModelAction.GetStateDown(primarySource))
            {
                if (destoryModelController.DestoryModel(false))
                {
                    hapticAction.Execute(0f, 0.1f, 25, 5, primarySource);
                }
            }


        }
        else if (currentActionRight.Equals(Action.Grab))
        {
            //right hand
            if (grabModelAction.GetLastStateUp(primarySource))
            {
                currentActionRight = Action.Idle;
                grabAddDelectController.ReleaseObject(false);
            }
        }
        else if (currentActionRight.Equals(Action.Zoom))
        {
            bool grableft = grabModelAction.GetLastStateUp(primarySource);
            bool grabright = grabModelAction.GetLastStateUp(secondarySource);
            if (!grableft && !grabright && zoomModelController.SameObjectInhand())
            {
                zoomModelController.setTouchPosition(touchPos.GetAxis(primarySource));

                float handsDistance = Vector3.Distance(Pos(primarySource), Pos(secondarySource));
                bool success = zoomModelController.UpdateZoom(handsDistance, out Vector3 newScale);
                zoomTips.UpdateZoom(Pos(primarySource), Pos(secondarySource), success, newScale);

            }
            else
            {//zoom update only once in the frame
                //Debug.Log("zoom end");
                zoomTips.EndZoom();

                currentActionLeft = Action.Idle;
                grabAddDelectController.ReleaseObject(true);

                currentActionRight = Action.Idle;
                grabAddDelectController.ReleaseObject(false);

            }
        }




        if (currentActionLeft.Equals(Action.Idle))
        {
            //left hand
            avoidStickingController.AvoidStickingLeft();

            if (grabModelAction.GetLastStateDown(secondarySource))
            {
                //currentActionLeft = Action.Grab;
                //grabAddDelectController.GrabObject(true);

                if (grabAddDelectController.GrabObject(true))
                {
                    if (currentActionRight.Equals(Action.Grab))
                    {
                        if (zoomModelController.SameObjectInhand())
                        {
                            currentActionLeft = Action.Zoom;
                            currentActionRight = Action.Zoom;

                            zoomTips.StartZoom();
                            float handsDistance = Vector3.Distance(Pos(primarySource), Pos(secondarySource));
                            zoomModelController.StartZoom(handsDistance);
                        }
                    }
                    else
                    {
                        currentActionLeft = Action.Grab;
                    }
                }
                
            }
            else if (unionAddAction.GetStateDown(secondarySource))
            {
                if (grabAddDelectController.AddOperation(BooleanType.Union, true))
                {
                    hapticAction.Execute(0f, 0.1f, 25, 5, secondarySource);
                }
            }
            else if (intersectionAddAction.GetStateDown(secondarySource))
            {
                if (grabAddDelectController.AddOperation(BooleanType.Intersection, true))
                {
                    hapticAction.Execute(0f, 0.1f, 25, 5, secondarySource);
                }
            }
            else if (subtractAddAction.GetStateDown(secondarySource))
            {
                if (grabAddDelectController.AddOperation(BooleanType.Subtract, true))
                {
                    hapticAction.Execute(0f, 0.1f, 25, 5, secondarySource);
                }
            }
            else if (delectAction.GetStateDown(secondarySource))
            {
                if (grabAddDelectController.DelectOperation(true))
                {
                    hapticAction.Execute(0f, 0.1f, 25, 5, secondarySource);
                }
            }
            //else if (editorAction.GetStateDown(secondarySource))
            //{
            //    updateMeshController.UpdateSDFMeshAways();
            //    Debug.Log("editorAction!");
            //}
            else if (updateAction.GetStateDown(secondarySource))
            {
                updateMeshController.UpdateSDFMeshOnce();
            }
            else if (saveMeshAction.GetStateDown(secondarySource))
            {
                if (saveMeshController.ExportToOBJ())
                {
                    hapticAction.Execute(0f, 0.1f, 25, 5, secondarySource);
                }
            }
            else if (destroyModelAction.GetStateDown(secondarySource))
            {
                if (destoryModelController.DestoryModel(true))
                {
                    hapticAction.Execute(0f, 0.1f, 25, 5, secondarySource);
                }
            }
        }
        else if (currentActionLeft.Equals(Action.Grab))
        {
            //left hand
            if (grabModelAction.GetLastStateUp(secondarySource))
            {
                currentActionLeft = Action.Idle;
                grabAddDelectController.ReleaseObject(true);
            }
        }
        else if (currentActionLeft.Equals(Action.Zoom))
        {
            bool grableft = grabModelAction.GetLastStateUp(primarySource);
            bool grabright = grabModelAction.GetLastStateUp(secondarySource);
            
            if (grableft || grabright)
            {
                zoomTips.EndZoom();

                currentActionLeft = Action.Idle;
                grabAddDelectController.ReleaseObject(true);

                currentActionRight = Action.Idle;
                grabAddDelectController.ReleaseObject(false);
            }
        }



        InputControllerMarker.End();
    }

    private Vector3 Pos(SteamVR_Input_Sources source)
    {
        return pose.GetLocalPosition(source);
    }

    private Quaternion Rot(SteamVR_Input_Sources source)
    {
        return pose.GetLocalRotation(source);
    }
}
