using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidStickingController : MonoBehaviour
{
    public SceneSDFArea sceneSDFArea;

    public void AvoidStickingLeft()
    {
        sceneSDFArea.AvoidStickingLeft();
    }

    public void AvoidStickingRight()
    {
        sceneSDFArea.AvoidStickingRight();
    }


}
