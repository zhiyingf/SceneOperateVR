using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomModelController : MonoBehaviour
{
    public SceneSDFArea scenesdfArea;
    public RadialMenu radialMenu;
    public float MaxScale = 4f;
    public float MinScale = 0.4f;

    private float startHandsDistance;
    private Vector3 startScale;

    public bool SameObjectInhand()
    {
        return scenesdfArea.SameObjectInhand();
    }


    public bool ZoomObjectInhand(Vector3 size)
    {
        return scenesdfArea.ZoomObjectInhand(size);
    }

    public void StartZoom(float handsDistance)
    {
        startHandsDistance = handsDistance;
        startScale = scenesdfArea.SizeObjectInhand();
    }

    public bool UpdateZoom(float currentHandsDistance, out Vector3 zoomSize)
    {
        float newScale = 0;
        float rateDis = currentHandsDistance / startHandsDistance;
        int index = GetIndex();
        zoomSize = Vector3.zero;
        if (index == 0)
        {
            //A
            Vector3 tmp = startScale * rateDis;
            newScale = Mathf.Max(tmp.x, tmp.y, tmp.z);
            if (newScale <= MinScale)
            {
                newScale = MinScale;
                return false;
            }
            if (newScale >= MaxScale)
            {
                newScale = MaxScale;
                return false;
            }
            zoomSize = new Vector3(tmp.x, tmp.y, tmp.z);

            

        }else if (index == 1)
        {
            //X
            newScale = startScale.x * rateDis;
            if (newScale <= MinScale)
            {
                newScale = MinScale;
                return false;
            }
            if (newScale >= MaxScale)
            {
                newScale = MaxScale;
                return false;
            }
            zoomSize = new Vector3(newScale, startScale.y, startScale.z);
            
        }else if(index == 2)
        {
            //Y
            newScale = startScale.y * rateDis;
            if (newScale <= MinScale)
            {
                newScale = MinScale;
                return false;
            }
            if (newScale >= MaxScale)
            {
                newScale = MaxScale;
                return false;
            }
            zoomSize = new Vector3(startScale.x, newScale, startScale.z);
        }
        else if(index == 3)
        {
            //Z
            newScale = startScale.z * rateDis;
            if (newScale <= MinScale)
            {
                newScale = MinScale;
                return false;
            }
            if (newScale >= MaxScale)
            {
                newScale = MaxScale;
                return false;
            }
            zoomSize = new Vector3(startScale.x, startScale.y, newScale);
        }
        return ZoomObjectInhand(zoomSize);

    }

    public int GetIndex()
    {
        return radialMenu.GetIndex();
    }

    public void SetTouch()
    {
        radialMenu.ActivateHighlightedSection();
    }

    public void setTouchPosition(Vector2 axis)
    {
        radialMenu.SetTouchPosition(axis);
    }
}
