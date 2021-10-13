using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[AddComponentMenu("SceneOperate/Scene Operation Area")]
[ExecuteInEditMode]

public class SceneSDFArea : MonoBehaviour
{
    public SceneBox SB;

    public ComputeShader McShader;
    public ComputeShader SdfShader;

    public SetCollider setColliderLeft;
    public SetCollider setColliderRight;

    private Dictionary<MeshFilter, BooleanType> operations;

    //A lock that controls the execution of a program
    public bool living = false;
    public bool isEditor = false;

    //private bool isInHanding = false;

    public MeshFilter colliderLeft;
    public MeshFilter colliderRight;

    public MeshFilter objectInhandLeft;
    public MeshFilter objectInhandRight;

    public Vector3 offset;
    Vector3 origin = Vector3.zero;
    GameObject sceneModel;
    string ExportFolderName;

    //public struct OpAndType
    //{
    //    public MeshFilter mesh;
    //    public BooleanType type;
    //    public OpState state;
    //    //int orderId;
    //    public OpAndType(MeshFilter mesh1, BooleanType type1, OpState state1)
    //    {
    //        mesh = mesh1;
    //        type = type1;
    //        state = state1;
    //    }

    //    public OpAndType(MeshFilter mesh1, BooleanType type1)
    //    {
    //        mesh = mesh1;
    //        type = type1;
    //        state = new OpState(mesh.transform);
    //    }
    //}

    //public struct OpState
    //{
    //    public Vector3 postion;
    //    public Quaternion rotation;
    //    public Vector3 scale;

    //    public OpState(Vector3 pos, Quaternion rot, Vector3 sc)
    //    {
    //        postion = pos;
    //        rotation = rot;
    //        scale = sc;
    //    }

    //    public OpState(in Transform tran)
    //    {
    //        postion = tran.position;
    //        rotation = tran.rotation;
    //        scale = tran.localScale;
    //    }

    //    public bool changed(in Transform tran)
    //    {
    //        if (tran.position != postion || tran.rotation != rotation || tran.lossyScale != scale)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //}


    // Start is called before the first frame update

    void Start()
    {
        SB = new SceneBox(SdfShader);
        operations = new Dictionary<MeshFilter, BooleanType>();

        string name = "SceneModel";
        sceneModel = transform.Find(name).gameObject;
    }

    public void toggleIsEditor()
    {
        isEditor = !isEditor;
    }

    public void ExecuteOnClick()
    {
        if (!living && !isEditor && UpdateSDF())
        {
            UpdateMesh();
        }
    }

    //private IEnumerator Execute()
    //{
    //    living = true;
    //    while (isEditor && isInHanding && operations.Count >= 2)
    //    {
    //        if (UpdateSDF())
    //        {
    //            UpdateMesh();
    //        }
    //        else
    //        {
    //            yield return null;
    //        }
    //    }
    //    living = false;
    //}

    //public void CoroutineUpdate()
    //{
    //    if (!living && isEditor && Application.isPlaying)
    //    {
    //        StartCoroutine(Execute());
    //    }
    //}

    public bool UpdateSDF()
    {
        int size = operations.Count;
        if (size < 1) return false;
        else
        {
            int i = 0;
            MeshFilter mesh0 = new MeshFilter();
            foreach (KeyValuePair<MeshFilter, BooleanType> kvp in operations)
            {
                i++;
                if (i == 1)
                {
                    mesh0 = kvp.Key;
                }
                else if (i == 2)
                {
                    SB.UpdateSDF(mesh0, kvp.Key, kvp.Value);
                }
                else
                {
                    SB.UpdateSDFLater(kvp.Key, kvp.Value);
                }
            }

            //copy function
            if (i == 1)
            {
                string name = DefaultFileName();
                operations.Remove(mesh0);
                RecoverColor(mesh0);

                GameObject newModel = Instantiate(mesh0.gameObject, mesh0.transform.position+offset, mesh0.transform.rotation);
                newModel.name = name;
                newModel.GetComponent<MeshRenderer>().material.color = new Color(0.4f, 0.4f, 0.4f, 1.0f);

                return false;
            }

            return true;
        }
    }

    public void UpdateMesh()
    {
        if (McShader)
        {
            //create newModel
            string name = DefaultFileName();
            GameObject newModel = Instantiate(sceneModel, transform.position, transform.rotation);
            newModel.tag = "operater";
            newModel.name = name;
            AttachScriptable scriptableData = newModel.AddComponent<AttachScriptable>();
            SB.SetScriptable(out scriptableData.Scriptable, false, name, true, ref origin);//if save texture3d and scriptable

            newModel.transform.position = origin + offset;

            UseMcShader mc = new UseMcShader(SB, McShader);
            mc.ComputeMC();
            newModel.GetComponent<MeshFilter>().mesh = mc.mesh;
            newModel.GetComponent<MeshCollider>().sharedMesh = mc.mesh;

            clearOperation();
        }
        else
        {
            print("need compute shader");
        }
    }

    //clear in updateMesh later
    private void clearOperation()
    {
        foreach (KeyValuePair<MeshFilter, BooleanType> kvp in operations)
        {
            RecoverColor(kvp.Key);
        }
        operations.Clear();
    }


    //public bool changed()
    //{
    //    bool flag = false;
    //    int size = operations.Count;
    //    for (int i = 0; i < size; i++)
    //    {
    //        Transform tran = operations[i].mesh.transform;
    //        OpState state = operations[i].state;
    //        if (state.changed(tran))
    //        {
    //            flag = true;

    //            OpAndType tmp = operations[i];
    //            tmp.state = new OpState(tran);
    //            operations[i] = tmp;
    //        }
    //    }
    //    return flag;
    //}



    private void SetColor(MeshFilter mesh,BooleanType type)
    {
        switch (type)
        {
            case BooleanType.Union:
                mesh.GetComponent<MeshRenderer>().material.color = new Color(0.4f, 0.75f, 0.9f, 1.0f);
                break;
            case BooleanType.Intersection:
                mesh.GetComponent<MeshRenderer>().material.color = new Color(0.9f, 0.7f, 0.7f, 1.0f);
                break;
            case BooleanType.Subtract:
                mesh.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 0.7f, 0.4f, 1.0f);
                break;
        }
    }

    private void RecoverColor(MeshFilter mesh)
    {
        mesh.GetComponent<MeshRenderer>().material.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
    }

    public bool AddOperation(BooleanType type,bool rightOrLeft)
    {
        //check mesh is exist or not
        //is in , check type , if != , change type
        //is not in , add it

        if (rightOrLeft)
        {
            if (colliderLeft)
            {
                if (operations.TryGetValue(colliderLeft, out BooleanType type1))
                {
                    if (type != type1)
                    {
                        operations[colliderLeft] = type;
                        SetColor(colliderLeft, type);
                        return true;
                    }
                }
                else
                {
                    operations.Add(colliderLeft, type);
                    SetColor(colliderLeft, type);
                    return true;
                }
            }
        }
        else
        {
            if (colliderRight)
            {
                if (operations.TryGetValue(colliderRight, out BooleanType type1))
                {
                    if (type != type1)
                    {
                        operations[colliderRight] = type;
                        SetColor(colliderRight, type);
                        return true;
                    }
                }
                else
                {
                    operations.Add(colliderRight, type);
                    SetColor(colliderRight, type);
                    return true;
                }
            }
        }
        return false;
    }

    public bool DelectOperation(bool rightOrLeft)
    {
        //check mesh is exist or not
        //if in , delect it
        if (rightOrLeft)
        {
            if (colliderLeft && operations.TryGetValue(colliderLeft, out _))
            {
                operations.Remove(colliderLeft);
                RecoverColor(colliderLeft);
                return true;
            }
        }
        else
        {
            if (colliderRight && operations.TryGetValue(colliderRight, out _))
            {
                operations.Remove(colliderRight);
                RecoverColor(colliderRight);
                return true;
            }
        }
        return false;
    }

    //grab object
    public bool GrabObject(bool rightOrLeft)
    {
        if (rightOrLeft)
        {
            if (colliderLeft)
            {
                //1 Move the colling GameObject into the player's hand and remove it from the collingObject variable.
                objectInhandLeft = colliderLeft;
                colliderLeft = null;

                //2 Add a new joint that connects the controller to the object using the AddFixedJoint() method above.
                var joint = setColliderLeft.AddFixedJoint();
                joint.connectedBody = objectInhandLeft.GetComponent<Rigidbody>();

                return true;

                //isInHanding = true;

                //CoroutineUpdate();
            }
        }
        else
        {
            if (colliderRight)
            {
                objectInhandRight = colliderRight;
                colliderRight = null;
                var joint = setColliderRight.AddFixedJoint();
                joint.connectedBody = objectInhandRight.GetComponent<Rigidbody>();

                return true;

                //isInHanding = true;

                //CoroutineUpdate();
            }
        }

        return false;
    }

    public void ReleaseObject(bool rightOrLeft)
    {
        if (rightOrLeft)
        {
            //1 Make sure there¡¯s a fixed joint attached to the controller.
            if (objectInhandLeft && setColliderLeft.GetComponent<FixedJoint>())
            {
                //2 Remove the connection to the object held by the joint and destroy the joint.
                setColliderLeft.GetComponent<FixedJoint>().connectedBody = null;
                Destroy(setColliderLeft.GetComponent<FixedJoint>());
            }
            //4 Remove the reference to the formerly attached object.
            objectInhandLeft = null;
        }
        else
        {
            //1 Make sure there¡¯s a fixed joint attached to the controller.
            if (objectInhandRight && setColliderRight.GetComponent<FixedJoint>())
            {
                //2 Remove the connection to the object held by the joint and destroy the joint.
                setColliderRight.GetComponent<FixedJoint>().connectedBody = null;
                Destroy(setColliderRight.GetComponent<FixedJoint>());
            }
            //4 Remove the reference to the formerly attached object.
            objectInhandRight = null;
        }
        //isInHanding = false;
    }


    //Destruction
    public bool DestoryModel(bool rightOrLeft)
    {
        if (rightOrLeft)
        {
            if (colliderLeft)
            {
                if (operations.TryGetValue(colliderLeft, out _))
                {
                    operations.Remove(colliderLeft);
                }
                Destroy(colliderLeft.gameObject);
                return true;
            }
        }
        else
        {
            if (colliderRight)
            {
                if (operations.TryGetValue(colliderRight, out _))
                {
                    operations.Remove(colliderRight);
                }
                Destroy(colliderRight.gameObject);
                return true;
            }
        }
        return false;
    }


    //zoomModelAction
    public bool SameObjectInhand()
    {
        return objectInhandLeft && (objectInhandLeft == objectInhandRight);
    }

    public Vector3 SizeObjectInhand()
    {
        if (objectInhandLeft != null && objectInhandLeft == objectInhandRight)
        {
            return objectInhandLeft.GetComponent<Transform>().localScale;
        }
        else
        {
            Debug.Log("Size objectInhandLeft is null or !=");
            return Vector3.one;//???
        }
    }

    public bool ZoomObjectInhand(Vector3 size)
    {
        if (objectInhandLeft != null && objectInhandLeft == objectInhandRight)
        {
            objectInhandLeft.GetComponent<Transform>().localScale = size;
            return true;
        }
        else
        {
            Debug.Log("Zoom objectInhandLeft is null or !=");
            return false;
        }
    }


    //save mesh
    public bool ExportMeshToObject()
    {
        if (colliderLeft && colliderLeft.tag == "operater")
        {
            GetExportMeshPath(ExportFolderName, colliderLeft.name, out string pathName);
            return ExportMesh.ExportMeshToObj(colliderLeft.gameObject, pathName);
        }
        return false;
    }

    //tool
    private string DefaultFileName()
    {
        return (DateTime.Now).ToString("yyyyMMddHHmmss");
    }

    private void TryCreateDirectory(string path)
    {
        // Try to create the directory
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

        }
        catch (IOException ex)
        {
            //Console.WriteLine(ex.Message);
            Debug.Log(ex.Message);
        }
    }

    public void GetExportMeshPath(string ExportFolderName, string modelName, out string pathName)
    {
        string path = Path.Combine(Application.dataPath, ExportFolderName);//Unity Editor: <path to project folder>/Assets
        ///
        path = Path.Combine(path, "models");
        ///
        TryCreateDirectory(path);

        pathName = Path.Combine(path, modelName + ".obj");
    }

    public void SetExportFolderName(string exportFolderName)
    {
        ExportFolderName = exportFolderName;
    }


    //AvoidSticking
    public void AvoidStickingLeft()
    {
        if (setColliderLeft.GetComponent<FixedJoint>())
        {
            setColliderLeft.GetComponent<FixedJoint>().connectedBody = null;
            Destroy(setColliderLeft.GetComponent<FixedJoint>());
        }
    }

    public void AvoidStickingRight()
    {
        if (setColliderRight.GetComponent<FixedJoint>())
        {
            setColliderRight.GetComponent<FixedJoint>().connectedBody = null;
            Destroy(setColliderRight.GetComponent<FixedJoint>());
        }
    }
}
