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
using System.Runtime;
using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Data;
using VR_Interaction;
using VR_Interaction.Convex_Hull;

namespace IO
{
    [Serializable]
    public class HistogramSaveable
    {
        public List<float> HistogramXValues;
        public List<float> HistogramYValues;
    }

    [Serializable]
    public class CloudSaveableData
    {
        /**
        public List<float[]> collumnData;
        public Dictionary<int, PointData> pointDataTable;
        
        public Dictionary<int, PointMetaData> pointMetaDataTable;
            **/

        #region Global Cloud Metadata

        public int[] displayCollumnsConfiguration;

        public string colormapName;
        public bool colormapReversed;
        public Dictionary<int, float> currentColormapVariable;
        public Vector2 currentNormedVariable;

        public float pointSize;
        public Vector3 scale;
        public string fileName;
        public Dictionary<int, List<int>> pointsByLocationDict;

        #region VR Tools
        //Counter
        public List<int> CounterContainerIDList; // remember ids since dict aren't iterated in order.
        public List<Vector3> CounterContainerPositions; // positions of the containers
        public List<int> CounterObjectsNumberList; // number of objects per containers   
        public List<int> CounterObjectIDList; // number associated to each object back to back
        public List<Vector3> CounterObjectsPositions; // positions of all the objects back to back


        //Ruler
        public List<Vector3> RulerContainerPositionsList;
        public List<int> RulerContainerIDList;
        public List<int> RulerObjectsNumberList;
        public List<Vector3> RulerObjectPositionsList;
        public List<float> RulerObjectDistancesList;
        public List<int> RulerObjectIDList;
        public List<int> RulerMeshIndicesNumberPerContainerList;
        public List<int> RulerMeshIndices;
        //Consider the need for ordered points to be able to link the mesh indices
        /**
    public List<Vector3> RulerContainerPosition;
    public List<List<float>> RulerDistanceList;
    public List<List<Vector3>> RulerPointsPositions;
    public List<int[]> RulerMeshIndices;
    **/

        //Sphere
        public List<Vector3> SpheresContainerPositionList;
        public List<int> SpheresContainerIDList;
        public List<float> RadiusList;

        //Convex Hull
        public List<Vector3> ConvexHullContainersPositionsList;
        public List<int> ConvexHullContainerIDList;
        public List<int> ConvexHullObjectNumberList;

        public List<Vector3> ConvexHullObjectPositionsList;
        public List<int> ConvexHullObjectIDList;

        public List<int> ConvexHullMeshIndicesNumberPerContainerList;
        public List<int> ConvexHullHullMeshIndices;

        //TODO
        //Angle Measure
        public List<Vector3> AngleMeasureContainersPositionsList;
        public List<int> AngleMeasureContainersIDList;
        public List<int> AngleMeasureObjectNumberList;

        public List<Vector3> AnglemeasureObjectsPositionsList;
        public List<int> AngleMeasureObjectsIDList;


        //Histograms
        public List<int> HistogramContainersIDList;
        public List<Vector3> HistogramContainersPositionsList;
        public List<Vector3> HistogramPositionList;
        public List<Vector3> HistogramRotationList;
        public List<Vector3> HistogramBaseCirclesPositionsList; // Contains positions for the baseCircle and secondCircle objects.
        public List<Vector3> HistogramSecondCirclesPositionsList;

        //public List<Quaternion> HistogramCircleRotationsList;
        public List<float> HistogramRadiusList;
        public List<int> HistogramSectionNumbersList;

        public List<HistogramSaveable> histogramsaveList;


        #endregion
        #endregion

        public CloudSaveableData(CloudData data)
        {
            /**
            this.collumnData = data.collumnData;
            this.pointDataTable = data.pointDataTable;
            
            this.pointMetaDataTable = data.pointMetaDataTable;
            **/

            this.displayCollumnsConfiguration = data.globalMetaData.displayCollumnsConfiguration;
            this.colormapName = data.globalMetaData.colormapName;
            this.colormapReversed = data.globalMetaData.colormapReversed;
            this.currentColormapVariable = data.globalMetaData.current_colormap_variable;
            this.currentNormedVariable = data.globalMetaData.current_normed_variable;
            this.pointSize = data.globalMetaData.point_size;
            this.scale = data.globalMetaData.scale;
            this.fileName = data.globalMetaData.fileName;
            this.pointsByLocationDict = data.globalMetaData.pointbyLocationList;

            Matrix4x4 worldtolocal = data.transform.worldToLocalMatrix;

            CounterContainerIDList = new List<int>();
            CounterContainerPositions = new List<Vector3>();
            CounterObjectsPositions = new List<Vector3>();
            CounterObjectsNumberList = new List<int>();
            CounterObjectIDList = new List<int>();

            if (data.globalMetaData.counterPointsList.Count != 0)
            {
                foreach (var obj in data.globalMetaData.counterPointsList)
                {
                    VRContainerCounter containerscript = obj.Value.GetComponent<VRContainerCounter>();
                    CounterContainerIDList.Add(containerscript.id);
                    CounterContainerPositions.Add(worldtolocal.MultiplyPoint3x4(obj.Value.transform.position));
                    CounterObjectsNumberList.Add(containerscript.GetObjectCount());

                    for (int i = 0; i < obj.Value.transform.childCount; i++)
                    {
                        CounterObjectsPositions.Add(obj.Value.transform.GetChild(i).transform.localPosition);
                        CounterObjectIDList.Add(obj.Value.transform.GetChild(i).GetComponent<VRObjectCounter>().id);
                    }
                }

            }
            RulerContainerPositionsList = new List<Vector3>();
            RulerContainerIDList = new List<int>();
            RulerObjectsNumberList = new List<int>();
            RulerObjectPositionsList = new List<Vector3>();
            RulerObjectDistancesList = new List<float>();
            RulerObjectIDList = new List<int>();
            RulerMeshIndicesNumberPerContainerList = new List<int>();
            RulerMeshIndices = new List<int>();
            int j = 0;
            if (data.globalMetaData.rulerPointsList.Count != 0)
            {
                foreach (var obj in data.globalMetaData.rulerPointsList)
                {
                    VRContainerRuler containerscript = obj.Value.GetComponent<VRContainerRuler>();
                    RulerContainerIDList.Add(containerscript.id);
                    RulerContainerPositionsList.Add(worldtolocal.MultiplyPoint3x4(obj.Value.transform.position));
                    RulerObjectsNumberList.Add(containerscript.GetObjectCount());
                    for (int i = 0; i < obj.Value.transform.childCount; i++)
                    {
                        RulerObjectPositionsList.Add(obj.Value.transform.GetChild(i).transform.localPosition);
                        RulerObjectIDList.Add(obj.Value.transform.GetChild(i).GetComponent<VRObjectMeasure>().id);
                        //RulerObjectDistancesList.Add(Single.Parse(obj.Value.transform.GetChild(i).GetComponent<VRObjectMeasure>().distance_to_display, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture));
                    }
                    int[] indices = containerscript.GetComponent<MeshFilter>().mesh.GetIndices(0);
                    RulerMeshIndicesNumberPerContainerList.Add(indices.Length);
                    for (int i = 0; i < indices.Length; i++)
                    {
                        RulerMeshIndices.Add(indices[i]);
                    }
                }

                //distanceLists[distanceLists.Count] = data.globalMetaData.rulerPointsDistanceList;
            }


            SpheresContainerPositionList = new List<Vector3>();
            RadiusList = new List<float>();
            SpheresContainerIDList = new List<int>();

            if (data.globalMetaData.sphereList.Count != 0)
            {
                foreach (var obj in data.globalMetaData.sphereList)
                {
                    SpheresContainerPositionList.Add(worldtolocal.MultiplyPoint3x4(obj.Value.transform.position));
                    RadiusList.Add(obj.Value.transform.GetChild(0).GetComponent<PointSelectorSphere>().radius);
                    SpheresContainerIDList.Add(obj.Value.GetComponent<VRContainerSelectionSphere>().id);
                }
            }


            ConvexHullContainersPositionsList = new List<Vector3>();
            ConvexHullContainerIDList = new List<int>();
            ConvexHullObjectNumberList = new List<int>();

            ConvexHullObjectPositionsList = new List<Vector3>();
            ConvexHullObjectIDList = new List<int>();

            ConvexHullMeshIndicesNumberPerContainerList = new List<int>();
            ConvexHullHullMeshIndices = new List<int>();

            if (data.globalMetaData.convexHullsList.Count != 0)
            {
                foreach (var obj in data.globalMetaData.convexHullsList)
                {
                    VRContainerConvexHull containerscript = obj.Value.GetComponent<VRContainerConvexHull>();
                    ConvexHullContainerIDList.Add(containerscript.id);
                    ConvexHullContainersPositionsList.Add(worldtolocal.MultiplyPoint3x4(obj.Value.transform.position));
                    PointSelectorConvexHull selectionscript = obj.Value.GetComponent<PointSelectorConvexHull>();
                    ConvexHullObjectNumberList.Add(selectionscript.hullPointList.Count);
                    foreach (var child in selectionscript.hullPointList)
                    {
                        ConvexHullObjectPositionsList.Add(child.transform.localPosition);
                        ConvexHullObjectIDList.Add(child.GetComponent<VRObjectConvexHull>().id);

                    }
                    int[] indices = containerscript.GetComponent<MeshFilter>().mesh.GetIndices(0);
                    ConvexHullMeshIndicesNumberPerContainerList.Add(indices.Length);
                    for (int i = 0; i < indices.Length; i++)
                    {
                        ConvexHullHullMeshIndices.Add(indices[i]);
                    }
                }
            }

            AngleMeasureContainersPositionsList = new List<Vector3>();
            AngleMeasureContainersIDList = new List<int>();
            AngleMeasureObjectNumberList = new List<int>();

            AnglemeasureObjectsPositionsList = new List<Vector3>();
            AngleMeasureObjectsIDList = new List<int>();

            if (data.globalMetaData.angleMeasurementsList.Count != 0)
            {
                foreach (var obj in data.globalMetaData.angleMeasurementsList)
                {
                    VRContainerAngleMeasurement containerscript = obj.Value.GetComponent<VRContainerAngleMeasurement>();
                    AngleMeasureContainersIDList.Add(containerscript.id);
                    AngleMeasureContainersPositionsList.Add(worldtolocal.MultiplyPoint3x4(obj.Value.transform.position));
                    AngleMeasureObjectNumberList.Add(containerscript.GetObjectCount());
                    for (int i = 0; i < obj.Value.transform.childCount; i++)
                    {
                        if (i <= 3)
                        {
                            AnglemeasureObjectsPositionsList.Add(obj.Value.transform.GetChild(i).transform.localPosition);
                            AngleMeasureObjectsIDList.Add(obj.Value.transform.GetChild(i).GetComponent<VRObjectAngleCylinder>().id);

                        }
                    }

                }
            }

            HistogramContainersIDList = new List<int>();
            HistogramContainersPositionsList = new List<Vector3>();
            HistogramBaseCirclesPositionsList = new List<Vector3>();
            HistogramSecondCirclesPositionsList = new List<Vector3>();
            HistogramPositionList = new List<Vector3>();
            HistogramRotationList = new List<Vector3>();
            HistogramRadiusList = new List<float>();
            HistogramSectionNumbersList = new List<int>();

            histogramsaveList = new List<HistogramSaveable>();

            if (data.globalMetaData.histogramList.Count != 0)
            {
                foreach (var obj in data.globalMetaData.histogramList)
                {
                    VRContainerHistogram containerscript = obj.Value.GetComponent<VRContainerHistogram>();
                    HistogramContainersIDList.Add(containerscript.id);
                    HistogramContainersPositionsList.Add(worldtolocal.MultiplyPoint3x4(obj.Value.transform.position));
                    HistogramPositionList.Add(containerscript.transform.GetChild(0).localPosition);
                    HistogramRotationList.Add(containerscript.transform.GetChild(0).rotation.eulerAngles);
                    HistogramBaseCirclesPositionsList.Add(containerscript.transform.GetChild(0).GetChild(0).localPosition);
                    HistogramSecondCirclesPositionsList.Add(containerscript.transform.GetChild(0).GetChild(1).localPosition);
                    HistogramRadiusList.Add(containerscript.transform.GetChild(0).GetChild(0).localScale.x);
                    HistogramSectionNumbersList.Add(containerscript.gameObject.GetComponent<HistogramPointSelector>().sectionsNumber);

                    HistogramSaveable hsave = new HistogramSaveable();
                    hsave.HistogramXValues = containerscript.gameObject.GetComponent<HistogramPointSelector>().xValues;
                    hsave.HistogramYValues = containerscript.gameObject.GetComponent<HistogramPointSelector>().xValues;
                    histogramsaveList.Add(hsave);
                }
            }
        }
    }
}