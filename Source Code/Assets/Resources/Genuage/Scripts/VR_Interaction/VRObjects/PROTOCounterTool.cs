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

namespace VR_Interaction
{

    

    public class PROTOCounterTool : IControllerTool
    {
        VRTK_ControllerEvents _controller;
        GameObject sphere;
        public Dictionary<int,VRContainer> containersList;
        public int selectedContainerID = 0;

        public GameObject objectplacementCanvas;
        public GameObject containerselectionCanvas;

        public int UIdisplayID;
        public TextMesh textmesh;

        public PROTOCounterTool(VRTK_ControllerEvents controller)
        {
            _controller = controller;
            containersList = new Dictionary<int, VRContainer>();
        }

        public override void OnToolActivated()
        {
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.SetParent(this.transform);
            sphere.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
            sphere.transform.localPosition = Vector3.forward * 0.05f;
            sphere.transform.localEulerAngles = Vector3.zero;
            containersList = new Dictionary<int, VRContainer>();

            objectplacementCanvas.SetActive(true);
            containerselectionCanvas.SetActive(false);

            _controller = GetComponent<VRTK_ControllerEvents>();
            _controller.TouchpadPressed += OnTriggerClicked;

            textmesh = containerselectionCanvas.GetComponent<VRContainerUIRef>().centertext;

            selectedContainerID = 0;
            UIdisplayID = 0;
            textmesh.text = "0";

            CreateContainer(0);

            //CloudSelector.instance.OnSelectionChange += ReloadPointList;
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
                    if(containersList.Count > 0)
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
                        if (containersList.ContainsKey(UIdisplayID))
                        {
                            containersList[UIdisplayID].UnselectObjects();
                        }

                        UIdisplayID--;

                        UpdateTextMesh();
                        if (containersList.ContainsKey(UIdisplayID))
                        {
                            containersList[UIdisplayID].SelectObjects();
                        }
                    }
                        //left
                }
                else if (e.touchpadAxis.x >= 0.15 && e.touchpadAxis.y <= (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y >= (-Mathf.Sqrt(2f) / 2))
                {
                    if (containersList.ContainsKey(UIdisplayID))
                    {
                        containersList[UIdisplayID].UnselectObjects();
                    }
                    UIdisplayID++;
                    UpdateTextMesh();
                    if (containersList.ContainsKey(UIdisplayID))
                    {
                        containersList[UIdisplayID].SelectObjects();
                    }
                    //right
                }

            }

        }

        public void UpdateTextMesh()
        {
            textmesh.text = UIdisplayID.ToString();
        }

        public void ChangeContainerSelection(int id)
        {
            if (containersList.ContainsKey(id))
            {
                selectedContainerID = id;
                Debug.Log("Selected Container ID set to : " + id);
            }
        }

        public void CreateContainer(int id)
        {
            if (!containersList.ContainsKey(id))
            {
                GameObject VRContainer = new GameObject("Container");
                VRContainer.AddComponent<VRContainerCounter>();
                containersList.Add(id, VRContainer.GetComponent<VRContainerCounter>());
            }
        }

        public void RemoveContainer(int id)
        {
            if (containersList.ContainsKey(id))
            {
                VRContainer currcontainer = containersList[id];
                containersList.Remove(id);
                Destroy(currcontainer.gameObject);
            }
        }

        public void AddPoint()
        {
            GameObject newpoint = CreatePoint(containersList[selectedContainerID].GetObjectCount());
            newpoint.GetComponent<VRObjectCounter>().id = containersList[selectedContainerID].GetObjectCount();
            containersList[selectedContainerID].AddVRObject(newpoint.GetComponent<VRObjectCounter>(), containersList[selectedContainerID].GetObjectCount());
        }

        public GameObject CreatePoint(int id)
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

        public override void OnToolDeactivated()
        {
            throw new System.NotImplementedException();
        }

        public override void OnDisabled()
        {
            throw new System.NotImplementedException();
        }
    }
}