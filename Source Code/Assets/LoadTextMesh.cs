using Data;
using Display;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using SFB;
using GK;
using DesktopInterface;

public class LoadTextMesh : MonoBehaviour
{
    public Material material;

    public void LoadFile()
    {
        var extensions = new[]
        {
            new ExtensionFilter("text format", "txt"),
            new ExtensionFilter("All Files", "*" )
        };
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensions, true, (string[] paths) => { LoadMesh(paths); });

    }

    private void LoadMesh(string[] paths)
    {
        List<float> CellIDList = new List<float>();
        List<float> XList = new List<float>();
        List<float> YList = new List<float>();
        List<float> ZList = new List<float>();
        Dictionary<float, List<Vector3>> VertexbyCellList = new Dictionary<float, List<Vector3>>();
        string filepath = paths[0];
        string[] lines = File.ReadAllLines(filepath);

        int columnNumber = lines[0].Split('\t').Length;
        Debug.Log(columnNumber);
        string[] indices_entries = lines[0].Split('\t');
        string[] X_entries = lines[1].Split('\t');
        string[] Y_entries = lines[2].Split('\t');
        string[] Z_entries = lines[3].Split('\t');

        for (int i = 0; i < columnNumber; i++)
        {
            float parsednumber;
            bool ParseSuccess = Single.TryParse(indices_entries[i], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out parsednumber);
            if (ParseSuccess)
            {
                CellIDList.Add(parsednumber);
            }
        }
        float xMax = Mathf.NegativeInfinity;
        float xMin = Mathf.Infinity;
        float yMax = Mathf.NegativeInfinity;
        float yMin = Mathf.Infinity;
        float zMax = Mathf.NegativeInfinity;
        float zMin = Mathf.Infinity;

        for (int i = 0; i < columnNumber; i++)
        {
            //float x, y, z;
            float parsednumber1;
            bool ParseSuccess = Single.TryParse(X_entries[i], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out parsednumber1);
            if (ParseSuccess)
            {
                if (parsednumber1 < xMin) { xMin = parsednumber1; }
                if (parsednumber1 > xMax) { xMax = parsednumber1; }
                XList.Add(parsednumber1);//CellIDList.Add(parsednumber);
            }
            float parsednumber2;
            bool ParseSuccess2 = Single.TryParse(Y_entries[i], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out parsednumber2);
            if (ParseSuccess2)
            {
                if (parsednumber2 < yMin) { yMin = parsednumber2; }
                if (parsednumber2 > yMax) { yMax = parsednumber2; }

                YList.Add(parsednumber2);//CellIDList.Add(parsednumber);
            }
            float parsednumber3;
            bool ParseSuccess3 = Single.TryParse(Z_entries[i], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out parsednumber3);
            if (ParseSuccess3)
            {
                if (parsednumber3 < zMin) { zMin = parsednumber3; }
                if (parsednumber3 > zMax) { zMax = parsednumber3; }

                ZList.Add(parsednumber3);//CellIDList.Add(parsednumber);
            }
        }
        float xRange = xMax - xMin;
        float yRange = yMax - yMin;
        float zRange = zMax - zMin;
        float[] rangeList = new float[] { xRange, yRange, zRange };
        float MaxRange = Mathf.Max(rangeList);
        Vector3 offsetVector = new Vector3((xMin + xMax) / 2,
                                   (yMin + yMax) / 2,
                                   (zMin + zMax) / 2);
        Debug.Log("xMax : " + xMax);
        Debug.Log("yMax : " + yMax);
        Debug.Log("zMax : " + zMax);
        Debug.Log("xMin : " + xMin);
        Debug.Log("yMin : " + yMin);
        Debug.Log("zMin : " + zMin);
        Debug.Log("MaxRange : " + MaxRange);
        Debug.Log(offsetVector);
        for(int j = 0; j < XList.Count; j++)
        {
            float Cellid = CellIDList[j];
            Vector3 tempvector = new Vector3(XList[j], YList[j], ZList[j]);
            Vector3 resultVector = (tempvector - offsetVector) / MaxRange;
            //Debug.Log(tempvector+" vs "+ resultVector);
            if (!VertexbyCellList.ContainsKey(Cellid))
            {
                VertexbyCellList.Add(Cellid, new List<Vector3>());
            }
            VertexbyCellList[Cellid].Add(resultVector);
        }

        //var calc = new ConvexHullCalculator();

        foreach (KeyValuePair<float, List<Vector3>> kvp in VertexbyCellList)
        {
            var verts = new List<Vector3>();
            var tris = new List<int>();
            var normals = new List<Vector3>();
            var points = kvp.Value;
            //Debug.Log(kvp.Value.Count + " vertices on Cell nb : " + kvp.Key);
            //calc.GenerateHull(points, true, ref verts, ref tris, ref normals);
            for (int k = 0; k < kvp.Value.Count; k++)
            {
                //Debug.Log(k);
                //Debug.Log("position : " + kvp.Value[k] + " Cell : " + kvp.Key);
                verts.Add(kvp.Value[k]);
                //tris.Add(k); // Counterclockwise

            }
            for (int k = 0; k < kvp.Value.Count; k+=3)
            {
                tris.Add(k);
                tris.Add(k + 1);
                tris.Add(k + 2);

                tris.Add(k + 2);
                tris.Add(k + 1);
                tris.Add(k);
            }
            GameObject go = new GameObject();
            MeshFilter Mfilter = go.AddComponent<MeshFilter>();
            MeshRenderer Mrenderer = go.AddComponent<MeshRenderer>();

            var mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            //mesh.SetNormals(normals);

            //material.SetColor("_Color", Color.Lerp(Color.red, Color.green, (c.ID+1) / ClusterList.Count));
            Mfilter.mesh = mesh;
            Mrenderer.material = material;
            ColorMap map = ColorMapManager.instance.GetColorMap("jet");
            Mrenderer.material.SetColor("_Color", map.texture.GetPixel(Mathf.RoundToInt((kvp.Key / VertexbyCellList.Count) * 256), 1));
            //Mrenderer.material.SetColor("_Color", Color.Lerp(new Color(1,0,0,0.45f), new Color(0, 1, 0, 0.45f), (float)(c.ID+1) / ClusterList.Count));
            go.AddComponent<DragMouse>();
            //Debug.Log("Hull Created");

        }

    }
}
