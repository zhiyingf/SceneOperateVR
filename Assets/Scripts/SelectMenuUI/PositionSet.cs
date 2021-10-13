using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace positionSet
{
    public struct PoseInfo
    {
        public Vector3 pos;
        public Quaternion rot;

        public PoseInfo(Vector3 pos, Quaternion rot)
        {
            this.pos = pos;
            this.rot = rot;
        }

        public static PoseInfo operator *(PoseInfo a, PoseInfo b)
        {
            return new PoseInfo
            {
                rot = a.rot * b.rot,
                pos = a.pos + a.rot * b.pos
            };
        }

        public PoseInfo(Transform t, bool useLocal = false)
        {
            if (t == null)
            {
                pos = Vector3.zero;
                rot = Quaternion.identity;
            }
            else if (!useLocal)
            {
                pos = t.position;
                rot = t.rotation;
            }
            else
            {
                pos = t.localPosition;
                rot = t.localRotation;
            }
        }
    }


    public class PositionSet : MonoBehaviour
    {
        public Transform objectTracker;
        public Vector3 posOffset;
        public Vector3 rotOffset;

        public void OnNewPoses()
        {
            Vector3 basePos = new Vector3(0.0f, -0.5f, 1.8f);
            basePos += posOffset;

            PoseInfo poseOrigin = new PoseInfo(objectTracker);
            poseOrigin = poseOrigin * new PoseInfo(basePos, Quaternion.Euler(rotOffset));

            poseOrigin.pos.y = 0.5f;

            //PoseFreezer
            var freezeEuler = transform.localEulerAngles;
            var poseEuler = poseOrigin.rot.eulerAngles;
            poseEuler.x = freezeEuler.x;
            poseEuler.z = freezeEuler.z;
            poseOrigin.rot = Quaternion.Euler(poseEuler);

            transform.localPosition = poseOrigin.pos;
            transform.localRotation = poseOrigin.rot;

        }

    }


}