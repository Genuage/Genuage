﻿/**
Copyright (c) 2020, 	Institut Curie, Institut Pasteur and CNRS
			Thomas BLanc, Mohamed El Beheiry, Jean Baptiste Masson, Bassam Hajj and Clement Caporal
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the fol lowing disclaimer.
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


using Data;
using Display;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using VR_Interaction;
using VR_Interaction.Convex_Hull;
using DesktopInterface;

namespace IO
{
    /// <summary>
    /// CloudLoader reads files from provided paths and loads the values in the CloudStorage instance.
    /// </summary>
    /// 
    public class CloudLoader : MonoBehaviour
    {
        private const char DefaultSeparator = '\t';
        public GameObject _cloudPointPrefab;
        public static CloudLoader instance = null;
        public Texture2D _sprite_texture;
        public Material _material;
        private CloudSaveableData savedata = null;

        private char Separator = '\t';
        private bool IgnoreHeader = false;
        private int StartLineNumber = 0;

        GameObject window;

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

        public void ChangeSeparator(char newSeparator)
        {
            Separator = newSeparator;

        }

        public void ChangeHeaderIgnore(bool ignore)
        {
            IgnoreHeader = ignore;

        }

        public delegate void OnCloudCreatedEvent(int id);
        public event OnCloudCreatedEvent OnCloudCreated;

        public void LoadFromFile(string file_path)
        {
            UIManager.instance.ChangeStatusText("Loading Cloud...");
            savedata = null;
            bool isJSON;
            string directory = Path.GetDirectoryName(file_path);
            string filename = Path.GetFileNameWithoutExtension(file_path);
            string jsonpath = directory + Path.DirectorySeparatorChar + filename + ".JSON";
            if (File.Exists(jsonpath))
            {
                isJSON = true;
            }
            else
            {
                isJSON = false;
            }

            if (IgnoreHeader)
            {
                //Don't read first line
                StartLineNumber = 1;
            }
            else
            {
                StartLineNumber = 0;
            }

            string[] lines = File.ReadAllLines(file_path);
           
            int N = lines.Length;
            
            List<List<float>> columnDataList = new List<List<float>>();

            int columnNumber = lines[StartLineNumber].Split(Separator).Length;
            for (int i = 0; i < columnNumber; i++)
            {
                columnDataList.Add(new List<float>());
            }
            for (int j = StartLineNumber; j< lines.Length; j++)
            {
                string[] entries = lines[j].Split(Separator);
                float?[] numberArray = new float?[entries.Length]; //Define array of nullable floats
                bool noNullValues = true;
                for (int k = 0; k < entries.Length; k++)
                {
                    if(entries[k] == "NaN" || entries[k] == "nan")
                    {
                        //Deal with unavailable entries
                    }
                    float parsednumber;
                    bool ParseSuccess = Single.TryParse(entries[k], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out parsednumber);
                    if (!ParseSuccess)
                    {
                        numberArray[k] = null;
                        noNullValues = false;
                        //UIManager.instance.ChangeStatusText("ERROR : Parsing Error at line " + k+1 + ", the file could not be loaded");
                        //return;
                    }
                    else
                    {
                        numberArray[k] = parsednumber;
                        //collumnDataList[k][j] = parsednumber;
                    }
                    //collumnDataList[k][j] = Single.Parse(entries[k], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                }
                
               
                if (noNullValues)
                {
                    for (int m = 0; m < entries.Length; m++)
                    {
                        columnDataList[m].Add((float)numberArray[m]);
                    }
                }
            }

            List<float[]> newDataList = new List<float[]>();
            foreach(List<float> list in columnDataList)
            {
                newDataList.Add(list.ToArray());
            }

            GameObject cloud = CreateCloudPoint(newDataList, file_path,isJSON,jsonpath);
            PutInMemory(cloud);

            if (isJSON)
            {
                CreateSubGameObjects(cloud.GetComponent<CloudData>());
            }

            Destroy(window);

            CloudSelector.instance.UpdateSelection(cloud.GetComponent<CloudData>().globalMetaData.cloud_id);

            //OnCloudCreated(cloud.GetComponent<CloudData>().globalMetaData.cloud_id);

        }

        public void LoadFromConnection(List<float[]> columnDataList)
        {
            savedata = null;
            //window = ModalWindowManager.instance.CreateModalWindow("Loading the cloud, please wait...");

            foreach (float[] i in columnDataList)
            {
                foreach (float pointvalue in i)
                {
                    if (!(pointvalue.GetType() == typeof(float)))
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : Data number format is not float, data not loaded");
                        UIManager.instance.ChangeStatusText("ERROR : Data number format can't be translated to float");
                        return;
                    }
                }
            }
            GameObject cloud = CreateCloudPoint(columnDataList);
            PutInMemory(cloud);
            //Destroy(window);
            CloudSelector.instance.UpdateSelection(cloud.GetComponent<CloudData>().globalMetaData.cloud_id);

            //OnCloudCreated(cloud.GetComponent<CloudData>().globalMetaData.cloud_id);

        }

        public void LoadFromRawData(List<float[]> columnDataList)
        {
            savedata = null;
            //window = ModalWindowManager.instance.CreateModalWindow("Loading the cloud, please wait...",false);

            GameObject cloud = CreateCloudPoint(columnDataList);
            PutInMemory(cloud);
            Destroy(window);
            CloudSelector.instance.UpdateSelection(cloud.GetComponent<CloudData>().globalMetaData.cloud_id);

            //OnCloudCreated(cloud.GetComponent<CloudData>().globalMetaData.cloud_id);

        }


        public void LoadJSON(string path, CloudData data)
        {
            string[] json = File.ReadAllLines(path);
            savedata = JsonUtility.FromJson<CloudSaveableData>(json[0]);

            //data.pointMetaDataTable = savedata.pointMetaDataTable;
            //data.globalMetaData.displayCollumnsConfiguration = savedata.displayCollumnsConfiguration;
            data.globalMetaData.colormapName = savedata.colormapName;
            data.globalMetaData.colormapReversed = savedata.colormapReversed;
            data.globalMetaData.current_colormap_variable = savedata.currentColormapVariable;
            data.globalMetaData.current_normed_variable = savedata.currentNormedVariable;
            data.globalMetaData.point_size = savedata.pointSize;
            data.globalMetaData.scale = savedata.scale;
            data.globalMetaData.fileName = savedata.fileName;
            if (savedata.pointsByLocationDict != null)
            {
                data.globalMetaData.pointbyLocationList = savedata.pointsByLocationDict;
            }
            else
            {
                data.globalMetaData.pointbyLocationList = new Dictionary<Vector3Int, List<int>>();

            }
        }
        #region CreateVRObjects
        public void CreateSubGameObjects(CloudData data)
        {
            int j = 0;
            #region Counters
            for (int i = 0; i < savedata.CounterContainerIDList.Count; i++)
            {
                //Create Container
                GameObject container = new GameObject("CounterContainer");
                container.AddComponent<MeshRenderer>();
                container.AddComponent<MeshFilter>();
                container.AddComponent<VRContainerCounter>();
                container.GetComponent<VRContainerCounter>().id = savedata.CounterContainerIDList[i];
                container.transform.position = savedata.CounterContainerPositions[i];
                container.AddComponent<CloudChildCounter>();
                container.GetComponent<CloudChildCounter>().id = savedata.CounterContainerIDList[i];
                container.AddComponent<BoxCollider>();



                for (int k = 0; k < savedata.CounterObjectsNumberList[i]; k++)
                {
                    if(j == savedata.CounterObjectsPositions.Count)
                    {
                        UIManager.instance.ChangeStatusText("Error While Loading Saved VR Objects");
                        ModalWindowManager.instance.CreateModalWindow("Error While Loading Saved VRObjects");
                        break;
                    }
                    GameObject countersphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //countersphere.transform.position = savedata.CounterObjectsPositions[j];
                    countersphere.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
                    countersphere.AddComponent<VRObjectCounter>();
                    countersphere.GetComponent<VRObjectCounter>().id = savedata.CounterObjectIDList[j];
                    //countersphere.AddComponent<CloudChildCounter>();
                    GameObject text_object = new GameObject("Text");
                    text_object.transform.SetParent(countersphere.transform);
                    text_object.AddComponent<MeshRenderer>();

                    //text_object.GetComponent<MeshRenderer>();
                    text_object.AddComponent<TextMesh>();
                    text_object.GetComponent<TextMesh>().text = savedata.CounterObjectIDList[j].ToString();
                    text_object.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
                    text_object.GetComponent<TextMesh>().color = Color.red;

                    text_object.transform.localPosition = Vector3.zero;
                    text_object.AddComponent<StaringLabel>();
                    text_object.transform.localScale = Vector3.one * 0.5f;

                    countersphere.transform.SetParent(container.transform,false);
                    countersphere.transform.localPosition = savedata.CounterObjectsPositions[j];
                    container.GetComponent<VRContainerCounter>().AddVRObject(countersphere.GetComponent<VRObjectCounter>());
                    j++;
                }
                data.globalMetaData.counterPointsList.Add(container.GetComponent<VRContainerCounter>().id, container);

            }
            #endregion

            j = 0;
            #region Ruler
            for (int i = 0; i < savedata.RulerContainerIDList.Count; i++)
            {
                //Create Container
                GameObject container = new GameObject("Ruler Container");
                container.AddComponent<MeshRenderer>();
                container.AddComponent<MeshFilter>();
                container.AddComponent<VRContainerRuler>();
                container.GetComponent<VRContainerRuler>().id = savedata.RulerContainerIDList[i];
                container.transform.position = savedata.RulerContainerPositionsList[i];
                container.AddComponent<CloudChildRuler>();
                container.GetComponent<CloudChildRuler>().id = savedata.RulerContainerIDList[i];
                container.AddComponent<BoxCollider>();

                for (int k = 0; k < savedata.RulerObjectsNumberList[i]; k++)
                {
                    if (j == savedata.RulerObjectPositionsList.Count)
                    {
                        UIManager.instance.ChangeStatusText("Error While Loading Saved VRObjects");
                        ModalWindowManager.instance.CreateModalWindow("Error While Loading Saved VRObjects");
                        break;
                    }

                    GameObject newpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    newpoint.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
                    newpoint.AddComponent<VRObjectMeasure>();
                    newpoint.GetComponent<VRObjectMeasure>().id = savedata.RulerObjectIDList[j];
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

                    newpoint.transform.SetParent(container.transform, false);
                    newpoint.transform.localPosition = savedata.RulerObjectPositionsList[j];

                    container.GetComponent<VRContainerRuler>().maxRange = data.globalMetaData.maxRange;
                    container.GetComponent<VRContainerRuler>().AddVRObject(newpoint.GetComponent<VRObjectMeasure>());
                    j++;

                }

                data.globalMetaData.rulerPointsList.Add(container.GetComponent<VRContainerRuler>().id, container);

            }

            #endregion


            #region Sphere Selection

            for (int l = 0; l < savedata.SpheresContainerPositionList.Count; l++)
            {
                GameObject container = new GameObject("Container");
                container.AddComponent<MeshRenderer>();
                container.AddComponent<MeshFilter>();
                container.AddComponent<VRContainerSelectionSphere>();
                container.GetComponent<VRContainerSelectionSphere>().id = savedata.SpheresContainerIDList[l];
                container.transform.position = savedata.SpheresContainerPositionList[l];
                container.gameObject.AddComponent<CloudChildSphere>();
                container.gameObject.GetComponent<CloudChildSphere>().id = savedata.SpheresContainerIDList[l];

                container.gameObject.AddComponent<BoxCollider>();

                GameObject selectionsphere = CreateSphereGameObject(savedata.SpheresContainerPositionList[l]);
                selectionsphere.transform.localScale = 2 * savedata.RadiusList[l] * Vector3.one;
                selectionsphere.GetComponent<PointSelectorSphere>().radius = savedata.RadiusList[l];
                selectionsphere.GetComponent<VRObjectSelectionSphere>().id = container.GetComponent<VRContainerSelectionSphere>().GetObjectCount();
                selectionsphere.transform.SetParent(container.transform, true);
                container.GetComponent<VRContainerSelectionSphere>().AddVRObject(selectionsphere.GetComponent<VRObjectSelectionSphere>(), 0);
                selectionsphere.GetComponent<PointSelectorSphere>().data = data;
                selectionsphere.GetComponent<PointSelectorSphere>().DoSelectionOnce();

                data.globalMetaData.sphereList.Add(container.GetComponent<VRContainerSelectionSphere>().id, container);

            }
            /**
            foreach(var kvp in data.globalMetaData.sphereList)
            {

            }
            **/
            #endregion

            j = 0;
            #region Convex Hull
            for (int i = 0; i < savedata.ConvexHullContainerIDList.Count; i++)
            {
                GameObject container = new GameObject("Convex Hull Container");
                container.AddComponent<MeshRenderer>();
                container.AddComponent<MeshFilter>();
                container.AddComponent<VRContainerConvexHull>();
                container.GetComponent<VRContainerConvexHull>().id = savedata.ConvexHullContainerIDList[i];
                container.transform.position = savedata.ConvexHullContainersPositionsList[i];
                //container.AddComponent<CloudChildConvexHull>();
                //container.GetComponent<CloudChildConvexHull>().id = savedata.ConvexHullContainerIDList[i];
                //container.AddComponent<BoxCollider>();

                for (int k = 0; k < savedata.ConvexHullObjectNumberList[i]; k++)
                {
                    if (j == savedata.ConvexHullObjectPositionsList.Count)
                    {
                        UIManager.instance.ChangeStatusText("Error While Loading Saved VRObjects");
                        ModalWindowManager.instance.CreateModalWindow("Error While Loading Saved VRObjects");
                        break;
                    }

                    GameObject newpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    newpoint.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
                    newpoint.AddComponent<VRObjectConvexHull>();
                    newpoint.GetComponent<VRObjectConvexHull>().id = savedata.ConvexHullObjectIDList[j];
                    newpoint.transform.SetParent(container.transform, false);
                    newpoint.transform.localPosition = savedata.ConvexHullObjectPositionsList[j];

                    container.GetComponent<VRContainerConvexHull>().AddVRObject(newpoint.GetComponent<VRObjectConvexHull>());

                    j++;

                }

                data.globalMetaData.convexHullsList.Add(container.GetComponent<VRContainerConvexHull>().id, container);

            }

            #endregion

            //CloudUpdater.instance.UpdatePointSelection();

            CloudUpdater.instance.ChangeColorMap(data.globalMetaData.cloud_id, data.globalMetaData.colormapName, data.globalMetaData.colormapReversed);


            #region ANgleMeasure
            j = 0;
            for (int i = 0; i < savedata.AngleMeasureContainersIDList.Count; i++)
            {
                GameObject container = new GameObject("Angle Measurement Container");
                container.transform.position = savedata.AngleMeasureContainersPositionsList[i];
                container.AddComponent<MeshRenderer>();
                container.AddComponent<MeshFilter>();
                container.AddComponent<VRContainerAngleMeasurement>();
                container.GetComponent<VRContainerAngleMeasurement>().id = savedata.AngleMeasureContainersIDList[i];

                container.gameObject.AddComponent<CloudChildAngleMeasure>();
                container.gameObject.GetComponent<CloudChildAngleMeasure>().id = savedata.AngleMeasureContainersIDList[i];
                container.gameObject.AddComponent<BoxCollider>();

                for (int k = 0; k < savedata.AngleMeasureObjectNumberList[i]; k++)
                {
                    if (j == savedata.AnglemeasureObjectsPositionsList.Count)
                    {
                        UIManager.instance.ChangeStatusText("Error While Loading Saved VRObjects");

                        ModalWindowManager.instance.CreateModalWindow("Error While Loading Saved VRObjects");
                        break;
                    }

                    GameObject newpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    newpoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    newpoint.AddComponent<VRObjectAngleCylinder>();
                    newpoint.GetComponent<VRObjectAngleCylinder>().id = savedata.AngleMeasureObjectsIDList[j];
                    newpoint.transform.SetParent(container.transform, false);
                    newpoint.transform.localPosition = savedata.AnglemeasureObjectsPositionsList[j];
                    container.GetComponent<VRContainerAngleMeasurement>().AddVRObject(newpoint.GetComponent<VRObjectAngleCylinder>(), newpoint.GetComponent<VRObjectAngleCylinder>().id);

                    j++;

                }

                data.globalMetaData.angleMeasurementsList.Add(container.GetComponent<VRContainerAngleMeasurement>().id, container);

            }
            #endregion

            #region Histogram
            for (int i = 0; i < savedata.HistogramContainersIDList.Count; i++)
            {
                
                GameObject container = new GameObject("Histogram Container");
                container.AddComponent<MeshRenderer>();
                container.AddComponent<MeshFilter>();
                container.AddComponent<VRContainerHistogram>();
                container.GetComponent<VRContainerHistogram>().id = savedata.HistogramContainersIDList[i];
                container.AddComponent<HistogramPointSelector>();
                container.transform.position = savedata.HistogramContainersPositionsList[i];

                container.AddComponent<CloudChildHistogram>();
                container.GetComponent<CloudChildHistogram>().id = savedata.HistogramContainersIDList[i];

                container.AddComponent<BoxCollider>();



                GameObject baseCircle = CreateCircleMesh(Vector3.zero);

                GameObject secondCircle = CreateCircleMesh(Vector3.zero);

                GameObject histogram = new GameObject("histogram");

                histogram.transform.SetParent(container.transform, true);
                histogram.transform.localPosition = savedata.HistogramPositionList[i];
                histogram.transform.Rotate(savedata.HistogramRotationList[i]);
                baseCircle.transform.SetParent(histogram.transform, true);
                secondCircle.transform.SetParent(histogram.transform, true);

                baseCircle.transform.localPosition = savedata.HistogramBaseCirclesPositionsList[i];
                secondCircle.transform.localPosition = savedata.HistogramSecondCirclesPositionsList[i];

                baseCircle.transform.localScale = savedata.HistogramRadiusList[i] * Vector3.one;
                secondCircle.transform.localScale = savedata.HistogramRadiusList[i] * Vector3.one;

                secondCircle.transform.LookAt(baseCircle.transform);

                baseCircle.transform.LookAt(secondCircle.transform);


                histogram.transform.SetParent(container.transform, true);


                GameObject go3 = new GameObject("center");
                go3.AddComponent<MeshRenderer>();
                go3.AddComponent<MeshFilter>();
                Mesh mesh = new Mesh();
                List<Vector3> vertices = new List<Vector3>();
                List<int> indices = new List<int>();
                vertices.Add(baseCircle.transform.position + (baseCircle.transform.up * baseCircle.transform.localScale.y));
                vertices.Add(secondCircle.transform.position + (secondCircle.transform.up * secondCircle.transform.localScale.y));
                vertices.Add(secondCircle.transform.position + (-secondCircle.transform.up * secondCircle.transform.localScale.y));
                vertices.Add(baseCircle.transform.position + (-baseCircle.transform.up * baseCircle.transform.localScale.y));

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


                for (int k = 0; k< vertices.Count; k++)
                {
                    indices.Add(k);
                }

                mesh.vertices = vertices.ToArray();
                mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

                go3.GetComponent<MeshRenderer>().material = VRMaterials.instance._default;

                go3.GetComponent<MeshFilter>().mesh = mesh;

                go3.transform.SetParent(histogram.transform, true);

                List<GameObject> sectionList = new List<GameObject>();

                float distance = Vector3.Distance(baseCircle.transform.position, secondCircle.transform.position);
                float sizeinterval = distance / savedata.HistogramSectionNumbersList[i];
                for (int u = 1; u < savedata.HistogramSectionNumbersList[i]; u++)
                {
                    GameObject newcircle = CreateCircleMesh(Vector3.zero);
                    newcircle.transform.position = baseCircle.transform.position;
                    newcircle.transform.localScale = baseCircle.transform.localScale;
                    newcircle.transform.rotation = baseCircle.transform.rotation;
                    Vector3 direction = secondCircle.transform.position - baseCircle.transform.position;
                    newcircle.transform.position += (direction.normalized * (u * sizeinterval));
                    newcircle.transform.SetParent(histogram.transform);
                    sectionList.Add(newcircle);


                }
                histogram.AddComponent<VRObjectHistogramCylinder>();
                histogram.GetComponent<VRObjectHistogramCylinder>().id = 0;
                container.GetComponent<VRContainerHistogram>().AddVRObject(histogram.GetComponent<VRObjectHistogramCylinder>(), histogram.GetComponent<VRObjectHistogramCylinder>().id);

                HistogramPointSelector selector = container.GetComponent<HistogramPointSelector>();
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
                selector.sectionsNumber = savedata.HistogramSectionNumbersList[i];
                selector.FindPointsProto(circleList, circlePositionsList);



                data.globalMetaData.histogramList.Add(container.GetComponent<VRContainerHistogram>().id, container);

            }
            #endregion
        }
        private GameObject CreateSphereGameObject(Vector3 positionVector)
        {
            GameObject _sphere = new GameObject("Sphere");
            _sphere.transform.position = positionVector;

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
            //_sphere.AddComponent<ParentConstraint>();
            //_sphere.AddComponent<CloudChildSphere>();

            return _sphere;

        }

        public GameObject CreateCircleMesh(Vector3 position)
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


            //newpoint.transform.rotation = sphere.transform.rotation;
            newpoint.transform.localScale = Vector3.one * 0.05f;
            newpoint.transform.position = position;
            //newpoint.transform.SetParent(sphere.transform, true);



            return newpoint;
        }
        #endregion



        private void PutInMemory(GameObject cloud)
        {
            CloudStorage.instance.AddCloud(cloud);
        }

        private GameObject CreateCloudPoint(List<float[]> columnDataList, string file_path = null, bool isJSON = false, string json_path = null)
        {
            GameObject container = Instantiate(_cloudPointPrefab) as GameObject; /// GameObject that is the parent of the CloudPoint (used for CloudParent offset)
            GameObject root = container.transform.Find("CloudPoint").gameObject; /// GameObject where the mesh will be displayed

            // Init Cloud Status and references
            CloudData rootdata = root.GetComponent<CloudData>();
            rootdata.columnData = columnDataList;

            if (isJSON && json_path != null)
            {
                LoadJSON(json_path, rootdata);
            }
            else
            {


                Dictionary<string, int> _CloudDataVariablesDict = new Dictionary<string, int>()
                {
                    {"x_position", 0},
                    {"y_position", 0},
                    {"z_position", 0},
                    {"color", 0},
                    {"time", 0},
                    {"trajectory_index", 0},
                    {"phi_angle", 0},
                    {"theta_angle", 0},
                    {"wobble_angle", 0},
                    {"point_size", 0},
                    {"precision_xy", 0},
                    {"precision_z", 0}

                };

                string[] strarray = new string[12]
                {
                    "x_position",
                    "y_position",
                    "z_position",
                    "color",
                    "time",
                    "trajectory_index",
                    "phi_angle",
                    "theta_angle",
                    "wobble_angle",
                    "point_size",
                    "precision_xy",
                    "precision_z"
                };
                int cpt = 0;
                foreach (string s in strarray)
                {
                    int v = 0;
                    if (cpt < columnDataList.Count)
                    {
                        v = cpt;
                        cpt++;
                    }
                    _CloudDataVariablesDict[s] = v;
                }

                rootdata.globalMetaData.CloudDataVariablesDict = _CloudDataVariablesDict;


            }

            for (int j = 0; j < columnDataList.Count; j++)
            {
                ColumnMetadata metadata = new ColumnMetadata();
                metadata.ColumnID = j;
                metadata.MinValue = Mathf.Infinity;
                metadata.MaxValue = Mathf.NegativeInfinity;
                metadata.Range = 0;
                metadata.MinThreshold = 0;
                metadata.MaxThreshold = 0;
                rootdata.globalMetaData.columnMetaDataList.Add(metadata);
            }

            for ( int i = 0 ; i < columnDataList[0].Length; i++)
            {
                rootdata.CreatePointData(i, Vector3.zero, Vector3.zero, 0f, 0f, 0f, Color.black, 0f);

                rootdata.CreatePointMetaData(i, 0);
                
                for(int j = 0; j < columnDataList.Count; j++)
                {
                    float nbr = columnDataList[j][i];
                    if(nbr < rootdata.globalMetaData.columnMetaDataList[j].MinValue)
                    {
                        rootdata.globalMetaData.columnMetaDataList[j].MinValue = nbr;
                    }

                    if(nbr > rootdata.globalMetaData.columnMetaDataList[j].MaxValue)
                    {
                        rootdata.globalMetaData.columnMetaDataList[j].MaxValue = nbr;
                    }
                }
                foreach(ColumnMetadata md in rootdata.globalMetaData.columnMetaDataList)
                {
                    md.MaxThreshold = md.MaxValue;
                    md.MinThreshold = md.MinValue;
                    md.Range = md.MaxValue - md.MinValue;
                }
            }

            LoadCloudData(rootdata,file_path,isJSON);

            root.name = "CloudPoint";
            root.transform.position = container.transform.position;
            root.transform.localScale = new Vector3(1, 1, 1);
            
            CloudBox box = root.AddComponent<CloudBox>();
            box.Activate();

           
            container.GetComponent<CloudObjectRefference>().cloud = root;
            return root;


        }


        public void LoadCloudData(CloudData cloud_data,string file_path = null,bool isJSON = false)
        {
            int N = cloud_data.columnData[0].Length;
            // Init lists
            int[] _indices = new int[N];
            Vector3[] _positions = new Vector3[N];

            float[] _intensity = new float[N];
            float[] _time = new float[N];
            float[] _trajectory = new float[N];
            Color[] _colors = new Color[N];

            float[] _phi = new float[N];
            float[] _theta = new float[N];
            float[] _wobble = new float[N];

            float[] _size = new float[N];
            float[] _precisionxy = new float[N];
            float[] _precisionz = new float[N];

            List<Vector2> uv0List = new List<Vector2>();

            List<Vector2> uv1List = new List<Vector2>();
            List<Vector2> uv2List = new List<Vector2>();
            List<Vector2> uv3List = new List<Vector2>();

            Vector3[] _normed_positions = new Vector3[N];

            //// Set default min max value
            float xMax = Mathf.NegativeInfinity;
            float xMin = Mathf.Infinity;
            float yMax = Mathf.NegativeInfinity;
            float yMin = Mathf.Infinity;
            float zMax = Mathf.NegativeInfinity;
            float zMin = Mathf.Infinity;
            float iMax = Mathf.NegativeInfinity;
            float iMin = Mathf.Infinity;
            float tMax = Mathf.NegativeInfinity;
            float tMin = Mathf.Infinity;


            float sMax = Mathf.NegativeInfinity;
            float sMin = Mathf.Infinity;

            xMax = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["x_position"]].MaxValue;
            xMin = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["x_position"]].MinValue;
            yMax = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["y_position"]].MaxValue;
            yMin = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["y_position"]].MinValue;
            zMax = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["z_position"]].MaxValue;
            zMin = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["z_position"]].MinValue;
            iMax = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["color"]].MaxValue;
            iMin = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["color"]].MinValue;
            tMax = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["time"]].MaxValue;
            tMin = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["time"]].MinValue;

            sMax = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["point_size"]].MaxValue;
            sMin = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["point_size"]].MinValue;

            float xRange = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["x_position"]].Range;
            float yRange = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["y_position"]].Range;
            float zRange = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["z_position"]].Range;
            float iRange = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["color"]].Range;
            float tRange = cloud_data.globalMetaData.columnMetaDataList[cloud_data.globalMetaData.CloudDataVariablesDict["time"]].Range;

            float[] rangeList = new float[] { xRange, yRange, zRange };
            float MaxRange = Mathf.Max(rangeList);

            Vector3 offsetVector = new Vector3((xMin + xMax) / 2,
                                               (yMin + yMax) / 2,
                                               (zMin + zMax) / 2);

            float normedxMax = Mathf.NegativeInfinity;
            float normedxMin = Mathf.Infinity;
            float normedyMax = Mathf.NegativeInfinity;
            float normedyMin = Mathf.Infinity;
            float normedzMax = Mathf.NegativeInfinity;
            float normedzMin = Mathf.Infinity;

            List<float> TimeList = new List<float>();


            float[] TimeArray = new float[N];
            cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["time"]].CopyTo(TimeArray, 0);
            IEnumerable<float> DistinctTimeArray = TimeArray.Distinct();
            TimeList = DistinctTimeArray.ToList();

            TimeList.Sort();

            Dictionary<float, int> FrameDict = new Dictionary<float, int>();
            for (int u = 0; u < TimeList.Count; u++)
            {
                FrameDict.Add(TimeList[u], u);
            }


            for (int i = 0; i < N; i++)
            {

                _positions[i] = new Vector3(cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["x_position"]][i],
                                            cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["y_position"]][i],
                                            cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["z_position"]][i]);

                _intensity[i] = cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["color"]][i];
                _trajectory[i] = cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["trajectory_index"]][i];

                _time[i] = cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["time"]][i];

                _phi[i] = cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["phi_angle"]][i];
                _theta[i] = cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["theta_angle"]][i];

                _wobble[i] = cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["wobble_angle"]][i];

                _size[i] = cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["point_size"]][i];

                _precisionxy[i] = cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["precision_xy"]][i];
                _precisionz[i] = cloud_data.columnData[cloud_data.globalMetaData.CloudDataVariablesDict["precision_z"]][i];

                _indices[i] = i;

                _normed_positions[i] = ((_positions[i] - offsetVector) / MaxRange);
                
                _colors[i] = new Color(((_intensity[i] - iMin) / (iMax - iMin)), ((_intensity[i] - iMin) / iMax - iMin), ((_intensity[i] - iMin) / iMax - iMin), 1);

                cloud_data.pointDataTable[i].position = _positions[i];
                cloud_data.pointDataTable[i].normed_position = _normed_positions[i];
                cloud_data.pointDataTable[i].intensity = _intensity[i];
                cloud_data.pointDataTable[i].time = _time[i];
                cloud_data.pointDataTable[i].frame = FrameDict[_time[i]];
                cloud_data.pointDataTable[i].trajectory = _trajectory[i];
                cloud_data.pointDataTable[i].size = (_size[i] - sMin) / (sMax - sMin);
                cloud_data.pointDataTable[i].phi_angle = _phi[i];
                cloud_data.pointDataTable[i].theta_angle = _theta[i];
                cloud_data.pointDataTable[i].wobble_angle = _wobble[i];
                cloud_data.pointDataTable[i].precision_xy = _precisionxy[i];
                cloud_data.pointDataTable[i].precision_z = _precisionz[i];

                //cloud_data.pointDataTable[point].color = _colors[point];
                cloud_data.pointDataTable[i].color_index = _colors[i].r;

                uv0List.Add(new Vector2(cloud_data.pointDataTable[i].size, 0f));
                uv1List.Add(new Vector2(cloud_data.pointDataTable[i].color_index, i));
                //uv3List.Add(new Vector2(cloud_data.pointDataTable[point].trajectory, cloud_data.pointDataTable[point].time));
                cloud_data.pointDataTable[i].depth = _positions[i].z;

                float selected = 0f;
                float hidden = 0f;
                if (cloud_data.pointMetaDataTable[i].isSelected)
                {
                    selected = 1f;
                }
                if (cloud_data.pointMetaDataTable[i].isHidden)
                {
                    hidden = 1f;
                }
                uv2List.Add(new Vector2(selected, hidden));

                uv3List.Add(new Vector2(cloud_data.pointDataTable[i].trajectory, cloud_data.pointDataTable[i].frame));

                if (normedxMax < _normed_positions[i].x) { normedxMax = _normed_positions[i].x; }
                if (normedxMin > _normed_positions[i].x) { normedxMin = _normed_positions[i].x; }
                if (normedyMax < _normed_positions[i].y) { normedyMax = _normed_positions[i].y; }
                if (normedyMin > _normed_positions[i].y) { normedyMin = _normed_positions[i].y; }
                if (normedzMax < _normed_positions[i].z) { normedzMax = _normed_positions[i].z; }
                if (normedzMin > _normed_positions[i].z) { normedzMin = _normed_positions[i].z; }
                


                // Update min max
                /**
                if (xMax < _positions[i].x) { xMax = _positions[i].x; }
                if (xMin > _positions[i].x) { xMin = _positions[i].x; }
                if (yMax < _positions[i].y) { yMax = _positions[i].y; }
                if (yMin > _positions[i].y) { yMin = _positions[i].y; }
                if (zMax < _positions[i].z) { zMax = _positions[i].z; }
                if (zMin > _positions[i].z) { zMin = _positions[i].z; }
                if (iMax < _intensity[i]) { iMax = _intensity[i]; }
                if (iMin > _intensity[i]) { iMin = _intensity[i]; }
                if (tMax < _time[i]) { tMax = _time[i]; }
                if (tMin > _time[i]) { tMin = _time[i]; }
                **/
            }
            



            cloud_data.globalMetaData.maxRange = MaxRange;
            cloud_data.globalMetaData.offsetVector = offsetVector;
            cloud_data.globalMetaData.normed_xMax = normedxMax;
            cloud_data.globalMetaData.normed_xMin = normedxMin;
            cloud_data.globalMetaData.normed_yMax = normedyMax;
            cloud_data.globalMetaData.normed_yMin = normedyMin;
            cloud_data.globalMetaData.normed_zMax = normedzMax;
            cloud_data.globalMetaData.normed_zMin = normedzMin;
            cloud_data.globalMetaData.timeList = TimeList;
            cloud_data.globalMetaData.lowerframeLimit = 0f;
            cloud_data.globalMetaData.upperframeLimit = TimeList.Count-1;
            cloud_data.globalMetaData.lowertimeLimit = cloud_data.globalMetaData.timeList[(int)cloud_data.globalMetaData.lowerframeLimit];
            cloud_data.globalMetaData.uppertimeLimit = cloud_data.globalMetaData.timeList[(int)cloud_data.globalMetaData.upperframeLimit];

            Debug.Log("NormedXMIN" + normedxMin);
            Debug.Log("NormedXMAX" + normedxMax);
            Debug.Log("NormedYMIN" + normedyMin);
            Debug.Log("NormedYMAX" + normedyMax);
            Debug.Log("NormedZMIN" + normedzMin);
            Debug.Log("NormedZMAX" + normedzMax);

            if (file_path == null)
            {
                cloud_data.globalMetaData.fileName = "Cloud - "+N+" points";
            }
            else
            {
                cloud_data.globalMetaData.fileName = ""+Path.GetFileName(file_path)+" - "+N+" points";
            }
            
            cloud_data.globalMetaData.box_scale = new Vector3(xRange, yRange, zRange) / MaxRange;



            //Create the global metadata for the cloud
            if (!isJSON)
            {
                cloud_data.globalMetaData.colormapName = "autumn";
                cloud_data.globalMetaData.colormapReversed = false;
                cloud_data.globalMetaData.point_size = 0.1f;
                cloud_data.globalMetaData.scale = Vector3.one;
            }
            cloud_data.InitGlobalCloudConstant(xMax, xMin, yMax, yMin, zMax, zMin, iMax, iMin, tMax, tMin);

            LoadCloudMesh(cloud_data,_normed_positions,_indices, uv0List.ToArray(), uv1List.ToArray(), uv2List.ToArray(), uv3List.ToArray());
        }

        private void LoadCloudMesh(CloudData root, Vector3[] vertices, int[] indices, Vector2[] uv0Array, Vector2[] uv1Array, Vector2[] uv2Array, Vector2[] uv3Array)
        {

            _material.SetTexture("_SpriteTex", _sprite_texture);
            _material.SetTexture("_ColorTex", ColorMapManager.instance.GetColorMap("autumn").texture);
            _material.SetFloat("UpperTimeLimit", root.globalMetaData.upperframeLimit);
            _material.SetFloat("LowerTimeLimit", root.globalMetaData.lowerframeLimit);
            root.gameObject.GetComponent<MeshRenderer>().material = _material;
            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices;
            mesh.uv = uv0Array;
            mesh.uv2 = uv1Array; // colorID, pointID
            mesh.uv3 = uv2Array; // isSelected, isHidden
            //mesh.uv4 = uv3Array; // trajectoryID, frame
            mesh.SetIndices(indices, MeshTopology.Points, 0);
            root.GetComponent<MeshFilter>().mesh = mesh;

            CreateLineMesh(root, vertices, uv0Array, uv1Array, uv2Array, uv3Array); 

            UIManager.instance.ChangeStatusText(indices.Length + " points loaded");
            Debug.Log(indices.Length + " points loaded into cloudData");
            Debug.Log(vertices.Length + " vertices");
            Debug.Log(" number of columns in data structure : " + root.columnData.Count);
            
        }

        private void CreateLineMesh(CloudData root, Vector3[] point_vertices, Vector2[] uv0Array, Vector2[] uv1Array, Vector2[] uv2Array, Vector2[] uv3Array)
        {
            float offsetxy = 0.0001f;
            float offsetz = 0.0001f;

            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();
            List<Vector2> uv0 = new List<Vector2>();
            List<Vector2> uv1 = new List<Vector2>();
            List<Vector2> uv2 = new List<Vector2>();
            List<Vector2> uv3 = new List<Vector2>();


            Vector3 OffsetVectorX = new Vector3(offsetxy, 0.0f, 0.0f);
            Vector3 OffsetVectorY = new Vector3(0.0f, offsetxy, 0.0f);
            Vector3 OffsetVectorZ = new Vector3(0.0f, 0.0f, offsetz);

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            
            int index = 0;
            int cpt = 0;
            for (int i = 0; i < root.columnData[0].Length; i++)
            {
                Vector3 newVectorX = OffsetVectorX + new Vector3((root.pointDataTable[i].precision_xy) / (root.globalMetaData.xMax - root.globalMetaData.xMin), 0.0f, 0.0f);
                vertices.Add(point_vertices[i] + newVectorX); // calculate offset here
                indices.Add(index);
                uv0.Add(uv0Array[cpt]);
                uv1.Add(uv1Array[cpt]);
                uv2.Add(uv2Array[cpt]);
                uv3.Add(uv3Array[cpt]);
                index++;

                vertices.Add(point_vertices[i] - newVectorX);
                indices.Add(index);
                uv0.Add(uv0Array[cpt]);
                uv1.Add(uv1Array[cpt]);
                uv2.Add(uv2Array[cpt]);
                uv3.Add(uv3Array[cpt]);
                index++;


                Vector3 newVectorY = OffsetVectorY + new Vector3(0.0f, (root.pointDataTable[i].precision_xy) / (root.globalMetaData.yMax - root.globalMetaData.yMin), 0.0f);

                vertices.Add(point_vertices[i] + newVectorY);
                indices.Add(index);
                uv0.Add(uv0Array[cpt]);
                uv1.Add(uv1Array[cpt]);
                uv2.Add(uv2Array[cpt]);
                uv3.Add(uv3Array[cpt]);
                index++;

                vertices.Add(point_vertices[i] - newVectorY);
                indices.Add(index);
                uv0.Add(uv0Array[cpt]);
                uv1.Add(uv1Array[cpt]);
                uv2.Add(uv2Array[cpt]);
                uv3.Add(uv3Array[cpt]);
                index++;

                Vector3 newVectorZ = OffsetVectorZ + new Vector3(0.0f, 0.0f, (root.pointDataTable[i].precision_z) / (root.globalMetaData.zMax - root.globalMetaData.zMin));

                vertices.Add(point_vertices[i] + newVectorZ);
                indices.Add(index);
                uv0.Add(uv0Array[cpt]);
                uv1.Add(uv1Array[cpt]);
                uv2.Add(uv2Array[cpt]);
                uv3.Add(uv3Array[cpt]);
                index++;

                vertices.Add(point_vertices[i] - newVectorZ);
                indices.Add(index);
                uv0.Add(uv0Array[cpt]);
                uv1.Add(uv1Array[cpt]);
                uv2.Add(uv2Array[cpt]);
                uv3.Add(uv3Array[cpt]);
                index++;

                cpt++;

            }
            mesh.vertices = vertices.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            mesh.uv = uv0.ToArray();
            mesh.uv2 = uv1.ToArray(); // colorID, pointID
            mesh.uv3 = uv2.ToArray(); // isSelected, isHidden

            GameObject go = new GameObject();
            MeshFilter mf = go.AddComponent<MeshFilter>();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mf.mesh = mesh;
            Material material = new Material(Shader.Find("Genuage/UnlitLineShader"));
            material.SetTexture("_ColorTex", ColorMapManager.instance.GetColorMap("autumn").texture);
            mr.material = material;

            go.transform.SetParent(root.transform, false);
            go.transform.localPosition = Vector3.zero;
            root.LineShaderObject = go;
            go.SetActive(false);

        }
    }
}


