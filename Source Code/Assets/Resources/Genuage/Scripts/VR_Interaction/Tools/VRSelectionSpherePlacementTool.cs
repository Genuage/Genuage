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
using UnityEngine.Animations;
using VRTK;
using VRTK.GrabAttachMechanics;
using Display;
using Data;

namespace VR_Interaction
{


    public class VRSelectionSpherePlacementTool : VRObjectPlacementTool
    {
        public bool _on_ROI_Selection = false;

        public VRSelectionSpherePlacementTool(VRTK_ControllerEvents controller) : base(controller)
        {
        }

        protected override void ReloadContainers(int id = 0)
        {
            if (!CloudSelector.instance.noSelection)
            {
                Debug.Log("At least one cloud is loaded");

                if (CloudUpdater.instance.LoadStatus(id).globalMetaData.sphereList.Count != 0)
                {
                    Debug.Log("Cloud Contains previous sphere data");
                    containersList = new Dictionary<int, VRContainer>();
                    foreach (var obj in CloudUpdater.instance.LoadStatus(id).globalMetaData.sphereList)
                    {
                        obj.Value.SetActive(true);

                        VRContainerSelectionSphere containerscript = obj.Value.GetComponent<VRContainerSelectionSphere>();

                        containersList.Add(containerscript.id, containerscript);
                    }
                    selectedContainerID = 0;
                    UIdisplayID = 0;
                    textmesh.text = "0";

                    pointNumber = containersList[selectedContainerID].GetObjectCount();
                    objectplacementCanvas.SetActive(false);
                    containerselectionCanvas.SetActive(true);

                }
                else
                {
                    Debug.Log("Cloud Contains no previous sphere data");
                    CreateNewContainerList();
                }

            }
            else
            {
                Debug.Log("No Cloud is loaded");
                CreateNewContainerList();

            }
        }


        public override void CreateContainer(int id)
        {
            if (!containersList.ContainsKey(id))
            {
                GameObject VRContainer = new GameObject("Container");
                VRContainer.AddComponent<MeshRenderer>();
                VRContainer.AddComponent<MeshFilter>();
                VRContainer.AddComponent<VRContainerSelectionSphere>();
                VRContainer.GetComponent<VRContainerSelectionSphere>().id = id;

                containersList.Add(id, VRContainer.GetComponent<VRContainerSelectionSphere>());

            }
        }

        public override void AddPoint()
        {
            if (containersList[selectedContainerID].GetObjectCount() == 0)
            {
                containersList[selectedContainerID].transform.position = sphere.transform.position;
                containersList[selectedContainerID].gameObject.AddComponent<CloudChildSphere>();
                containersList[selectedContainerID].gameObject.GetComponent<CloudChildSphere>().id = containersList[selectedContainerID].id;

                containersList[selectedContainerID].gameObject.AddComponent<BoxCollider>();

                VRObjectsManager.instance.ContainerCreated(containersList[selectedContainerID].id, containersList[selectedContainerID].gameObject, "Sphere");


            }

            if (containersList[selectedContainerID].CheckID(0))
            {
                GameObject obj = containersList[selectedContainerID].GetVRObject(0).gameObject;
                containersList[selectedContainerID].DeleteVRObject(0);
                Destroy(obj);
            }
            GameObject newpoint = CreatePoint(containersList[selectedContainerID].GetObjectCount());
            newpoint.GetComponent<VRObjectSelectionSphere>().id = containersList[selectedContainerID].GetObjectCount();
            newpoint.transform.SetParent(containersList[selectedContainerID].transform, true);
            containersList[selectedContainerID].AddVRObject(newpoint.GetComponent<VRObjectSelectionSphere>(), 0);
            _on_ROI_Selection = true;
        }

        public override void RemovePoint()
        {
            base.RemovePoint();
        }



        public override GameObject CreatePoint(int id)
        {
            GameObject _sphere = new GameObject("Sphere");
            _sphere.transform.position = sphere.transform.position;

            MeshRenderer mr = _sphere.AddComponent<MeshRenderer>();
            MeshFilter mf = _sphere.AddComponent<MeshFilter>();


            // create vertices
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();
            int counter = 0;

            int n = 50;
            float radius = 0.5f;
            float x, y, z;
            int h = 25;
            for (int j = 0; j < h; j++) // theta
            {
                for (int i = 0; i < n; i++) // phi
                {
                    x = radius * Mathf.Sin((Mathf.PI * j) / h) * Mathf.Cos((2 * Mathf.PI * i) / n);
                    y = radius * Mathf.Sin((Mathf.PI * j) / h) * Mathf.Sin((2 * Mathf.PI * i) / n);
                    z = radius * Mathf.Cos((Mathf.PI * j) / h);
                    vertices.Add(new Vector3(x, y, z));
                    indices.Add(counter++);

                    x = radius * Mathf.Sin((Mathf.PI * j) / h) * Mathf.Cos((2 * Mathf.PI * (i + 1)) / n);
                    y = radius * Mathf.Sin((Mathf.PI * j) / h) * Mathf.Sin((2 * Mathf.PI * (i + 1)) / n);
                    z = radius * Mathf.Cos((Mathf.PI * j) / h);
                    vertices.Add(new Vector3(x, y, z));
                    indices.Add(counter++);
                }
            }

            mf.mesh.vertices = vertices.ToArray();
            mf.mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

            mr.material = new Material(Shader.Find("Standard"));

            _sphere.AddComponent<VRObjectSelectionSphere>();

            _sphere.AddComponent<Rigidbody>();
            _sphere.GetComponent<Rigidbody>().useGravity = false;
            _sphere.GetComponent<Rigidbody>().isKinematic = true;
            _sphere.AddComponent<SphereCollider>();
            _sphere.GetComponent<SphereCollider>().isTrigger = true;
            _sphere.AddComponent<PointSelectorSphere>();
            _sphere.GetComponent<PointSelectorSphere>().radius = radius;
            //_sphere.GetComponent<PointSelectorSphere>().taskOn = true;
            _sphere.AddComponent<ParentConstraint>();
            //_sphere.AddComponent<CloudChildSphere>();

            return _sphere;

        }

        public override void OnTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            if (objectplacementCanvas.activeInHierarchy)
            {
                if (e.touchpadAxis.x >= 0.15 && e.touchpadAxis.y < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y > (-Mathf.Sqrt(2f) / 2))
                {
                    //Debug.Log("controller position check");
                    //containersList[selectedContainerID].GetVRObject(0).GetComponent<PointSelectorSphere>().taskOn = false;
                    containersList[selectedContainerID].GetVRObject(0).GetComponent<PointSelectorSphere>().DoSelectionOnce();
                    containersList[selectedContainerID].GetVRObject(0).GetComponent<PointSelectorSphere>().UpdateSelection();

                    _on_ROI_Selection = false;

                }

                
            }
        }

        public void Update()
        {
            if (_on_ROI_Selection)
            {




                    GameObject _current_sphere = containersList[selectedContainerID].GetVRObject(0).gameObject;
                    float distance = Vector3.Distance(_current_sphere.transform.position, sphere.transform.position);
                    _current_sphere.transform.localScale = 2 * distance * Vector3.one;
                    _current_sphere.GetComponent<PointSelectorSphere>().radius = distance;


                
            }
        }

    }
}


