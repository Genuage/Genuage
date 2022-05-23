using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Data;
using VR_Interaction;
using Display;
using IO;

//This class manages the camera waypoints and cloud states given to the Animation script on the camera object.
//When invoked, it should spawn a VR and Desktop Menus and link their events to its waypoints and cloudstates related functions.
//The VRCAmera tool is responsible for calling this class when a new waypoint is placed in VR
//PModules in IO can call this class when a script file is succesfully read.
//It should not allow the user to select another cloud while it's running.
//TODO : Hook with VRCameraTool, test everything
public class VRCameraWaypointsManager : MonoBehaviour
{
    //WAYPOINTS
    private Dictionary<int, CameraWaypoint> CameraWaypointsDict = new Dictionary<int, CameraWaypoint>();
    private int WaypointMasterID = 0;
    //CLOUDSTATES
    private Dictionary<int, CloudStateInTime> CloudStateDict = new Dictionary<int, CloudStateInTime>();
    private CloudStateInTime InitialState;
    private CloudStateInTime StateToRestore;
    private int StateMasterID = 0;

    //WAYPOINTCONTAINER
    public GameObject WaypointContainer;
    //LINE MESH VARIABLES
    List<Vector3> MeshVertices;
    List<int> MeshIndices;

    //CAMERA 
    public Camera MobileCamera;
    public RenderTexture CameraFeed;

    //ANIMATOR REFFERENCE
    public AnimateObject animationScript;

    //DEFAULT ANIMATION VARIABLES
    public int AnimationDuration = 0; //In seconds
    public int KeyFrameTimestep = 5;
    public int FramesPerSecond = 30;

    //UI VARIABLES
    public GameObject MenuPrefab;
    public VideoCaptureUIMenu UIMenu;
    //public VideoCaptureUIMenu UIMenuVR;

    //Controller Variable
    private Transform ControllerTransform;


    public static VRCameraWaypointsManager instance = null;
    #region Initialization

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void Initialize()
    {

        GameObject go = Instantiate(DesktopApplication.instance.VideoScriptingVRUIMenu) as GameObject;
        UIMenu = go.GetComponent<VideoCaptureUIMenu>();
        go.transform.position = ((CameraManager.instance.vr_camera.transform.forward * 1.5f)
                                + (CameraManager.instance.vr_camera.transform.up / 2));

        UIMenu.OnTimingEdit += ChangeAllAnimationTimings;
        UIMenu.OnTimingEdit += ChangeAllCloudStatesTime;
        UIMenu.OnCloudStateDeleted += RemoveCloudState;
        UIMenu.OnRecordStart += PlayAndRecord;
        UIMenu.OnAddCloudState += AddCloudState;
        UIMenu.OnWaypointDeleted += DeleteWaypoint;
        UIMenu.OnCloudStateDeleted += RemoveCloudState;
        UIMenu.OnSaveTextFile += SaveTextFile;

    }

    public void InitializeDesktop()
    {
        GameObject go = Instantiate(DesktopApplication.instance.VideoScriptingDesktopUIMenu) as GameObject;
        UIMenu = go.GetComponent<VideoCaptureUIMenu>();
        go.transform.position = ((CameraManager.instance.vr_camera.transform.forward * 1.5f)
                                + (CameraManager.instance.vr_camera.transform.up / 2));

        UIMenu.OnTimingEdit += ChangeAllAnimationTimings;
        UIMenu.OnTimingEdit += ChangeAllCloudStatesTime;
        UIMenu.OnCloudStateDeleted += RemoveCloudState;
        UIMenu.OnRecordStart += PlayAndRecord;
        UIMenu.OnAddCloudState += AddCloudState;
        UIMenu.OnWaypointDeleted += DeleteWaypoint;
        UIMenu.OnCloudStateDeleted += RemoveCloudState;
        UIMenu.OnDelete += DeleteSelf;


        UIMenu.transform.SetParent(DesktopApplication.instance.Canvas.transform);
        UIMenu.transform.localPosition = Vector3.zero;
        UIMenu.transform.localScale = Vector3.one * 200f;

        
        //UIMenu.transform.Rotate(Vector3.zero);
    }

    //Create camera from scratch if not launched from VR
    public GameObject CreateCamera()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.SetParent(this.transform);
        sphere.transform.localScale = new Vector3(0.017f, 0.017f, 0.017f);
        sphere.transform.localPosition = Vector3.forward * 0.05f;
        sphere.transform.localEulerAngles = Vector3.zero;

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.SetParent(sphere.transform);
        cylinder.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
        cylinder.transform.localPosition = new Vector3(0f, 0f, 0.494f);
        cylinder.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        cylinder.GetComponent<MeshRenderer>().material.color = Color.red;

        CameraFeed = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);

        GameObject obj = new GameObject("Mobile Camera");
        obj.transform.SetParent(sphere.transform, false);
        MobileCamera = obj.AddComponent<Camera>();
        MobileCamera.targetDisplay = 2;
        //MobileCamera.fieldOfView = 80;
        MobileCamera.nearClipPlane = 0.01f;
        MobileCamera.clearFlags = CameraClearFlags.SolidColor;
        MobileCamera.backgroundColor = Color.black;
        MobileCamera.targetTexture = CameraFeed;
        MobileCamera.forceIntoRenderTexture = true;
        MobileCamera.transform.localPosition = Vector3.zero;
        MobileCamera.transform.localRotation = Quaternion.identity;
        MobileCamera.transform.localScale = Vector3.one;
        animationScript = sphere.gameObject.AddComponent<AnimateObject>();
        animationScript.AnimationDuration = AnimationDuration; //Time in seconds
        animationScript.keyframeTimestep = KeyFrameTimestep; //Time in seconds
        animationScript.SetAnimationSpeed(1);

        return sphere;

    }

    #endregion

    #region Waypoints
    public  void PlaceWaypoint(Vector3 position, Quaternion rotation, float time = -1)
    {
        //animationScript.AddKeyframe(tr);
        GameObject s = CreateWaypointGameObject(position, rotation);

        if (CameraWaypointsDict.Count == 0)
        {
            CreateWaypointContainer(position);
            
            if (!CloudSelector.instance.noSelection)
            {
                CloudStateInTime state = CreateCloudState();
                InitialState = state;
            }
            
        }
        s.transform.SetParent(WaypointContainer.transform);

        //Debug.Log("s : " + s.transform.localRotation.eulerAngles);
        //Debug.Log("sphere : " + sphere.transform.rotation.eulerAngles);
        float TimeToAssign;
        if (time >= 0)
        {
            TimeToAssign = time;
        }
        else
        {
            TimeToAssign = WaypointMasterID * KeyFrameTimestep;
        }
        
        CameraWaypoint campoint = new CameraWaypoint(WaypointMasterID, Mathf.RoundToInt(TimeToAssign));
        //campoint.Position = data.transform.worldToLocalMatrix.MultiplyPoint3x4(s.transform.position);
        //campoint.Rotation = s.transform.localRotation * data.transform.rotation;
        campoint.obj = s;
        int indexkey;
        indexkey = animationScript.AddKeyframe(s.transform, campoint.Time);
        WaypointMasterID++;
        campoint.IndexKey = indexkey;

        //Debug.Log("Adding key for point "+campoint.ID+" at time "+campoint.Time+" / Result : "+ campoint.IndexKey);
        //Debug.Log(animationScript.curvePositionX[campoint.IndexKey].ToString());
        CameraWaypointsDict.Add(campoint.ID, campoint);
        //CREATE NEW BUTTON
        UIMenu.CreateTimeButton(campoint.ID, campoint.Time);
        CalculateAnimationDuration();
        ReloadMesh();
    }

    private void DeleteWaypoint(int id)
    {
        if (CameraWaypointsDict.ContainsKey(id))
        {
            Destroy(CameraWaypointsDict[id].obj);
            CameraWaypointsDict.Remove(id);
            animationScript.ClearAnimation();
            foreach (KeyValuePair<int, CameraWaypoint> current in CameraWaypointsDict)
            {
                int newIndex;
                newIndex = animationScript.AddKeyframe(CameraWaypointsDict[current.Value.ID].obj.transform, current.Value.Time);
                CameraWaypointsDict[current.Value.ID].IndexKey = newIndex;
                CameraWaypointsDict[current.Value.ID].Time = current.Value.Time;
            }
            CalculateAnimationDuration();
            ReloadMesh();
        }
    }

    private void ChangeAllAnimationTimings()
    {
        if (CameraWaypointsDict.Count != 0)
        {
            animationScript.ClearAnimation();
            animationScript.ClearEvents();
            List<CameraWaypoint> newTimingsList = UIMenu.GetWaypointTimingInfo();
            List<CloudStateInTime> newStateList = UIMenu.GetCloudStateTimingInfo();
            foreach (CameraWaypoint current in newTimingsList)
            {
                if (CameraWaypointsDict.ContainsKey(current.ID))
                {
                    int newIndex;
                    newIndex = animationScript.AddKeyframe(CameraWaypointsDict[current.ID].obj.transform, current.Time);
                    /**
                    if(current.Time == 0)
                    {
                        newIndex = animationScript.AddKeyframe(CameraWaypointsDict[current.ID].obj.transform, 0.001f);
                    }
                    else
                    {
                        newIndex = animationScript.AddKeyframe(CameraWaypointsDict[current.ID].obj.transform, current.Time);
                    }
                    **/
                    Debug.Log("Moving key for point " + CameraWaypointsDict[current.ID].ID + " To Time" + current.Time + " / Result : " + newIndex);

                    CameraWaypointsDict[current.ID].IndexKey = newIndex;
                    CameraWaypointsDict[current.ID].Time = current.Time;
                }
            }
            foreach (CloudStateInTime current in newStateList)
            {
                if (CloudStateDict.ContainsKey(current.ID))
                {
                    CloudStateDict[current.ID].Time = current.Time;
                    animationScript.AddAnimationEvent(CloudStateDict[current.ID]);
                }
            }
            CalculateAnimationDuration();
            ReloadMesh();
        }
    }



    private void CalculateAnimationDuration()
    {
        int durationMax = 0;
        foreach (KeyValuePair<int, CameraWaypoint> current in CameraWaypointsDict)
        {
            if (durationMax < current.Value.Time)
            {
                durationMax = current.Value.Time;
            }
        }
        AnimationDuration = durationMax;

    }

    private void CreateWaypointContainer(Vector3 position)
    {
        WaypointContainer = new GameObject("container");
        WaypointContainer.transform.position = position;
        if (!CloudSelector.instance.noSelection)
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();
            WaypointContainer.transform.SetParent(data.transform);
        }
        WaypointContainer.AddComponent<MeshFilter>();
        WaypointContainer.AddComponent<MeshRenderer>();
        WaypointContainer.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard");

    }


    private GameObject CreateWaypointGameObject(Vector3 position, Quaternion rotation)
    {
        GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        s.transform.position = position;
        // NEED TO BE THE POSITIONS RELATIVE TO THE FIRST OBJECT IN THE LIST IN ORDER TO 
        //BYPASS THE ANIMATIONS STARTING AT 0/0/0
        //ALSO NEED TO MAKE ALL WAYPOINTS FOLLOW THE CLOUD
        s.transform.rotation = rotation;
        s.transform.localScale = Vector3.one * 0.017f;
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.SetParent(s.transform);
        cylinder.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
        cylinder.transform.localPosition = new Vector3(0f, 0f, 0.494f);
        cylinder.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        cylinder.GetComponent<MeshRenderer>().material.color = Color.red;
        return s;
    }

    private void ReloadMesh()
    {
        Mesh mesh = new Mesh();
        MeshVertices = new List<Vector3>();
        MeshIndices = new List<int>();
        MeshVertices.Add(Vector3.zero);
        float meshPointNumber = Mathf.Round(AnimationDuration * 15f);
        float Time = AnimationDuration / meshPointNumber;
        //MeshIndices.Add(0);
        for (int i = 1; i < meshPointNumber; i++)
        {
            MeshIndices.Add(i - 1);
            //MeshVertices.Add(WaypointList[i].transform.localPosition);
            //Debug.Log("i: " + i + " Time : " + (Time * i) + 
            //          " Position : " + animationScript.GetPositionatTime(Time * i));
            MeshVertices.Add(animationScript.GetPositionatTime(Time * i));
            MeshIndices.Add(i);
        }

        mesh.vertices = MeshVertices.ToArray();
        mesh.SetIndices(MeshIndices.ToArray(), MeshTopology.Lines, 0);
        WaypointContainer.GetComponent<MeshFilter>().mesh = mesh;
    }

    #endregion

    #region CloudStates

    private CloudStateInTime CreateCloudState()
    {
        CloudData clouddata = CloudUpdater.instance.LoadCurrentStatus();
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = clouddata.gameObject.GetComponent<MeshFilter>().mesh.vertices;
        mesh.SetIndices(clouddata.gameObject.GetComponent<MeshFilter>().mesh.GetIndices(0), MeshTopology.Points, 0);
        mesh.uv = clouddata.gameObject.GetComponent<MeshFilter>().mesh.uv;
        mesh.uv2 = clouddata.gameObject.GetComponent<MeshFilter>().mesh.uv2;
        mesh.uv3 = clouddata.gameObject.GetComponent<MeshFilter>().mesh.uv3;
        mesh.uv4 = clouddata.gameObject.GetComponent<MeshFilter>().mesh.uv4;
        mesh.uv5 = clouddata.gameObject.GetComponent<MeshFilter>().mesh.uv5;
        int time = StateMasterID * KeyFrameTimestep;
        float psize = clouddata.globalMetaData.point_size;
        Vector3 scale = clouddata.globalMetaData.scale;
        Vector3 boxscale = clouddata.globalMetaData.box_scale;
        string color = clouddata.globalMetaData.colormapName;
        CloudStateInTime state = new CloudStateInTime(StateMasterID, time, mesh, psize, scale, boxscale, color);
        StateMasterID++;
        return state;
    }

    private void AddCloudState()
    {
        if (!CloudSelector.instance.noSelection)
        {
            CloudStateInTime state = CreateCloudState();
            CloudStateDict.Add(state.ID, state);
            animationScript.AddAnimationEvent(state);
            UIMenu.CreateCloudStateTimingButton(state.ID, state.Time);
        }
        //get mesh from cloudData
    }

    private void ChangeAllCloudStatesTime()
    {
        animationScript.ClearEvents();
        List<CloudStateInTime> newTimingsList = UIMenu.GetCloudStateTimingInfo();
        foreach (CloudStateInTime current in newTimingsList)
        {
            if (CloudStateDict.ContainsKey(current.ID))
            {
                CloudStateDict[current.ID].Time = current.Time;
                animationScript.AddAnimationEvent(CloudStateDict[current.ID]);

            }
        }
        CalculateAnimationDuration();
    }

    private void RemoveCloudState(int id)
    {
        animationScript.ClearEvents();
        CloudStateDict.Remove(id);
        foreach (KeyValuePair<int, CloudStateInTime> item in CloudStateDict)
        {
            animationScript.AddAnimationEvent(item.Value);
        }
    }


    private void ChangeCloudState(CloudStateInTime state)
    {
        CloudUpdater.instance.OverrideBoxScale(state.BoxScale);
        CloudUpdater.instance.OverrideMesh(state.Mesh);
        CloudUpdater.instance.ChangePointSize(state.PointSize);
        CloudUpdater.instance.ChangeCloudScale(state.Scale);
        CloudUpdater.instance.ChangeCurrentColorMap(state.ColorMap);
    }

    #endregion

    #region Recording
    private void PlayAnimation()
    {
        if(CloudStateDict.Count > 0)
        {
            ChangeCloudState(InitialState);
        }
        UIMenu.gameObject.SetActive(false);
        GameObject sphere = MobileCamera.gameObject.transform.parent.gameObject;
        ControllerTransform = sphere.transform.parent;
        sphere.transform.parent = null;
        sphere.transform.position = CameraWaypointsDict[0].obj.transform.position;
        sphere.transform.rotation = CameraWaypointsDict[0].obj.transform.rotation;
        //animationScript.InitializeFirstKeyframe(sphere.transform);
        //GameObject go = new GameObject("dummy");
        //go.transform.position = WaypointList[0].transform.position;
        sphere.transform.SetParent(WaypointContainer.transform);
        /**
        foreach (GameObject g in WaypointList)
        {
            g.SetActive(false);
        }
        container.GetComponent<MeshRenderer>().enabled = false;
        **/
        animationScript.PlayAnimation();

    }

    private void PlayAndRecord()
    {

        StateToRestore = CreateCloudState();
        ChangeCloudState(InitialState);

        UIMenu.gameObject.SetActive(false);
        GameObject sphere = MobileCamera.gameObject.transform.parent.gameObject;
        ControllerTransform = sphere.transform.parent;
        sphere.transform.parent = null;
        sphere.transform.position = CameraWaypointsDict[0].obj.transform.position;
        sphere.transform.rotation = CameraWaypointsDict[0].obj.transform.rotation;
        //animationScript.InitializeFirstKeyframe(sphere.transform);
        //GameObject go = new GameObject("dummy");
        //go.transform.position = WaypointList[0].transform.position;
        sphere.transform.SetParent(WaypointContainer.transform);
        foreach (KeyValuePair<int, CameraWaypoint> g in CameraWaypointsDict)
        {
            g.Value.obj.SetActive(false);
        }
        WaypointContainer.GetComponent<MeshRenderer>().enabled = false;

        //TODO : Disable all UI when recording

        MobileCamera.gameObject.AddComponent<ScreenRecorder>();
        Debug.Log(AnimationDuration);
        MobileCamera.gameObject.GetComponent<ScreenRecorder>().maxFrames = (AnimationDuration * FramesPerSecond) + FramesPerSecond;
        
        MobileCamera.gameObject.GetComponent<ScreenRecorder>().OnRecordingEnd += RestoreObjects;
        
        animationScript.PlayAnimation();
        MobileCamera.gameObject.GetComponent<ScreenRecorder>().ActivateRecording();
    }

    private void RestoreObjects()
    {
        ChangeCloudState(StateToRestore);
        UIMenu.gameObject.SetActive(true);
        GameObject sphere = MobileCamera.gameObject.transform.parent.gameObject;
        sphere.transform.parent = null;
        sphere.transform.SetParent(ControllerTransform);
        sphere.transform.localScale = new Vector3(0.017f, 0.017f, 0.017f);
        sphere.transform.localPosition = Vector3.forward * 0.05f;
        sphere.transform.localEulerAngles = Vector3.zero;

        foreach (KeyValuePair<int, CameraWaypoint> g in CameraWaypointsDict)
        {
            g.Value.obj.SetActive(true);
        }
        WaypointContainer.GetComponent<MeshRenderer>().enabled = true;
        MobileCamera.gameObject.GetComponent<ScreenRecorder>().OnRecordingEnd -= RestoreObjects;
        //Destroy(MobileCamera.gameObject.GetComponent<ScreenRecorder>());
        //TODO : Restore all UI when recording is over

    }
    #endregion

    private void SaveTextFile()
    {
        CloudData data = CloudUpdater.instance.LoadCurrentStatus();
        data.transform.parent.gameObject.GetComponent<CloudObjectRefference>().box.transform.position = Vector3.zero;
        data.transform.parent.gameObject.GetComponent<CloudObjectRefference>().box.transform.eulerAngles = Vector3.zero;
        data.transform.position = Vector3.zero;
        data.transform.eulerAngles = Vector3.zero;


        DateTime now = DateTime.Now;
        string outputname = " _"+ now.Year.ToString()+"_" + now.Month.ToString() + "_" + now.Day.ToString() + "_" + now.Hour.ToString() + "_" + now.Minute.ToString();

        string filename = "NewVideoText"+ outputname+".txt";
        string datapath = Application.dataPath + "/Records/VideoScripts/" + filename;
        using (StreamWriter sw = File.AppendText(datapath))
        {
            for (int i = 0; i <= WaypointMasterID;i++)
            {
                if (CameraWaypointsDict.ContainsKey(i))
                {
                    CameraWaypoint cw = CameraWaypointsDict[i];
                    cw.Position = cw.obj.transform.position;
                    cw.Rotation = cw.obj.transform.rotation;
                    sw.WriteLine("WAYPOINT|POS="+ cw.Position.x.ToString(CultureInfo.InvariantCulture) + ","+ cw.Position.y.ToString(CultureInfo.InvariantCulture) + "," + cw.Position.z.ToString(CultureInfo.InvariantCulture) +
                                 ";ROT="+ cw.Rotation.eulerAngles.x.ToString(CultureInfo.InvariantCulture) + ","+ cw.Rotation.eulerAngles.y.ToString(CultureInfo.InvariantCulture) + "," + 
                                 cw.Rotation.eulerAngles.z.ToString(CultureInfo.InvariantCulture) + ";TIME="+cw.Time.ToString(CultureInfo.InvariantCulture));

                }
            }
            sw.Close();
        }
        ModalWindowManager.instance.CreateModalWindow("Text File "+ filename + " has been saved in the Records Folder");
    }

    public void OnDisabled()
    {
        UIMenu.OnTimingEdit -= ChangeAllAnimationTimings;
        UIMenu.OnCloudStateDeleted -= RemoveCloudState;
        UIMenu.OnRecordStart -= PlayAndRecord;
        UIMenu.OnAddCloudState -= AddCloudState;
        animationScript.ClearEvents();
        animationScript.ClearAnimation();
        Destroy(MobileCamera.gameObject);
        foreach (KeyValuePair<int, CameraWaypoint> item in CameraWaypointsDict)
        {
            Destroy(item.Value.obj);
        }
        Destroy(WaypointContainer);
        CameraWaypointsDict.Clear();
        CloudStateDict.Clear();
        Destroy(UIMenu.gameObject);
        WaypointMasterID = 0;

    }
    private void DeleteSelf()
    {
        Destroy(this.gameObject);
    }

    public void OnDestroy()
    {
        instance = null;
        OnDisabled();
    }

}
