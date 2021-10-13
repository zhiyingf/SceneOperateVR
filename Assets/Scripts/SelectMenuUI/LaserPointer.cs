using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;

namespace SelectMenuUI
{
    public struct PointerEventArgs
    {
        public SteamVR_Input_Sources fromInputSource;
        public uint flags;
        //public float distance;
        public RaycastHit hit;
        public Transform target;
    }

    public delegate void PointerEventHandler(object sender, PointerEventArgs e);

    public delegate void ProcessDragDownHandler(object sender, Pointer3DEventData e);

    public delegate void ProcessDragHandler(object sender, Pointer3DEventData e);

    public delegate void ProcessDragUpHandler(object sender, Pointer3DEventData e);

    public class LaserPointer : MonoBehaviour
    {
        public SteamVR_Behaviour_Pose pose;
        public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");
        public float thickness = 0.002f;
        //public GameObject holder;
        public GameObject pointer;
        public GameObject reticle;
        public Canvas cans;
        GraphicRaycaster raycaster;



        public event PointerEventHandler PointerClick;

        public event ProcessDragHandler PointerDragDown;
        public event ProcessDragHandler PointerDrag;
        public event ProcessDragHandler PointerDragUp;

        Pointer3DEventData pointerData = new Pointer3DEventData(EventSystem.current);

        private const float distMost = 100f;

        private void Start()
        {
            raycaster = cans.GetComponent<GraphicRaycaster>();
        }

        public static Vector2 ScreenCenterPoint { get { return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f); } }

        public virtual void OnPointerClick(PointerEventArgs e)
        {
            if (PointerClick != null)
                PointerClick(this, e);
        }

        public virtual void OnProcessDragDown(Pointer3DEventData e)
        {
            if (PointerDragDown != null)
            {
                PointerDragDown(this, e);
            }
        }

        public virtual void OnProcessDrag(Pointer3DEventData e)
        {
            if (PointerDrag != null)
            {
                PointerDrag(this, e);
            }
        }

        public virtual void OnProcessDragUp(Pointer3DEventData e)
        {
            if (PointerDragUp != null)
            {
                PointerDragUp(this, e);
            }
        }

        private void OnDisable()
        {
            pointer.SetActive(false);
            reticle.SetActive(false);
        }


        private void Update()
        {
            Ray raycast = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            bool bHit = Physics.Raycast(raycast, out hit, 100f);

            if (!bHit)
            {
                pointer.SetActive(false);
                reticle.SetActive(false);
                return;
            }
            GameObject gameObject = hit.collider.gameObject;
            string tag = gameObject.tag;
           
            if (tag != "scrollview" && tag != "image" && tag != "scrollbar")
            {
                pointer.SetActive(false);
                reticle.SetActive(false);
                return;
            }
            
            if (hit.distance < distMost)
            {
                float dist = hit.distance;

                pointer.SetActive(true);
                reticle.SetActive(true);

                pointer.transform.localScale = new Vector3(thickness, thickness, dist);
                pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);
                pointer.transform.localRotation = Quaternion.identity;

                reticle.transform.position = hit.point;
            }
            else
            {
                return;
            }

            if (tag == "image" && interactWithUI.GetStateUp(pose.inputSource))
            {
                PointerEventArgs argsClick = new PointerEventArgs();
                argsClick.fromInputSource = pose.inputSource;
                //argsClick.distance = hit.distance;
                argsClick.hit = hit;
                argsClick.flags = 0;
                argsClick.target = hit.transform;
                OnPointerClick(argsClick);
            }else if(tag == "scrollbar")
            {
                //Pointer3DEventData pointerData = new Pointer3DEventData(EventSystem.current);
                //pointerData.enterEventCamera = Camera.main;
                pointerData.delta = Vector2.zero;//
                pointerData.button = PointerEventData.InputButton.Left;//
                pointerData.position = Camera.main.WorldToScreenPoint(hit.point);//ScreenCenterPoint;//

                


                pointerData.pointerCurrentRaycast = new RaycastResult
                {
                    gameObject = gameObject,
                    module = raycaster,
                    distance = hit.distance,
                    worldPosition = hit.point,//raycast.GetPoint(hit.distance),
                    worldNormal = -gameObject.transform.forward,
                    screenPosition = pointerData.position//,ScreenCenterPoint
                    //index = raycastResults.Count,
                    //depth = graphic.depth,
                    //sortingLayer = canvas.sortingLayerID,
                    //sortingOrder = canvas.sortingOrder
                };


                if (interactWithUI.GetStateDown(pose.inputSource))
                {
                    OnProcessDragDown(pointerData);
                }
                else if (interactWithUI.GetState(pose.inputSource))
                {
                    OnProcessDrag(pointerData);
                }
                else if (interactWithUI.GetStateUp(pose.inputSource))
                {
                    OnProcessDragUp(pointerData);
                }
            }
            
        }

    }
}
