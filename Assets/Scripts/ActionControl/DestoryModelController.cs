using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryModelController : MonoBehaviour
{
    public SceneSDFArea scenesdfArea;

    public bool DestoryModel(bool rightOrLeft)
    {
        return scenesdfArea.DestoryModel(rightOrLeft);
    }

}
