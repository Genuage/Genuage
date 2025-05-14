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
        private enum OrientationColor
        {
            ORIENTATION_ANGLE,
            WOBBLE_ANGLE
        }


        const float DEGREE_MAX = 180f;
        const float RADIANS_MAX = 6.28319f;
        const float ORIENTATION_SEGMENT_SIZE = 0.0075f;
        private int cone_subdivisions = 15;
        private int circle_subdivisions = 15;

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

        //Point Sampling Thread
        private Queue<RandomThresholdThreadHandler> RandomThresholdThreadQueue;
        public int maxRandomThresholdThreads = 10;
        public bool randomthresholdJobON = false;

        //Sliding Window Thread
        private Queue<SlidingWindowThreadHandler> SlidingWindowThreadQueue;
        public int maxSlidingWindowThreads = 10;
        public bool slidingwindowJobON = false;



        private ZMQSynchronizedCommunicator ZMQSyncCom = null;

        //Trajectory Animation Variables
        private bool TrajectoryAnimationRunning = false;
        private float TrajectoryAnimationSpeed = 1f;

        private OrientationColor segment_color_variable = OrientationColor.ORIENTATION_ANGLE;
        private OrientationColor cone_color_variable = OrientationColor.ORIENTATION_ANGLE;
        private float cone_size_multiplier = 1f;
        private void Start()
        {
            PointSelectionThreadList = new Queue<SelectPointsThreadHandler>();
            ThresholdThreadQueue = new Queue<ThresholdThreadHandler>();
            RandomThresholdThreadQueue = new Queue<RandomThresholdThreadHandler>();
            SlidingWindowThreadQueue = new Queue<SlidingWindowThreadHandler>();

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
            for (int i = 0; i < data.globalMetaData.counterPointsList.Count; i++)
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

            currcloud.globalMetaData.box_scale = new_scale;
        }


        public void ReloadAllBoxes()
        {
            List<int> IDlist = LoadAllIDs();
            foreach (int id in IDlist)
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
            currentCloud.globalMetaData.ScaleBarDistanceX = (int)value;
            currentCloud.gameObject.GetComponent<CloudBox>().CreateBox();
        }
        public void ChangeBoxGraduationsY(float value)
        {
            CloudData currentCloud = LoadCurrentStatus();
            currentCloud.globalMetaData.ScaleBarDistanceY = (int)value;
            currentCloud.gameObject.GetComponent<CloudBox>().CreateBox();
        }
        public void ChangeBoxGraduationsZ(float value)
        {
            CloudData currentCloud = LoadCurrentStatus();
            currentCloud.globalMetaData.ScaleBarDistanceZ = (int)value;
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
            currentCloud.LineShaderObject.GetComponent<MeshRenderer>().material.SetTexture("_ColorTex", texture);
            if (reverse == true)
            {
                currentCloud.GetComponent<MeshRenderer>().material.EnableKeyword("COLORMAP_REVERSED");
                currentCloud.LineShaderObject.GetComponent<MeshRenderer>().material.EnableKeyword("COLORMAP_REVERSED");
            }
            else
            {
                currentCloud.GetComponent<MeshRenderer>().material.DisableKeyword("COLORMAP_REVERSED");
                currentCloud.LineShaderObject.GetComponent<MeshRenderer>().material.DisableKeyword("COLORMAP_REVERSED");
            }
            currentCloud.globalMetaData.colormapReversed = reverse;

            if (OnColorMapChange != null)
            {
                OnColorMapChange(newMapName);
            }
            //Debug.Log("eventcheck");
            currentCloud.globalMetaData.colormapName = newMapName;

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
            if (cmax - cmin > 0)
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
                else if (i >= (int)(maxspace - 1))
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

        public void ShowHidePointSpritesSwitch()
        {
            CloudData data = LoadCurrentStatus();
            if(data.globalMetaData.pointspritesEnabled == true)
            {
                HidePointSprites();
            }
            else
            {
                DisplayPointSprites();
            }
        }

        private void DisplayPointSprites()
        {
            CloudData data = LoadCurrentStatus();
            data.GetComponent<MeshRenderer>().enabled = true;
            data.globalMetaData.pointspritesEnabled = true;
        }
        private void HidePointSprites()
        {
            CloudData data = LoadCurrentStatus();
            data.GetComponent<MeshRenderer>().enabled = false;
            data.globalMetaData.pointspritesEnabled = false;

        }

        public void ShowHidePointBarsSwitch()
        {
            CloudData data = LoadCurrentStatus();
            if (data.globalMetaData.pointbarsEnabled == true)
            {
                HidePointBars();
            }
            else
            {
                DisplayPointBars();
            }
        }

        private void DisplayPointBars()
        {
            CloudData data = LoadCurrentStatus();
            data.LineShaderObject.SetActive(true);
            data.globalMetaData.pointbarsEnabled = true;
        }
        private void HidePointBars()
        {
            CloudData data = LoadCurrentStatus();
            data.LineShaderObject.SetActive(false);
            data.globalMetaData.pointbarsEnabled = false;

        }



        #endregion


        #region VolumeRendering

        public delegate void OnVolumeRenderingActivatedEvent();
        public event OnVolumeRenderingActivatedEvent OnVolumeRenderingActivated;

        public void ActivateVolumeRendering(GameObject VolumeObject, Vector3 dimensions)
        {
            
            CloudData data = LoadCurrentStatus();
            data.globalMetaData.volume_rendered_gameobject = VolumeObject;
            data.globalMetaData.volume_rendered_object_pixel_dimensions = dimensions;
            VolumeObject.transform.SetParent(data.transform, false);
            VolumeObject.transform.localPosition = Vector3.zero;
            VolumeObject.transform.localRotation = Quaternion.identity;
            VolumeObject.transform.localScale = data.globalMetaData.box_scale;
            //Calculate voxel size
            float xRange = data.globalMetaData.xMax - data.globalMetaData.xMin;
            float yRange = data.globalMetaData.yMax - data.globalMetaData.yMin;
            float zRange = data.globalMetaData.zMax - data.globalMetaData.zMin;
            float maxRange = Mathf.Max(xRange, yRange, zRange);
            float maxScaleXY = Mathf.Max(VolumeObject.transform.localScale.x, VolumeObject.transform.localScale.y);
            //dimensions
            float VoxelSizeXY = 0;
            if (VolumeObject.transform.localScale.x == maxScaleXY)
            {
                VoxelSizeXY = (maxScaleXY * maxRange) /dimensions.x;

            }
            else
            {
                VoxelSizeXY = (maxScaleXY * maxRange) / dimensions.y;
            }

            float VoxelSizeZ = (VolumeObject.transform.localScale.z * maxRange) / dimensions.z;
            data.globalMetaData.voxel_size = VoxelSizeXY;
            data.globalMetaData.voxel_size_z = VoxelSizeZ;

            OnVolumeRenderingActivated();
        }

        public void ChangeVolumeRenderedObjectVoxelSize(float newVoxelSizeXY, float newVoxelSizeZ)
        {
            CloudData data = LoadCurrentStatus();
            if (data.globalMetaData.volume_rendered_gameobject)
            {
                data.globalMetaData.voxel_size = newVoxelSizeXY;
                data.globalMetaData.voxel_size_z = newVoxelSizeZ;
                Vector3 dimensions = data.globalMetaData.volume_rendered_object_pixel_dimensions;
                Debug.Log("image stack dimensions : " + dimensions);

                Vector3 totalvoxelSize = new Vector3(newVoxelSizeXY * dimensions.x,
                                                     newVoxelSizeXY * dimensions.y,
                                                     newVoxelSizeZ * dimensions.z);

                float xRange = data.globalMetaData.xMax - data.globalMetaData.xMin;
                float yRange = data.globalMetaData.yMax - data.globalMetaData.yMin;
                float zRange = data.globalMetaData.zMax - data.globalMetaData.zMin;
                float maxRange = Mathf.Max(xRange, yRange, zRange);
                Debug.Log("totalvoxelSizeX : " + totalvoxelSize.x + " totalvoxelSizeY : " + totalvoxelSize.y + " totalvoxelSizeZ : " + totalvoxelSize.z);

                Debug.Log("xRange : " + xRange + " yRange : " + yRange + " zRange : " + zRange);
                Vector3 newScale = new Vector3(totalvoxelSize.x / xRange,
                                               totalvoxelSize.y / yRange,
                                               totalvoxelSize.z / zRange);
                Debug.Log(newScale);

                Vector3 finalScale = new Vector3(newScale.x * (xRange / maxRange),
                                                 newScale.y * (yRange / maxRange),
                                                 newScale.z * (zRange / maxRange));

                Debug.Log(finalScale);

                data.globalMetaData.volume_rendered_gameobject.transform.localScale = finalScale;


            }
        }

        public void ChangeVolumeRenderedObjectLocalCoordinates(Vector3 newposition, Vector3 newrotation, Vector3 newscale)
        {
            CloudData data = LoadCurrentStatus();
            if (data.globalMetaData.volume_rendered_gameobject)
            {
                data.globalMetaData.volume_rendered_gameobject.transform.localPosition = newposition;
                data.globalMetaData.volume_rendered_gameobject.transform.localRotation = Quaternion.identity;
                data.globalMetaData.volume_rendered_gameobject.transform.Rotate(newrotation, Space.Self);
                Vector3 newlocalscale = new Vector3(data.globalMetaData.volume_rendered_gameobject.transform.localScale.x * newscale.x,
                                                    data.globalMetaData.volume_rendered_gameobject.transform.localScale.y * newscale.y,
                                                    data.globalMetaData.volume_rendered_gameobject.transform.localScale.z * newscale.z);
                
                data.globalMetaData.volume_rendered_gameobject.transform.localScale = newlocalscale;

            }
        }

        public void HideVolumeObject()
        {
            CloudData data = LoadCurrentStatus();
            if (data.globalMetaData.volume_rendered_gameobject)
            {
                data.globalMetaData.volume_rendered_gameobject.SetActive(false);
            }
        }

        
        public void ShowVolumeObject()
        {
            CloudData data = LoadCurrentStatus();
            if (data.globalMetaData.volume_rendered_gameobject)
            {
                data.globalMetaData.volume_rendered_gameobject.SetActive(true);
            }
        }

        #endregion


            #region Point Selection
            public void UpdatePointSelection()
        {
            if (PointSelectionThreadList.Count < maxSelectionThreads)
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
            //BUGS OUT SOMETIMES, NO REASON FOUNT YET (2023/10/18) NEED TO TEST
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

        public void ChangeCollumnSelection(List<int> columnList)
        {
            CloudData currcloud = LoadCurrentStatus();

            float xMax = currcloud.globalMetaData.columnMetaDataList[columnList[0]].MaxThreshold;
            float xMin = currcloud.globalMetaData.columnMetaDataList[columnList[0]].MinThreshold;
            float yMax = currcloud.globalMetaData.columnMetaDataList[columnList[1]].MaxThreshold;
            float yMin = currcloud.globalMetaData.columnMetaDataList[columnList[1]].MinThreshold;
            float zMax = currcloud.globalMetaData.columnMetaDataList[columnList[2]].MaxThreshold;
            float zMin = currcloud.globalMetaData.columnMetaDataList[columnList[2]].MinThreshold;
            float iMax = currcloud.globalMetaData.columnMetaDataList[columnList[3]].MaxThreshold;
            float iMin = currcloud.globalMetaData.columnMetaDataList[columnList[3]].MinThreshold;
            float tMax = currcloud.globalMetaData.columnMetaDataList[columnList[4]].MaxThreshold;
            float tMin = currcloud.globalMetaData.columnMetaDataList[columnList[4]].MinThreshold;
            float sMax = currcloud.globalMetaData.columnMetaDataList[columnList[9]].MaxThreshold;
            float sMin = currcloud.globalMetaData.columnMetaDataList[columnList[9]].MinThreshold;



            GameObject trajobj = currcloud.trajectoryObject;
            currcloud.trajectoryObject = null;
            currcloud.pointTrajectoriesTable.Clear();
            Destroy(trajobj);

            GameObject oriobj = currcloud.orientationObject;
            currcloud.orientationObject = null;
            currcloud.globalMetaData.orientation_2d = false;
            currcloud.globalMetaData.orientation_3d = false;
            Destroy(oriobj);

            GameObject volobj = currcloud.globalMetaData.volume_rendered_gameobject;
            currcloud.globalMetaData.volume_rendered_gameobject = null;
            Destroy(volobj);

            List<float> TimeList = new List<float>();
            HashSet<float> TimeHash = new HashSet<float>();
            foreach (int key in currcloud.pointDataTable.Keys)
            {
                currcloud.pointDataTable[key].position = new Vector3(currcloud.columnData[columnList[0]][key],
                                                                     currcloud.columnData[columnList[1]][key],
                                                                     currcloud.columnData[columnList[2]][key]);

                currcloud.pointDataTable[key].time = currcloud.columnData[columnList[4]][key];
                currcloud.pointDataTable[key].phi_angle = currcloud.columnData[columnList[6]][key];
                currcloud.pointDataTable[key].theta_angle = currcloud.columnData[columnList[7]][key];

                currcloud.pointDataTable[key].trajectory = currcloud.columnData[columnList[5]][key];
                currcloud.pointDataTable[key].intensity = currcloud.columnData[columnList[3]][key];

                currcloud.pointDataTable[key].precision_xy = currcloud.columnData[columnList[10]][key];
                currcloud.pointDataTable[key].precision_z = currcloud.columnData[columnList[11]][key];




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
            foreach (float f in TimeHash)
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
                currcloud.pointDataTable[key].color_index = (currcloud.pointDataTable[key].intensity - iMin) / (iMax - iMin);
                currcloud.pointDataTable[key].frame = FrameDict[currcloud.pointDataTable[key].time];

                currcloud.pointDataTable[key].wobble_angle = currcloud.columnData[columnList[8]][key];

                currcloud.pointDataTable[key].size = (currcloud.columnData[columnList[9]][key] - sMin) / (sMax - sMin);

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
                coloruv[key] = new Vector2(currcloud.pointDataTable[key].color_index, key);
                hiddenselecteduv[key] = new Vector2(selected, hidden);
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
            currcloud.globalMetaData.iMax = iMax;
            currcloud.globalMetaData.tMax = tMax;
            currcloud.globalMetaData.xMin = xMin;
            currcloud.globalMetaData.yMin = yMin;
            currcloud.globalMetaData.zMin = zMin;
            currcloud.globalMetaData.tMin = tMin;
            currcloud.globalMetaData.iMin = iMin;



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

            //currcloud.globalMetaData.displayCollumnsConfiguration = columnList.ToArray();
            int cpt = 0;
            foreach (string s in currcloud.globalMetaData.CloudDataVariablesDict.Keys.ToList())
            {
                currcloud.globalMetaData.CloudDataVariablesDict[s] = columnList[cpt];
                cpt++;
            }

            ChangeThreshold();
            ReloadBox(currcloud.globalMetaData.cloud_id);

            CreateLineMesh(currcloud, verts, uv, coloruv, hiddenselecteduv, trajectoryuv);
            ChangeCurrentColorMap(currcloud.globalMetaData.colormapName, currcloud.globalMetaData.colormapReversed);

            if (OnCloudReloaded != null)
            {
                OnCloudReloaded(currcloud.globalMetaData.cloud_id);
            }

        }

        private void CreateLineMesh(CloudData root, Vector3[] point_vertices, Vector2[] uv0Array, Vector2[] uv1Array, Vector2[] uv2Array, Vector2[] uv3Array)
        {
            float offsetxy = 0.0f;
            float offsetz = 0.0f;

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
                Vector3 newVectorX = OffsetVectorX + new Vector3((root.pointDataTable[i].precision_xy) / root.globalMetaData.maxRange, 0.0f, 0.0f);

                vertices.Add(point_vertices[i] + (newVectorX/2)); // calculate offset here
                indices.Add(index);
                uv0.Add(uv0Array[cpt]);
                uv1.Add(uv1Array[cpt]);
                uv2.Add(uv2Array[cpt]);
                uv3.Add(uv3Array[cpt]);
                index++;

                vertices.Add(point_vertices[i] - (newVectorX/2));
                indices.Add(index);
                uv0.Add(uv0Array[cpt]);
                uv1.Add(uv1Array[cpt]);
                uv2.Add(uv2Array[cpt]);
                uv3.Add(uv3Array[cpt]);
                index++;

                Vector3 newVectorY = OffsetVectorY + new Vector3(0.0f, (root.pointDataTable[i].precision_xy) / root.globalMetaData.maxRange, 0.0f);

                vertices.Add(point_vertices[i] + newVectorY/2);
                indices.Add(index);
                uv0.Add(uv0Array[cpt]);
                uv1.Add(uv1Array[cpt]);
                uv2.Add(uv2Array[cpt]);
                uv3.Add(uv3Array[cpt]);
                index++;

                vertices.Add(point_vertices[i] - newVectorY/2);
                indices.Add(index);
                uv0.Add(uv0Array[cpt]);
                uv1.Add(uv1Array[cpt]);
                uv2.Add(uv2Array[cpt]);
                uv3.Add(uv3Array[cpt]);
                index++;

                Vector3 newVectorZ = OffsetVectorZ + new Vector3(0.0f, 0.0f, (root.pointDataTable[i].precision_z) / root.globalMetaData.maxRange);

                vertices.Add(point_vertices[i] + newVectorZ/2);
                indices.Add(index);
                uv0.Add(uv0Array[cpt]);
                uv1.Add(uv1Array[cpt]);
                uv2.Add(uv2Array[cpt]);
                uv3.Add(uv3Array[cpt]);
                index++;

                vertices.Add(point_vertices[i] - newVectorZ/2);
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

            MeshFilter mf = root.LineShaderObject.GetComponent<MeshFilter>();
            MeshRenderer mr = root.LineShaderObject.GetComponent<MeshRenderer>();
            mf.mesh = mesh;


        }


        public void HideNaNValues(bool value)
        {
            CloudData data = LoadCurrentStatus();
            data.globalMetaData.HideNaNValues = value;
            List<int> columnList = new List<int>();


            foreach (string s in data.globalMetaData.CloudDataVariablesDict.Keys.ToList())
            {
                columnList.Add(data.globalMetaData.CloudDataVariablesDict[s]);
            }

            ChangeCollumnSelection(columnList);
        }
        #endregion

        #region Thresholding
        public void ChangeThreshold()
        {
            if (ThresholdThreadQueue.Count < maxThresholdThreads)
            {
                CloudData currcloud = LoadCurrentStatus();
                ThresholdThreadHandler newThread = new ThresholdThreadHandler();
                newThread.currcloud = currcloud;
                newThread.uv2List = currcloud.GetComponent<MeshFilter>().mesh.uv3;
                newThread.StartThread();
                ThresholdThreadQueue.Enqueue(newThread);
                if (thresholdJobON == false)
                {
                    thresholdJobON = true;
                }
            }

        }

        public void RandomThreshold(float percentage_value)
        {
            
            if (RandomThresholdThreadQueue.Count < maxRandomThresholdThreads)
            {
                CloudData currcloud = LoadCurrentStatus();
                RandomThresholdThreadHandler newThread = new RandomThresholdThreadHandler();
                newThread.currcloud = currcloud;
                newThread.uv2List = currcloud.GetComponent<MeshFilter>().mesh.uv3;
                newThread.point_limit_percentage = percentage_value;
                newThread.DuplicatesNumberSet = new HashSet<int>();
                newThread.rnd = new System.Random();
                newThread.StartThread();
                RandomThresholdThreadQueue.Enqueue(newThread);
                if (randomthresholdJobON == false)
                {
                    randomthresholdJobON = true;
                }
            }
            
            /**
            HashSet<int> DuplicatesNumberSet = new HashSet<int>();
            CloudData currcloud = LoadCurrentStatus();
            Vector2[] uv2List = currcloud.GetComponent<MeshFilter>().mesh.uv3;
            Run((int)percentage_value, currcloud, uv2List, DuplicatesNumberSet);

            Mesh mesh = currcloud.GetComponent<MeshFilter>().mesh;

            mesh.uv3 = uv2List;
            //Debug.Log("point limit number : " + point_limit_number.ToString());
            //Debug.Log("point table count : " + thread.currcloud.pointDataTable.Count.ToString());

            currcloud.gameObject.GetComponent<MeshFilter>().mesh = mesh;
            **/

        }

        public void ChangeSlidingWindow(int columnID, float WindowSize, float WindowCenterPosition, bool WrapAround = true)
        {
            if (SlidingWindowThreadQueue.Count < maxSlidingWindowThreads)
            {
                CloudData currcloud = LoadCurrentStatus();
                SlidingWindowThreadHandler newThread = new SlidingWindowThreadHandler();
                newThread.currcloud = currcloud;
                newThread.uv2List = currcloud.GetComponent<MeshFilter>().mesh.uv3;
                newThread.ColumnID = columnID;
                float WindowLimitdown, WindowLimitup;
                if (WrapAround==true && (WindowCenterPosition - (WindowSize / 2)) < currcloud.globalMetaData.columnMetaDataList[columnID].MinValue)
                {
                    WindowLimitdown = (currcloud.globalMetaData.columnMetaDataList[columnID].MaxValue - (WindowSize / 2)) + (WindowCenterPosition - currcloud.globalMetaData.columnMetaDataList[columnID].MinValue);
                    WindowLimitup = WindowCenterPosition + (WindowSize / 2);

                }
                else if(WrapAround == true && (WindowCenterPosition + (WindowSize / 2)) > currcloud.globalMetaData.columnMetaDataList[columnID].MaxValue)
                {
                    WindowLimitdown = WindowCenterPosition - (WindowSize / 2);

                    WindowLimitup = (currcloud.globalMetaData.columnMetaDataList[columnID].MinValue + (WindowSize / 2)) - (currcloud.globalMetaData.columnMetaDataList[columnID].MaxValue - WindowCenterPosition);
                }
                else
                {
                    WindowLimitdown = WindowCenterPosition - (WindowSize / 2);
                    WindowLimitup = WindowCenterPosition + (WindowSize / 2);
                }
                newThread.WindowLimitDown = WindowLimitdown;
                newThread.WindowLimitUp = WindowLimitup;
                newThread.StartThread();
                SlidingWindowThreadQueue.Enqueue(newThread);
                if (slidingwindowJobON == false)
                {
                    slidingwindowJobON = true;
                }
            }
        }

        public void ResetSlidingWindow()
        {
            CloudData data = LoadCurrentStatus();
            Vector2[] uv2Array = data.GetComponent<MeshFilter>().mesh.uv3;
            foreach (KeyValuePair<int, PointData> kvp in data.pointDataTable)
            {
                //Debug.Log(kvp.Value.time);
                if (data.pointMetaDataTable[kvp.Key].isHidden == false)
                {
                    uv2Array[kvp.Key].y = 0f;

                }
                else
                {
                    uv2Array[kvp.Key].y = 1f;

                }
            }

            data.GetComponent<MeshFilter>().mesh.uv3 = uv2Array;

        }


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
            foreach (float id in data.pointTrajectoriesTable.Keys)
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
                TrajectoryUV1List.Add(new Vector2(item.Value.trajectoryID / maxtrajectoryID, item.Value.pointsIDList[item.Value.pointsIDList.Count - 1]));

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

            Material trajectorymaterial = new Material(Shader.Find("Genuage/ThickLineShader"));
            trajectorymaterial.SetTexture("_ColorTex", ColorMapManager.instance.GetColorMap("jet").texture);
            trajectorymaterial.SetFloat("_Thickness", 0.0005f);
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

        public void PlayTrajectoryAnimation()
        {
            CloudData data = LoadCurrentStatus();
            if (data.trajectoryObject)
            {
                TrajectoryAnimationRunning = true;
            }
        }

        public void StopTrajectoryAnimation()
        {
            CloudData data = LoadCurrentStatus();
            if (data.trajectoryObject)
            {
                TrajectoryAnimationRunning = false;
            }
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

        public void SetTrajectoryShaderColor(string colormap_name)
        {
            CloudData data = LoadCurrentStatus();

            if (data.trajectoryObject)
            {
                MeshRenderer TrajectoriesRenderer = data.trajectoryObject.GetComponent<MeshRenderer>();
                //MeshRenderer PointsRenderer = data.GetComponent<MeshRenderer>();

                TrajectoriesRenderer.material.SetTexture("_ColorTex", ColorMapManager.instance.GetColorMap(colormap_name).texture);
            }
        }

        public void ChangeTrajectorySegmentSize(float value)
        {
            CloudData data = LoadCurrentStatus();

            if (data.trajectoryObject)
            {
                MeshRenderer TrajectoriesRenderer = data.trajectoryObject.GetComponent<MeshRenderer>();
                //MeshRenderer PointsRenderer = data.GetComponent<MeshRenderer>();

                TrajectoriesRenderer.material.SetFloat("_Thickness", value * 0.2f);

            }
        }

        public void ChangeTrajectoryAnimSpeed(float i)
        {
            TrajectoryAnimationSpeed = i;
        }
        #endregion

        #region Orientation Visualization

        public void SwitchOrientation2D()
        {
            CloudData data = LoadCurrentStatus();

            if (data.globalMetaData.orientation_2d)
            {
                if (data.orientationObject)
                {
                    GameObject obj = data.orientationObject;
                    data.orientationObject = null;
                    Destroy(obj);
                    data.globalMetaData.orientation_3d = false;
                    data.globalMetaData.orientation_2d = false;
                }
            }
            else if (data.globalMetaData.orientation_3d)
            {
                if (data.orientationObject)
                {
                    GameObject obj = data.orientationObject;
                    data.orientationObject = null;
                    Destroy(obj);
                    data.globalMetaData.orientation_3d = false;
                    data.globalMetaData.orientation_2d = false;

                    CreateOrientationMesh(data, false);
                    data.globalMetaData.orientation_2d = true;
                }
            }
            else
            {
                CreateOrientationMesh(data, false);
                data.globalMetaData.orientation_2d = true;

            }
        }


        public void SwitchOrientation3D()
        {
            CloudData data = LoadCurrentStatus();

            if (data.globalMetaData.orientation_3d)
            {
                if (data.orientationObject)
                {
                    GameObject obj = data.orientationObject;
                    data.orientationObject = null;
                    Destroy(obj);
                    data.globalMetaData.orientation_3d = false;
                    data.globalMetaData.orientation_2d = false;
                }
            }
            else if (data.globalMetaData.orientation_2d)
            {
                if (data.orientationObject)
                {
                    GameObject obj = data.orientationObject;
                    data.orientationObject = null;
                    Destroy(obj);
                    data.globalMetaData.orientation_3d = false;
                    data.globalMetaData.orientation_2d = false;

                    CreateOrientationMesh(data, true);
                    data.globalMetaData.orientation_3d = true;
                }
            }
            else
            {
                CreateOrientationMesh(data, true);
                data.globalMetaData.orientation_3d = true;

            }
        }

       public void ChangeOrientationSegmentSize(float newsize)
        {
            if (!CloudSelector.instance.noSelection)
            {
                CloudData data = LoadCurrentStatus();
                data.globalMetaData.orientation_segment_size = newsize;
                if (data.globalMetaData.orientation_2d || data.globalMetaData.orientation_3d)
                {
                    ReloadOrientationMesh(data);
                }
            }
        }

        private void CreateOrientationMesh(CloudData data, bool is3D)
        {
            AngleUnit angle_unit = data.globalMetaData.angleUnit;
            List<float> xvalues = new List<float>();
            List<float> yvalues = new List<float>();
            List<float> zvalues = new List<float>();

            List<Vector2> UV1List = new List<Vector2>();
            List<Vector2> UV2List = new List<Vector2>();

            List<Vector2> UV3List = new List<Vector2>();

            //List<Color> color = new List<Color>();
            List<float> coloruv = new List<float>();

            foreach (var kvp in data.pointDataTable)
            {
                float theta = kvp.Value.theta_angle;
                float phi = kvp.Value.phi_angle;
                float wobble = kvp.Value.wobble_angle;
                if (is3D)
                {
                    CalculateOrientationAngle3D(xvalues, yvalues, zvalues, 
                                                coloruv, theta, phi, wobble, angle_unit,
                                                ORIENTATION_SEGMENT_SIZE * data.globalMetaData.orientation_segment_size);
                }
                else
                {
                    CalculateOrientationAngle2D(xvalues, yvalues, zvalues, 
                                                coloruv, theta, wobble, angle_unit,
                                                ORIENTATION_SEGMENT_SIZE * data.globalMetaData.orientation_segment_size);
                }
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            int index = 0;
            float hidden = 0f;
            float selected = 0f;

            for (int i = 0; i < xvalues.Count; i++)
            {
                if (!data.pointMetaDataTable[i].isHidden)
                {
                    hidden = 0f;
                    selected = 0f;
                    if (data.pointMetaDataTable[i].isHidden)
                    {
                        hidden = 1f;
                    }
                    if (data.pointMetaDataTable[i].isSelected)
                    {
                        selected = 1f;
                    }

                    vertices.Add(data.pointDataTable[i].normed_position - (new Vector3(xvalues[i], yvalues[i], zvalues[i])));
                    //color.Add(Color.blue);
                    indices.Add(index);
                    UV3List.Add(new Vector2(data.pointDataTable[i].trajectory, data.pointDataTable[i].frame));
                    UV1List.Add(new Vector2(coloruv[i], data.pointDataTable[i].pointID));
                    UV2List.Add(new Vector2(selected, hidden));

                    index++;
                    vertices.Add(data.pointDataTable[i].normed_position + (new Vector3(xvalues[i], yvalues[i], zvalues[i])));
                    //color.Add(Color.blue);
                    indices.Add(index);
                    UV3List.Add(new Vector2(data.pointDataTable[i].trajectory, data.pointDataTable[i].frame));
                    UV1List.Add(new Vector2(coloruv[i], data.pointDataTable[i].pointID));
                    UV2List.Add(new Vector2(selected, hidden));

                    index++;
                }
            }
            mesh.vertices = vertices.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            mesh.uv2 = UV1List.ToArray();
            mesh.uv3 = UV2List.ToArray();
            mesh.uv4 = UV3List.ToArray();

            GameObject child = new GameObject();
            child.transform.SetParent(data.transform, false);
            child.AddComponent<MeshFilter>();
            child.AddComponent<MeshRenderer>();
            child.GetComponent<MeshFilter>().mesh = mesh;
            Material material = new Material(Shader.Find("Genuage/UnlitLineShader"));
            material.SetTexture("_ColorTex", ColorMapManager.instance.GetColorMap(data.globalMetaData.orientationcolormapName).texture);

            material.SetFloat("_UpperTimeLimit", data.globalMetaData.timeList.Count - 1);
            material.SetFloat("_LowerTimeLimit", 0f);


            child.GetComponent<MeshRenderer>().material = material;
            data.orientationObject = child;

        }

        private void ReloadOrientationMesh(CloudData data)
        {
            AngleUnit angle_unit = data.globalMetaData.angleUnit;
            List<float> xvalues = new List<float>();
            List<float> yvalues = new List<float>();
            List<float> zvalues = new List<float>();

            List<Vector2> UV1List = new List<Vector2>();
            List<Vector2> UV2List = new List<Vector2>();

            List<Vector2> UV3List = new List<Vector2>();

            //List<Color> color = new List<Color>();
            List<float> coloruv = new List<float>();

            foreach (var kvp in data.pointDataTable)
            {
                float theta = kvp.Value.theta_angle;
                float phi = kvp.Value.phi_angle;
                float wobble = kvp.Value.wobble_angle;
                if (data.globalMetaData.orientation_3d)
                {
                    CalculateOrientationAngle3D(xvalues, yvalues, zvalues,
                                                coloruv, theta, phi, wobble, angle_unit,
                                                ORIENTATION_SEGMENT_SIZE * data.globalMetaData.orientation_segment_size);
                }
                else
                {
                    CalculateOrientationAngle2D(xvalues, yvalues, zvalues,
                                                coloruv, theta, wobble, angle_unit,
                                                ORIENTATION_SEGMENT_SIZE * data.globalMetaData.orientation_segment_size);
                }
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            int index = 0;
            float hidden = 0f;
            float selected = 0f;

            for (int i = 0; i < xvalues.Count; i++)
            {
                if (!data.pointMetaDataTable[i].isHidden)
                {
                    hidden = 0f;
                    selected = 0f;
                    if (data.pointMetaDataTable[i].isHidden)
                    {
                        hidden = 1f;
                    }
                    if (data.pointMetaDataTable[i].isSelected)
                    {
                        selected = 1f;
                    }

                    vertices.Add(data.pointDataTable[i].normed_position - (new Vector3(xvalues[i], yvalues[i], zvalues[i])));
                    //color.Add(Color.blue);
                    indices.Add(index);
                    UV3List.Add(new Vector2(data.pointDataTable[i].trajectory, data.pointDataTable[i].frame));
                    UV1List.Add(new Vector2(coloruv[i], data.pointDataTable[i].pointID));
                    UV2List.Add(new Vector2(selected, hidden));

                    index++;
                    vertices.Add(data.pointDataTable[i].normed_position + (new Vector3(xvalues[i], yvalues[i], zvalues[i])));
                    //color.Add(Color.blue);
                    indices.Add(index);
                    UV3List.Add(new Vector2(data.pointDataTable[i].trajectory, data.pointDataTable[i].frame));
                    UV1List.Add(new Vector2(coloruv[i], data.pointDataTable[i].pointID));
                    UV2List.Add(new Vector2(selected, hidden));

                    index++;
                }
            }
            mesh.vertices = vertices.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            mesh.uv2 = UV1List.ToArray();
            mesh.uv3 = UV2List.ToArray();
            mesh.uv4 = UV3List.ToArray();
            data.orientationObject.GetComponent<MeshFilter>().mesh = mesh;
        }


        private void CalculateOrientationAngle2D(List<float> xvalues, List<float> yvalues, List<float> zvalues, 
                                         List<float> coloruv, float theta, float wobble, AngleUnit unit, float lineSize)
        {
            float divider = 1.0f;

            switch (unit)
            {
                case AngleUnit.DEGREES:
                    xvalues.Add(lineSize * Mathf.Cos(Mathf.Deg2Rad * theta));
                    yvalues.Add(lineSize * Mathf.Sin(Mathf.Deg2Rad * theta));
                    zvalues.Add(0f);
                    divider = DEGREE_MAX;
                    break;
                case AngleUnit.RADIANS:
                    xvalues.Add(lineSize * Mathf.Cos(theta));
                    yvalues.Add(lineSize * Mathf.Sin(theta));
                    zvalues.Add(0f);
                    divider = RADIANS_MAX;
                    break;
            }

            switch (segment_color_variable)
            {
                case OrientationColor.ORIENTATION_ANGLE:
                    coloruv.Add(theta / divider);
                    break;

                case OrientationColor.WOBBLE_ANGLE:
                    coloruv.Add(wobble / divider);
                    break;
            }



        }

        private void CalculateOrientationAngle3D(List<float> xvalues, List<float> yvalues, List<float> zvalues,
                                         List<float> coloruv, float theta, float phi, float wobble, AngleUnit unit, float lineSize)
        {
            float mean = ((phi + theta) / 2f);
            float divider = 1.0f;

            switch (unit)
            {
                case AngleUnit.DEGREES:
                    xvalues.Add(lineSize * Mathf.Sin(Mathf.Deg2Rad * theta) * Mathf.Cos(Mathf.Deg2Rad * phi));
                    yvalues.Add(lineSize * Mathf.Sin(Mathf.Deg2Rad * theta) * Mathf.Sin(Mathf.Deg2Rad * phi));
                    zvalues.Add(lineSize * Mathf.Cos(Mathf.Deg2Rad * theta));
                    divider = DEGREE_MAX;
                    break;

             
                case AngleUnit.RADIANS:
                    xvalues.Add(lineSize * Mathf.Sin(Mathf.Deg2Rad * theta) * Mathf.Cos(Mathf.Deg2Rad * phi));
                    yvalues.Add(lineSize * Mathf.Sin(Mathf.Deg2Rad * theta) * Mathf.Sin(Mathf.Deg2Rad * phi));
                    zvalues.Add(lineSize * Mathf.Cos(Mathf.Deg2Rad * theta));
                    divider = RADIANS_MAX;
                    break;
            }
            switch (segment_color_variable)
            {
                case OrientationColor.ORIENTATION_ANGLE:
                    coloruv.Add(mean / divider); 
                    break;

                case OrientationColor.WOBBLE_ANGLE:
                    coloruv.Add(wobble / divider); 
                    break;
            }

 

        }


        public void ChangeAngleUnit(AngleUnit new_unit)
        {
            if (!CloudSelector.instance.noSelection)
            {
                CloudData data = LoadCurrentStatus();
                data.globalMetaData.angleUnit = new_unit;

            }
        }

        public void SetOrientationShaderColor(string colormap_name)
        {
            if (!CloudSelector.instance.noSelection)
            {
                CloudData data = LoadCurrentStatus();

                if (data.orientationObject)
                {
                    MeshRenderer OrientationRenderer = data.orientationObject.GetComponent<MeshRenderer>();
                    //MeshRenderer PointsRenderer = data.GetComponent<MeshRenderer>();

                    OrientationRenderer.material.SetTexture("_ColorTex", ColorMapManager.instance.GetColorMap(colormap_name).texture);
                    data.globalMetaData.orientationcolormapName = colormap_name;
                }
            }
        }


        public void CalculateMeanOrientation(bool is2D)
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
            string str = "";
            if (is2D == false)
            {
                 str = "Mean Orientation value :\n" +
                    "Theta : " + Math.Round(thetasum, 3).ToString() + "\n" +
                    "Phi : " + Math.Round(phisum, 3).ToString();
            }
            else
            {
                str = "Mean Orientation value :\n" +
                    "Theta : " + Math.Round(thetasum, 3).ToString();
            }
            GameObject container = new GameObject("CylinderContainer");

            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.transform.SetParent(container.transform,true);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = new Vector3(0.01f, 0.05f, 0.01f);
            obj.transform.Rotate(new Vector3(90, 0, 0), Space.World);
            container.transform.SetParent(currcloud.transform, true);
            container.transform.localPosition = new Vector3(xaverage, yaverage, zaverage);
            float xvalue, zvalue, yvalue;
            if (is2D == false)
            {
                xvalue = (Mathf.Sin(Mathf.Deg2Rad * thetasum) * Mathf.Cos(Mathf.Deg2Rad * phisum));
                yvalue = (Mathf.Sin(Mathf.Deg2Rad * thetasum) * Mathf.Sin(Mathf.Deg2Rad * phisum));
                zvalue = (Mathf.Cos(Mathf.Deg2Rad * thetasum));
            }
            else
            {
                xvalue = (Mathf.Sin(Mathf.Deg2Rad * thetasum));
                yvalue = (Mathf.Sin(Mathf.Deg2Rad * thetasum));
                zvalue = (Mathf.Cos(Mathf.Deg2Rad * thetasum));
            }
                //Debug.Log(xvalue + " / " + yvalue + " / " + zvalue + " / ");
                Vector3 anglevector = currcloud.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(xaverage + xvalue, yaverage + yvalue, zaverage + zvalue));
            container.transform.LookAt(anglevector);
            
            //TODO : Inplement saving function into metadata
            ModalWindowManager.instance.CreateModalWindow(str);
        }

        public void ChangeOrientationColorVariable(int value)
        {
            if (value == 0)
            {
                segment_color_variable = OrientationColor.ORIENTATION_ANGLE;
            }
            else if (value == 1)
            {
                segment_color_variable = OrientationColor.WOBBLE_ANGLE;
            }
        }
        #endregion

        #region Orientation Wobble Visualization
        public void SwitchWobble2D(float size_multiplier)
        {
            CloudData data = LoadCurrentStatus();
            cone_size_multiplier = size_multiplier;
            if (data.globalMetaData.wobble_2d)
            {
                if (data.wobbleObject)
                {
                    GameObject obj = data.wobbleObject;
                    data.wobbleObject = null;
                    Destroy(obj);
                    data.globalMetaData.wobble_3d = false;
                    data.globalMetaData.wobble_2d = false;

                }
            }
            else if (data.globalMetaData.wobble_3d)
            {
                if (data.wobbleObject)
                {
                    GameObject obj = data.wobbleObject;
                    data.wobbleObject = null;
                    Destroy(obj);
                    data.globalMetaData.wobble_3d = false;
                    data.globalMetaData.wobble_2d = false;

                    CreateWobbleMesh();
                    data.globalMetaData.wobble_2d = true;
                }
            }
            else
            {
                CreateWobbleMesh();
                data.globalMetaData.wobble_2d = true;

            }
        }

        public void SetWobbleShaderColor(string colormap_name)
        {
            if (!CloudSelector.instance.noSelection)
            {
                CloudData data = LoadCurrentStatus();

                if (data.wobbleObject)
                {
                    MeshRenderer WobbleRenderer = data.wobbleObject.GetComponent<MeshRenderer>();
                    //MeshRenderer PointsRenderer = data.GetComponent<MeshRenderer>();

                    WobbleRenderer.material.SetTexture("_MainTex", ColorMapManager.instance.GetColorMap(colormap_name).texture);
                    data.globalMetaData.wobblecolormapname = colormap_name;
                }
            }
        }


        private void CreateWobbleMesh()
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();

            //construct end of cone like in the orientation shader for each point
            AngleUnit angle_unit = data.globalMetaData.angleUnit;
            List<float> xvalues = new List<float>();
            List<float> yvalues = new List<float>();
            List<float> zvalues = new List<float>();

            List<Vector2> uv = new List<Vector2>();
            List<Vector2> UV1List = new List<Vector2>();

            List<Vector2> UV2List = new List<Vector2>();
            List<Vector2> UV3List = new List<Vector2>();

            //List<Color> color = new List<Color>();
            List<float> coloruv = new List<float>();

            List<Vector3> FrontConeBaseCenterPointList = new List<Vector3>();
            List<Vector3> BackConeBaseCenterPointList = new List<Vector3>();

            List<Vector3> ConeEndPointList = new List<Vector3>();

            List<Vector3> FrontConePerpendicularPointList = new List<Vector3>();
            List<Vector3> BackConePerpendicularPointList = new List<Vector3>();

            List<Vector3> FrontConeCirclePointsList = new List<Vector3>();
            List<Vector3> BackConeCirclePointsList = new List<Vector3>();

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            float ConeSize = data.globalMetaData.wobble_cone_size * cone_size_multiplier;

            //System.Random random = new System.Random();
            int cpt = 0;

            foreach (var kvp in data.pointDataTable)
            {
                if (data.pointMetaDataTable[kvp.Key].isHidden == false)
                {
                    cpt++;


                    float theta = kvp.Value.theta_angle;
                    float wobble = kvp.Value.wobble_angle;
                    //float theta = random.Next(0, 180);

                    Vector3 HypothenusPoint = CalculateHypothenusPoint2D(theta, wobble, angle_unit, ConeSize);
                    Vector3 SquareAnglePoint = CalculateRightAnglePoint2D(theta, wobble, angle_unit, ConeSize);

                    //CalculateOrientationAngle2D(HypothenusPoint, SquareAnglePoint, theta, wobble, angle_unit,ConeSize);

                    //Debug.Log(HypothenusPoint);
                    //Debug.Log(SquareAnglePoint);
                    Vector3 FrontConeHypothenusPoint = kvp.Value.normed_position + HypothenusPoint;
                    Vector3 BackConeHypothenusPoint = kvp.Value.normed_position - HypothenusPoint;






                    Vector3 FrontConeCircleCenterPoint = kvp.Value.normed_position + SquareAnglePoint;
                    Vector3 BackConeCircleCenterPoint = kvp.Value.normed_position - SquareAnglePoint;

                    AddCircleMesh(kvp.Value.normed_position, FrontConeCircleCenterPoint, FrontConeHypothenusPoint, theta, wobble, angle_unit, true, ConeSize, vertices, triangles, UV1List);
                    AddCircleMesh(kvp.Value.normed_position, BackConeCircleCenterPoint, BackConeHypothenusPoint, theta, wobble, angle_unit, false, ConeSize, vertices, triangles, UV1List);


                }
            }
            //Debug.Log("Verts : " + vertices.Count + " Trigs : " + triangles.Count + " iterations = " + cpt);
            //create all circle points by rotating from the first point



            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.vertices = vertices.ToArray();
            mesh.SetIndices(triangles.ToArray(), MeshTopology.Triangles, 0);
            mesh.uv = UV1List.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();


            //GO.GetComponent<MeshFilter>().mesh = mesh;


            GameObject child = new GameObject();
            child.transform.SetParent(data.transform, false);
            child.AddComponent<MeshFilter>();
            child.AddComponent<MeshRenderer>();
            child.GetComponent<MeshFilter>().mesh = mesh;
            Material material = new Material(Shader.Find("Genuage/MapSurface"));
            material.SetColor("_Color", new Color(1f, 1f, 1f, 0.8f));
            material.SetTexture("_MainTex", ColorMapManager.instance.GetColorMap(data.globalMetaData.wobblecolormapname).texture);




            child.GetComponent<MeshRenderer>().material = material;
            data.wobbleObject = child;
        }
    


        private void AddCircleMesh(Vector3 point_position, Vector3 center, Vector3 hypothenuspoint, float theta, float wobble, AngleUnit unit, bool FrontCone, 
                                   float ConeSize, List<Vector3> vertices, List<int> triangles, List<Vector2> UV1List) 
        {

            float divider = 1.0f;
            float UV_Division = 0.0f;

            switch (unit)
            {
                case AngleUnit.DEGREES:
                    divider = DEGREE_MAX;
                    break;
                case AngleUnit.RADIANS:
                    divider = RADIANS_MAX;
                    break;
            }

            switch (cone_color_variable)
            {
                case OrientationColor.ORIENTATION_ANGLE:
                    UV_Division = theta / divider;
                    break;

                case OrientationColor.WOBBLE_ANGLE:
                    UV_Division = wobble / divider;
                    break;
            }


            List<Vector3> LastCircleList = new List<Vector3>();
            List<int> LastCircleIDs = new List<int>();
            //Add base circle
            vertices.Add(point_position);
            int ConeEndPointID = vertices.Count - 1;
            UV1List.Add(new Vector3(UV_Division, 0f));

            vertices.Add(center);
            int FrontConeCenterPointID = vertices.Count - 1;
            UV1List.Add(new Vector3(UV_Division, 0f));

            vertices.Add(hypothenuspoint);
            int FrontConeFirstCirclePoint = vertices.Count - 1;
            UV1List.Add(new Vector3(UV_Division, 0f));


            LastCircleList.Add(hypothenuspoint);
            LastCircleIDs.Add(FrontConeFirstCirclePoint);

            float angleincrement = 360f / cone_subdivisions;
            int cpt = 0;
            for (int i = 1; i <= cone_subdivisions; i++)
            {
                Vector3 offset = hypothenuspoint - center;
                Vector3 rotationAxis = center - point_position;
                var rotation = Quaternion.AngleAxis(i * angleincrement, rotationAxis);
                Vector3 newOffset = rotation * offset;
                Vector3 newFrontCirclePoint = center + newOffset;
                //FrontConeCirclePointsList.Add(newFrontCirclePoint);
                vertices.Add(newFrontCirclePoint);
                UV1List.Add(new Vector3(UV_Division, 0f));

                LastCircleList.Add(newFrontCirclePoint);
                LastCircleIDs.Add(vertices.Count - 1);

                //Debug.Log("new cone circle point : " + newFrontCirclePoint.ToString("F4"));

                //base triangle
                //triangles.Add(vertices.Count - 1);
                //triangles.Add(FrontConeCenterPointID);
                //triangles.Add(vertices.Count - 2);


                //length triangle
                triangles.Add(vertices.Count - 1);
                triangles.Add(vertices.Count - 2);
                triangles.Add(ConeEndPointID);



                //Vector3 newBackCirclePoint = Quaternion.AngleAxis(angleincrement * i, BackConeCenterVector) * BackFirstCirclePoint;
                //BackConeCirclePointsList.Add(newBackCirclePoint);

            }

            List<Vector3> newCirclePointsList = new List<Vector3>();
            List<int> newCircleIDs = new List<int>();

            float increment = wobble / circle_subdivisions;

            //Now Make all circles recursively
            for (int i = 1; i < circle_subdivisions; i++)
            {
                newCirclePointsList = new List<Vector3>();
                newCircleIDs = new List<int>();

                Vector3 newCenterPoint = CalculateRightAnglePoint2D(theta, wobble - (increment * (i)), unit, ConeSize);

                Vector3 newHypothenusPoint = CalculateHypothenusPoint2D(theta, wobble - (increment * (i)), unit, ConeSize);
                Vector3 ConeHypothenusPoint;
                Vector3 ConeCenterPoint;

                if (FrontCone)
                {
                    ConeHypothenusPoint = point_position + newHypothenusPoint;
                    ConeCenterPoint = point_position + newCenterPoint;

                }
                else
                {
                    ConeHypothenusPoint = point_position - newHypothenusPoint;
                    ConeCenterPoint = point_position - newCenterPoint;

                }


                vertices.Add(ConeHypothenusPoint);
                UV1List.Add(new Vector3(UV_Division, 0f));

                newCirclePointsList.Add(ConeHypothenusPoint);
                newCircleIDs.Add(vertices.Count - 1);


                for (int j = 1; j <= cone_subdivisions; j++)
                {

                    Vector3 offset = ConeHypothenusPoint - ConeCenterPoint;
                    Vector3 rotationAxis = ConeCenterPoint - point_position;
                    var rotation = Quaternion.AngleAxis(j * angleincrement, rotationAxis);
                    Vector3 newOffset = rotation * offset;
                    Vector3 newFrontCirclePoint = ConeCenterPoint + newOffset;
                    //FrontConeCirclePointsList.Add(newFrontCirclePoint);
                    vertices.Add(newFrontCirclePoint);
                    UV1List.Add(new Vector3(UV_Division, 0f));

                    //Debug.Log("x = "+ newFrontCirclePoint.x + "y = "+ newFrontCirclePoint.y+"z = "+ newFrontCirclePoint.z);
                    newCirclePointsList.Add(newFrontCirclePoint);
                    newCircleIDs.Add(vertices.Count - 1);

                }

                for (int k = 0; k < cone_subdivisions; k++)
                {

                    triangles.Add(LastCircleIDs[k]);
                    triangles.Add(LastCircleIDs[k + 1]);
                    triangles.Add(newCircleIDs[k + 1]);

                    triangles.Add(LastCircleIDs[k]);
                    triangles.Add(newCircleIDs[k + 1]);
                    triangles.Add(newCircleIDs[k]);


                }

                triangles.Add(LastCircleIDs[0]);
                triangles.Add(LastCircleIDs[LastCircleIDs.Count - 1]);
                triangles.Add(newCircleIDs[newCircleIDs.Count - 1]);

                triangles.Add(LastCircleIDs[0]);
                triangles.Add(newCircleIDs[newCircleIDs.Count - 1]);
                triangles.Add(newCircleIDs[0]);


                LastCircleIDs.Clear();
                LastCircleIDs = newCircleIDs;
                LastCircleList = newCirclePointsList;
            }

            //fill the hole at the top

            //Vector3 newCenterPoint2 = CalculateRightAnglePoint2D(theta, wobble - (increment * (i)), unit, ConeSize);



        }



        private Vector3 CalculateHypothenusPoint2D(float theta, float wobble, AngleUnit unit, float lineSize)

        {
            float Angle, HypothenusDistance, ConeHeight;
            Angle = wobble / 2;
            HypothenusDistance = lineSize;
            Vector3 ConeHypothenusPoint = Vector3.zero;
            float x, y, z;
            switch (unit)
            {
                case AngleUnit.DEGREES:
                    x = (HypothenusDistance * Mathf.Cos(Mathf.Deg2Rad * (theta + Angle)));
                    y = (HypothenusDistance * Mathf.Sin(Mathf.Deg2Rad * (theta + Angle)));
                    z = 0f;
                    ConeHypothenusPoint = new Vector3(x, y, z);

                    break;
                case AngleUnit.RADIANS:
                    x = (HypothenusDistance * Mathf.Cos(theta + Angle));
                    y = (HypothenusDistance * Mathf.Sin(theta + Angle));
                    z = 0f;
                    ConeHypothenusPoint = new Vector3(x, y, z);


                    break;

            }
            return ConeHypothenusPoint;
        }

        private Vector3 CalculateRightAnglePoint2D(float theta, float wobble, AngleUnit unit, float lineSize)
        {
            float Angle, HypothenusDistance, ConeHeight;
            Angle = wobble / 2;
            HypothenusDistance = lineSize;
            Vector3 ConeCenterPoint = Vector3.zero;
            float x, y, z;

            switch (unit)
            {
                case AngleUnit.DEGREES:
                    ConeHeight = Mathf.Cos(Mathf.Deg2Rad * Angle) * HypothenusDistance;
                    x = (ConeHeight * Mathf.Cos(Mathf.Deg2Rad * (theta)));
                    y = (ConeHeight * Mathf.Sin(Mathf.Deg2Rad * (theta)));
                    z = 0f;
                    ConeCenterPoint = new Vector3(x, y, z);
                    break;
                case AngleUnit.RADIANS:
                    ConeHeight = Mathf.Cos(Angle) * HypothenusDistance;

                    x = (ConeHeight * Mathf.Cos((theta)));
                    y = (ConeHeight * Mathf.Sin((theta)));
                    z = 0f;
                    ConeCenterPoint = new Vector3(x, y, z);
                    break;
            }
            return ConeCenterPoint;
        }


        public void ChangeWobbleOpacity(float value)
        {
            if (!CloudSelector.instance.noSelection)
            {
                CloudData data = LoadCurrentStatus();

                if (data.wobbleObject)
                {
                    MeshRenderer WobbleRenderer = data.wobbleObject.GetComponent<MeshRenderer>();
                    //MeshRenderer PointsRenderer = data.GetComponent<MeshRenderer>();
                    WobbleRenderer.material.SetColor("_Color", new Color(1f,1f,1f,value));
                }
            }
        }

        public void ChangeWobbleColorVariable(int value)
        {
            if (value == 0)
            {
                cone_color_variable = OrientationColor.ORIENTATION_ANGLE;
            }
            else if (value == 1)
            {
                cone_color_variable = OrientationColor.WOBBLE_ANGLE;
            }
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
            if (ZMQSyncCom != null)
            {
                ZMQSyncCom.ChangeDefaultAlphaValue(value);
            }
        }

        public void SwitchZMQSyncComDefaultMode(bool value)
        {
            if (ZMQSyncCom != null)
            {
                ZMQSyncCom.SwitchTrajectoryParameter(value);
            }
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
            #region RandomThreshold
            if (randomthresholdJobON == true)
            {
                if (RandomThresholdThreadQueue.Count != 0)
                {
                    RandomThresholdThreadHandler thread = RandomThresholdThreadQueue.Peek();
                    if (!thread.isRunning)
                    {
                        thread = RandomThresholdThreadQueue.Dequeue();
                        Mesh mesh = thread.currcloud.GetComponent<MeshFilter>().mesh;

                        mesh.uv3 = thread.uv2List;
                        Debug.Log("point limit number : "+thread.point_limit_number.ToString());
                        Debug.Log("point table count : "+thread.currcloud.pointDataTable.Count.ToString());

                        thread.currcloud.gameObject.GetComponent<MeshFilter>().mesh = mesh;
                        thread.StopThread();
                    }
                }
                else
                {
                    randomthresholdJobON = false;
                }
            }


            #endregion
            #region Sliding Window
            if (slidingwindowJobON == true)
            {
                if (SlidingWindowThreadQueue.Count != 0)
                {
                    SlidingWindowThreadHandler thread = SlidingWindowThreadQueue.Peek();
                    if (!thread.isRunning)
                    {
                        thread = SlidingWindowThreadQueue.Dequeue();
                        Mesh mesh = thread.currcloud.GetComponent<MeshFilter>().mesh;

                        mesh.uv3 = thread.uv2List;

                        thread.currcloud.gameObject.GetComponent<MeshFilter>().mesh = mesh;
                        thread.StopThread();
                    }
                }
                else
                {
                    slidingwindowJobON = false;
                }
            }

            #endregion
            #region TrajectoryAnimation
            if (TrajectoryAnimationRunning)
            {
                CloudData data = LoadCurrentStatus();
                float currentTimeIndex = data.globalMetaData.upperframeLimit;
                float minTimeIndex = data.globalMetaData.lowerframeLimit;
                currentTimeIndex += TrajectoryAnimationSpeed * Time.deltaTime;
                minTimeIndex += TrajectoryAnimationSpeed * Time.deltaTime;
                if (currentTimeIndex > data.globalMetaData.timeList.Count - 1)
                {
                    float difference = currentTimeIndex - minTimeIndex;
                    currentTimeIndex = 0f;
                    minTimeIndex = 0f - difference;
                }
                SetShaderFrame(minTimeIndex, currentTimeIndex);
                //data.globalMetaData.lowerframeLimit = minframe;

            }
            #endregion
        }
        #endregion
    }

    #region Threshold Thread  Class
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

            currcloud.globalMetaData.kept_points_id_list.Clear();

            foreach (KeyValuePair<int, PointData> kvp in currcloud.pointDataTable)
            {
                //Debug.Log(kvp.Value.time);
                currcloud.pointMetaDataTable[kvp.Key].isHidden = false;
                uv2List[kvp.Key].y = 0f;

                for (int i = 0; i < currcloud.columnData.Count; i++)
                {
                    if (currcloud.globalMetaData.HideNaNValues == true && float.IsNaN(currcloud.columnData[i][kvp.Key]))
                    {
                        currcloud.pointMetaDataTable[kvp.Key].isHidden = true;


                        uv2List[kvp.Key].y = 1f;

                    }

                    if (currcloud.columnData[i][kvp.Key] < currcloud.globalMetaData.columnMetaDataList[i].MinThreshold || currcloud.columnData[i][kvp.Key] > currcloud.globalMetaData.columnMetaDataList[i].MaxThreshold)
                    {
                        currcloud.pointMetaDataTable[kvp.Key].isHidden = true;
                        

                        uv2List[kvp.Key].y = 1f;
                        break;
                    }

                }




                if (currcloud.pointMetaDataTable[kvp.Key].isHidden == false)
                {
                    currcloud.globalMetaData.kept_points_id_list.Add(kvp.Key);
                }
            }

            //Debug.Log(counter);
            isRunning = false;
        }
    }

    public class RandomThresholdThreadHandler : RunnableThread
    {
        public CloudData currcloud;

        public List<Vector3> vertices;
        public List<int> indices;
        public Vector2[] uv2List;
        public HashSet<int> DuplicatesNumberSet;
        public System.Random rnd;
        public float point_limit_percentage;
        public int point_limit_number;
        
        public RandomThresholdThreadHandler()
        {
            point_limit_number = 0;
        }

        protected override void Run()
        {
            if (point_limit_percentage == 0)
            {
                foreach (KeyValuePair<int, PointData> kvp in currcloud.pointDataTable)
                {
                    //Debug.Log(kvp.Value.time);
                    currcloud.pointMetaDataTable[kvp.Key].isHidden = true;
                    uv2List[kvp.Key].y = 1f;
                }
            }
            else
            {
                if(currcloud.globalMetaData.kept_points_id_list.Count == 0)
                {
                    foreach (KeyValuePair<int, PointData> kvp in currcloud.pointDataTable)
                    {

                        //Debug.Log(kvp.Value.time);
                        currcloud.pointMetaDataTable[kvp.Key].isHidden = false;
                        uv2List[kvp.Key].y = 0f;
                    }

                }
                else
                {
                    foreach (int i in currcloud.globalMetaData.kept_points_id_list)
                    {

                        //Debug.Log(kvp.Value.time);
                        currcloud.pointMetaDataTable[i].isHidden = false;
                        uv2List[i].y = 0f;
                    }
                }
            }


            if (point_limit_percentage != 100.0f && point_limit_percentage != 0.0f)
            {

                /**
                //generate X random entries that don't repeat, add the hidden points list to the list of forbidden entries.
                if (point_limit_percentage < 50)
                {
                    foreach (KeyValuePair<int, PointData> kvp in currcloud.pointDataTable)
                    {
                        currcloud.pointMetaDataTable[kvp.Key].isHidden = true;
                        uv2List[kvp.Key].y = 1f;
                    }
                    CalculatePointsToKeep();
                }
                else
                {
                **/

                CalculatePointsToHide();
                

  
            }
            isRunning = false;
        }


        //case when the percentage of points to show is low 
        private void CalculatePointsToKeep()
        {
            //point_limit_number = currcloud.pointDataTable.Count * (point_limit_percentage / 100);

            GenerateRandomPointsKept();

        }


        //case when the percentage of points to show is high, we calculate the inverse percentage of points to hide to save on random gen resources 
        private void CalculatePointsToHide()
        {
            if (currcloud.globalMetaData.kept_points_id_list.Count == 0)
            {
                float percentage = 1 - (point_limit_percentage / 100f);
                float nbr = currcloud.pointDataTable.Count * percentage;
                point_limit_number = Mathf.RoundToInt(nbr);
            }
            else
            {
                float percentage = 1 - (point_limit_percentage / 100f);
                float nbr = currcloud.globalMetaData.kept_points_id_list.Count * percentage;
                point_limit_number = Mathf.RoundToInt(nbr);

            }

            GenerateRandomPointsHidden();
        }


        private void GenerateRandomPointsHidden()
        {
            //HashSet<int> DuplicatesNumberSet = new HashSet<int>();

            if (currcloud.globalMetaData.kept_points_id_list.Count == 0)
            {

                for (int i = 0; i < point_limit_number; i++)
                {

                    int randomNumber = RecursiveNumberGeneration(DuplicatesNumberSet, currcloud.pointDataTable.Count - 1);
                    //int randomNumber = rnd.Next(0, currcloud.pointDataTable.Count);
                    DuplicatesNumberSet.Add(randomNumber);
                    currcloud.pointMetaDataTable[randomNumber].isHidden = true;
                    uv2List[randomNumber].y = 1f;


                }
            }
            else
            {
                for (int i = 0; i < point_limit_number; i++)
                {

                    int randomNumber = RecursiveNumberGeneration(DuplicatesNumberSet, currcloud.globalMetaData.kept_points_id_list.Count - 1);
                    //int randomNumber = rnd.Next(0, currcloud.pointDataTable.Count);
                    DuplicatesNumberSet.Add(randomNumber);
                    currcloud.pointMetaDataTable[currcloud.globalMetaData.kept_points_id_list[randomNumber]].isHidden = true;
                    uv2List[currcloud.globalMetaData.kept_points_id_list[randomNumber]].y = 1f;


                }

            }
        }

        private void GenerateRandomPointsKept()
        {
            //HashSet<int> DuplicatesNumberSet = new HashSet<int>();


            foreach (int i in currcloud.globalMetaData.kept_points_id_list)
            {
                DuplicatesNumberSet.Add(i);
            }

            for (int i = 0; i < point_limit_number; i++)
            {
                int randomNumber = rnd.Next(0, currcloud.pointDataTable.Count);//RecursiveNumberGeneration(DuplicatesNumberSet);
                DuplicatesNumberSet.Add(randomNumber);
                currcloud.pointMetaDataTable[randomNumber].isHidden = false;
                uv2List[randomNumber].y = 0f;               
            }
        }

        private int RecursiveNumberGeneration(HashSet<int> DuplicatesNumberSet, int maxNumberValue)
        {
            int result;
            //System.Random rnd = new System.Random();
            int randomNumber = rnd.Next(0, maxNumberValue);//UnityEngine.Random.Range(0, currcloud.pointDataTable.Count - 1);

            if (DuplicatesNumberSet.Contains(randomNumber))
            {
                return RecursiveNumberGeneration(DuplicatesNumberSet, maxNumberValue);
            }
            else
            {
                result = randomNumber;
                return result;
            }

        }
    }

    #endregion

    #region Sliding Window
    public class SlidingWindowThreadHandler : RunnableThread
    {
        public CloudData currcloud;

        public List<Vector3> vertices;
        public List<int> indices;
        public Vector2[] uv2List;

        public int ColumnID;
        public float WindowLimitUp;
        public float WindowLimitDown;

        public SlidingWindowThreadHandler()
        {

        }

        protected override void Run()
        {

            //currcloud.globalMetaData.kept_points_id_list.Clear();

            foreach (KeyValuePair<int, PointData> kvp in currcloud.pointDataTable)
            {
                //Debug.Log(kvp.Value.time);
                //currcloud.pointMetaDataTable[kvp.Key].isHidden = false;
                uv2List[kvp.Key].y = 0f;
                if (currcloud.columnData[ColumnID][kvp.Key] < WindowLimitDown && currcloud.columnData[ColumnID][kvp.Key] > WindowLimitUp)
                {
                    //currcloud.pointMetaDataTable[kvp.Key].isHidden = true;
                    uv2List[kvp.Key].y = 1f;
                }
                

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


                //Debug.Log(currentCloud.globalMetaData.pointbyLocationList.Count);
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
            Debug.Log("Selected Points : " + FinalList.Count);
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