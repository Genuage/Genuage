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


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using Data;

namespace VR_Interaction
{


    public class VRHistogramPlacementTool : VRObjectPlacementTool
    {
        public bool task1_On = false;
        public bool task2_On = false;
        public bool taskSectionSelection_On = false;

        public GameObject histogramCanvasPrefab;

        public GameObject baseCircle;
        public GameObject secondCircle;
        public Vector3 secondCicleOriginalPosition;
        int sign;

        public GameObject histogramContainer;
        public List<GameObject> sectionList = new List<GameObject>();

        GameObject lineobject;

        public VRHistogramPlacementTool(VRTK_ControllerEvents controller) : base(controller)
        {

        }

        protected override void ReloadContainers(int id = 0)
        {
            if (!CloudSelector.instance.noSelection)
            {
                Debug.Log("At least one cloud is loaded");

                if (CloudUpdater.instance.LoadStatus(id).globalMetaData.histogramList.Count != 0)
                {
                    Debug.Log("Cloud Contains previous histogram data");
                    containersList = new Dictionary<int, VRContainer>();
                    foreach (var obj in CloudUpdater.instance.LoadStatus(id).globalMetaData.histogramList)
                    {
                        obj.Value.SetActive(true);

                        VRContainerHistogram containerscript = obj.Value.GetComponent<VRContainerHistogram>();

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
                    Debug.Log("Cloud Contains no previous histogram data");
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
                GameObject VRContainer = new GameObject("Histogram Container");
                VRContainer.transform.position = this.transform.position;
                VRContainer.AddComponent<MeshRenderer>();
                VRContainer.AddComponent<MeshFilter>();
                VRContainer.AddComponent<VRContainerHistogram>();
                VRContainer.GetComponent<VRContainerHistogram>().id = id;
                VRContainer.AddComponent<HistogramPointSelector>();

                containersList.Add(id, VRContainer.GetComponent<VRContainerHistogram>());

            }
        }
        
        public override void AddPoint()
        {
            if (!task1_On && !task2_On)
            {
                if (containersList[selectedContainerID].GetObjectCount() == 0)
                {

                    containersList[selectedContainerID].transform.position = sphere.transform.position;
                    containersList[selectedContainerID].gameObject.AddComponent<CloudChildHistogram>();
                    containersList[selectedContainerID].gameObject.GetComponent<CloudChildHistogram>().id = containersList[selectedContainerID].id;

                    containersList[selectedContainerID].gameObject.AddComponent<BoxCollider>();

                    VRObjectsManager.instance.ContainerCreated(containersList[selectedContainerID].id, containersList[selectedContainerID].gameObject, "Histogram");


                    /**
                    containersList[selectedContainerID].gameObject.GetComponent<CapsuleCollider>().center = containersList[selectedContainerID].transform.position;

                    containersList[selectedContainerID].gameObject.GetComponent<CapsuleCollider>().height = Vector3.Distance(baseCircle.transform.position, secondCircle.transform.position);
                    containersList[selectedContainerID].gameObject.GetComponent<CapsuleCollider>().radius = baseCircle.transform.localScale.x;
                    **/


                }


                GameObject go = CreatePoint(0);
                baseCircle = go;
                baseCircle.transform.SetParent(containersList[selectedContainerID].transform,true);
                GameObject go2 = CreatePoint(0);
                secondCircle = go2;

                secondCircle.transform.position = baseCircle.transform.position;
                secondCircle.transform.localScale = baseCircle.transform.localScale;
                secondCircle.transform.rotation = baseCircle.transform.rotation;

                secondCicleOriginalPosition = go2.transform.position;



                controllerRefs.SetUpperTextDisplayActive(true);

                GameObject obj = new GameObject();
                //obj.transform.SetParent(baseCircle.transform, false);
                MeshRenderer mr = obj.AddComponent<MeshRenderer>();
                MeshFilter mf = obj.AddComponent<MeshFilter>();

                List<Vector3> vertices = new List<Vector3>();
                List<int> indices = new List<int>();

                vertices.Add(baseCircle.transform.position);
                vertices.Add(secondCircle.transform.position);
                indices.Add(0);
                indices.Add(1);
                mf.mesh.vertices = vertices.ToArray();
                mf.mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

                mr.material = new Material(Shader.Find("Standard"));
                lineobject = obj;
                task1_On = true;
                return;
            }

            if(task1_On && !task2_On)
            {
                //secondCircle.transform.SetParent(sphere.transform, false);


                task1_On = false;
                secondCircle.transform.SetParent(containersList[selectedContainerID].transform, true);

                task2_On = true;
                return;
            }

            if(task2_On && !task1_On)
            {
                task2_On = false;
                controllerRefs.SetUpperTextDisplayActive(false);

                Destroy(lineobject);

                GameObject go3 = new GameObject("center");
                go3.AddComponent<MeshRenderer>();
                go3.AddComponent<MeshFilter>();
                Mesh mesh = new Mesh();
                List<Vector3> vertices = new List<Vector3>();
                List<int> indices = new List<int>();
                vertices.Add(baseCircle.transform.position + (baseCircle.transform.up * baseCircle.transform.localScale.y));
                vertices.Add(secondCircle.transform.position + (secondCircle.transform.up * secondCircle.transform.localScale.y));
                vertices.Add(secondCircle.transform.position + (-secondCircle.transform.up * secondCircle.transform.localScale.y));
                vertices.Add(baseCircle.transform.position + ( - baseCircle.transform.up * baseCircle.transform.localScale.y));

                vertices.Add(baseCircle.transform.position + (baseCircle.transform.right * baseCircle.transform.localScale.y));
                vertices.Add(secondCircle.transform.position + (-secondCircle.transform.right * secondCircle.transform.localScale.y));
                vertices.Add(secondCircle.transform.position + (secondCircle.transform.right * secondCircle.transform.localScale.y));
                vertices.Add(baseCircle.transform.position + (-baseCircle.transform.right * baseCircle.transform.localScale.y));

                /**
                vertices.Add(baseCircle.transform.position + (new Vector3(Mathf.Sqrt(2)/2, Mathf.Sqrt(2) / 2, 0) * baseCircle.transform.localScale.y));
                vertices.Add(secondCircle.transform.position + (new Vector3(Mathf.Sqrt(2) / 2, Mathf.Sqrt(2) / 2, 0) * secondCircle.transform.localScale.y));
                vertices.Add(secondCircle.transform.position + (new Vector3( - Mathf.Sqrt(2) / 2, Mathf.Sqrt(2) / 2, 0) * secondCircle.transform.localScale.y));
                vertices.Add(baseCircle.transform.position + (new Vector3( - Mathf.Sqrt(2) / 2, Mathf.Sqrt(2) / 2, 0) * baseCircle.transform.localScale.y));
                **/


                for (int i = 0; i < vertices.Count; i++)
                {
                    indices.Add(i);
                }

                mesh.vertices = vertices.ToArray();
                mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

                go3.GetComponent<MeshRenderer>().material = VRMaterials.instance._default;

                go3.GetComponent<MeshFilter>().mesh = mesh;

                GameObject container = new GameObject("Histogram");
                baseCircle.transform.SetParent(container.transform, true);
                secondCircle.transform.SetParent(container.transform, true);
                go3.transform.SetParent(container.transform, true);
                container.AddComponent<VRObjectHistogramCylinder>();

                if (containersList[selectedContainerID].CheckID(0))
                {
                    GameObject obj = containersList[selectedContainerID].GetVRObject(0).gameObject;
                    containersList[selectedContainerID].DeleteVRObject(0);
                    Destroy(obj);
                }
                
                //container.GetComponent<VRObjectHistogramCylinder>().id = containersList[selectedContainerID].GetObjectCount();
                container.transform.SetParent(containersList[selectedContainerID].transform, true);
                containersList[selectedContainerID].gameObject.GetComponent<HistogramPointSelector>().canvasPrefab = histogramCanvasPrefab;
                containersList[selectedContainerID].AddVRObject(container.GetComponent<VRObjectHistogramCylinder>(), 0);
                taskSectionSelection_On = true;
                histogramContainer = container;
                SelectSections();
                return;
            }
        }



        protected override void OnSelectionValueChanged()
        {
            if(sectionList.Count != 0)
            {
                for (int j = 0; j < sectionList.Count; j++)
                {
                    Destroy(sectionList[j]);
                }
                sectionList.Clear();

            }

            float distance = Vector3.Distance(baseCircle.transform.position, secondCircle.transform.position);
            float sizeinterval = distance / selectiondisplayID;
            for (int i = 1; i < selectiondisplayID; i++)
            {
                GameObject newcircle = CreatePoint(0);
                newcircle.transform.position = baseCircle.transform.position;
                newcircle.transform.localScale = baseCircle.transform.localScale;
                newcircle.transform.rotation = baseCircle.transform.rotation;
                Vector3 direction = secondCircle.transform.position - baseCircle.transform.position;
                newcircle.transform.position += (direction.normalized * (i*sizeinterval));
                newcircle.transform.SetParent(histogramContainer.transform);
                sectionList.Add(newcircle);


            }
            HistogramPointSelector selector = containersList[selectedContainerID].GetComponent<HistogramPointSelector>();
            List<GameObject> circleList = new List<GameObject>();
            List<Vector3> circlePositionsList = new List<Vector3>();
            
            circlePositionsList.Add(baseCircle.transform.position);
            circleList.Add(baseCircle);
            foreach (var go in sectionList)
            {
                circlePositionsList.Add(go.transform.position);
                circleList.Add(go);
            }
            circlePositionsList.Add(secondCircle.transform.position);
            circleList.Add(secondCircle);
            selector.radius = baseCircle.transform.localScale.x;
            selector.sectionsNumber = selectiondisplayID;
            selector.FindPointsProto(circleList, circlePositionsList);

        }

        protected override void OnSelectionEnd()
        {
            sectionList.Clear();
            histogramContainer.GetComponent<VRObjectHistogramCylinder>().ReloadMeshList();
            base.OnSelectionEnd();
        }

        public void SelectSections()
        {
            objectplacementCanvas.SetActive(false);
            containerselectionCanvas.SetActive(false);
            sectionselectionCanvas.SetActive(true);
            selectiondisplayID = 1;
            sectionselectiontextmesh.text = selectiondisplayID.ToString();
        }

        public override void RemovePoint()
        {
            base.RemovePoint();
        }

        public override GameObject CreatePoint(int id)
        {
            GameObject go = CreateCircleMesh();
            return go;
        }

        public GameObject CreateCircleMesh()
        {
            GameObject newpoint = new GameObject("Circle");

            //Creates a circle

            MeshRenderer mr = newpoint.AddComponent<MeshRenderer>();
            MeshFilter mf = newpoint.AddComponent<MeshFilter>();

            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();



            int counter = 0;


            int n = 50;
            float radius = 1f;
            float x, y, z;
            for (int i = 0; i < n; i++) // phi
            {
                x = radius * Mathf.Cos((2 * Mathf.PI * i) / n);
                y = radius * Mathf.Sin((2 * Mathf.PI * i) / n);
                z = 0;
                vertices.Add(new Vector3(x, y, z));
                indices.Add(counter++);

                x = radius * Mathf.Cos((2 * Mathf.PI * (i + 1)) / n);
                y = radius * Mathf.Sin((2 * Mathf.PI * (i + 1)) / n);
                z = 0;
                vertices.Add(new Vector3(x, y, z));
                indices.Add(counter++);
            }

            mf.mesh.vertices = vertices.ToArray();
            mf.mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

            mr.material = new Material(Shader.Find("Standard"));



            //newpoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            newpoint.AddComponent<VRObjectAngleCylinder>();


            newpoint.transform.rotation = sphere.transform.rotation;
            newpoint.transform.localScale = Vector3.one * 0.05f;
            newpoint.transform.position = sphere.transform.position;
            //newpoint.transform.SetParent(sphere.transform, true);



            return newpoint;
        }

        public override void RemoveContainer(int id)
        {
            base.RemoveContainer(id);
        }

        public void Update()
        {
            if (task1_On)
            {

                secondCircle.transform.position = sphere.transform.position;
                secondCircle.transform.rotation = sphere.transform.rotation;

                baseCircle.transform.LookAt(secondCircle.transform, secondCircle.transform.up);
                secondCircle.transform.LookAt(baseCircle.transform, baseCircle.transform.up);
                float l = Vector3.Distance(baseCircle.transform.position, secondCircle.transform.position);
                controllerRefs.UpdateUpperTextDisplay("L = "+ System.Math.Round(l,3).ToString());

                MeshRenderer mr = lineobject.GetComponent<MeshRenderer>();
                MeshFilter mf = lineobject.GetComponent<MeshFilter>();

                List<Vector3> vertices = new List<Vector3>();
                List<int> indices = new List<int>();

                vertices.Add(baseCircle.transform.position);
                vertices.Add(secondCircle.transform.position);
                indices.Add(0);
                indices.Add(1);
                mf.mesh.vertices = vertices.ToArray();
                mf.mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);


            }

            if (task2_On)
            {
               

                float distance = Vector2.Distance(new Vector2(secondCircle.transform.position.x, secondCircle.transform.position.y),
                                  new Vector2(sphere.transform.position.x, sphere.transform.position.y));




                baseCircle.transform.localScale = 2 * distance * Vector3.one;
                secondCircle.transform.localScale = 2 * distance * Vector3.one;

                controllerRefs.UpdateUpperTextDisplay("H = " + (System.Math.Round(2*distance,3).ToString()));


                /**
                float distancefull = Vector3.Distance(baseCircle.transform.position, sphere.transform.position);


                float dot = Vector3.Dot(secondCircle.transform.forward, sphere.transform.position - secondCicleOriginalPosition);

                if (dot > 0.0001f)
                {
                    sign = 1;
                }
                else
                {
                    sign = -1;
                }
                secondCircle.transform.position = secondCicleOriginalPosition + (sign * (baseCircle.transform.forward * distancefull));
                **/
            }
        }

        protected override void OnContainerSelected(int id)
        {
            if (containersList[id].GetComponent<HistogramPointSelector>())
            {
                containersList[id].GetComponent<HistogramPointSelector>().enabled = true;
            }
        }

        protected override void OnContainerUnselected(int id)
        {
            if (containersList[id].GetComponent<HistogramPointSelector>())
            {
                containersList[id].GetComponent<HistogramPointSelector>().enabled = false;
            }
        }
    }
}