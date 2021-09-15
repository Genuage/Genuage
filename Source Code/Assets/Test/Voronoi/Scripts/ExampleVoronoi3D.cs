using UnityEngine;
using System.Collections.Generic;

using HullDelaunayVoronoi.Voronoi;
using HullDelaunayVoronoi.Delaunay;
using HullDelaunayVoronoi.Hull;
using HullDelaunayVoronoi.Primitives;
using Data;
using DesktopInterface;
using GK;
namespace HullDelaunayVoronoi
{

    public class ExampleVoronoi3D : MonoBehaviour
    {

        public int NumberOfVertices = 100;

        public float sizex = 1;
        public float sizey = 1;
        public float sizez = 1;

        public Vertex3[] vertices;

        public int seed = 0;

        public bool drawLines;

        public Material material;

        private Material lineMaterial;

        private Matrix4x4 rotation = Matrix4x4.identity;

        private float theta;

        private VoronoiMesh3 voronoi;

        public List<Mesh> meshes;

        public void Launch()
        {
            lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));

            //Vertex3[] vertices = new Vertex3[NumberOfVertices];
            /**
            Random.InitState(seed);
            for (int i = 0; i < NumberOfVertices; i++)
            {
                float x = size * Random.Range(-1.0f, 1.0f);
                float y = size * Random.Range(-1.0f, 1.0f);
                float z = size * Random.Range(-1.0f, 1.0f);

                vertices[i] = new Vertex3(x, y, z);
            }
            **/
            voronoi = new VoronoiMesh3();
            voronoi.Generate(vertices);

            RegionsToMeshes();

        }

        private void RegionsToMeshes()
        {
            var calc = new ConvexHullCalculator();

            meshes = new List<Mesh>();
            int cpt = 0;
            Debug.Log(voronoi.Regions.Count);
            foreach (VoronoiRegion<Vertex3> region in voronoi.Regions)
            {
                bool draw = true;

                List<Vertex3> verts = new List<Vertex3>();

                foreach (DelaunayCell<Vertex3> cell in region.Cells)
                {
                    Vertex3 v = new Vertex3(0f,0f,0f);
                    if (!InBound(cell.CircumCenter))
                    {
                        v = cell.CircumCenter;
                        //TODO : Do something more elegant ffs
                        if (cell.CircumCenter.X < -sizex)
                        {
                            v.X = -sizex;
                        }
                        else if (cell.CircumCenter.X > sizex)
                        {
                            v.X = sizex;
                        }
                        if (cell.CircumCenter.Y < -sizey)
                        {
                            v.Y = -sizey;
                        }
                        else if (cell.CircumCenter.Y > sizey)
                        {
                            v.Y = sizey;
                        }
                        if (cell.CircumCenter.Z < -sizez)
                        {
                            v.Z = -sizez;
                        }
                        else if (cell.CircumCenter.Z > sizez)
                        {
                            v.Z = sizez;
                        }
                        verts.Add(v);
                    }
                    else
                    {
                        verts.Add(cell.CircumCenter);
                    }
                }

                if (!draw) continue;

                //If you find the convex hull of the voronoi region it
                //can be used to make a triangle mesh.
                
                ConvexHull3 hull = new ConvexHull3();
                hull.Generate(verts, false);
                

                List<Vector3> positions = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<int> indices = new List<int>();
                var points = new List<Vector3>();

                
                for (int i = 0; i < hull.Simplexs.Count; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Vector3 v = new Vector3();
                        v.x = hull.Simplexs[i].Vertices[j].X;
                        v.y = hull.Simplexs[i].Vertices[j].Y;
                        v.z = hull.Simplexs[i].Vertices[j].Z;

                        positions.Add(v);
                    }

                    Vector3 n = new Vector3();
                    n.x = hull.Simplexs[i].Normal[0];
                    n.y = hull.Simplexs[i].Normal[1];
                    n.z = hull.Simplexs[i].Normal[2];

                    if (hull.Simplexs[i].IsNormalFlipped)
                    {
                        indices.Add(i * 3 + 2);
                        indices.Add(i * 3 + 1);
                        indices.Add(i * 3 + 0);
                    }
                    else
                    {
                        indices.Add(i * 3 + 0);
                        indices.Add(i * 3 + 1);
                        indices.Add(i * 3 + 2);
                    }

                    normals.Add(n);
                    normals.Add(n);
                    normals.Add(n);
                }
                
                Mesh mesh = new Mesh();
                mesh.SetVertices(positions);
                mesh.SetNormals(normals);
                mesh.SetTriangles(indices, 0);
                cpt++;
                Debug.Log("triangles on voronoi region : " + cpt+" - "+(indices.Count/3));
                mesh.RecalculateBounds();
                //mesh.RecalculateNormals();

                meshes.Add(mesh);

            }
            

        }

        private void Update()
        {
            /**
            if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.KeypadMinus))
            {
                theta += (Input.GetKey(KeyCode.KeypadPlus)) ? 0.005f : -0.005f;

                rotation[0, 0] = Mathf.Cos(theta);
                rotation[0, 2] = Mathf.Sin(theta);
                rotation[2, 0] = -Mathf.Sin(theta);
                rotation[2, 2] = Mathf.Cos(theta);
            }
            **/
            /**
            if (meshes != null)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();

                foreach (Mesh mesh in meshes)
                    Graphics.DrawMesh(mesh, rotation, material, 0, Camera.main, 0, block, true, true);
            }
            **/

        }

        private void OnGUI()
        {
            //GUI.Label(new Rect(20, 20, Screen.width, Screen.height), "Numpad +/- to Rotate");
        }

        private void OnPostRender()
        {
            /**
            if (!drawLines) return;

            if (voronoi == null || voronoi.Regions.Count == 0) return;

            GL.PushMatrix();

            GL.LoadIdentity();
            GL.MultMatrix(GetComponent<Camera>().worldToCameraMatrix * rotation);
            GL.LoadProjectionMatrix(GetComponent<Camera>().projectionMatrix);

            lineMaterial.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Color(Color.red);

            foreach (VoronoiRegion<Vertex3> region in voronoi.Regions)
            {
                bool draw = true;

                foreach (DelaunayCell<Vertex3> cell in region.Cells)
                {
                    if (!InBound(cell.CircumCenter))
                    {
                        draw = false;
                        break;
                    }
                }

                if (!draw) continue;

                foreach (VoronoiEdge<Vertex3> edge in region.Edges)
                {
                    Vertex3 v0 = edge.From.CircumCenter;
                    Vertex3 v1 = edge.To.CircumCenter;

                    DrawLine(v0, v1);
                }
            }

            GL.End();
            GL.PopMatrix();
            **/
        }

        private void DrawLine(Vertex3 v0, Vertex3 v1)
        {
            GL.Vertex3(v0.X, v0.Y, v0.Z);
            GL.Vertex3(v1.X, v1.Y, v1.Z);
        }

        private bool InBound(Vertex3 v)
        {
            if (v.X < -sizex || v.X > sizex) return false;
            if (v.Y < -sizey || v.Y > sizey) return false;
            if (v.Z < -sizez || v.Z > sizez) return false;

            return true;
        }

    }

}



















