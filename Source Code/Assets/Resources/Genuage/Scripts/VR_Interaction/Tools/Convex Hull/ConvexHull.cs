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
using UnityEngine.Assertions;


namespace VR_Interaction
{
    namespace Convex_Hull
    {


        public struct Face
        {
            public int Vertex0;
            public int Vertex1;
            public int Vertex2;
            public int Opposite0;
            public int Opposite1;
            public int Opposite2;

            public Vector3 normal;

            public Face(int Vertex0, int Vertex1, int Vertex2, int opposite0, int opposite1, int opposite2, Vector3 normal)
            {
                this.Vertex0 = Vertex0;
                this.Vertex1 = Vertex1;
                this.Vertex2 = Vertex2;
                this.Opposite0 = opposite0;
                this.Opposite1 = opposite1;
                this.Opposite2 = opposite2;
                this.normal = normal;
            }

            public bool Equals(Face other)
            {
                return (this.Vertex0 == other.Vertex0)
                    && (this.Vertex1 == other.Vertex1)
                    && (this.Vertex2 == other.Vertex2)
                    && (this.Opposite0 == other.Opposite0)
                    && (this.Opposite1 == other.Opposite1)
                    && (this.Opposite2 == other.Opposite2)
                    && (this.normal == other.normal);
            }
        }

        public struct PointFace
        {
            public int Point;
            public int Face;
            public float Distance;

            public PointFace(int p, int f, float d)
            {
                Point = p;
                Face = f;
                Distance = d;
            }
        }

        public struct PointToFaceMap
        {
            public int point;
            public int face;
            public float distance;

            public PointToFaceMap(int p, int f, float d)
            {
                point = p;
                face = f;
                distance = d;
            }
        }

        struct HorizonEdge
        {
            public int Face;
            public int Edge0;
            public int Edge1;


        }



        public class ConvexHull 
        {
            public List<GameObject> pointList;
            public List<Vector3> positionList;
            Dictionary<int[], Plane> planeDict;

            public Dictionary<int, Face> faces;
            HashSet<int> litFaces;
            List<HorizonEdge> horizon;
            Dictionary<int, int> hullverts;
            List<PointFace> openSet;
            int faceCount = 0;
            int openSetTail;
            const float EPSILON = 0.0001f;
            const int INSIDE = -1;
            const int UNASSIGNED = -2;

            public ConvexHull()
            {
                pointList = new List<GameObject>();
                planeDict = new Dictionary<int[], Plane>();
                positionList = new List<Vector3>();


                faces = new Dictionary<int, Face>();
                litFaces = new HashSet<int>();
                openSet = new List<PointFace>();
                horizon = new List<HorizonEdge>();


            }

            #region Convex Hull creation
            public Mesh CreateMesh()
            {
                Assert.IsTrue(pointList.Count >= 4);
                if (pointList.Count == 4)
                {
                    initializeHull();
                    //assign each point to a specific face, unless they are inside the hull. 
                    for (int i = 0; i < openSet.Count; i++)
                    {
                        PointFace pf = openSet[i];

                        bool assigned = false;
                        for (int j = 0; j < 4; j++)
                        {
                            Face face = faces[j];
                            float dist = PointFaceDistance(positionList[pf.Point], positionList[face.Vertex0], face);

                            if (dist > 0)
                            {
                                pf.Face = j;
                                pf.Distance = dist;
                                openSet[i] = pf;
                                assigned = true;
                                break;
                            }
                        }
                        if (!assigned)
                        {
                            pf.Face = INSIDE;
                            pf.Distance = float.NaN;
                        }
                    }




                }

                if (pointList.Count > 4)
                {

                    initializeHull();

                    openSetTail = openSet.Count - 5;
                    Assert.IsTrue(openSet.Count == pointList.Count);

                    for (int k = 0; k <= openSetTail; k++)
                    {
                        bool assigned = false;
                        PointFace pf = openSet[k];
                        for (int i = 0; i < faces.Count; i++)
                        {
                            Face face = faces[i];
                            var distance = PointFaceDistance(positionList[pf.Point], positionList[face.Vertex0], face);
                            if (distance > EPSILON)
                            {
                                pf.Face = i;
                                pf.Distance = distance;
                                openSet[k] = pf;
                                assigned = true;
                                break;
                            }
                        }

                        if (!assigned)
                        {
                            pf.Face = INSIDE;
                            pf.Distance = float.NaN;

                            openSet[k] = openSet[openSetTail];
                            openSet[openSetTail] = pf;

                            openSetTail -= 1;
                            k--;
                        }
                    }
                    while (openSetTail >= 0)
                    {
                        int farthestPoint = 0;
                        float dist = openSet[0].Distance;

                        for (int l = 1; l <= openSetTail; l++)
                        {
                            if (openSet[l].Distance > dist)
                            {
                                farthestPoint = l;
                                dist = openSet[l].Distance;
                            }
                        }

                        FindHorizon(positionList, positionList[openSet[farthestPoint].Point], openSet[farthestPoint].Face, faces[openSet[farthestPoint].Face]);

                        ConstructCone(openSet[farthestPoint].Point);

                        ReassignPoints();

                    }

                }

                Mesh mesh = ExportMesh();
                return mesh;


            }

            public void initializeHull()
            {
                Assert.IsTrue(pointList.Count >= 4);
                openSet.Clear();
                faces.Clear();


                var b0 = 0;
                var b1 = 1;
                var b2 = 2;
                var b3 = 3;

                var above = Vector3.Dot(positionList[3] - positionList[1], Vector3.Cross(positionList[1] - positionList[0], positionList[2] - positionList[0])) > 0.0f;

                faceCount = 0;
                if (above)
                {
                    faces[faceCount++] = new Face(b0, b2, b1, 3, 1, 2, Normal(positionList[b0], positionList[b2], positionList[b1]));
                    faces[faceCount++] = new Face(b0, b1, b3, 3, 2, 0, Normal(positionList[b0], positionList[b1], positionList[b3]));
                    faces[faceCount++] = new Face(b0, b3, b2, 3, 0, 1, Normal(positionList[b0], positionList[b3], positionList[b2]));
                    faces[faceCount++] = new Face(b1, b2, b3, 2, 1, 0, Normal(positionList[b1], positionList[b2], positionList[b3]));
                }
                else
                {
                    faces[faceCount++] = new Face(b0, b1, b2, 3, 2, 1, Normal(positionList[b0], positionList[b1], positionList[b2]));
                    faces[faceCount++] = new Face(b0, b3, b1, 3, 0, 2, Normal(positionList[b0], positionList[b3], positionList[b1]));
                    faces[faceCount++] = new Face(b0, b2, b3, 3, 1, 0, Normal(positionList[b0], positionList[b2], positionList[b3]));
                    faces[faceCount++] = new Face(b1, b3, b2, 2, 0, 1, Normal(positionList[b1], positionList[b3], positionList[b2]));
                }
                VerifyFaces();
                for (int i = 0; i < pointList.Count; i++)
                {
                    if(!(i==b0 || i == b1 || i == b2 || i == b3))
                    {
                        openSet.Add(new PointFace(i, UNASSIGNED, 0f));
                    }
                }
                openSet.Add(new PointFace(0, -1, float.NaN));
                openSet.Add(new PointFace(1, -1, float.NaN));
                openSet.Add(new PointFace(2, -1, float.NaN));
                openSet.Add(new PointFace(3, -1, float.NaN));



            }


            void FindHorizon(List<Vector3> points, Vector3 point, int fi, Face face)
            {

                litFaces.Clear();
                horizon.Clear();

                litFaces.Add(fi);

                Assert.IsTrue(PointFaceDistance(point, points[face.Vertex0], face) > 0.0f);

                {
                    var oppositeFace = faces[face.Opposite0];

                    var dist = PointFaceDistance(
                        point,
                        points[oppositeFace.Vertex0],
                        oppositeFace);

                    if (dist <= 0.0f)
                    {
                        horizon.Add(new HorizonEdge
                        {
                            Face = face.Opposite0,
                            Edge0 = face.Vertex1,
                            Edge1 = face.Vertex2,
                        });
                    }
                    else
                    {
                        SearchHorizon(points, point, fi, face.Opposite0, oppositeFace);
                    }
                }

                if (!litFaces.Contains(face.Opposite1))
                {
                    var oppositeFace = faces[face.Opposite1];

                    var dist = PointFaceDistance(
                        point,
                        points[oppositeFace.Vertex0],
                        oppositeFace);

                    if (dist <= 0.0f)
                    {
                        horizon.Add(new HorizonEdge
                        {
                            Face = face.Opposite1,
                            Edge0 = face.Vertex2,
                            Edge1 = face.Vertex0,
                        });
                    }
                    else
                    {
                        SearchHorizon(points, point, fi, face.Opposite1, oppositeFace);
                    }
                }

                if (!litFaces.Contains(face.Opposite2))
                {
                    var oppositeFace = faces[face.Opposite2];

                    var dist = PointFaceDistance(
                        point,
                        points[oppositeFace.Vertex0],
                        oppositeFace);

                    if (dist <= 0.0f)
                    {
                        horizon.Add(new HorizonEdge
                        {
                            Face = face.Opposite2,
                            Edge0 = face.Vertex0,
                            Edge1 = face.Vertex1,
                        });
                    }
                    else
                    {
                        SearchHorizon(points, point, fi, face.Opposite2, oppositeFace);
                    }
                }
            }

            void SearchHorizon(List<Vector3> points, Vector3 point, int prevFaceIndex, int faceCount, Face face)
            {


                litFaces.Add(faceCount);

                int nextFaceIndex0;
                int nextFaceIndex1;
                int edge0;
                int edge1;
                int edge2;

                if (prevFaceIndex == face.Opposite0)
                {
                    nextFaceIndex0 = face.Opposite1;
                    nextFaceIndex1 = face.Opposite2;

                    edge0 = face.Vertex2;
                    edge1 = face.Vertex0;
                    edge2 = face.Vertex1;
                }
                else if (prevFaceIndex == face.Opposite1)
                {
                    nextFaceIndex0 = face.Opposite2;
                    nextFaceIndex1 = face.Opposite0;

                    edge0 = face.Vertex0;
                    edge1 = face.Vertex1;
                    edge2 = face.Vertex2;
                }
                else
                {
                    Assert.IsTrue(prevFaceIndex == face.Opposite2);

                    nextFaceIndex0 = face.Opposite0;
                    nextFaceIndex1 = face.Opposite1;

                    edge0 = face.Vertex1;
                    edge1 = face.Vertex2;
                    edge2 = face.Vertex0;
                }

                if (!litFaces.Contains(nextFaceIndex0))
                {
                    var oppositeFace = faces[nextFaceIndex0];

                    var dist = PointFaceDistance(
                        point,
                        points[oppositeFace.Vertex0],
                        oppositeFace);

                    if (dist <= 0.0f)
                    {
                        horizon.Add(new HorizonEdge
                        {
                            Face = nextFaceIndex0,
                            Edge0 = edge0,
                            Edge1 = edge1,
                        });
                    }
                    else
                    {
                        SearchHorizon(points, point, faceCount, nextFaceIndex0, oppositeFace);
                    }
                }

                if (!litFaces.Contains(nextFaceIndex1))
                {
                    var oppositeFace = faces[nextFaceIndex1];

                    var dist = PointFaceDistance(
                        point,
                        points[oppositeFace.Vertex0],
                        oppositeFace);

                    if (dist <= 0.0f)
                    {
                        horizon.Add(new HorizonEdge
                        {
                            Face = nextFaceIndex1,
                            Edge0 = edge1,
                            Edge1 = edge2,
                        });
                    }
                    else
                    {
                        SearchHorizon(points, point, faceCount, nextFaceIndex1, oppositeFace);
                    }
                }
            }

            void ConstructCone(int pointToLinkID)
            {

                foreach (int f in litFaces)
                {
                    Assert.IsTrue(faces.ContainsKey(f));
                    faces.Remove(f);
                }



                int firstNewFace = faceCount;

                for (int i = 0; i < horizon.Count; i++)
                {
                    int v0 = pointToLinkID;
                    int v1 = horizon[i].Edge0;
                    int v2 = horizon[i].Edge1;

                    int o0 = horizon[i].Face;
                    var o1 = (i == horizon.Count - 1) ? firstNewFace : firstNewFace + i + 1;
                    var o2 = (i == 0) ? (firstNewFace + horizon.Count - 1) : firstNewFace + i - 1;

                    int fi = faceCount++;

                    faces[fi] = new Face(v0, v1, v2, o0, o1, o2, Normal(positionList[v0], positionList[v1], positionList[v2]));

                    var horizonFace = faces[horizon[i].Face];

                    if (horizonFace.Vertex0 == v1)
                    {
                        Assert.IsTrue(v2 == horizonFace.Vertex2);
                        horizonFace.Opposite1 = fi;
                    }
                    else if (horizonFace.Vertex1 == v1)
                    {
                        Assert.IsTrue(v2 == horizonFace.Vertex0);
                        horizonFace.Opposite2 = fi;
                    }
                    else
                    {
                        Assert.IsTrue(v1 == horizonFace.Vertex2);
                        Assert.IsTrue(v2 == horizonFace.Vertex1);
                        horizonFace.Opposite0 = fi;
                    }

                    faces[horizon[i].Face] = horizonFace;
                }


            }

            void ReassignPoints()
            {
                for (int i = 0; i <= openSetTail; i++)
                {
                    PointFace pf = openSet[i];
                    if (litFaces.Contains(pf.Face))
                    {
                        bool assigned = false;
                        Vector3 point = positionList[pf.Point];

                        foreach (var kvp in faces)
                        {
                            var fi = kvp.Key;
                            var face = kvp.Value;

                            float dist = PointFaceDistance(point, positionList[face.Vertex0], face);

                            if (dist > EPSILON)
                            {
                                assigned = true;
                                pf.Face = fi;
                                pf.Distance = dist;
                                openSet[i] = pf;
                                break;
                            }
                        }

                        if (!assigned)
                        {
                            pf.Face = INSIDE;
                            pf.Distance = float.NaN;

                            openSet[i] = openSet[openSetTail];
                            openSet[openSetTail] = pf;

                            i--;
                            openSetTail--;
                        }
                    }

                }
            }

            Mesh ExportMesh(bool splitVerts = true)
            {
                List<Vector3> verts = new List<Vector3>();
                List<int> tris = new List<int>();
                List<Vector3> normals = new List<Vector3>();

                foreach (Face face in faces.Values)
                {
                    int vi0, vi1, vi2;

                    if (splitVerts)
                    {
                        vi0 = verts.Count;
                        verts.Add(positionList[face.Vertex0]);
                        vi1 = verts.Count;
                        verts.Add(positionList[face.Vertex1]);
                        vi2 = verts.Count;
                        verts.Add(positionList[face.Vertex2]);

                        normals.Add(face.normal);
                        normals.Add(face.normal);
                        normals.Add(face.normal);
                    }
                    else
                    {
                        if (!hullverts.TryGetValue(face.Vertex0, out vi0))
                        {
                            vi0 = verts.Count;
                            hullverts[face.Vertex0] = vi0;
                            verts.Add(positionList[face.Vertex0]);
                        }
                        if (!hullverts.TryGetValue(face.Vertex1, out vi1))
                        {
                            vi1 = verts.Count;
                            hullverts[face.Vertex1] = vi1;
                            verts.Add(positionList[face.Vertex1]);
                        }
                        if (!hullverts.TryGetValue(face.Vertex2, out vi2))
                        {
                            vi2 = verts.Count;
                            hullverts[face.Vertex2] = vi2;
                            verts.Add(positionList[face.Vertex2]);
                        }
                    }
                    tris.Add(vi0);
                    tris.Add(vi1);
                    tris.Add(vi0);
                    tris.Add(vi2);
                    tris.Add(vi1);
                    tris.Add(vi2);

                }

                Mesh mesh = new Mesh();
                mesh.vertices = verts.ToArray();
                mesh.SetIndices(tris.ToArray(), MeshTopology.Lines, 0);
                mesh.SetNormals(normals);
                return mesh;
            }

            void VerifyFaces()
            {
                foreach (KeyValuePair<int, Face> kvp in faces)
                {
                    Assert.IsTrue(faces.ContainsKey(kvp.Value.Opposite0));
                    Assert.IsTrue(faces.ContainsKey(kvp.Value.Opposite1));
                    Assert.IsTrue(faces.ContainsKey(kvp.Value.Opposite2));

                    Assert.IsTrue(kvp.Value.Opposite0 != kvp.Key);
                    Assert.IsTrue(kvp.Value.Opposite1 != kvp.Key);
                    Assert.IsTrue(kvp.Value.Opposite2 != kvp.Key);

                    Assert.IsTrue(kvp.Value.Vertex0 != kvp.Value.Vertex1);
                    Assert.IsTrue(kvp.Value.Vertex0 != kvp.Value.Vertex2);
                    Assert.IsTrue(kvp.Value.Vertex2 != kvp.Value.Vertex1);

                    Assert.IsTrue(HasEdge(faces[kvp.Value.Opposite0], kvp.Value.Vertex2, kvp.Value.Vertex1));
                    Assert.IsTrue(HasEdge(faces[kvp.Value.Opposite1], kvp.Value.Vertex0, kvp.Value.Vertex2));
                    Assert.IsTrue(HasEdge(faces[kvp.Value.Opposite2], kvp.Value.Vertex1, kvp.Value.Vertex0));

                    Assert.IsTrue((kvp.Value.normal - Normal(positionList[kvp.Value.Vertex0], positionList[kvp.Value.Vertex1], positionList[kvp.Value.Vertex2])).magnitude < EPSILON);

                }
            }

            float PointFaceDistance(Vector3 point, Vector3 pointOnFace, Face face)
            {
                return Vector3.Dot(face.normal, point - pointOnFace);
            }

            Vector3 Normal(Vector3 v0, Vector3 v1, Vector3 v2)
            {
                return Vector3.Cross(v1 - v0, v2 - v0).normalized;
            }

            bool HasEdge(Face f, int e0, int e1)
            {
                return (f.Vertex0 == e0 && f.Vertex1 == e1)
                    || (f.Vertex1 == e0 && f.Vertex2 == e1)
                    || (f.Vertex2 == e0 && f.Vertex0 == e1);
            }

            #endregion

        }
    }
}
