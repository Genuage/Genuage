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
    public class VRObjectPlacementTool : IControllerTool
    {
        protected VRTK_ControllerEvents _controller;
        protected GameObject sphere;
        public Dictionary<int, VRContainer> containersList;
        public int selectedContainerID = 0;

        public GameObject objectplacementCanvas;
        public GameObject containerselectionCanvas;
        public GameObject sectionselectionCanvas;


        public int UIdisplayID;
        public int selectiondisplayID;
        public TextMesh textmesh;
        public TextMesh sectionselectiontextmesh;

        public GameObject selectedGO = null;


        public int pointNumber = 0;

        public ControllerObjectsRefference controllerRefs;


        public VRObjectPlacementTool(VRTK_ControllerEvents controller)
        {
            _controller = controller;
            //containersList = new Dictionary<int, VRContainer>();
        }

        public override void OnToolActivated()
        {
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.SetParent(this.transform);
            sphere.transform.localScale = new Vector3(0.007f, 0.007f, 0.007f);
            sphere.transform.localPosition = Vector3.forward * 0.05f;
            sphere.transform.localEulerAngles = Vector3.zero;
            sphere.GetComponent<SphereCollider>().isTrigger = true;
            ColliderProxy colliderproxy = sphere.AddComponent<ColliderProxy>();
            colliderproxy.OnCollisionStart += OnSphereCollisionStart;
            colliderproxy.OnCollisionEnd += OnSphereCollisionStop;
            objectplacementCanvas.SetActive(true);
            containerselectionCanvas.SetActive(false);
            sectionselectionCanvas.SetActive(false);

            _controller = GetComponent<VRTK_ControllerEvents>();
            _controller.TouchpadPressed += OnTriggerClicked;
            _controller.TouchpadReleased += OnTriggerReleased;

            textmesh = containerselectionCanvas.GetComponent<VRContainerUIRef>().centertext;

            sectionselectiontextmesh = sectionselectionCanvas.GetComponent<VRContainerUIRef>().centertext;
            selectiondisplayID = 1;
            sectionselectiontextmesh.text = "1";
            containersList = new Dictionary<int, VRContainer>();

            ReloadContainers();

            CloudSelector.instance.OnSelectionChange += ReloadContainers;
            CloudStorage.instance.OnCloudCreated += ReloadContainers;
            //CloudStorage.instance.OnCloudDeleted += ReloadContainers;

            /**
            if (containersList == null)
            {
                containersList = new Dictionary<int, VRContainer>();
                CreateContainer(0);
                selectedContainerID = 0;
                UIdisplayID = 0;
                
                textmesh.text = "0";
                pointNumber = 0;

            }
            else
            {
                pointNumber = containersList[selectedContainerID].GetObjectCount();
            }
            **/

            //CloudSelector.instance.OnSelectionChange += ReloadPointList;
        }

        protected virtual void ReloadContainers(int id = 0)
        {
        }

        protected void CreateNewContainerList()
        {
            /**
            foreach(var kvp in containersList)
            {
                Destroy(kvp.Value.gameObject);
            }
            **/
            containersList = new Dictionary<int, VRContainer>();
            CreateContainer(0);
            selectedContainerID = 0;
            UIdisplayID = 0;

            textmesh.text = "0";
            pointNumber = 0;


        }

        private void OnSphereCollisionStart(Collider other)
        {
            if (other.gameObject.GetComponent<VRObject>())
            {
                other.gameObject.GetComponent<VRObject>().OnSelected();
                selectedGO = other.gameObject;
            }
            //Debug.Log("Collision");
        }

        private void OnSphereCollisionStop(Collider other)
        {
            if (other.gameObject.GetComponent<VRObject>())
            {
                other.gameObject.GetComponent<VRObject>().OnUnselected();
                selectedGO = null;
            }
            //Debug.Log("End Collision");
        }

        public void OnTriggerClicked(object sender, ControllerInteractionEventArgs e)
        {
            if (objectplacementCanvas.activeInHierarchy)
            {
                if (e.touchpadAxis.y >= 0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
                {
                    //up
                    objectplacementCanvas.SetActive(false);
                    containerselectionCanvas.SetActive(true);
                    if (containersList.Count > 0)
                    {
                        containersList[UIdisplayID].SelectObjects();
                    }


                }
                else if (e.touchpadAxis.y <= -0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
                {
                    //down

                }
                else if (e.touchpadAxis.x <= -0.15 && e.touchpadAxis.y < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y > (-Mathf.Sqrt(2f) / 2))
                {
                    RemovePoint();
                    //left
                }
                else if (e.touchpadAxis.x >= 0.15 && e.touchpadAxis.y < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y > (-Mathf.Sqrt(2f) / 2))
                {
                    //right
                    AddPoint();

                }
            }

            else if (containerselectionCanvas.activeInHierarchy)
            {
                if (e.touchpadAxis.y >= 0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
                {
                    //up
                    if (!containersList.ContainsKey(UIdisplayID))
                    {
                        CreateContainer(UIdisplayID);
                    }

                    ChangeContainerSelection(UIdisplayID);
                    containersList[UIdisplayID].UnselectObjects();
                    objectplacementCanvas.SetActive(true);
                    containerselectionCanvas.SetActive(false);


                }
                else if (e.touchpadAxis.y <= -0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
                {
                    if (containersList.ContainsKey(UIdisplayID))
                    {
                        RemoveContainer(UIdisplayID);
                    }
                    //down
                }
                else if (e.touchpadAxis.x <= -0.15 && e.touchpadAxis.y <= (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y >= (-Mathf.Sqrt(2f) / 2))
                {
                    if (UIdisplayID - 1 >= 0)
                    {
                        UnselectContainer(UIdisplayID);
                        UIdisplayID--;

                        UpdateTextMesh();
                        SelectContainer(UIdisplayID);
                    }
                    //left
                }
                else if (e.touchpadAxis.x >= 0.15 && e.touchpadAxis.y <= (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y >= (-Mathf.Sqrt(2f) / 2))
                {
                    UnselectContainer(UIdisplayID);

                    UIdisplayID++;
                    UpdateTextMesh();

                    SelectContainer(UIdisplayID);
                    //right
                }

            }
            else if (sectionselectionCanvas.activeInHierarchy)
            {
                if (e.touchpadAxis.y >= 0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
                {
                    //up


                }
                else if (e.touchpadAxis.y <= -0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
                {
                    //down
                    OnSelectionEnd();
                    /**
                    sectionselectionCanvas.SetActive(false);
                    objectplacementCanvas.SetActive(true);
                **/
                }
                else if (e.touchpadAxis.x <= -0.15 && e.touchpadAxis.y <= (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y >= (-Mathf.Sqrt(2f) / 2))
                {
                    if (selectiondisplayID - 1 >= 1)
                    {
                        selectiondisplayID--;
                        sectionselectiontextmesh.text = selectiondisplayID.ToString();
                        OnSelectionValueChanged();

                    }
                    //left
                }
                else if (e.touchpadAxis.x >= 0.15 && e.touchpadAxis.y <= (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y >= (-Mathf.Sqrt(2f) / 2))
                {
                    selectiondisplayID++;
                    sectionselectiontextmesh.text = selectiondisplayID.ToString();
                    OnSelectionValueChanged();


                    //right
                }


            }

        }

        protected void SelectContainer(int id)
        {
            if (containersList.ContainsKey(id))
            {
                containersList[id].SelectObjects();
                OnContainerSelected(id);

            }

        }

        protected virtual void OnContainerSelected(int id)
        {

        }

        protected void UnselectContainer(int id)
        {
            if (containersList.ContainsKey(id))
            {
                containersList[id].UnselectObjects();
                OnContainerUnselected(id);

            }

        }

        protected virtual void OnContainerUnselected(int id)
        {
            
        }

        protected virtual void OnSelectionEnd()
        {
            sectionselectionCanvas.SetActive(false);
            objectplacementCanvas.SetActive(true);

        }

        protected virtual void OnSelectionValueChanged()
        {

        }

        public virtual void OnTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {

        }

        public void UpdateTextMesh()
        {
            textmesh.text = UIdisplayID.ToString();
        }

        public virtual void ChangeContainerSelection(int id)
        {
            if (containersList.ContainsKey(id))
            {
                selectedContainerID = id;
                Debug.Log("Selected Container ID set to : " + id);
                pointNumber = containersList[selectedContainerID].GetObjectCount();
            }
        }

        public virtual void CreateContainer(int id)
        {
           
        }

        public virtual void RemoveContainer(int id)
        {
            if (containersList.ContainsKey(id))
            {
                VRContainer currcontainer = containersList[id];
                containersList.Remove(id);
                Destroy(currcontainer.gameObject);
            }
        }


        public virtual void RemovePoint()
        {
            if (selectedGO && !(containersList[selectedContainerID].GetObjectCount() == 1))
            {
                containersList[selectedContainerID].DeleteVRObject(selectedGO.GetComponent<VRObject>().id);
                
                Destroy(selectedGO);
            }

            
        }

        public virtual void AddPoint()
        {
            /**
            GameObject newpoint = CreatePoint(containersList[selectedContainerID].GetObjectCount());
            newpoint.GetComponent<VRObjectCounter>().id = containersList[selectedContainerID].GetObjectCount();
            containersList[selectedContainerID].AddVRObject(newpoint.GetComponent<VRObjectCounter>(), containersList[selectedContainerID].GetObjectCount());
            **/
        }

        public virtual GameObject CreatePoint(int id)
        {
            return null;
        }
         /**
        {
           
            GameObject newpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newpoint.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
            newpoint.transform.position = sphere.transform.position;
            //newpoint.AddComponent<CloudChildCounter>();
            newpoint.AddComponent<VRObjectCounter>();
            GameObject text_object = new GameObject("Text");
            text_object.transform.SetParent(newpoint.transform);
            text_object.AddComponent<MeshRenderer>();
            text_object.AddComponent<TextMesh>();
            text_object.GetComponent<TextMesh>().text = id.ToString();
            text_object.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
            text_object.GetComponent<TextMesh>().color = Color.red;

            text_object.transform.localPosition = Vector3.zero;
            text_object.AddComponent<StaringLabel>();
            text_object.transform.localScale = Vector3.one * 0.5f;
            return newpoint;
    
        }
        **/
        public override void OnToolDeactivated()
        {
            objectplacementCanvas.SetActive(false);
            containerselectionCanvas.SetActive(false);
            sectionselectionCanvas.SetActive(false);

            CloudSelector.instance.OnSelectionChange -= ReloadContainers;
            CloudStorage.instance.OnCloudCreated -= ReloadContainers;

            //TEst Feature
            Debug.Log("Test Feature OBjectPlamcement TOOl");
            int[] keys = new int[containersList.Count];
            containersList.Keys.CopyTo(keys,0);
            foreach (var i in keys)
            {
                if (containersList[i].GetObjectCount() == 0) 
                {
                    Destroy(containersList[i].gameObject);

                    containersList.Remove(i);

                }
            }

            //containersList.Clear();
            if (sphere != null)
            {
                Destroy(sphere);
            }

            if (_controller)
            {
                _controller.TouchpadPressed -= OnTriggerClicked;
                _controller.TouchpadReleased -= OnTriggerReleased;
                _controller = null;

            }

            if (selectedGO)
            {
                selectedGO.GetComponent<VRObject>().OnUnselected();
                selectedGO = null;
            }

        }

        public override void OnDisabled()
        {
            objectplacementCanvas.SetActive(false);
            containerselectionCanvas.SetActive(false);
            sectionselectionCanvas.SetActive(false);

            this.enabled = false;
        }
    }




}