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
using System.Threading;
using UnityEngine;
using VRTK;
using Display;
using Data;
using IO;
using System.Linq;
//using UnityEditor.SceneManagement;

namespace VR_Interaction
{

    public class VRFreeSelectionTool : IControllerTool
    {
        protected VRTK_ControllerEvents _controller;

        public float diameter;

        protected GameObject sphere;
        public Material material;
        public bool jobON = false;
        private GameObject FreeSelectionUICanvas;

        public Vector3 sphereposition;

        private PointSelectorDistanceBased pointSelector;


        public VRFreeSelectionTool(VRTK_ControllerEvents controller)
        {
            //_controller = controller;
        }

        //DEBUG
        //public void Awake()
        //{
            //OnToolActivated();
        //}

        public override void OnToolActivated()
        {
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<MeshRenderer>().material = material;
            sphere.GetComponent<SphereCollider>().enabled = false;
            sphere.transform.SetParent(this.transform);
            sphere.transform.localScale = 0.07f * Vector3.one;
            sphere.transform.localPosition = Vector3.forward * 0.05f;
            sphere.transform.localEulerAngles = Vector3.zero;
            sphere.GetComponent<SphereCollider>().isTrigger = true;
            pointSelector = gameObject.AddComponent<PointSelectorDistanceBased>();


            _controller = GetComponent<VRTK_ControllerEvents>();
            _controller.TouchpadPressed += OnTriggerClicked;

            //_controller.TouchpadAxisChanged += new ControllerInteractionEventHandler(OnTouchPadPressed);
            //_controller.TouchpadTouchEnd += new ControllerInteractionEventHandler(OnTouchPadReleased);

            diameter = 0.07f;
            FreeSelectionUICanvas = GetComponent<ControllerObjectsRefference>().CanvasFreeSelection;
            FreeSelectionUICanvas.SetActive(true);
            CloudUpdater.instance.ChangeFreeSelectionActivation(true);
            jobON = true;
            Shader.EnableKeyword("FREE_SELECTION");


        }

        private void DoSelection()
        {
            if (!CloudSelector.instance.noSelection)
            {
                pointSelector.data = CloudUpdater.instance.LoadCurrentStatus();
                pointSelector.worldPosition = sphere.transform.position;
                pointSelector.selectionRadius = diameter/2;
                pointSelector.DoSelection();
            }
        }

        private void DeleteSelection()
        {
            if (!CloudSelector.instance.noSelection)
            {
                pointSelector.data = CloudUpdater.instance.LoadCurrentStatus();
                pointSelector.worldPosition = sphere.transform.position;
                pointSelector.selectionRadius = diameter / 2;
                pointSelector.DoDeletion();
            }

        }


        public void OnTriggerClicked(object sender, ControllerInteractionEventArgs e)
        {

            if (e.touchpadAxis.y >= 0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
            {
                //up
                DoSelection();
            }

            else if (e.touchpadAxis.y <= -0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
            {
                //down
                DeleteSelection();
            }

            else if (e.touchpadAxis.x <= -0.15 && e.touchpadAxis.y < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y > (-Mathf.Sqrt(2f) / 2))
            {
                //left
                //Debug.Log("Left");
                diameter -= 0.01f;
                if(diameter < 0)
                {
                    diameter = 0f;
                }
                sphere.transform.localScale = diameter * Vector3.one;


            }
            else if (e.touchpadAxis.x >= 0.15 && e.touchpadAxis.y < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y > (-Mathf.Sqrt(2f) / 2))
            {
                //right
                //Debug.Log("Right");
                diameter += 0.01f; 
                sphere.transform.localScale = diameter * Vector3.one;


            }
        }

        private void Update()
        {
            if(jobON == true)
            {
                if (!CloudSelector.instance.noSelection)
                {
                    sphereposition = sphere.transform.position;
                    CloudData data = CloudUpdater.instance.LoadCurrentStatus();
                    Transform box = data.transform.parent.GetComponent<CloudObjectRefference>().box.transform;
                    Shader.SetGlobalMatrix("_BoxWorldToLocalMatrix", box.worldToLocalMatrix);
                    Shader.SetGlobalVector("_BoxLocalScale", box.localScale);

                    Shader.SetGlobalVector("_SpherePosition", sphere.transform.position);
                    Shader.SetGlobalFloat("_SphereRadius", sphere.transform.localScale.x);

                }

            }
        }


        public override void OnToolDeactivated()
        {
            jobON = false;
            Shader.DisableKeyword("FREE_SELECTION");
            FreeSelectionUICanvas.SetActive(false);

            if (sphere != null)
            {
                Destroy(sphere);
            }

            if (_controller)
            {
                _controller.TouchpadPressed -= OnTriggerClicked;
                _controller = null;

            }

            Destroy(pointSelector);
        }

        public override void OnDisabled()
        {
            //FreeSelectionUICanvas.SetActive(false);

            this.enabled = false;
        }

    }


    public class PointSelectorDistanceBased : MonoBehaviour
    {
        //Selects all points in the current cloud based on their distance from a position

        public CloudData data;
        public Vector3 worldPosition;
        public float selectionRadius;

        SphereSelectionThreadHandler Thread;

        bool SelectionTaskOn = false;
        bool DeletionTaskOn = false;

        public HashSet<int> selectedPoints;

        /**
        private void Awake()
        {
            worldPosition = Vector3.zero;
            selectionRadius = 0f;
            //unitaryTaskOn = false;

        }
        **/

        private void PrepareThread()
        {
            Matrix4x4 world_to_local = data.gameObject.transform.worldToLocalMatrix;

            Vector3 selflocalpos = world_to_local.MultiplyPoint3x4(worldPosition);

            Mesh mesh = data.gameObject.GetComponent<MeshFilter>().mesh;
            Vector3[] points = mesh.vertices;
            Color[] colors = new Color[points.Length];
            selectedPoints = data.globalMetaData.FreeSelectionIDList;
            SphereSelectionThreadHandler newHandler = new SphereSelectionThreadHandler(selectionRadius, data, selflocalpos, colors);
            Thread newThread = new Thread(new ThreadStart(newHandler.ThreadLoop));
            newHandler.thread = newThread;
            newThread.Start();
            Thread = newHandler;

        }

        public void DoSelection()
        {
            PrepareThread();
            SelectionTaskOn = true;

        }

        public void DoDeletion()
        {
            PrepareThread();
            DeletionTaskOn = true;
        }

        private void Update()
        {
            //Debug.Log("UpdateStart");
            if (SelectionTaskOn == true)
            {
                if (Thread.isFinished == true)
                {
                    SelectionTaskOn = false;

                    selectedPoints.UnionWith(Thread.pointSelectionList);
                    data.globalMetaData.FreeSelectionIDList = selectedPoints;
                    Thread.thread.Join();
                    UpdateSelection();
                    Debug.Log(selectedPoints.Count);
                }
            }

            if (DeletionTaskOn == true)
            {
                if (Thread.isFinished == true)
                {
                    DeletionTaskOn = false;

                    selectedPoints.ExceptWith(Thread.pointSelectionList);
                    data.globalMetaData.FreeSelectionIDList = selectedPoints;
                    Thread.thread.Join();
                    UpdateSelection();
                    Debug.Log(selectedPoints.Count);
                }
            }

        }

        public void UpdateSelection()
        {
            CloudUpdater.instance.UpdatePointSelection();

        }


    }

}