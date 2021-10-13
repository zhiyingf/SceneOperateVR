using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveMeshController : MonoBehaviour
{
    [SerializeField]
    private SceneSDFArea scenesdfArea = null;

    [SerializeField]
    private string ExportFolderName = "SceneResult";


    public bool ExportToOBJ()
    {
        scenesdfArea.SetExportFolderName(ExportFolderName);
        return scenesdfArea.ExportMeshToObject();
    }

    
}
