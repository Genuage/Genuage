/**
Copyright (c) 2020, 	Institut Curie, Institut Pasteur and CNRS
			Thomas BLanc, Mohamed El Beheiry, Jean Baptiste Masson, Bassam Hajj and Clement caporal
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
using System.Threading;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using Unity.Jobs;
using Unity.Collections;
using VR_Interaction;
using IO;
using DesktopInterface;
using SFB;


/// <summary>
/// CloudUpdater performs operations to change the data or metadata of a cloud, its functions can be called from the UI elements.
/// </summary>
namespace Data
{


    public class CloudUpdater : ICloudUpdater
    {
        public int _id;
        string defaultMapName = "autumn";
        public static CloudUpdater instance = null;


        CalculateDensityThreadHandler densityThreadHandle;
        //Thread calculatedensity;
        bool densityJobON = false;
        int idThreaded;

        private Queue<SelectPointsThreadHandler> PointSelectionThreadList;
        public int maxSelectionThreads = 10;
        SelectPointsThreadHandler selectionThreadHandle;
        bool selectionJobON = false;

        private Queue<ThresholdThreadHandler> ThresholdThreadQueue;
        public int maxThresholdThreads = 10;
        public bool thresholdJobON = false;

        private ZMQSynchronizedCommunicator ZMQSyncCom = null;
        private void Start()
        {
            PointSelectionThreadList = new Queue<SelectPointsThreadHandler>();
            ThresholdThreadQueue = new Queue<ThresholdThreadHandler>();
            CloudStorage.instance.OnCloudCreated += InitializeColorMap;
            CloudStorage.instance.OnCloudDeleted += DestroyCloudSubObjects;
            CloudSelector.instance.OnSelectionChange += DisplayOneSelectedCloud;

        }

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
        #region Data Loading
        public override int GetSelection()
        {
            /**
            if (CloudSelector.instance.noSelection)
            {
            }
            **/
            return CloudSelector.instance.GetID();
        }

        public CloudData LoadCurrentStatus()
        {
            /**
            if (CloudSelector.instance.noSelection)
            {
            }
            **/
            _id = GetSelection();
            return LoadStatus(_id);
        }

        public override CloudData LoadStatus(int id)
        {
            CloudData cloud = CloudStorage.instance.GetStatus(id);
            return cloud;
        }

        public List<int> LoadAllIDs()
        {
            List<int> IDList = CloudStorage.instance.GetAllIDs();
            return IDList;
        }
        #endregion

        #region Activate Clouds And GameObjects
        public override void SetCloudActive(int id, bool setting)
        {
            if (CloudStorage.instance.CheckID(id))
            {
                //Debug.Log("Setting cloud number " + id + " to " + setting);
                CloudStorage.instance.table[id].transform.parent.gameObject.SetActive(setting);

                SetCloudSubObjectsActive(id, setting);
                

            }
        }

        public void SetCloudSubObjectsActive(int id, bool setting)
        {
            CloudData data = LoadStatus(id);
            foreach (var obj in data.globalMetaData.counterPointsList)
            {
                obj.Value.SetActive(setting);
            }

            foreach (var obj in data.globalMetaData.convexHullsList)
            {
                obj.Value.SetActive(setting);
            }

            foreach (var obj in data.globalMetaData.sphereList)
            {
                obj.Value.SetActive(setting);
            }

            foreach (var obj in data.globalMetaData.rulerPointsList)
            {
                obj.Value.SetActive(setting);
            }

            foreach (var obj in data.globalMetaData.angleMeasurementsList)
            {
                obj.Value.SetActive(setting);
            }

            foreach (var obj in data.globalMetaData.histogramList)
            {
                obj.Value.SetActive(setting);
            }


        }

        public void DestroyCloudSubObjects(int id)
        {
            CloudData data = LoadStatus(id);
            for (int i = 0;i< data.globalMetaData.counterPointsList.Count;i++)
            {
                Destroy(data.globalMetaData.counterPointsList[i]);
            }

            for (int j = 0; j < data.globalMetaData.rulerPointsList.Count; j++)
            {
                Destroy(data.globalMetaData.rulerPointsList[j]);
            }

            for (int k = 0; k < data.globalMetaData.convexHullsList.Count; k++)
            {
                Destroy(data.globalMetaData.convexHullsList[k]);
            }

            for (int l = 0; l < data.globalMetaData.sphereList.Count; l++)
            {
                Destroy(data.globalMetaData.sphereList[l]);
            }

            for (int m = 0; m < data.globalMetaData.angleMeasurementsList.Count; m++)
            {
                Destroy(data.globalMetaData.angleMeasurementsList[m]);
            }

            for (int n = 0; n < data.globalMetaData.histogramList.Count; n++)
            {
                Destroy(data.globalMetaData.histogramList[n]);
            }


        }


        public void DisplayOneSelectedCloud(int id)
        {
            if (CloudStorage.instance.CheckID(id))
            {
                foreach (KeyValuePair<int, CloudData> item in CloudStorage.instance.table)
                {
                    if (id != item.Key)
                    {
                        SetCloudActive(item.Key, false);
                    }
                    else
                    {
                        SetCloudActive(item.Key, true);

                    }
                }
            }
        }
        public override void DisplayLinkedClouds(List<int> cloudIDList)
        {
            foreach (KeyValuePair<int, CloudData> item in CloudStorage.instance.table)
            {
                if (cloudIDList.Contains(item.Key))
                {
                    SetCloudActive(item.Key, true);

                }
                else
                {
                    SetCloudActive(item.Key, false);
                }

            }

        }
        #endregion

        #region Visualization Parameters

        #region Cloud Box Parameters
        //USED BY THE VIDEO CAPTURE SYSTEM FOR EFFCIENT MESH CHANGES
        public void OverrideBoxScale(Vector3 new_scale)
        {
            CloudData currcloud = LoadCurrentStatus();
            Transform box = currcloud.transform.parent.GetChild(1);
            //box.localScale = new_scale;
            currcloud.globalMetaData.box_scale = new_scale;
        }


        public void ReloadAllBoxes()
        {
            List<int> IDlist = LoadAllIDs();
            foreach(int id in IDlist)
            {
                ReloadBox(id);
            }
        }

        public void ReloadBox(int id)
        {
            CloudData currcloud = LoadStatus(id);
            currcloud.gameObject.GetComponent<CloudBox>().CreateBox();
        }

        public void ChangeBoxGraduationsX(float value)
        {
            CloudData currentCloud = LoadCurrentStatus();
            currentCloud.globalMetaData.ScaleBarNumberX = (int)value;
            currentCloud.gameObject.GetComponent<CloudBox>().CreateBox();
        }
        public void ChangeBoxGraduationsY(float value)
        {
            CloudData currentCloud = LoadCurrentStatus();
            currentCloud.globalMetaData.ScaleBarNumberY = (int)value;
            currentCloud.gameObject.GetComponent<CloudBox>().CreateBox();
        }
        public void ChangeBoxGraduationsZ(float value)
        {
            CloudData currentCloud = LoadCurrentStatus();
            currentCloud.globalMetaData.ScaleBarNumberZ = (int)value;
            currentCloud.gameObject.GetComponent<CloudBox>().CreateBox();
        }

        #endregion
        public override void ChangeCloudScale(Vector3 new_scale)
        {
            CloudData currentCloud = LoadCurrentStatus();
            currentCloud.globalMetaData.scale = new_scale;
            currentCloud.transform.localScale = new_scale;
            GameObject box = currentCloud.transform.parent.GetComponent<CloudObjectRefference>().box;
            box.transform.localScale = new Vector3(currentCloud.globalMetaData.box_scale.x * new_scale.x,
                                                   currentCloud.globalMetaData.box_scale.y * new_scale.y,
                                                   currentCloud.globalMetaData.box_scale.z * new_scale.z);
        }

        public delegate void OnColorMapChangeEvent(string name);
        public event OnColorMapChangeEvent OnColorMapChange;

        public delegate void OnColorMapReversedEvent(bool isReversed);
        public event OnColorMapReversedEvent OnColorMapReversed;


        public void InitializeColorMap(int id)
        {
            CloudData data = LoadStatus(id);
            string mapname = data.globalMetaData.colormapName;
            bool reversed = data.globalMetaData.colormapReversed;
            ChangeColorMap(id, mapname, reversed);
        }

        public void ChangeCurrentColorMap(string newMapName = "autumn", bool reverse = false)
        {
            ChangeColorMap(GetSelection(), newMapName, reverse);
            OnColorMapReversed(reverse);
            
        }

        public override void ChangeColorMap(int id, string newMapName = "autumn", bool reverse = false)
        {
            CloudData currentCloud = LoadStatus(id);

            Texture2D texture = ColorMapManager.instance.GetColorMap(newMapName).texture;

            currentCloud.GetComponent<MeshRenderer>().material.SetTexture("_ColorTex", texture);
            if(reverse == true)
            {
                currentCloud.GetComponent<MeshRenderer>().material.EnableKeyword("COLORMAP_REVERSED");
            }
            else
            {
                currentCloud.GetComponent<MeshRenderer>().material.DisableKeyword("COLORMAP_REVERSED");
            }
            currentCloud.globalMetaData.colormapReversed = reverse;
            
            
            OnColorMapChange(newMapName);
            //Debug.Log("eventcheck");
            currentCloud.globalMetaData.colormapName = newMapName;
            
            /**
            CloudData currentCloud = LoadStatus(id);
            Texture2D texture = ColorMapManager.instance.GetColorMap(newMapName).texture;
            Mesh mesh = currentCloud.gameObject.GetComponent<MeshFilter>().mesh;
            Color[] colors = new Color[mesh.GetIndices(0).Length];
            Color[] colormap = texture.GetPixels();

            if (reverse)
            {
                float diff = (currentCloud.globalMetaData.current_normed_variable[1] - currentCloud.globalMetaData.current_normed_variable[0]);
                if (diff == 0)
                {
                    diff = 1;
                }

                foreach (int point in currentCloud.pointDataTable.Keys)
                {
                    if (currentCloud.pointMetaDataTable[point].isHidden == false)
                    {


                        if (currentCloud.pointMetaDataTable[point].isSelected == true && currentCloud.pointMetaDataTable[point].trueColorOverride == false)
                        {
                            //Debug.Log("selectedpoint");

                            colors[point] = Color.green;
                        }
                        else
                        {


                            int x = Mathf.RoundToInt(256 * ((currentCloud.globalMetaData.current_colormap_variable[point] - currentCloud.globalMetaData.current_normed_variable[0]) / diff));
                            if (x < 256 && x >= 0)
                            {
                                colors[point] = colormap[255 - x];
                            }
                            else
                            {
                                colors[point] = colormap[0];
                            }
                            currentCloud.pointDataTable[point].color = colors[point];

                        }
                    }
                    else
                    {
                        colors[point] = Color.clear;
                    }

                    //currentCloud.pointDataTable[point].color = colors[point];

                }

            }
            else
            {
                float diff = (currentCloud.globalMetaData.current_normed_variable[1] - currentCloud.globalMetaData.current_normed_variable[0]);
                if (diff == 0)
                {
                    diff = 1;
                }

                foreach (int point in currentCloud.pointDataTable.Keys)
                {
                    if (currentCloud.pointMetaDataTable[point].isHidden == false)
                    {
                        if (currentCloud.pointMetaDataTable[point].isSelected == true && currentCloud.pointMetaDataTable[point].trueColorOverride == false)
                        {
                            //Debug.Log("selectedpoint");
                            colors[point] = Color.green;
                        }
                        else
                        {

                            int x = Mathf.RoundToInt(256 * ((currentCloud.globalMetaData.current_colormap_variable[point] - currentCloud.globalMetaData.current_normed_variable[0]) / diff));
                            if (x < 256 && x >= 0)
                            {
                                colors[point] = colormap[x];
                            }
                            else
                            {
                            colors[point] = colormap[255];
                            }
                            currentCloud.pointDataTable[point].color = colors[point];

                        }

                    }
                    else
                    {
                        colors[point] = Color.clear;
                    }

                }
            }

            mesh.colors = colors;
            currentCloud.gameObject.GetComponent<MeshFilter>().mesh = mesh;
            currentCloud.globalMetaData.colormapReversed = reverse;
            **/
            if (newMapName != currentCloud.globalMetaData.colormapName)
            {
                OnColorMapChange(newMapName);
                currentCloud.globalMetaData.colormapName = newMapName;
            }
        }

        public void ColorOverride(List<int> selectedPoints, bool setting)
        {
            CloudData currcloud = LoadCurrentStatus();
            foreach (int i in selectedPoints) 
            {
                currcloud.pointMetaDataTable[i].trueColorOverride = setting;
            }
            ChangeCurrentColorMap(currcloud.globalMetaData.colormapName, currcloud.globalMetaData.colormapReversed);
        }

        public delegate void OnColorMapSaturationChangeEvent(float value1, float value2);
        public event OnColorMapSaturationChangeEvent OnColorMapSaturationChange;

        //New Version
        public override void ChangeColorMapSaturation(float value1, float value2, string id)
        {

            CloudData currentCloud = LoadCurrentStatus();
            ColorMap ColorMap = ColorMapManager.instance.GetColorMap(currentCloud.globalMetaData.colormapName);
            Texture2D texture = ColorMap.texture;
            //Color[] colormapArray = texture.GetPixels();

            float delta = 1.0f;

            //Debug.Log("delta : " + delta);

            Texture2D newtexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            newtexture.wrapMode = TextureWrapMode.Clamp;
            Color[] colormapArray = newtexture.GetPixels();

            float cmin = 0.0f;
            float cmax = 1.0f;

            delta = Mathf.Abs(value1 - value2);
            switch (id)
            {
                case "cmin":
                    cmin = value1;
                    cmax = value2;
                    break;
                case "cmax":
                    cmin = value2;
                    cmax = value1;
                    break;
                default:
                    Debug.Log("colormap saturation error");
                    break;


            }

            float minspace = cmin * 255;
            float maxspace = cmax * 255;
            Color[] tempcolorarray = null;
            if (cmax-cmin > 0)
            {
                Texture2D tempTex = ColorMap.CreateColormap(ColorMap.colorArray, (int)(maxspace - minspace));
                tempcolorarray = tempTex.GetPixels();
            }
            int counter = 0;
            for (int i = 0; i < colormapArray.Length; i++)
            {

                if (i <= (int)(minspace))
                {
                    colormapArray[i] = ColorMap.colorArray[0];
                }
                else if (i >= (int)(maxspace-1))
                {
                    colormapArray[i] = ColorMap.colorArray[ColorMap.colorArray.Length - 1];//Color.Lerp(ColorMap.colorArray[0], ColorMap.colorArray[1], (float)(i - cmin * 255f) / (255f) / delta);
                }
                else
                {
                    if (tempcolorarray != null)
                    {
                        colormapArray[i] = tempcolorarray[counter];
                        counter++;

                    }
                    else
                    {
                        Debug.Log("ERROR : access to undefined texture");
                    }
                }

            }


            newtexture.SetPixels(colormapArray);
            newtexture.Apply();
            ColorMap.texture = newtexture;
            OnColorMapSaturationChange(value1, value2);
            ChangeCurrentColorMap(ColorMap.name);
            ColorMap.cmaxValue = cmax;
            ColorMap.cminValue = cmin;
            /**
            delta = Mathf.Abs(cmax - cmin);
            if(delta == 0)
            {
                delta = 0.00000000001f;
            }
            int spacing = (int)(256/ (ColorMap.colorArray.Length-1));
            int deltaspacing = (int)(delta*256 / (ColorMap.colorArray.Length-1)); //length-1

            for (int i = 0; i < colormapArray.Length; i++)
            {

                if (i <= (int)(cmin * 255f))
                {
                    colormapArray[i] = ColorMap.colorArray[0];
                }
                else if (i >= (int)(cmax * 255f))
                {
                    colormapArray[i] = ColorMap.colorArray[ColorMap.colorArray.Length-1];//Color.Lerp(ColorMap.colorArray[0], ColorMap.colorArray[1], (float)(i - cmin * 255f) / (255f) / delta);
                }
                
            }

            //Recode with one for loop
            for (int i = (int)(cmin * 255f); i < (int)(cmax * 255f); i++)
            {
                //Careful, can divide by zero, fix later

                int j = Mathf.Min((int)(i - cmin*255f) / deltaspacing, ColorMap.colorArray.Length - 2);

                colormapArray[i] = Color.Lerp(ColorMap.colorArray[j], ColorMap.colorArray[j + 1], (float)((i - (cmin * 255f) - (j * deltaspacing)) /deltaspacing / delta)); //Color.Lerp(ColorMap.colorArray[0], ColorMap.colorArray[1], (float)(i - cmin * 255f) / (255f) / delta);

            }

            
            //.globalMetaData.cmaxslidervalue = cmax;
            //currentCloud.globalMetaData.cminslidervalue = cmin;
            **/

        }

        //OLD VERSION, IN CASE OF PROBLEMS

        /**
        public override void ChangeColorMapSaturation(float value1, float value2, string id)
        {

            CloudData currentCloud = LoadCurrentStatus();
            ColorMap ColorMap = ColorMapManager.instance.GetColorMap(currentCloud.globalMetaData.colormapName);
            Texture2D texture = ColorMap.texture;
            //Color[] colormapArray = texture.GetPixels();
            Mesh mesh = currentCloud.gameObject.GetComponent<MeshFilter>().mesh;
            Color[] colors = new Color[mesh.GetIndices(0).Length];

            float delta = 1.0f;

            //Debug.Log("delta : " + delta);

            Texture2D newtexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            newtexture.wrapMode = TextureWrapMode.Clamp;
            Color[] colormapArray = newtexture.GetPixels();

            float cmin = 0.0f;
            float cmax = 1.0f;

            delta = Mathf.Abs(value1 - value2);
            switch (id)
            {
                case "cmin":
                    cmin = value1;
                    cmax = value2;
                    break;
                case "cmax":
                    cmin = value2;
                    cmax = value1;
                    break;
                default:
                    Debug.Log("colormap saturation error");
                    break;
            }

            

            for (int i = 0 ; i < colormapArray.Length; i++)
            {
                if (i < (int)(cmin * 255f))
                {
                    colormapArray[i] = ColorMap.colorArray[0];
                }
                else
                {
                    colormapArray[i] = Color.Lerp(ColorMap.colorArray[0], ColorMap.colorArray[1], (float)(i - cmin * 255f) / (255f) / delta);
                }
            }

            newtexture.SetPixels(colormapArray);
            newtexture.Apply();
            ColorMap.texture = newtexture;
            OnColorMapSaturationChange(value1, value2);
            ChangeCurrentColorMap(ColorMap.name);
            ColorMap.cmaxValue = cmax;
            ColorMap.cminValue = cmin;
            //.globalMetaData.cmaxslidervalue = cmax;
            //currentCloud.globalMetaData.cminslidervalue = cmin;


        }
        **/

        public void UpdateFreezableClippingPlanes(GameObject[] goArray)
        {
            CloudData currcloud = LoadCurrentStatus();
            currcloud.globalMetaData.ClippingPlanesList = goArray;
        }


        public override void ChangePointSize(float value)
        {
            CloudData currentCloud = LoadCurrentStatus();
            Material material = currentCloud.gameObject.GetComponent<MeshRenderer>().material;
            material.SetFloat("_Size", value / 50);
            currentCloud.globalMetaData.point_size = value;

            //Debug.Log("PointSize Changed");
        }

        //Used by the video capture tools to change cloud states
        //without heavy computation, 
        //Should not be used elsewhere
        public void OverrideMesh(Mesh mesh)
        {
            CloudData data = LoadCurrentStatus();
            Debug.Log(data == data.gameObject.GetComponent<MeshFilter>().mesh);
            data.gameObject.GetComponent<MeshFilter>().mesh = mesh;
        }

        #endregion

        #region Point Selection
        public void UpdatePointSelection()
        {
            if(PointSelectionThreadList.Count < maxSelectionThreads)
            {
                if (CloudStorage.instance.CheckID(CloudSelector.instance._selectedID))
                {



                    List<HashSet<int>> Lists = new List<HashSet<int>>();
                    CloudData currcloud = LoadCurrentStatus();

                    Debug.Log(currcloud.globalMetaData.lowerframeLimit);
                    Debug.Log(currcloud.globalMetaData.upperframeLimit);
                    foreach (var kvp in currcloud.globalMetaData.sphereList)
                    {
                        if (kvp.Value.activeInHierarchy)
                        {
                            Lists.Add(kvp.Value.transform.GetChild(0).GetComponent<PointSelectorSphere>().selectedPoints);

                        }
                    }
                    foreach (var kvp in currcloud.globalMetaData.convexHullsList)
                    {
                        if (kvp.Value.activeInHierarchy)
                        {
                            Lists.Add(kvp.Value.GetComponent<PointSelectorConvexHull>().selectedPoints);
                        }
                    }

                    if (currcloud.globalMetaData.FreeSelectionON)
                    {
                        Lists.Add(currcloud.globalMetaData.FreeSelectionIDList);

                    }

                    if (currcloud.trajectoryObject)
                    {
                        selectionThreadHandle = new SelectPointsThreadHandler(currcloud, Lists, currcloud.GetComponent<MeshFilter>().mesh, currcloud.trajectoryObject.GetComponent<MeshFilter>().mesh);
                    }
                    else
                    {
                        selectionThreadHandle = new SelectPointsThreadHandler(currcloud, Lists, currcloud.GetComponent<MeshFilter>().mesh);

                    }
                    selectionThreadHandle.StartThread();
                    PointSelectionThreadList.Enqueue(selectionThreadHandle);
                    selectionJobON = true;

                }

            }
        }


        public void ResetPointSelection()
        {
            CloudData currcloud = LoadCurrentStatus();
            currcloud.globalMetaData.SelectedPointsList.Clear();
            
            foreach (var go in currcloud.globalMetaData.sphereList)
            {
                GameObject g = go.Value;
                Destroy(g);
            }
            currcloud.globalMetaData.sphereList.Clear();
            foreach (var go in currcloud.globalMetaData.convexHullsList)
            {
                GameObject g = go.Value;
                Destroy(g);
            }
            currcloud.globalMetaData.convexHullsList.Clear();
            currcloud.globalMetaData.FreeSelectionIDList.Clear();
            UpdatePointSelection();
            //TODO : Iterate over convex hull and sphere lists and delete them.
        }

        public void ResetFreeSelection()
        {
            //TODO : Tidy up and make a global function
            CloudData currcloud = LoadCurrentStatus();
            currcloud.globalMetaData.FreeSelectionIDList.Clear();
            UpdatePointSelection();

        }

        public void SwitchFreeSelectionActivation()
        {
            CloudData currcloud = LoadCurrentStatus();
            currcloud.globalMetaData.FreeSelectionON = !currcloud.globalMetaData.FreeSelectionON;
            UpdatePointSelection();
        }
        public void ChangeFreeSelectionActivation(bool status)
        {
            CloudData currcloud = LoadCurrentStatus();
            currcloud.globalMetaData.FreeSelectionON = status;
            UpdatePointSelection();
        }

        public void CreateCloudFromSelection()
        {
            //PROTOTYPE
            if (!CloudSelector.instance.noSelection)
            {
                CloudData data = CloudUpdater.instance.LoadCurrentStatus();
                if (data.globalMetaData.SelectedPointsList.Count > 0)
                {
                    List<float[]> RawDataList = new List<float[]>();

                    for (int i = 0; i < data.columnData.Count; i++)
                    {
                        float[] dataArray = new float[data.globalMetaData.SelectedPointsList.Count];
                        RawDataList.Add(dataArray);
                    }

                    int k = 0;
                    foreach (int id in data.globalMetaData.SelectedPointsList)
                    {
                        for (int j = 0; j < data.columnData.Count; j++)
                        {
                            RawDataList[j][k] = data.columnData[j][id];
                        }
                        k++;
                    }

                    ResetPointSelection();
                    CloudLoader.instance.LoadFromRawData(RawDataList);

                }
            }

        }

        #endregion

        #region Link Clouds


        public delegate void OnCloudLinkedEvent();
        public event OnCloudLinkedEvent OnCloudLinked;

        public override void LinkClouds(List<int> cloudIDList)
        {
            float[] xminArray = new float[cloudIDList.Count];
            float[] yminArray = new float[cloudIDList.Count];
            float[] zminArray = new float[cloudIDList.Count];
            float[] xmaxArray = new float[cloudIDList.Count];
            float[] ymaxArray = new float[cloudIDList.Count];
            float[] zmaxArray = new float[cloudIDList.Count];
            int i = 0;



            foreach (int id in cloudIDList)
            {
                CloudData currcloud = LoadStatus(id);
                xminArray[i] = currcloud.globalMetaData.xMin;
                yminArray[i] = currcloud.globalMetaData.yMin;
                zminArray[i] = currcloud.globalMetaData.zMin;
                xmaxArray[i] = currcloud.globalMetaData.xMax;
                ymaxArray[i] = currcloud.globalMetaData.yMax;
                zmaxArray[i] = currcloud.globalMetaData.zMax;

                i++;
            }

            float max_xmax = Mathf.Max(xmaxArray);
            float max_ymax = Mathf.Max(ymaxArray);
            float max_zmax = Mathf.Max(zmaxArray);
            float min_xmin = Mathf.Min(xminArray);
            float min_ymin = Mathf.Min(yminArray);
            float min_zmin = Mathf.Min(zminArray);

            Vector3 new_offset = new Vector3((min_xmin + max_xmax) / 2,
                                             (min_ymin + max_ymax) / 2,
                                             (min_zmin + max_zmax) / 2);

            float maxRange = Mathf.Max(max_xmax - min_xmin, max_ymax - min_ymin, max_zmax - min_zmin);

            foreach (int id in cloudIDList)
            {

                CloudData currcloud = LoadStatus(id);


                currcloud.globalMetaData.maxLinkedRange = maxRange;


                float normedxMax = -10000000.0f;
                float normedxMin = 10000000.0f;
                float normedyMax = -10000000.0f;
                float normedyMin = 10000000.0f;
                float normedzMax = -10000000.0f;
                float normedzMin = 10000000.0f;


                foreach (KeyValuePair<int, PointData> item in currcloud.pointDataTable)
                {

                    //item.Value.position -= new_offset;
                    item.Value.normed_position = ((item.Value.position - new_offset) / currcloud.globalMetaData.maxLinkedRange);

                    /**
                    item.Value.normed_position = new Vector3(((item.Value.position.x - min_xmin) / currcloud.globalMetaData.maxRange),
                                                            ((item.Value.position.y - min_ymin) / currcloud.globalMetaData.maxRange),
                                                            ((item.Value.position.z - min_zmin) / currcloud.globalMetaData.maxRange));
        **/
                    //item.Value.normed_position -= 0.5f * Vector3.one;
                    if (normedxMax < item.Value.normed_position.x) { normedxMax = item.Value.normed_position.x; }
                    if (normedxMin > item.Value.normed_position.x) { normedxMin = item.Value.normed_position.x; }
                    if (normedyMax < item.Value.normed_position.y) { normedyMax = item.Value.normed_position.y; }
                    if (normedyMin > item.Value.normed_position.y) { normedyMin = item.Value.normed_position.y; }
                    if (normedzMax < item.Value.normed_position.z) { normedzMax = item.Value.normed_position.z; }
                    if (normedzMin > item.Value.normed_position.z) { normedzMin = item.Value.normed_position.z; }


                }
                //currcloud.gameObject.GetComponent<MeshFilter>().mesh.vertices = newpositionList;



            }



            float[] normedxminList = new float[cloudIDList.Count];
            float[] normedyminList = new float[cloudIDList.Count];
            float[] normedzminList = new float[cloudIDList.Count];
            float[] normedxmaxList = new float[cloudIDList.Count];
            float[] normedymaxList = new float[cloudIDList.Count];
            float[] normedzmaxList = new float[cloudIDList.Count];
            i = 0;



            foreach (int id in cloudIDList)
            {
                CloudData currcloud = LoadStatus(id);

                normedxminList[i] = currcloud.globalMetaData.normed_xMin;
                normedyminList[i] = currcloud.globalMetaData.normed_yMin;
                normedzminList[i] = currcloud.globalMetaData.normed_zMin;
                normedxmaxList[i] = currcloud.globalMetaData.normed_xMax;
                normedymaxList[i] = currcloud.globalMetaData.normed_yMax;
                normedzmaxList[i] = currcloud.globalMetaData.normed_zMax;
                i++;

            }

            float normedmax_xmax = Mathf.Max(normedxmaxList);
            float normedmax_ymax = Mathf.Max(normedymaxList);
            float normedmax_zmax = Mathf.Max(normedzmaxList);
            float normedmin_xmin = Mathf.Min(normedxminList);
            float normedmin_ymin = Mathf.Min(normedyminList);
            float normedmin_zmin = Mathf.Min(normedzminList);



            Vector3 normednew_offset = new Vector3(normedmin_xmin + normedmax_xmax / 2,
                                             normedmin_ymin + normedmax_ymax / 2,
                                             normedmin_zmin + normedmax_zmax / 2);

            float normedxrange = normedmax_xmax - normedmin_xmin;
            float normedyrange = normedmax_ymax - normedmin_ymin;
            float normedzrange = normedmax_zmax - normedmin_zmin;

            float maxnormedRange = Mathf.Max(normedxrange, normedyrange, normedzrange);

            foreach (int id in cloudIDList)
            {

                CloudData currcloud = LoadStatus(id);
                Vector3[] newpositionList = new Vector3[currcloud.pointDataTable.Count];

                foreach (KeyValuePair<int, PointData> item in currcloud.pointDataTable)
                {
                    //item.Value.normed_position += normednew_offset;
                    newpositionList[item.Key] = item.Value.normed_position;



                }
                currcloud.gameObject.GetComponent<MeshFilter>().mesh.vertices = newpositionList;
                if (id == cloudIDList[0])
                {
                    Vector3 newScale = new Vector3(normedxrange, normedyrange, normedzrange) / maxnormedRange;
                    currcloud.transform.parent.Find("Box").localScale = Vector3.Scale(newScale, currcloud.globalMetaData.scale);
                }
                else
                {

                    currcloud.transform.parent.SetParent(LoadStatus(cloudIDList[0]).transform.parent);
                    currcloud.GetComponent<VRTK_RigidbodyFollow>().gameObjectToFollow = LoadStatus(cloudIDList[0]).gameObject;
                    currcloud.transform.parent.Find("Box").gameObject.SetActive(false);
                }

            }
            DisplayLinkedClouds(cloudIDList);
            OnCloudLinked();
        }

        public delegate void OnCloudUnlinkedEvent();
        public OnCloudUnlinkedEvent OnCloudUnlinked;

        public override void UnlinkClouds(List<int> cloudIDList)
        {
            foreach (int id in cloudIDList)
            {
                CloudData currcloud = LoadStatus(id);
                Vector3[] newpositionList = new Vector3[currcloud.pointDataTable.Count];

                foreach (KeyValuePair<int, PointData> item in currcloud.pointDataTable)
                {
                    item.Value.normed_position = (item.Value.position - currcloud.globalMetaData.offsetVector) / currcloud.globalMetaData.maxRange;
                    newpositionList[item.Key] = item.Value.normed_position;
                }
                currcloud.transform.parent.parent = null;
                currcloud.gameObject.GetComponent<MeshFilter>().mesh.vertices = newpositionList;

                GameObject box = currcloud.transform.parent.Find("Box").gameObject;
                if (id == cloudIDList[0])
                {
                    box.transform.localScale = Vector3.Scale(currcloud.globalMetaData.box_scale, currcloud.globalMetaData.scale);
                }
                box.SetActive(true);
                currcloud.GetComponent<VRTK_RigidbodyFollow>().gameObjectToFollow = box;


            }
            CloudSelector.instance.UpdateSelection(CloudSelector.instance._selectedID);
            OnCloudUnlinked();
        }
        #endregion

        #region Column Selection and reload

        public delegate void OnCloudReloadedEvent(int id);
        public event OnCloudReloadedEvent OnCloudReloaded;

        public void ChangeCollumnSelection(List<int> collumnList)
        {
            CloudData currcloud = LoadCurrentStatus();

            float xMax = currcloud.globalMetaData.columnMetaDataList[collumnList[0]].MaxThreshold;
            float xMin = currcloud.globalMetaData.columnMetaDataList[collumnList[0]].MinThreshold;
            float yMax = currcloud.globalMetaData.columnMetaDataList[collumnList[1]].MaxThreshold;
            float yMin = currcloud.globalMetaData.columnMetaDataList[collumnList[1]].MinThreshold;
            float zMax = currcloud.globalMetaData.columnMetaDataList[collumnList[2]].MaxThreshold;
            float zMin = currcloud.globalMetaData.columnMetaDataList[collumnList[2]].MinThreshold;
            float iMax = currcloud.globalMetaData.columnMetaDataList[collumnList[3]].MaxThreshold;
            float iMin = currcloud.globalMetaData.columnMetaDataList[collumnList[3]].MinThreshold;
            float tMax = currcloud.globalMetaData.columnMetaDataList[collumnList[4]].MaxThreshold;
            float tMin = currcloud.globalMetaData.columnMetaDataList[collumnList[4]].MinThreshold;
            float sMax = currcloud.globalMetaData.columnMetaDataList[collumnList[7]].MaxThreshold;
            float sMin = currcloud.globalMetaData.columnMetaDataList[collumnList[7]].MinThreshold;



            GameObject trajobj = currcloud.trajectoryObject;
            currcloud.trajectoryObject = null;
            currcloud.pointTrajectoriesTable.Clear();
            Destroy(trajobj);

            GameObject oriobj = currcloud.orientationObject;
            currcloud.orientationObject = null;
            Destroy(oriobj);


            List<float> TimeList = new List<float>();
            HashSet<float> TimeHash = new HashSet<float>();
            foreach (int key in currcloud.pointDataTable.Keys)
            {
                currcloud.pointDataTable[key].position = new Vector3(currcloud.columnData[collumnList[0]][key],
                                                                     currcloud.columnData[collumnList[1]][key],
                                                                     currcloud.columnData[collumnList[2]][key]);

                currcloud.pointDataTable[key].time = currcloud.columnData[collumnList[4]][key];
                currcloud.pointDataTable[key].phi_angle = currcloud.columnData[collumnList[6]][key];
                currcloud.pointDataTable[key].theta_angle = currcloud.columnData[collumnList[7]][key];

                currcloud.pointDataTable[key].trajectory = currcloud.columnData[collumnList[5]][key];
                currcloud.pointDataTable[key].intensity = currcloud.columnData[collumnList[3]][key];

                /**
                if (xMax < currcloud.pointDataTable[key].position.x) { xMax = currcloud.pointDataTable[key].position.x; }
                if (xMin > currcloud.pointDataTable[key].position.x) { xMin = currcloud.pointDataTable[key].position.x; }
                if (yMax < currcloud.pointDataTable[key].position.y) { yMax = currcloud.pointDataTable[key].position.y; }
                if (yMin > currcloud.pointDataTable[key].position.y) { yMin = currcloud.pointDataTable[key].position.y; }
                if (zMax < currcloud.pointDataTable[key].position.z) { zMax = currcloud.pointDataTable[key].position.z; }
                if (zMin > currcloud.pointDataTable[key].position.z) { zMin = currcloud.pointDataTable[key].position.z; }
                if (iMax < currcloud.pointDataTable[key].intensity) { iMax = currcloud.pointDataTable[key].intensity; }
                if (iMin > currcloud.pointDataTable[key].intensity) { iMin = currcloud.pointDataTable[key].intensity; }
                if (tMax < currcloud.pointDataTable[key].time) { tMax = currcloud.pointDataTable[key].time; }
                if (tMin > currcloud.pointDataTable[key].time) { tMin = currcloud.pointDataTable[key].time; }
            **/
                TimeHash.Add(currcloud.pointDataTable[key].time);

            }
            foreach(float f in TimeHash)
            {
                TimeList.Add(f);
            }
            TimeList.Sort();

            Dictionary<float, int> FrameDict = new Dictionary<float, int>();
            for (int u = 0; u < TimeList.Count; u++)
            {
                FrameDict.Add(TimeList[u], u);
            }

            float xRange = xMax - xMin;
            float yRange = yMax - yMin;
            float zRange = zMax - zMin;
            float iRange = iMax - iMin;
            float tRange = tMax - tMin;
            float MaxRange = Mathf.Max(xRange, yRange, zRange);

            Vector3 offsetVector = new Vector3((xMin + xMax) / 2,
                                       (yMin + yMax) / 2,
                                       (zMin + zMax) / 2);

            float normedxMax = Mathf.NegativeInfinity;
            float normedxMin = Mathf.Infinity;
            float normedyMax = Mathf.NegativeInfinity;
            float normedyMin = Mathf.Infinity;
            float normedzMax = Mathf.NegativeInfinity;
            float normedzMin = Mathf.Infinity;

            int[] indices = new int[currcloud.pointDataTable.Count];
            Vector3[] verts = new Vector3[currcloud.pointDataTable.Count];
            Vector2[] uv = new Vector2[currcloud.pointDataTable.Count];
            Vector2[] coloruv = new Vector2[currcloud.pointDataTable.Count];
            Vector2[] hiddenselecteduv = new Vector2[currcloud.pointDataTable.Count];
            Vector2[] trajectoryuv = new Vector2[currcloud.pointDataTable.Count]; 
            foreach (int key in currcloud.pointDataTable.Keys)
            {

                Vector3 normedposition = (currcloud.pointDataTable[key].position - offsetVector) / MaxRange;
                currcloud.pointDataTable[key].normed_position = normedposition;
                currcloud.pointDataTable[key]._color_index = (currcloud.pointDataTable[key].intensity - iMin) / (iMax - iMin);
                currcloud.pointDataTable[key].frame = FrameDict[currcloud.pointDataTable[key].time];
                
                currcloud.pointDataTable[key].size = (currcloud.columnData[collumnList[8]][key] - sMin) / (sMax - sMin);

                indices[key] = key;
                verts[key] = normedposition;
                float hidden;
                float selected = 0f;
                if (currcloud.pointMetaDataTable[key].isHidden)
                {
                    hidden = 1f;
                }
                else
                {
                    hidden = 0f;
                }
                if (currcloud.pointMetaDataTable[key].isSelected)
                {
                    selected = 1f;
                }

                uv[key] = new Vector2(currcloud.pointDataTable[key].size, 0f);
                coloruv[key] = new Vector2(currcloud.pointDataTable[key]._color_index,key);
                hiddenselecteduv[key] = new Vector2(selected,hidden);
                trajectoryuv[key] = new Vector2(currcloud.pointDataTable[key].trajectory, currcloud.pointDataTable[key].frame);

                //currcloud.pointDataTable[key].color = new Color(0, (currcloud.pointDataTable[key].normed_position.z - zMin) / (2 * zRange), (currcloud.pointDataTable[key].normed_position.z - zMin) / (2 * zRange), 1);

                if (normedxMax < normedposition.x) { normedxMax = normedposition.x; }
                if (normedxMin > normedposition.x) { normedxMin = normedposition.x; }
                if (normedyMax < normedposition.y) { normedyMax = normedposition.y; }
                if (normedyMin > normedposition.y) { normedyMin = normedposition.y; }
                if (normedzMax < normedposition.z) { normedzMax = normedposition.z; }
                if (normedzMin > normedposition.z) { normedzMin = normedposition.z; }

            }
            currcloud.globalMetaData.normed_x = new Vector2(xMin, xMax);
            currcloud.globalMetaData.normed_y = new Vector2(yMin, yMax);
            currcloud.globalMetaData.normed_z = new Vector2(zMin, zMax);
            currcloud.globalMetaData.normed_i = new Vector2(iMin, iMax);
            currcloud.globalMetaData.normed_t = new Vector2(tMin, tMax);

            Dictionary<int, float> intensityList = new Dictionary<int, float>();
            foreach (KeyValuePair<int, PointData> item in currcloud.pointDataTable)
            {
                intensityList.Add(item.Key, item.Value.intensity);
            }
            currcloud.globalMetaData.current_colormap_variable = intensityList;
            currcloud.globalMetaData.current_normed_variable = currcloud.globalMetaData.normed_i;


            currcloud.globalMetaData.xMax = xMax;
            currcloud.globalMetaData.yMax = yMax;
            currcloud.globalMetaData.zMax = zMax;
            currcloud.globalMetaData.tMax = tMax;
            currcloud.globalMetaData.xMin = xMin;
            currcloud.globalMetaData.yMin = yMin;
            currcloud.globalMetaData.zMin = zMin;
            currcloud.globalMetaData.tMin = tMin;



            currcloud.globalMetaData.maxRange = MaxRange;
            currcloud.globalMetaData.offsetVector = offsetVector;
            currcloud.globalMetaData.normed_xMax = normedxMax;
            currcloud.globalMetaData.normed_yMax = normedyMax;
            currcloud.globalMetaData.normed_zMax = normedzMax;
            currcloud.globalMetaData.normed_xMin = normedxMin;
            currcloud.globalMetaData.normed_yMin = normedyMin;
            currcloud.globalMetaData.normed_zMin = normedzMin;
            currcloud.globalMetaData.timeList = TimeList;
            currcloud.globalMetaData.lowerframeLimit = 0f;
            currcloud.globalMetaData.upperframeLimit = TimeList.Count - 1;

            currcloud.globalMetaData.box_scale = new Vector3(xRange, yRange, zRange) / MaxRange;

            currcloud.globalMetaData.lowertimeLimit = currcloud.globalMetaData.timeList[(int)currcloud.globalMetaData.lowerframeLimit];
            currcloud.globalMetaData.uppertimeLimit = currcloud.globalMetaData.timeList[(int)currcloud.globalMetaData.upperframeLimit];

            Transform box = currcloud.transform.parent.GetChild(1);
            box.localScale = currcloud.globalMetaData.box_scale;

            Mesh mesh = currcloud.gameObject.GetComponent<MeshFilter>().mesh;
            mesh.vertices = verts;
            mesh.SetIndices(indices, MeshTopology.Points, 0);
            mesh.uv = uv;
            mesh.uv2 = coloruv;     // colorID, pointID
            mesh.uv3 = hiddenselecteduv;    // isSelected, isHidden
            //mesh.uv4 = trajectoryuv;
            currcloud.gameObject.GetComponent<MeshFilter>().mesh = mesh;

            ChangeCurrentColorMap(currcloud.globalMetaData.colormapName,currcloud.globalMetaData.colormapReversed);
            currcloud.globalMetaData.displayCollumnsConfiguration = collumnList.ToArray();
            ChangeThreshold();
            ReloadBox(currcloud.globalMetaData.cloud_id);
            if(OnCloudReloaded != null)
            {
                OnCloudReloaded(currcloud.globalMetaData.cloud_id);
            }
        }
        #region Thresholding
        public void ChangeThreshold()
        {
            if(ThresholdThreadQueue.Count < maxThresholdThreads)
            {
                CloudData currcloud = LoadCurrentStatus();
                ThresholdThreadHandler newThread = new ThresholdThreadHandler();
                newThread.currcloud = currcloud;
                newThread.uv2List = currcloud.GetComponent<MeshFilter>().mesh.uv3;
                newThread.StartThread();
                ThresholdThreadQueue.Enqueue(newThread);
                if(thresholdJobON == false)
                {
                    thresholdJobON = true;
                }
            }

        }
        #endregion


        #endregion

        #region Trajectory Visualization

        public void DisplayTrajectories()
        {
            CloudData data = LoadCurrentStatus();
            if (!data.trajectoryObject)
            {


                CreateTrajectoryData(data);
                CreateTrajectoryObject(data);
            }
            else
            {
                MeshRenderer TrajectoriesRenderer = data.trajectoryObject.GetComponent<MeshRenderer>();
                TrajectoriesRenderer.enabled = true;
            }
        }

        public void HideTrajectories()
        {
            CloudData data = LoadCurrentStatus();
            if (data.trajectoryObject)
            {


                MeshRenderer TrajectoriesRenderer = data.trajectoryObject.GetComponent<MeshRenderer>();
                MeshRenderer PointsRenderer = data.GetComponent<MeshRenderer>();

                TrajectoriesRenderer.enabled = false;
                PointsRenderer.enabled = true;
                PointsRenderer.material.SetFloat("_UpperTimeLimit", data.globalMetaData.timeList.Count - 1);
                PointsRenderer.material.SetFloat("_LowerTimeLimit", 0);
                if (data.orientationObject)
                {
                    data.orientationObject.GetComponent<MeshRenderer>().material.SetFloat("_UpperTimeLimit", data.globalMetaData.timeList.Count - 1);
                    data.orientationObject.GetComponent<MeshRenderer>().material.SetFloat("_LowerTimeLimit", 0);

                }

                data.globalMetaData.lowerframeLimit = 0;
                data.globalMetaData.upperframeLimit = data.globalMetaData.timeList.Count - 1;
            }
        }

        private void CreateTrajectoryData(CloudData data)
        {
            data.pointTrajectoriesTable.Clear();

            List<float> TimeList = new List<float>();

            foreach (var kvp in data.pointDataTable)
            {
                if (!data.pointTrajectoriesTable.ContainsKey(kvp.Value.trajectory))
                {

                    TrajectoryData trajectory = new TrajectoryData(kvp.Value.trajectory);
                    data.pointTrajectoriesTable.Add(kvp.Value.trajectory, trajectory);

                }
                data.pointTrajectoriesTable[kvp.Value.trajectory].pointsIDList.Add(kvp.Value.pointID);
                data.pointTrajectoriesTable[kvp.Value.trajectory].pointsTimeList.Add(kvp.Value.time);
                data.pointTrajectoriesTable[kvp.Value.trajectory].pointsNormedPositionList.Add(kvp.Value.normed_position);

                /**
                bool check = false;
                foreach (float time in TimeList)
                {
                    if (kvp.Value.time == time)
                    {
                        check = true;
                        break;
                    }
                }
                if (check == false)
                {
                    TimeList.Add(kvp.Value.time);
                }
            **/
            }

            //TimeList.Sort();
            //data.globalMetaData.timeList = TimeList;
        }

        private void CreateTrajectoryObject(CloudData data)
        {
            List<float> trajectoryIDList = new List<float>();
            foreach(float id in data.pointTrajectoriesTable.Keys)
            {
                trajectoryIDList.Add(id);
            }

            List<Vector3> vertices = new List<Vector3>();
            List<int> Indices = new List<int>();
            List<Color> colors = new List<Color>();

            List<Vector2> TrajectoryUV1List = new List<Vector2>();
            List<Vector2> TrajectoryUV2List = new List<Vector2>();
            List<Vector2> TrajectoryUV3List = new List<Vector2>();
            List<Vector2> PointCloudUV3List = new List<Vector2>();

            float maxtrajectoryID = Mathf.Max(trajectoryIDList.ToArray());

            int i = 0;
            foreach (KeyValuePair<float, TrajectoryData> item in data.pointTrajectoriesTable)
            {
                //Color color = ColorGenerator.instance.GenerateRandomColor();
                vertices.Add(item.Value.pointsNormedPositionList[0]);



                PointCloudUV3List.Add(new Vector2(item.Value.trajectoryID, data.pointDataTable[item.Value.pointsIDList[0]].frame));
                TrajectoryUV3List.Add(new Vector2(item.Value.trajectoryID, data.pointDataTable[item.Value.pointsIDList[0]].frame));

                Indices.Add(i);
                colors.Add(new Color(item.Value.trajectoryID / maxtrajectoryID, 1f, 1f));
                TrajectoryUV1List.Add(new Vector2(item.Value.trajectoryID / maxtrajectoryID, item.Value.pointsIDList[0]));

                float hidden = 0f;
                float selected = 0f;
                if (data.pointMetaDataTable[item.Value.pointsIDList[0]].isHidden)
                {
                    hidden = 1f;
                }
                if (data.pointMetaDataTable[item.Value.pointsIDList[0]].isSelected)
                {
                    selected = 1f;
                }

                TrajectoryUV2List.Add(new Vector2(selected, hidden));

                i++;
                for (int j = 1; j < item.Value.pointsIDList.Count - 1; j++)
                {
                    float f = (float)j;
                    Indices.Add(i);
                    vertices.Add(item.Value.pointsNormedPositionList[j]);

                    TrajectoryUV3List.Add(new Vector2(item.Value.trajectoryID, data.pointDataTable[item.Value.pointsIDList[j]].frame));

                    Indices.Add(i);
                    colors.Add(new Color(item.Value.trajectoryID / maxtrajectoryID, 1f, 1f));
                    TrajectoryUV1List.Add(new Vector2(item.Value.trajectoryID / maxtrajectoryID, item.Value.pointsIDList[j]));

                    PointCloudUV3List.Add(new Vector2(item.Value.trajectoryID, data.pointDataTable[item.Value.pointsIDList[j]].frame));

                    hidden = 0f;
                    selected = 0f;
                    if (data.pointMetaDataTable[item.Value.pointsIDList[j]].isHidden)
                    {
                        hidden = 1f;
                    }
                    if (data.pointMetaDataTable[item.Value.pointsIDList[j]].isSelected)
                    {
                        selected = 1f;
                    }

                    TrajectoryUV2List.Add(new Vector2(selected, hidden));

                    i++;
                }
                vertices.Add(item.Value.pointsNormedPositionList[item.Value.pointsNormedPositionList.Count - 1]);
                TrajectoryUV1List.Add(new Vector2(item.Value.trajectoryID / maxtrajectoryID, item.Value.pointsIDList[item.Value.pointsIDList.Count-1]));

                PointCloudUV3List.Add(new Vector2(item.Value.trajectoryID, data.pointDataTable[item.Value.pointsIDList[item.Value.pointsIDList.Count - 1]].frame));
                TrajectoryUV3List.Add(new Vector2(item.Value.trajectoryID, data.pointDataTable[item.Value.pointsIDList[item.Value.pointsIDList.Count - 1]].frame));

                hidden = 0f;
                selected = 0f;
                if (data.pointMetaDataTable[item.Value.pointsIDList[item.Value.pointsIDList.Count - 1]].isHidden)
                {
                    hidden = 1f;
                }
                if (data.pointMetaDataTable[item.Value.pointsIDList[item.Value.pointsIDList.Count - 1]].isSelected)
                {
                    selected = 1f;
                }

                TrajectoryUV2List.Add(new Vector2(selected, hidden));


                Indices.Add(i);
                colors.Add(new Color(item.Value.trajectoryID / maxtrajectoryID, 1f, 1f));
                i++;
            }

            //Debug.Log(PointCloudUV3List.Count);
            //Debug.Log(TrajectoryUV3List.Count);
            //Debug.Log(vertices.Count);

            Mesh trajectorymesh = new Mesh();
            trajectorymesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            trajectorymesh.vertices = vertices.ToArray();
            trajectorymesh.SetIndices(Indices.ToArray(), MeshTopology.Lines, 0);
            trajectorymesh.colors = colors.ToArray();
            trajectorymesh.uv2 = TrajectoryUV1List.ToArray();
            trajectorymesh.uv3 = TrajectoryUV2List.ToArray(); 
            trajectorymesh.uv4 = TrajectoryUV3List.ToArray(); 
            //data.gameObject.GetComponent<MeshFilter>().mesh = trajectorymesh;

            Material trajectorymaterial = new Material(Shader.Find("Genuage/UnlitLineShader"));
            trajectorymaterial.SetTexture("_ColorTex", ColorMapManager.instance.GetColorMap("jet").texture);

            GameObject trajectorychild = new GameObject();
            trajectorychild.transform.SetParent(data.transform, false);
            trajectorychild.AddComponent<MeshFilter>();
            trajectorychild.AddComponent<MeshRenderer>();
            trajectorychild.GetComponent<MeshRenderer>().material = trajectorymaterial;
            trajectorychild.GetComponent<MeshFilter>().mesh = trajectorymesh;
            //trajectorychild.GetComponent<MeshRenderer>().material.SetTexture("_ColorTex", texture);
            data.trajectoryObject = trajectorychild;

            MeshRenderer TrajectoriesRenderer = trajectorychild.GetComponent<MeshRenderer>();


            Mesh pointcloudMesh = data.GetComponent<MeshFilter>().mesh;
            pointcloudMesh.uv4 = PointCloudUV3List.ToArray();


            SetShaderFrame(0f, 1f);
            /**
            currentTimeIndex = 0;
            frameSlider.minValue = 0f;
            frameSlider.maxValue = TimeList.Count - 1;
            **/
        }

        public void SetShaderFrame(float minframe, float maxframe)
        {
            CloudData data = LoadCurrentStatus();

            if (data.trajectoryObject)
            {
                MeshRenderer TrajectoriesRenderer = data.trajectoryObject.GetComponent<MeshRenderer>();
                MeshRenderer PointsRenderer = data.GetComponent<MeshRenderer>();

                TrajectoriesRenderer.material.SetFloat("_UpperTimeLimit", maxframe);
                TrajectoriesRenderer.material.SetFloat("_LowerTimeLimit", minframe);

                PointsRenderer.material.SetFloat("_UpperTimeLimit", maxframe);
                PointsRenderer.material.SetFloat("_LowerTimeLimit", minframe);

                if (data.orientationObject)
                {
                    data.orientationObject.GetComponent<MeshRenderer>().material.SetFloat("_UpperTimeLimit", maxframe);
                    data.orientationObject.GetComponent<MeshRenderer>().material.SetFloat("_LowerTimeLimit", minframe);

                }
                //Debug.Log((int)maxframe);
                if (minframe < 0)
                {
                    minframe = 0;
                }

                data.globalMetaData.lowerframeLimit = minframe;
                data.globalMetaData.upperframeLimit = maxframe;

                data.globalMetaData.lowertimeLimit = data.globalMetaData.timeList[(int)minframe];
                data.globalMetaData.uppertimeLimit = data.globalMetaData.timeList[(int)maxframe];
                
            }
        }
        #endregion

        #region Orientation

        public void ChangeAngleUnit(AngleUnit new_unit)
        {
            if (!CloudSelector.instance.noSelection)
            {
                CloudData data = LoadCurrentStatus();
                data.globalMetaData.angleUnit = new_unit;

            }
        }

        public void CalculateMeanOrientation()
        {
            CloudData currcloud = LoadCurrentStatus();
            float phisum = 0f;
            float thetasum = 0f;
            float xsum = 0;
            float ysum = 0;
            float zsum = 0;
            int pointnumber = 0;
            /**
            foreach (var i in currcloud.pointDataTable)
            {
                phisum += currcloud.pointDataTable[i.Key].phi_angle;
                thetasum += currcloud.pointDataTable[i.Key].theta_angle;
                xsum += currcloud.pointDataTable[i.Key].normed_position.x;
                ysum += currcloud.pointDataTable[i.Key].normed_position.y;
                zsum += currcloud.pointDataTable[i.Key].normed_position.z;
                pointnumber++;

            }
            **/
            
            foreach (int i in currcloud.globalMetaData.SelectedPointsList)
            {
                phisum += currcloud.pointDataTable[i].phi_angle;
                thetasum += currcloud.pointDataTable[i].theta_angle;
                xsum += currcloud.pointDataTable[i].normed_position.x;
                ysum += currcloud.pointDataTable[i].normed_position.y;
                zsum += currcloud.pointDataTable[i].normed_position.z;
                pointnumber++;
            }
            
            float xaverage = xsum / pointnumber;
            float yaverage = ysum / pointnumber;
            float zaverage = zsum / pointnumber;

            thetasum = thetasum / pointnumber;
            phisum = phisum / pointnumber;
            string str = "Mean Orientation value :\n" +
                "Theta : " + Math.Round(thetasum,3).ToString() + "\n" +
                "Phi : " + Math.Round(phisum, 3).ToString();
            GameObject container = new GameObject("CylinderContainer");

            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.transform.SetParent(container.transform,true);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(0.01f, 0.05f, 0.01f);
            obj.transform.Rotate(new Vector3(90, 0, 0), Space.World);
            container.transform.SetParent(currcloud.transform, true);
            container.transform.localPosition = new Vector3(xaverage, yaverage, zaverage);
            float xvalue = (Mathf.Sin(Mathf.Deg2Rad * thetasum) * Mathf.Cos(Mathf.Deg2Rad * phisum));
            float yvalue = (Mathf.Sin(Mathf.Deg2Rad * thetasum) * Mathf.Sin(Mathf.Deg2Rad * phisum));
            float zvalue = (Mathf.Cos(Mathf.Deg2Rad * thetasum));
            //Debug.Log(xvalue + " / " + yvalue + " / " + zvalue + " / ");
            Vector3 anglevector = currcloud.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(xaverage + xvalue, yaverage + yvalue, zaverage + zvalue));
            container.transform.LookAt(anglevector);
            
            //TODO : Inplement saving function into metadata
            ModalWindowManager.instance.CreateModalWindow(str);
        }
        #endregion

        #region Calculate Density



        public override void CalculateDensity(float radius)
        {
            CloudData currentCloud = LoadCurrentStatus();
            Debug.Log("Thread started on cloud " + currentCloud.globalMetaData.cloud_id);
            densityThreadHandle = new CalculateDensityThreadHandler(radius, currentCloud);
            densityThreadHandle.StartThread();
            densityJobON = true;
            idThreaded = currentCloud.globalMetaData.cloud_id;
        }


        #endregion

        #region Clipping Plane
        public void SavePointsFromMultiClippingPlane()
        {
            //TODO Implement UI and Test / Implement thread
            CloudData currcloud = LoadCurrentStatus();
            if (currcloud.globalMetaData.ClippingPlanesList.Length != 0)
            {
                List<int> pointIDList = new List<int>();
                foreach (KeyValuePair<int, PointData> item in currcloud.pointDataTable)
                {
                    bool selected = true;

                    if (currcloud.pointMetaDataTable[item.Key].isHidden)
                    { 
                        selected = false;
                    }
                    foreach (GameObject go in currcloud.globalMetaData.ClippingPlanesList)
                    {
                        if (go != null)
                        {
                            float d = Vector3.Dot(go.transform.forward, item.Value.normed_position - currcloud.transform.worldToLocalMatrix.MultiplyPoint3x4(go.transform.position));
                            if (d <= 0)
                            {
                                selected = false;
                            }
                        }
                    }

                    if(selected == true)
                    {
                        pointIDList.Add(item.Key);
                    }
                    
                }
                Debug.Log(pointIDList.Count);
                var extensions = new[] {
                new ExtensionFilter("3D Format", "3d")};
                StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "", extensions, (string path) => { CloudSaver.instance.savePoints(pointIDList,path); });

            }
        }
        #endregion

        #region ZMQSyncCommunicator
        public void CreateZMQSyncCommunicator()
        {
            ZMQSyncCom = gameObject.AddComponent<ZMQSynchronizedCommunicator>();

        }

        public void SwitchZMQSyncCommunicator()
        {
            if(ZMQSyncCom == null)
            {
                CreateZMQSyncCommunicator();
            }

            if (ZMQSyncCom.InferenceModeActive)
            {
                ZMQSyncCom.Deactivate();
            }
            else
            {
                ZMQSyncCom.Activate();
            }
        }

        public void SetZMQSyncComDefaultValue(int value)
        {
            ZMQSyncCom.ChangeDefaultAlphaValue(value);
        }

        public void SwitchZMQSyncComDefaultMode(bool value)
        {
            ZMQSyncCom.SwitchTrajectoryParameter(value);
        }

        #endregion
        public delegate void OnSelectionUpdateEvent();
        public event OnSelectionUpdateEvent OnSelectionUpdate;

        #region Update
        private void LateUpdate()
        {
            #region Density Calculation
            if (densityJobON)
            {
                if (!densityThreadHandle.isRunning)
                {
                    CloudData cloud = LoadStatus(idThreaded);
                    densityJobON = false;
                    ChangeColorMap(idThreaded, cloud.globalMetaData.colormapName);
                    densityThreadHandle.StopThread();
                    UIManager.instance.ChangeStatusText("Thread Status : " + densityThreadHandle.StatusMessage);
                }
                else
                {
                    UIManager.instance.ChangeStatusText("Thread Status : "+densityThreadHandle.StatusMessage);
                }
            }
            #endregion
            #region Point Selection
            if (selectionJobON)
            {
                if(PointSelectionThreadList.Count != 0)
                {
                    SelectPointsThreadHandler thread = PointSelectionThreadList.Peek();
                    if (!thread.isRunning)
                    {
                        thread = PointSelectionThreadList.Dequeue();
                        Debug.Log("thread dequeued");
                        thread.PointCloudMesh.uv3 = thread.PCuv3Array;
                        thread.data.GetComponent<MeshFilter>().mesh = thread.PointCloudMesh;
                        if (thread.data.trajectoryObject)
                        {
                            thread.TrajectoryMesh.uv3 = thread.TRuv3Array;
                            thread.data.trajectoryObject.GetComponent<MeshFilter>().mesh = thread.TrajectoryMesh;
                        }
                        thread.StopThread();
                        Debug.Log(thread.data.globalMetaData.SelectedTrajectories.Count);
                        if(OnSelectionUpdate != null)
                        {
                            OnSelectionUpdate();
                        }
                        //selectionJobON = false;

                    }
                }
                else
                {
                    selectionJobON = false;

                }
                /**
                if (!selectionThreadHandle.isRunning)
                {
                    ChangeColorMap(selectionThreadHandle.data.globalMetaData.cloud_id, selectionThreadHandle.data.globalMetaData.colormapName, selectionThreadHandle.data.globalMetaData.colormapReversed);
                    selectionThreadHandle.StopThread();
                    selectionJobON = false;

                }
                **/
            }
            #endregion
            #region Thresholding
            if (thresholdJobON == true)
            {
                if(ThresholdThreadQueue.Count != 0)
                {
                    ThresholdThreadHandler thread = ThresholdThreadQueue.Peek();
                    if (!thread.isRunning)
                    {
                        thread = ThresholdThreadQueue.Dequeue();
                        Mesh mesh = thread.currcloud.GetComponent<MeshFilter>().mesh;

                        mesh.uv3 = thread.uv2List;

                        thread.currcloud.gameObject.GetComponent<MeshFilter>().mesh = mesh;
                        thread.StopThread();
                    }
                }
                else
                {
                    thresholdJobON = false;
                }
            }
            #endregion
        }
        #endregion
    }

    #region Threshold THread  Class
    public class ThresholdThreadHandler : RunnableThread
    {
        public CloudData currcloud;

        public List<Vector3> vertices;
        public List<int> indices;
        public Vector2[] uv2List;


        public ThresholdThreadHandler()
        {

        }

        protected override void Run()
        {
            //mesh = new Mesh();

            /**
            Debug.Log("xmin thresh"+currcloud.globalMetaData.xMinThreshold);
            Debug.Log("xmax thresh" + currcloud.globalMetaData.xMaxThreshold);
            Debug.Log("ymin thresh" + currcloud.globalMetaData.yMinThreshold);
            Debug.Log("ymax thresh" + currcloud.globalMetaData.yMaxThreshold);
            Debug.Log("zmin thresh" + currcloud.globalMetaData.zMinThreshold);
            Debug.Log("zmax thresh" + currcloud.globalMetaData.zMaxThreshold);
            Debug.Log("tmin thresh" + currcloud.globalMetaData.tMinThreshold);
            Debug.Log("tmax thresh" + currcloud.globalMetaData.tMaxThreshold);
            **/

            foreach (KeyValuePair<int, PointData> kvp in currcloud.pointDataTable)
            {
                //Debug.Log(kvp.Value.time);
                currcloud.pointMetaDataTable[kvp.Key].isHidden = false;
                uv2List[kvp.Key].y = 0f;

                foreach (int i in currcloud.globalMetaData.displayCollumnsConfiguration)
                {
                    if(currcloud.columnData[i][kvp.Key] < currcloud.globalMetaData.columnMetaDataList[i].MinThreshold || currcloud.columnData[i][kvp.Key] > currcloud.globalMetaData.columnMetaDataList[i].MaxThreshold)
                    {
                        currcloud.pointMetaDataTable[kvp.Key].isHidden = true;
                        uv2List[kvp.Key].y = 1f;


                    }
                }
                /**
                if (kvp.Value.position.x > currcloud.globalMetaData.xMinThreshold && kvp.Value.position.y > currcloud.globalMetaData.yMinThreshold && kvp.Value.position.z > currcloud.globalMetaData.zMinThreshold &&
                   kvp.Value.position.x < currcloud.globalMetaData.xMaxThreshold && kvp.Value.position.y < currcloud.globalMetaData.yMaxThreshold && kvp.Value.position.z < currcloud.globalMetaData.zMaxThreshold &&
                   kvp.Value.time > currcloud.globalMetaData.tMinThreshold && kvp.Value.time < currcloud.globalMetaData.tMaxThreshold)
                {
                    currcloud.pointMetaDataTable[kvp.Key].isHidden = false;
                    uv2List[kvp.Key].y = 0f;

                }
                else
                {
                    currcloud.pointMetaDataTable[kvp.Key].isHidden = true;
                    uv2List[kvp.Key].y = 1f;
                }
                **/
            }

            //Debug.Log(counter);
            isRunning = false;
        }
    }
    #endregion

    #region Calculate Density Thread Class

    public class CalculateDensityThreadHandler : RunnableThread
    {
        public float radius;
        public CloudData currentCloud;
        public float[] densityArray;

        public int defaultlocationsNumber = 20; //The number of sections for one dimension in each dimension
        

        public CalculateDensityThreadHandler(float radius, CloudData cloud)
        {

            this.radius = radius;
            this.currentCloud = cloud;
        }

        protected override void Run()
        {
            densityArray = new float[currentCloud.pointDataTable.Count];
            for(int e = 0 ; e<densityArray.Length ; e++)
            {
                densityArray[e] = 0f;
            }
            StatusMessage = "Looking for point locations";
            FindAllLocations(currentCloud, radius);
            Dictionary<Vector3Int, List<int>> locations = currentCloud.globalMetaData.pointbyLocationList;
            int index = 0;
            long distanceindex = 0;
            int totalindex = locations.Count;
            foreach (KeyValuePair<Vector3Int, List<int>> item in locations)
            {
                index++;
                StatusMessage = "Calculating density in points at location " + index+"/"+totalindex;
                foreach (int point in item.Value)
                {
                    
                    float pointneighbors = 0f;

                    //We iterate over the curent cube and all its neighbors
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            for (int z = -1; z < 2; z++)
                            {
                                Vector3Int originalPosition = item.Key;
                                Vector3Int add = new Vector3Int(x, y, z);
                                Vector3Int neighborPosition = originalPosition + add;

                                if (locations.ContainsKey(neighborPosition))
                                {
                                    foreach (int id in locations[neighborPosition])
                                    {
                                        //The distance calculation is very costly and iterated over a lot of points, optimization possible ?

                                        float distance = Vector3.Distance(currentCloud.pointDataTable[id].position, currentCloud.pointDataTable[point].position);
                                        distanceindex++;
                                        if (distance <= radius)
                                        {
                                            pointneighbors++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    currentCloud.globalMetaData.current_colormap_variable[point] = pointneighbors;
                    currentCloud.pointMetaDataTable[point].local_density = pointneighbors;
                    densityArray[point] = pointneighbors;

                    if (currentCloud.globalMetaData.dMax < pointneighbors) { currentCloud.globalMetaData.dMax = pointneighbors; }
                    if (currentCloud.globalMetaData.dMin > pointneighbors) { currentCloud.globalMetaData.dMin = pointneighbors; }



                }
            }
            StatusMessage = "Saving density data";

            if (currentCloud.globalMetaData.densityCalculated == false)
            {
                currentCloud.globalMetaData.densityCalculated = true;
                currentCloud.columnData.Add(densityArray);
                currentCloud.globalMetaData.columnMetaDataList.Add(new ColumnMetadata());
            }
            else
            {
                currentCloud.columnData[currentCloud.columnData.Count-1] = densityArray;

            }
            currentCloud.globalMetaData.columnMetaDataList[currentCloud.globalMetaData.columnMetaDataList.Count - 1].ColumnID = currentCloud.globalMetaData.columnMetaDataList.Count-1;
            currentCloud.globalMetaData.columnMetaDataList[currentCloud.globalMetaData.columnMetaDataList.Count - 1].MaxValue = currentCloud.globalMetaData.dMax;
            currentCloud.globalMetaData.columnMetaDataList[currentCloud.globalMetaData.columnMetaDataList.Count - 1].MinValue = currentCloud.globalMetaData.dMin;
            currentCloud.globalMetaData.columnMetaDataList[currentCloud.globalMetaData.columnMetaDataList.Count - 1].MaxThreshold = currentCloud.globalMetaData.dMax;
            currentCloud.globalMetaData.columnMetaDataList[currentCloud.globalMetaData.columnMetaDataList.Count - 1].MinThreshold = currentCloud.globalMetaData.dMin;

            Debug.Log(distanceindex);
            isRunning = false;
        }










        public void FindAllLocations(CloudData currentCloud, float radius)
        {

            long counter = 0;
            int pointnbr = currentCloud.pointDataTable.Count;
            Dictionary<int, PointMetaData> metadata = currentCloud.pointMetaDataTable;
            foreach (KeyValuePair<int, PointData> item in currentCloud.pointDataTable)
            {
                StatusMessage = "Finding location for point " + counter + " / " + pointnbr;
                counter++;
                Vector3Int location = FindLocation(radius, item.Value.position,
                                new Vector3(currentCloud.globalMetaData.xMin, currentCloud.globalMetaData.yMin, currentCloud.globalMetaData.zMin),
                                new Vector3(currentCloud.globalMetaData.xMax, currentCloud.globalMetaData.yMax, currentCloud.globalMetaData.zMax));
                
                

                if (currentCloud.globalMetaData.pointbyLocationList.ContainsKey(location))
                {
                    currentCloud.globalMetaData.pointbyLocationList[location].Add(item.Key);
                }
                else
                {
                    currentCloud.globalMetaData.pointbyLocationList.Add(location, new List<int>());
                    currentCloud.globalMetaData.pointbyLocationList[location].Add(item.Key);
                }
                metadata[item.Key].cloudzone = location;

                
            }
            Debug.Log("number of sections : " + currentCloud.globalMetaData.pointbyLocationList.Count);



        }

        public Vector3Int FindLocation(float radius, Vector3 point_position, Vector3 min, Vector3 max)
        {
            float xrange = max.x - min.x;
            float yrange = max.y - min.y;
            float zrange = max.z - min.z;
            float maxrange = Mathf.Max(xrange, yrange, zrange);
            float cubesize = 2 * radius;
            float cubenumber = maxrange / cubesize; //cube size is (2R) so we get the number of cubes 
                                                    //by dividing the range of the cloud's max dimension by the size of the cubes

            int xvalue = Mathf.FloorToInt(point_position.x / cubesize);
            int yvalue = Mathf.FloorToInt(point_position.y / cubesize);
            int zvalue = Mathf.FloorToInt(point_position.z / cubesize);

            Vector3Int id = new Vector3Int(xvalue, yvalue, zvalue);
            return id;
        }
    }
    #endregion

    #region Select Points Thread Class
    public class SelectPointsThreadHandler : RunnableThread
    {
        public CloudData data;
        public List<HashSet<int>> selectedPointsList;
        public HashSet<int> FinalList;
        public Mesh PointCloudMesh;
        public Mesh TrajectoryMesh = null;
        public Vector2[] PCuv3Array;
        public Vector2[] TRuv2Array;
        public Vector2[] TRuv3Array;

        public SelectPointsThreadHandler(CloudData cloud, List<HashSet<int>> Lists, Mesh Mesh, Mesh trajectorymesh = null)
        {
            this.data = cloud;
            this.selectedPointsList = Lists;
            this.PointCloudMesh = Mesh;
            PCuv3Array = PointCloudMesh.uv3;
            this.TrajectoryMesh = trajectorymesh;

            if (TrajectoryMesh)
            {
                TRuv3Array = TrajectoryMesh.uv3;
                TRuv2Array = TrajectoryMesh.uv2;
            }
            
        }

        protected override void Run()
        {
            FinalList = new HashSet<int>();

            foreach (HashSet<int> list in selectedPointsList)
            {
                HashSet<int> newList = ConcatenateLists(FinalList, list);
                FinalList = newList;

            }
            Debug.Log("concatenation finished");

            foreach (int k in data.pointDataTable.Keys)
            {
                data.pointMetaDataTable[k].isSelected = false;
                PCuv3Array[k].x = 0f;

                //uv4Array[k] = new Vector2()
            }

            if (data.pointTrajectoriesTable.Count > 0)
            {
                foreach (float id in data.pointTrajectoriesTable.Keys)
                {
                    data.pointTrajectoriesTable[id].metadata.isSelected = false;
                    data.pointTrajectoriesTable[id].metadata.selectedpointsIDList.Clear();

                }
                data.globalMetaData.SelectedTrajectories.Clear();
            }
            int cpt = 0;
            foreach (int i in FinalList)
            {

                if (data.pointMetaDataTable[i].isHidden == false
                    && data.pointDataTable[i].time >= data.globalMetaData.lowertimeLimit
                    && data.pointDataTable[i].time <= data.globalMetaData.uppertimeLimit)
                {
                    data.pointMetaDataTable[i].isSelected = true;
                    PCuv3Array[i].x = 1f;
                    cpt++;
                    if (data.pointTrajectoriesTable.Count > 0)
                    {
                        data.globalMetaData.SelectedTrajectories.Add(data.pointDataTable[i].trajectory);
                        data.pointTrajectoriesTable[data.pointDataTable[i].trajectory].metadata.isSelected = true;
                        data.pointTrajectoriesTable[data.pointDataTable[i].trajectory].metadata.selectedpointsIDList.Add(i);
                    }

                }
            }
            Debug.Log(cpt);
            if (TrajectoryMesh)
            {
                for (int j = 0; j < TRuv3Array.Length; j++)
                {
                    if (FinalList.Contains((int)TRuv2Array[j].y))
                    {
                        TRuv3Array[j].x = 1f;
                    }
                    else
                    {
                        TRuv3Array[j].x = 0f;
                    }
                }
            }

            //mesh.uv4 = uv4Array;
            data.globalMetaData.SelectedPointsList = FinalList;
            Debug.Log("selection finished");
            isRunning = false;

        }
        
        private HashSet<int> ConcatenateLists(HashSet<int> MainList, HashSet<int> ListToAdd)
        {
            HashSet<int> list = new HashSet<int>(MainList);
            list.UnionWith(ListToAdd);
            return list;
        }

    }
    #endregion

}