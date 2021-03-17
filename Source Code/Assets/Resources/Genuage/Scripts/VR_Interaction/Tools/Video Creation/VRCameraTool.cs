/**
Copyright (c) 2020, 	Institut Curie, Institut Pasteur and CNRS
			Thomas BLanc, Mohamed El Beheiry, Jean Baptiste Masson, Bassam Hajj and Clement Caporal
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. All advertising materials mentioning features or use of this software
   must display the following acknowledgement:
   This product includes software developed by the Institut Curie, Insitut Pasteur and CNRS.
4. Neither the name of the Institut Curie, Insitut Pasteur and CNRS nor the
   names of its contributors may be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
**/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Display;
using Data;
using IO;
namespace VR_Interaction
{
    public class CameraWaypoint
    {
        public int ID;
        public int Time;
        public int IndexKey;
        public GameObject obj = null;
        //public Vector3 Position;
        //public Quaternion Rotation;
        //Vector3 Scale;
        public CameraWaypoint(int id, int time)
        {
            ID = id;
            Time = time;
            //Position = position;
            //Rotation = rotation;
            //Scale = scale;
        }
    }

    //Represents a state the cloud is in, namely 
    //it's mesh (containing all the point info), point size, scale and color map
    public class CloudStateInTime
    {
        public int ID;
        public int Time;
        public Mesh Mesh;
        public float PointSize;
        public Vector3 Scale;
        public Vector3 BoxScale;
        public string ColorMap;

        public CloudStateInTime(int id,int time, Mesh mesh, float pointsize, Vector3 scale, Vector3 boxscale, string colormap)
        {
            ID = id;
            Time = time;
            Mesh = mesh;
            PointSize = pointsize;
            Scale = scale;
            BoxScale = boxscale;
            ColorMap = colormap;
        }
    }

    public class VRCameraTool : IControllerTool
    {
        //INTERNAL VARIABLEs
        private Dictionary<int, CameraWaypoint> CameraWaypointsDict;
        private int WaypointMasterID = 0;
        private int StateMasterID = 0;
        private Dictionary<int, CloudStateInTime> CloudStateDict;
        private CloudStateInTime InitialState;
        private CloudStateInTime StateToRestore;

        public Camera MobileCamera;
        public GameObject Screen;
        public RenderTexture CameraFeed;
        public GameObject SpherePrefab;
        private GameObject sphere;
        private List<GameObject> WaypointList;
        private AnimateObject animationScript;
        private GameObject container;

        protected VRTK_ControllerEvents _controller;
        //DEFAULT ANIMATION VARIABLES
        public int AnimationDuration = 0; //In seconds
        public int KeyFrameTimestep = 5;
        public int FramesPerSecond = 30; 

        //LINE MESH VARIABLES
        List<Vector3> MeshVertices;
        List<int> MeshIndices;

        //UI VARIABLES
        public GameObject MenuPrefab;
        public VideoCaptureUIMenu UIMenu;


        private GameObject ManagerGo;

        public override void OnToolActivated()
        {
            _controller = GetComponent<VRTK_ControllerEvents>();
            _controller.TouchpadPressed += OnTriggerClicked;
            WaypointList = new List<GameObject>();
            CameraWaypointsDict = new Dictionary<int, CameraWaypoint>();
            CloudStateDict = new Dictionary<int, CloudStateInTime>();
            //_controller.TouchpadReleased += OnTriggerReleased;
            //GameObject SpherePrefab = sphere;
            //sphere = Instantiate(SpherePrefab);
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
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
            obj.transform.SetParent(sphere.transform,false);
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
            Screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Screen.name = "Screen";
            Screen.transform.SetParent(this.transform,false);
            Screen.transform.localPosition = new Vector3(0, 0.15f, 0.05f);
            Screen.transform.localScale = new Vector3(0.4f, 0.25f, 0.001f);
            Screen.transform.Rotate(new Vector3(0f, 180f, 0f));
            Screen.GetComponent<MeshRenderer>().material.mainTexture = CameraFeed;


            //BAD, use prefab and Application layer
            ManagerGo = new GameObject();
            ManagerGo.AddComponent<VRCameraWaypointsManager>();
            VRCameraWaypointsManager.instance.MenuPrefab = this.MenuPrefab;
            VRCameraWaypointsManager.instance.MobileCamera = MobileCamera;
            VRCameraWaypointsManager.instance.animationScript = this.animationScript;
            VRCameraWaypointsManager.instance.Initialize();
            /**
            GameObject go = Instantiate(MenuPrefab) as GameObject;
            UIMenu = go.GetComponent<VideoCaptureUIMenu>();
            go.transform.position = ((CameraManager.instance.vr_camera.transform.forward * 1.5f) 
                                    + (CameraManager.instance.vr_camera.transform.up / 2));

            UIMenu.OnTimingEdit += ChangeAllAnimationTimings;
            UIMenu.OnCloudStateDeleted += RemoveCloudState;
            UIMenu.OnRecordStart += PlayAndRecord;
            UIMenu.OnAddCloudState += AddCloudState;
            **/
        }

        public void OnTriggerClicked(object sender, ControllerInteractionEventArgs e)
        {
            
                PlaceWaypoint();
            
                //down
                //PlayAndRecord();
            
            /**
            else if (e.touchpadAxis.x <= -0.15 && e.touchpadAxis.y < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y > (-Mathf.Sqrt(2f) / 2))
            {
                //left
            }
            else if (e.touchpadAxis.x >= 0.15 && e.touchpadAxis.y < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y > (-Mathf.Sqrt(2f) / 2))
            {
                //right

            }
            **/
        }

        private void PlaceWaypoint()
        {

            VRCameraWaypointsManager.instance.PlaceWaypoint(sphere.transform.position, sphere.transform.rotation);
            /**
            //animationScript.AddKeyframe(tr);
            GameObject s = CreateWaypointGameObject();

            if(WaypointList.Count == 0)
            {
                CreateWaypointContainer();
                if (!CloudSelector.instance.noSelection)
                {
                    CloudStateInTime state = CreateCloudState();
                    InitialState = state;                 
                }
            }
                s.transform.SetParent(container.transform);

            //Debug.Log("s : " + s.transform.localRotation.eulerAngles);
            //Debug.Log("sphere : " + sphere.transform.rotation.eulerAngles);

            WaypointList.Add(s);
            CameraWaypoint campoint = new CameraWaypoint(WaypointMasterID, WaypointMasterID * KeyFrameTimestep);
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
            **/
        }

        public override void OnToolDeactivated()
        {
            /**
            if (sphere != null)
            {
                Destroy(sphere);
            }
            **/
            //UIMenu.OnTimingEdit -= ChangeAllAnimationTimings;
           // UIMenu.OnCloudStateDeleted -= RemoveCloudState;
            //UIMenu.OnRecordStart -= PlayAndRecord;
            //UIMenu.OnAddCloudState -= AddCloudState;


            Destroy(Screen);
            Destroy(ManagerGo);
            //foreach(GameObject go in WaypointList)
            //{
               // Destroy(go);
            //}
           // Destroy(container);
            //WaypointList.Clear();
            //CameraWaypointsDict.Clear();
            //CloudStateDict.Clear();
            //Destroy(UIMenu.gameObject);
            //WaypointMasterID = 0;
            _controller.TouchpadPressed -= OnTriggerClicked;

        }

        
        public override void OnDisabled()
        {
            //OnToolDeactivated();
        }
        
    }
}