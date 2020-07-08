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

namespace VR_Interaction
{
    public class VRRulerPlacementTool : VRObjectPlacementTool
    {


        public VRRulerPlacementTool(VRTK_ControllerEvents controller) : base(controller)
        {
        }

        protected override void ReloadContainers(int id = 0)
        {
            if (!CloudSelector.instance.noSelection)
            {
                Debug.Log("At least one cloud is loaded");

                if (CloudUpdater.instance.LoadStatus(id).globalMetaData.rulerPointsList.Count != 0)
                {
                    Debug.Log("Cloud Contains previous ruler data");
                    containersList = new Dictionary<int, VRContainer>();
                    foreach (var obj in CloudUpdater.instance.LoadStatus(id).globalMetaData.rulerPointsList)
                    {
                        obj.Value.SetActive(true);

                        VRContainerRuler containerscript = obj.Value.GetComponent<VRContainerRuler>();

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
                    Debug.Log("Cloud Contains no previous ruler data");
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
                GameObject VRContainer = new GameObject("Ruler Container");
                VRContainer.AddComponent<MeshRenderer>();
                VRContainer.AddComponent<MeshFilter>();
                VRContainer.AddComponent<VRContainerRuler>();
                VRContainer.GetComponent<VRContainerRuler>().id = id;
                containersList.Add(id, VRContainer.GetComponent<VRContainerRuler>());
            }
        }

        public override void AddPoint()
        {
            if (containersList[selectedContainerID].GetObjectCount() == 0)
            {
                containersList[selectedContainerID].transform.position = sphere.transform.position;
                //containersList[selectedContainerID].gameObject.AddComponent<BoxCollider>();
                containersList[selectedContainerID].gameObject.AddComponent<CloudChildRuler>();
                containersList[selectedContainerID].gameObject.GetComponent<CloudChildRuler>().id = containersList[selectedContainerID].id;
                containersList[selectedContainerID].gameObject.AddComponent<BoxCollider>();
                VRObjectsManager.instance.ContainerCreated(containersList[selectedContainerID].id, containersList[selectedContainerID].gameObject, "Ruler");

            }

            GameObject newpoint = CreatePoint(pointNumber);
            //newpoint.GetComponent<VRObjectMeasure>().id = pointNumber;
            newpoint.transform.SetParent(containersList[selectedContainerID].transform, true);

            containersList[selectedContainerID].AddVRObject(newpoint.GetComponent<VRObjectMeasure>());
            pointNumber++;

        }

        public override void RemovePoint()
        {
            base.RemovePoint();
        }

        public override GameObject CreatePoint(int id)
        {
            GameObject newpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newpoint.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
            newpoint.transform.position = sphere.transform.position;
            newpoint.AddComponent<VRObjectMeasure>();

            //newpoint.transform.SetParent(container.transform, true);

            GameObject text_object = new GameObject("Text");
            text_object.transform.SetParent(newpoint.transform);
            text_object.AddComponent<MeshRenderer>();
            //text_object.GetComponent<MeshRenderer>();
            text_object.AddComponent<TextMesh>();
            
            text_object.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
            text_object.GetComponent<TextMesh>().color = Color.red;
            text_object.transform.localPosition = Vector3.up;
            text_object.AddComponent<StaringLabel>();
            text_object.transform.localScale = Vector3.one * 0.5f;

            return newpoint;

        }

    }


}