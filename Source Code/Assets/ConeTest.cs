using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeTest : MonoBehaviour
{
    public MeshFilter mfilter;
    public MeshRenderer mrenderer;
    public Vector3 startingpoint = Vector3.zero;
    public Vector3 endpoint = new Vector3(1f, 1f, 1f);
    public float height = 1.0f;
    public float width = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        mrenderer = gameObject.GetComponent<MeshRenderer>();
        mfilter = gameObject.GetComponent<MeshFilter>();


        //Mesh mesh = CreateConeMesh(startingpoint, Vector3.up, width);
        Mesh mesh = Create(Vector3.zero, 50, 0.05f, 0.2f);
        mfilter.mesh = mesh;

    }
    private Mesh CreateConeMesh(Vector3 startpos, Vector3 endpos, float width)
    {

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        //make circle
        int numOfPoints = 50;
        float angleStep = 360.0f / (float)numOfPoints;
        Quaternion quaternion = Quaternion.Euler(0.0f, angleStep, 0f);
        // Make first triangle.

        //vertices.Add(new Vector3(0f, 0f, 0.5f));

        vertices.Add(endpos);  // 1. Circle center.
        vertices.Add(endpos + Vector3.right);  // 2. First vertex on circle outline (radius = 0.5f)
        vertices.Add(quaternion * vertices[1]);     // 3. First vertex on circle outline rotated by angle)

        //vertices.Add(quaternion * vertices[1]);     // 3. First vertex on circle outline rotated by angle)
        
        // Add circle triangles indices.
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);
        Vector3 normal = Vector3.Cross(vertices[0] - vertices[1], vertices[0] - vertices[2]);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);


        triangles.Add(2);
        triangles.Add(1);
        triangles.Add(0);

        /**
        //cone triangles
        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(3);
        triangles.Add(3);
        triangles.Add(2);
        triangles.Add(0);
        **/

        for (int i = 0; i < numOfPoints - 1; i++)
        {
            //circle triangles
            triangles.Add(0);                      // Index of circle center.
            triangles.Add(vertices.Count - 1);
            triangles.Add(vertices.Count);

            normal = Vector3.Cross( vertices[vertices.Count - 2] - vertices[0], vertices[vertices.Count-1] - vertices[0]);
            normals.Add(normal);



            triangles.Add(vertices.Count);                      // Index of circle center.
            triangles.Add(vertices.Count - 1);
            triangles.Add(0);


            vertices.Add(quaternion * vertices[vertices.Count - 1]);
        }
        vertices.Add(startpos); //cone end, point position
        normal = Vector3.Cross(vertices[1] - vertices[vertices.Count - 1], vertices[2] - vertices[vertices.Count - 1]);
        normals.Add(normal);

        for (int i = 1; i < vertices.Count-2; i++)
        {
            //cone triangles
            triangles.Add(vertices.Count-1);
            triangles.Add(i);
            triangles.Add(i + 1);


            triangles.Add(i + 1);
            triangles.Add(i);
            triangles.Add(vertices.Count-1);



        }

        Debug.Log("vertice array size : " + vertices.Count + " normals array size : " + normals.Count);
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        //mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
        mesh.SetIndices(triangles.ToArray(), MeshTopology.Triangles, 0);
        //mesh.SetNormals(normals);
        //mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }

    Mesh Create(Vector3 centerpos, int subdivisions, float radius, float height)
    {
        int IDCenter1;
        int IDMiddle;
        int IDCenter2;
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        //Vector3[] vertices = new Vector3[subdivisions + 2];
        //Vector2[] uv = new Vector2[vertices.Length];
        //int[] triangles = new int[(subdivisions * 2) * 3];

        vertices.Add(centerpos+ new Vector3(0.0f, height, 0.0f)); // center of the circle
        IDCenter1 = vertices.Count - 1;
        uv.Add(new Vector2(0.5f, 0f));
        for (int i = 0, n = subdivisions - 1; i < subdivisions; i++)
        {
            float ratio = (float)i / n;
            float r = ratio * (Mathf.PI * 2f);
            float x = Mathf.Cos(r) * radius;
            float z = Mathf.Sin(r) * radius;
            //vertices[i + 1] = new Vector3(x, 0f, z);
            vertices.Add(new Vector3(x, height, z));

            Debug.Log(ratio);
            //uv[i + 1] = new Vector2(ratio, 0f);
            uv.Add(new Vector2(ratio, 0f));
        }
        vertices.Add(centerpos); // center
        IDMiddle = vertices.Count - 1;
        uv.Add(new Vector2(0.5f, 1f));
        //vertices[subdivisions + 1] = new Vector3(0f, height, 0f);
        //uv[subdivisions + 1] = new Vector2(0.5f, 1f);

        // construct bottom

        for (int i = 0, n = subdivisions - 1; i < n; i++)
        {
            triangles.Add(IDCenter1);
            triangles.Add(i + 1);
            triangles.Add(i + 2);

            /**
            int offset = i * 3;
            triangles[offset] = 0;
            triangles[offset + 1] = i + 1;
            triangles[offset + 2] = i + 2;
            **/
        }

        // construct sides
         
        //int bottomOffset = subdivisions * 3;
        for (int i = 0, n = subdivisions - 1; i < n; i++)
        {

            triangles.Add(i + 1);
            triangles.Add(IDMiddle);
            triangles.Add(i + 2);
            /**
            int offset = i * 3 + bottomOffset;
            triangles[offset] = i + 1;
            triangles[offset + 1] = subdivisions + 1;
            triangles[offset + 2] = i + 2;
            **/
        }

        //SECOND CONE STARTS
        
        vertices.Add(centerpos + new Vector3(0.0f, - height, 0.0f)); // center of the circle
        IDCenter2 = vertices.Count - 1;
        uv.Add(new Vector2(0.5f, 0f));
        for (int i = 0, n = subdivisions - 1; i < subdivisions; i++)
        {
            float ratio = (float)i / n;
            float r = ratio * (Mathf.PI * 2f);
            float x = Mathf.Cos(r) * radius;
            float z = Mathf.Sin(r) * radius;
            //vertices[i + 1] = new Vector3(x, 0f, z);
            vertices.Add(new Vector3(x, -height, z));

            Debug.Log(ratio);
            //uv[i + 1] = new Vector2(ratio, 0f);
            uv.Add(new Vector2(ratio, 0f));
        }

        for (int i = IDCenter2+1; i < vertices.Count-2; i++)
        {
            triangles.Add(IDCenter2);
            triangles.Add(i + 1);
            triangles.Add(i + 2);
           
        }

        // construct sides

        //int bottomOffset = subdivisions * 3;
        for (int i = IDCenter2 + 1; i < vertices.Count - 2; i++)
        {

            triangles.Add(i + 1);
            triangles.Add(IDMiddle);
            triangles.Add(i + 2);
         
        }
        



        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

}
