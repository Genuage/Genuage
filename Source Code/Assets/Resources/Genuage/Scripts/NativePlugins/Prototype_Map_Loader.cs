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


using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using GK;
using DesktopInterface;
using HullDelaunayVoronoi;
using HullDelaunayVoronoi.Primitives;
using SFB;

public unsafe class Prototype_Map_Loader : NativePlugin
{
    const string DLLName = "K-Means Clustering Native DLL";
    //const string DLLName = "Matlab Kmeans DLL";

    [DllImport(DLLName)]

    private static extern void Execute(int point_number, void* x_coordinates, void* y_coordinates, void* z_coordinates,
    void* colors, void* times, void* trajectory_numbers, void* phi_angle,
    void* theta_angle, void* point_sizes,
    int cluster_number, int iteration_number,
    float* results, float* centroids_x, float* centroids_y, float* centroids_z);
    
    public int clusternumber = 50;
    
    public int iterations = 20;

    [DllImport("Infer3DPlugin")]
    private static extern void Infer3D(int headerID, int paramID, double sigma, double sigmaxy, double sigmaz,
                                   int NumberOfPoints, void* TrajectoryNumber, void* xCoordinates, void* yCoordinates,
                                   void* zCoordinates, void* TimeStamp, double* Diffusion, double* ForceX, double* ForceY, double* ForceZ);

    //Cluster Variables
    private List<Cluster> ClusterList;
    public float minClusterValue = Mathf.Infinity;
    public float maxClusterValue = Mathf.NegativeInfinity;
    public Material material;

    //public static Type mlType;
    //public static object matlab;
    //public ExampleVoronoi3D voroscript;
    public bool alreadyran = false;

    public GameObject lineparent;
    public GameObject solidparent;
    public GameObject arrowparent;

    //UI
    public InputField clusternbrInput;
    public InputField iterationnbrInput;

    public Button ShowHideLinesButton;
    public Button ShowHideSolidsButton;
    public Button ResetButton;

    public Button OpacityApplyButton;
    public Slider LinesOpacitySlider;
    public Slider SolidsOpacitySlider;

    public ColorMapLoader MapLoader;
    public string loadedmapname = "jet";
    //Infer3D UI
    public Dropdown headerDropdown;
    public Dropdown paramDropdown;
    public InputField SigmaInput;
    public InputField SigmaxyInput;
    public InputField SigmazInput;
    public Button ExportButton;


    private void Awake()
    {
        CloudSelector.instance.OnSelectionChange+=ResetAlgo;
        ExportButton.onClick.AddListener(delegate { ExportData(); });
    }
    protected override void LaunchPluginFunction(CloudData data)
    {
        //mlType = Type.GetTypeFromProgID("Matlab.Application");
        //matlab = Activator.CreateInstance(mlType);
        if (alreadyran)
        {
            Destroy(lineparent);
            Destroy(solidparent);
        }
        List<float> TrajectoriesList = new List<float>();
        List<float> Xvalues = new List<float>();
        List<float> Yvalues = new List<float>();
        List<float> Zvalues = new List<float>();
        List<float> Cvalues = new List<float>();
        List<float> Tvalues = new List<float>();
        List<float> PHvalues = new List<float>();
        List<float> THvalues = new List<float>();
        List<float> Svalues = new List<float>();
        int pointnumber = 0;

        //selectionSent = false;
        UnityEngine.Debug.Log("Sending all points");
        foreach (var i in data.pointDataTable)
        {
            TrajectoriesList.Add(i.Value.trajectory);
            Xvalues.Add(i.Value.normed_position.x);
            Yvalues.Add(i.Value.normed_position.y);
            Zvalues.Add(i.Value.normed_position.z);
            Cvalues.Add(i.Value._color_index);
            Tvalues.Add(i.Value.time);
            PHvalues.Add(i.Value.phi_angle);
            THvalues.Add(i.Value.theta_angle);
            Svalues.Add(i.Value.size);
            pointnumber++;

        }

        float[] XvaluesArray = Xvalues.ToArray();
        float[] YvaluesArray = Yvalues.ToArray();
        float[] ZvaluesArray = Zvalues.ToArray();
        float[] CvaluesArray = Cvalues.ToArray();
        float[] TvaluesArray = Tvalues.ToArray();
        float[] TrajectoriesArray = TrajectoriesList.ToArray();
        float[] PHvaluesArray = PHvalues.ToArray();
        float[] THvaluesArray = THvalues.ToArray();
        float[] SvaluesArray = Svalues.ToArray();
        int pointNumber = XvaluesArray.Length;
        float[] Results = new float[pointNumber];
        float[] Centroid_Positions_X = new float[clusternumber];
        float[] Centroid_Positions_Y = new float[clusternumber];
        float[] Centroid_Positions_Z = new float[clusternumber];


        //Get Input Values
        int clusterparsednumber;
        bool ParseSuccess = int.TryParse(clusternbrInput.text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out clusterparsednumber);
        if (ParseSuccess)
        {
            clusternumber = clusterparsednumber;
        }
        UnityEngine.Debug.Log(ParseSuccess);
        int iterationparsednumber;
        bool ParseSuccess2 = int.TryParse(iterationnbrInput.text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out iterationparsednumber);
        if (ParseSuccess2)
        {
            iterations = iterationparsednumber;
        }
        UnityEngine.Debug.Log(ParseSuccess2);
        fixed (float* Trajectoriespointer = TrajectoriesArray, xvalues = XvaluesArray, yvalues = YvaluesArray,
       zvalues = ZvaluesArray, cvalues = CvaluesArray, tvalues = TvaluesArray, thvalues = THvaluesArray,
       phvalues = PHvaluesArray, svalues = SvaluesArray, results = Results,
       centroids_x = Centroid_Positions_X, centroids_y = Centroid_Positions_Y, centroids_z = Centroid_Positions_Z)
        {
            //Vertex3[] vorovertices = new Vertex3[clusternumber];

            if (!alreadyran)
            {

                
                Execute(pointNumber, xvalues, yvalues, zvalues, cvalues, tvalues,
                        Trajectoriespointer, phvalues, thvalues, svalues,
                        clusternumber, iterations, results, centroids_x, centroids_y, centroids_z);
                
                /**
                for (int i = 0; i < clusternumber; i++)
                {
                    vorovertices[i] = new Vertex3(Centroid_Positions_X[i], Centroid_Positions_Y[i], Centroid_Positions_Z[i]);
                    UnityEngine.Debug.Log("Centroid of cluster ; " + i + " -s x : " + Centroid_Positions_X[i] + " - y :" + Centroid_Positions_Y[i] + " - z :" + Centroid_Positions_Z[i]);
                }
                **/

                //DEBUG MATLAB
                /**
                string[] lines = File.ReadAllLines("E:/Documents/Genuage/Matlab JB latest/beadsclusters.txt");
                int N = lines.Length;
                float[] newColumn = new float[N];

                if (data.pointDataTable.Count == N)
                {
                    //float[] idcolumn = new float[N];

                    for (int i = 0; i < N; i++)
                    {
                        string[] entries = lines[i].Split('\t');

                        //idcolumn[i] = Single.Parse(entries[0], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                        newColumn[i] = Single.Parse(entries[0], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);

                    }

                    float max = Mathf.NegativeInfinity;
                    float min = Mathf.Infinity;

                    for (int i = 0; i < newColumn.Length; i++)
                    {
                        if (newColumn[i] <= min)
                        {
                            min = newColumn[i];
                        }

                        if (newColumn[i] >= max)
                        {
                            max = newColumn[i];
                        }
                    }

                }
                **/
                //END DEBUG
                UnityEngine.Debug.Log("Kmeans Calculated");
                UpdateCloudClusterData(data, Results, clusternumber);
                float[] pointdiffusion = new float[pointNumber];
                CalculateProperty(data, pointdiffusion);
                AddDataColumn(data, pointdiffusion, false);
                
            }
            alreadyran = true;

            ActivateUI();

            UnityEngine.Debug.Log("Property Calculated");
            CreateConvexHull(data);
            /**
            voroscript.NumberOfVertices = clusternumber;
            UnityEngine.Debug.Log(data.globalMetaData.box_scale);
            voroscript.sizex = data.globalMetaData.box_scale.x / 2;
            voroscript.sizey = data.globalMetaData.box_scale.y / 2;
            voroscript.sizez = data.globalMetaData.box_scale.z / 2;
            voroscript.vertices = vorovertices;
            voroscript.Launch();
            **/
            /**
            foreach (Mesh m in voroscript.meshes)
            {
                GameObject go = new GameObject();
                MeshFilter Mfilter = go.AddComponent<MeshFilter>();
                MeshRenderer Mrenderer = go.AddComponent<MeshRenderer>();



                //material.SetColor("_Color", Color.Lerp(Color.red, Color.green, (c.ID+1) / ClusterList.Count));
                Mfilter.mesh = m;
                Mrenderer.material = material;
                ColorMap map = ColorMapManager.instance.GetColorMap("jet");
                Mrenderer.material.SetColor("_Color", map.texture.GetPixel(Mathf.RoundToInt((float)(UnityEngine.Random.Range(0f, 1.0f) * 256)), 1));
                //Mrenderer.material.SetColor("_Color", Color.Lerp(new Color(1,0,0,0.45f), new Color(0, 1, 0, 0.45f), (float)(c.ID+1) / ClusterList.Count));
                go.AddComponent<DragMouse>();

            }
            **/

        }

    }

    private void UpdateCloudClusterData(CloudData data, float[] resultColumn, int clusterNumber)
    {
        ClusterList = new List<Cluster>();
        for (int i = 0; i < clusterNumber; i++)
        {
            ClusterList.Add(new Cluster(i));
        }

        AddDataColumn(data, resultColumn, true);
        
    }

    private void AddDataColumn (CloudData data, float[] resultColumn, bool fillClusterdata)
    {
        bool checkfull = true;

        for (int i = 0; i < resultColumn.Length; i++)
        {
            if (float.IsNaN(resultColumn[i]))
            {
                UnityEngine.Debug.Log("Result array is not full");
                checkfull = false;
                //Results = new float[0];
            }
        }

        //if checkfull
        if (checkfull)
        {
            float max = Mathf.NegativeInfinity;
            float min = Mathf.Infinity;
            for (int i = 0; i < resultColumn.Length; i++)
            {
                if (resultColumn[i] <= min)
                {
                    min = resultColumn[i];
                }

                if (resultColumn[i] >= max)
                {
                    max = resultColumn[i];
                }
                if (fillClusterdata)
                {
                    int clusterID = (int)resultColumn[i] - 1;
                    if (clusterID >= ClusterList.Count) { UnityEngine.Debug.Log(clusterID); }

                    data.pointMetaDataTable[i].clusterID = clusterID;
                    Assert(data.pointDataTable[i].pointID == i);
                    ClusterList[clusterID].AddPoint(data.pointDataTable[i].pointID);

                }
            }
            float[] restocopy = new float[data.pointDataTable.Count];
            resultColumn.CopyTo(restocopy, 0);
            data.columnData.Add(restocopy);
            ColumnMetadata metadata = new ColumnMetadata();
            metadata.ColumnID = data.columnData.Count - 1;
            metadata.MaxValue = max;
            metadata.MinValue = min;
            metadata.MinThreshold = min;
            metadata.MaxThreshold = max;
            metadata.Range = max - min;
            data.globalMetaData.columnMetaDataList.Add(metadata);
        }
    }
    private void CalculateProperty(CloudData data, float[] PointResults)
    {
        maxClusterValue = Mathf.NegativeInfinity;
        //TODO : FOR ALL POINTS WITHIN A CLUSTER, Calculate diffusion and store it
        foreach (Cluster c in ClusterList)
        {
            float value;
            value = LaunchInfer3D(data, c.pointsIDList, c);
            c.UpdatePropertyValue(value);
            UnityEngine.Debug.Log("Region : " + c.ID + " - " + value);
            if (value >= maxClusterValue) { maxClusterValue = value; }
            if (value <= minClusterValue) { minClusterValue = value; }

            foreach(int i in c.pointsIDList)
            {
                PointResults[data.pointDataTable[i].pointID] = value; 
            }
        }
    }

    private float LaunchInfer3D(CloudData data, List<int> PointIDList, Cluster c)
    {
        List<double> TrajectoriesList = new List<double>();
        List<double> Xvalues = new List<double>();
        List<double> Yvalues = new List<double>();
        List<double> Zvalues = new List<double>();
        List<double> Tvalues = new List<double>();

        int N = 0;
        //Debug.Log("lowtime " + data.globalMetaData.lowertimeLimit);

        //Debug.Log("uptime " + data.globalMetaData.uppertimeLimit);

        float xaverage, yaverage, zaverage;
        float xsum = 0;
        float ysum = 0;
        float zsum = 0;

        int pointnumber = 0;
        foreach (int i in PointIDList)
        {

            if (data.pointMetaDataTable[i].isHidden == false)
            {
                //Debug.Log("time " + data.pointDataTable[j].time);

                if (data.pointDataTable[i].time >= data.globalMetaData.lowertimeLimit && data.pointDataTable[i].time <= data.globalMetaData.uppertimeLimit)
                {
                    //Debug.Log("check");
                    N++;
                    TrajectoriesList.Add((double)data.pointDataTable[i].trajectory);
                    Xvalues.Add((double)data.pointDataTable[i].position.x);
                    Yvalues.Add((double)data.pointDataTable[i].position.y);
                    Zvalues.Add((double)data.pointDataTable[i].position.z);
                    Tvalues.Add((double)data.pointDataTable[i].time);

                    xsum += data.pointDataTable[i].normed_position.x;
                    ysum += data.pointDataTable[i].normed_position.y;
                    zsum += data.pointDataTable[i].normed_position.z;

                    pointnumber++;

                }
            }
        }

        xaverage = xsum / pointnumber;
        yaverage = ysum / pointnumber;
        zaverage = zsum / pointnumber;

        Vector3 ArrowLocalPosition = new Vector3(xaverage, yaverage, zaverage);


        double diffusionres;

        double[] TrajectoriesArray = TrajectoriesList.ToArray();
        double[] XvaluesArray = Xvalues.ToArray();
        double[] YvaluesArray = Yvalues.ToArray();
        double[] ZvaluesArray = Zvalues.ToArray();
        double[] TvaluesArray = Tvalues.ToArray();

        fixed (double* Trajectoriespointer = TrajectoriesArray)
        {
            fixed (double* xvalues = XvaluesArray)
            {
                fixed (double* yvalues = YvaluesArray)
                {
                    fixed (double* zvalues = ZvaluesArray)
                    {
                        fixed (double* tvalues = TvaluesArray)
                        {


                            double diffusion = 145.0;
                            double forceX = 14.0;
                            double forceY = 16.0;
                            double forceZ = 17.0;

                            double* Diffusion = &diffusion;
                            double* ForceX = &forceX;
                            double* ForceY = &forceY;
                            double* ForceZ = &forceZ;

                            //functionID = (FunctionID)dropdown.value;

                            NativePluginInfer3D.HeaderID headerID = (NativePluginInfer3D.HeaderID)headerDropdown.value;
                            NativePluginInfer3D.ParamID paramID = (NativePluginInfer3D.ParamID)paramDropdown.value;
                            // Debug.Log("FunctionID - "+(int)functionID);
                            double Sigma = 0.0;
                            double SigmaXY = 0.0;
                            double SigmaZ = 0.0;

                            double.TryParse(SigmaInput.text, out Sigma);
                            double.TryParse(SigmaxyInput.text, out SigmaXY);
                            double.TryParse(SigmazInput.text, out SigmaZ);


                            //Debug.Log("fid " + functionID);

                            Infer3D((int)headerID, (int)paramID, Sigma, SigmaXY, SigmaZ, N, Trajectoriespointer,
                                    xvalues, yvalues, zvalues, tvalues, Diffusion, ForceX, ForceY, ForceZ);

                            diffusionres = *Diffusion;
                            //Debug.Log(diffusionres);
                            double forceXres = *ForceX;
                            double forceYres = *ForceY;
                            double forceZres = *ForceZ;
                            diffusionres = Math.Round(diffusionres, 3);
                            //Debug.Log(diffusionres);
                            forceXres = Math.Round(forceXres, 3);
                            forceYres = Math.Round(forceYres, 3);
                            forceZres = Math.Round(forceZres, 3);
                            //outValue = (float)diffusionres;
                            if(headerID == NativePluginInfer3D.HeaderID.Diffusion_Velocity)
                            {
                                c.AddArrowPosition(ArrowLocalPosition, new Vector3((float)*ForceX, (float)*ForceY, (float)*ForceZ).normalized);
                            }
                            

                        }
                    }
                }
            }
        }
        return (float)diffusionres;
    }

    private void CreateConvexHull(CloudData data)
    {
        solidparent = new GameObject("SolidParent");
        //solidparent.AddComponent<DragMouse>();
        lineparent = new GameObject("LineParent");
        arrowparent = new GameObject("ArrowsParent");
        //lineparent.AddComponent<DragMouse>();
        solidparent.transform.position = Vector3.zero;
        solidparent.transform.rotation = Quaternion.identity;
        lineparent.transform.position = Vector3.zero;
        lineparent.transform.rotation = Quaternion.identity;
        arrowparent.transform.position = Vector3.zero;
        arrowparent.transform.rotation = Quaternion.identity;
        //Transform t = data.transform;//.parent.GetComponent<CloudObjectRefference>().box.transform;
        data.transform.parent.GetComponent<CloudObjectRefference>().box.transform.position = Vector3.zero;
        data.transform.parent.GetComponent<CloudObjectRefference>().box.transform.rotation = Quaternion.identity;
        Transform t = data.transform;
        solidparent.transform.SetParent(t, true);
        lineparent.transform.SetParent(t, true);
        arrowparent.transform.SetParent(t, true);

        solidparent.transform.localPosition = Vector3.zero;
        solidparent.transform.localRotation = Quaternion.identity;
        lineparent.transform.localPosition = Vector3.zero;
        lineparent.transform.localRotation = Quaternion.identity;
        arrowparent.transform.localPosition = Vector3.zero;
        arrowparent.transform.localRotation = Quaternion.identity;


        //TODO : FOR ALL POINTS WITHIN A CLUSTER, CREATE A CH
        foreach (Cluster c in ClusterList)
        {
            //HashSet<Vector3> knownpointsSet = new HashSet<Vector3>();

            UnityEngine.Debug.Log("Iteration " + c.ID + " starts");
            var calc = new ConvexHullCalculator();

            var verts = new List<Vector3>();
            var tris = new List<int>();
            var normals = new List<Vector3>();
            var points = new List<Vector3>();
            List<Vector3> line_verts = new List<Vector3>();
            List<int> line_indices = new List<int>();
            var line_normals = new List<Vector3>();

            foreach (int pointID in c.pointsIDList)
            {

                points.Add(data.pointDataTable[pointID].normed_position);

            }
            calc.GenerateHull(points, true, ref verts, ref tris, ref normals, ref line_verts, ref line_indices, ref line_normals);

            var mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetNormals(normals);

            Mesh line_mesh = new Mesh();
            line_mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            line_mesh.SetVertices(line_verts);
            line_mesh.SetIndices(line_indices.ToArray(), MeshTopology.Lines, 0);
            //line_mesh.SetNormals(normals);

            GameObject go = new GameObject();
            MeshFilter Mfilter = go.AddComponent<MeshFilter>();
            MeshRenderer Mrenderer = go.AddComponent<MeshRenderer>();
            go.transform.SetParent(solidparent.transform, false);

            GameObject go2 = new GameObject();
            MeshFilter Mfilter2 = go2.AddComponent<MeshFilter>();
            MeshRenderer Mrenderer2 = go2.AddComponent<MeshRenderer>();
            go2.transform.SetParent(lineparent.transform, false);


            //material.SetColor("_Color", Color.Lerp(Color.red, Color.green, (c.ID+1) / ClusterList.Count));
            Mfilter.mesh = mesh;
            Mrenderer.material = material;
            ColorMap map = ColorMapManager.instance.GetColorMap(loadedmapname);

            Mrenderer.material.SetColor("_Color", map.texture.GetPixel(Mathf.RoundToInt((float)(c.propertyValue / maxClusterValue) * 255), 1));
            //Mrenderer.material.SetColor("_Color", Color.Lerp(new Color(1,0,0,0.45f), new Color(0, 1, 0, 0.45f), (float)(c.ID+1) / ClusterList.Count));

            Mfilter2.mesh = line_mesh;
            Mrenderer2.material = material;


            UnityEngine.Debug.Log("Iteration " + c.ID + " ends");

            //Create arrows
            if (c.containsArrow)
            {
                GameObject go3 = new GameObject("arrow");
                go3.transform.position = Vector3.zero;
                go3.transform.rotation = Quaternion.identity;
                go3.transform.SetParent(arrowparent.transform, true);

                //go.transform.SetParent(data.transform);
                UnityEngine.Debug.Log(c.VectorMagnitude);
                go3.AddComponent<GenerateArrow>();
                go3.GetComponent<GenerateArrow>().GenerateSquarePyramidMesh(c.VectorMagnitude);
                go3.transform.localPosition = c.ArrowPosition;

            }
            //Debug.Log("Hull Created");
        }
        data.globalMetaData.mapLineParent = lineparent;
        data.globalMetaData.mapSolidsParent = solidparent;
    }

    private void ChangeMeshColor(string mapname)
    {
        loadedmapname = mapname;
        for (int i = 0; i < solidparent.transform.childCount; i++)
        {
            ColorMap map = ColorMapManager.instance.GetColorMap(mapname);
            MeshRenderer Mrenderer = solidparent.transform.GetChild(i).GetComponent<MeshRenderer>();
            //ColorMap map = ColorMapManager.instance.GetColorMap("jet");
            Mrenderer.material.SetColor("_Color", map.texture.GetPixel(Mathf.RoundToInt((float)(ClusterList[i].propertyValue / maxClusterValue) * 255), 1));
        }
    }

    private void ActivateUI()
    {
        ShowHideLinesButton.onClick.AddListener(ShowLines);
        ShowHideSolidsButton.onClick.AddListener(ShowSolids);
        ResetButton.onClick.AddListener(ResetFromButton);
        OpacityApplyButton.onClick.AddListener(ChangeOpacity);
        MapLoader.OnTextureChange += ChangeMeshColor;
    }

    private void DeActivateUI()
    {
        ShowHideLinesButton.onClick.RemoveListener(ShowLines);
        ShowHideSolidsButton.onClick.RemoveListener(ShowSolids);
        ResetButton.onClick.RemoveListener(ResetFromButton);
        OpacityApplyButton.onClick.RemoveListener(ChangeOpacity);
        MapLoader.OnTextureChange -= ChangeMeshColor;
    }

    private void ResetFromButton()
    {
        ResetAlgo(0);
    }

    private void ResetAlgo(int id)
    {
        if (alreadyran)
        {
            Destroy(lineparent);
            Destroy(solidparent);
            ClusterList.Clear();
            DeActivateUI();
        }
        alreadyran = false;   
    }

    private void ChangeOpacity()
    {
        float lines_opacity = LinesOpacitySlider.value;
        float solids_opacity = SolidsOpacitySlider.value;
        ColorMap map = ColorMapManager.instance.GetColorMap(loadedmapname);

        for (int i = 0; i < lineparent.transform.childCount; i++)
        {
            GameObject g = lineparent.transform.GetChild(i).gameObject;
            MeshRenderer Mrenderer = g.GetComponent<MeshRenderer>();
            Color c = new Color(1,1,1,lines_opacity);
            Mrenderer.material.SetColor("_Color", c);
        }

        for (int i = 0; i < solidparent.transform.childCount; i++)
        {
            GameObject g = solidparent.transform.GetChild(i).gameObject;
            MeshRenderer Mrenderer = g.GetComponent<MeshRenderer>();
            Color c = map.texture.GetPixel(Mathf.RoundToInt((float)(ClusterList[i].propertyValue / maxClusterValue) * 255),1);
            c.a = solids_opacity;
            Mrenderer.material.SetColor("_Color", c);

        }
    }

    private void ShowLines()
    {
        lineparent.SetActive(!lineparent.activeInHierarchy);
    }
    private void ShowSolids()
    {
        solidparent.SetActive(!solidparent.activeInHierarchy);
    }
    protected override void DisplayResults()
    {
        ModalWindowManager.instance.CreateModalWindow("Kmeans Calculated, column added \n Max Diffusion value : "+ maxClusterValue + "\n Min Diffusion value : "+ minClusterValue);
    }

    static void Assert(bool condition)
    {
        if (!condition)
        {
            throw new UnityEngine.Assertions.AssertionException("Assertion failed", "");
        }

    }

    private void ExportData()
    {
        if (alreadyran)
        {


            MapsSaveable mapsave = new MapsSaveable();
            mapsave.ClusterList = new List<ClusterSaveable>();
            foreach (Cluster c in ClusterList)
            {
                ClusterSaveable clustersave = new ClusterSaveable();
                clustersave.ID = c.ID;
                clustersave.pointsIDList = c.pointsIDList;
                clustersave.diffusionCoefficient = c.propertyValue;
                mapsave.ClusterList.Add(clustersave);
            }

            var extensions = new[] {
                new ExtensionFilter("JSON", ".JSON")};
            StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "", extensions, (string path) => { SaveJSON(path, mapsave); });
        }
    }

    private void SaveJSON(string path, MapsSaveable saveable)
    {
        string JSON = JsonUtility.ToJson(saveable);
        string directory = Path.GetDirectoryName(path);
        string filename = Path.GetFileNameWithoutExtension(path);

        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(directory + Path.DirectorySeparatorChar + filename + ".JSON"))
        {
            writer.WriteLine(JSON);
        }
    }
}
public class Cluster
{
    public int ID;
    public List<int> pointsIDList;
    public float propertyValue;
    public bool containsArrow;
    public Vector3 ArrowPosition;
    public Vector3 VectorMagnitude;
    public Cluster(int id)
    {
        this.ID = id;
        pointsIDList = new List<int>();
        this.containsArrow = false;
        //this.pointsID = points;
    }

    public void UpdatePropertyValue(float newValue)
    {
        this.propertyValue = newValue;
    }

    public void AddPoint(int id)
    {
        pointsIDList.Add(id);
    }

    public void UpdatePointIDList(List<int> points)
    {
        this.pointsIDList = points;
    }

    public void AddArrowPosition(Vector3 pos, Vector3 magnitude)
    {
        this.containsArrow = true;
        this.ArrowPosition = pos;
        this.VectorMagnitude = magnitude;
    }
}
public class MapsSaveable
{
    public List<ClusterSaveable> ClusterList;
}

[Serializable]
public class ClusterSaveable
{
    public int ID;
    public List<int> pointsIDList;
    public float diffusionCoefficient;
}