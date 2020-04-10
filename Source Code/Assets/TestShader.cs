using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Display;
public class TestShader : MonoBehaviour
{
    public List<Vector3> vertices;
    public List<int> indices;
    public List<Color> colors;
    public List<Vector2> uv;

    private void Awake()
    {
        vertices = new List<Vector3>();
        indices = new List<int>();
        colors = new List<Color>();
        uv = new List<Vector2>();

        vertices.Add(new Vector3(0.01f, 0.5f, 0.449f));
        indices.Add(vertices.Count - 1);
        colors.Add(Color.green);
        uv.Add(new Vector2(0f, 0f));

        vertices.Add(new Vector3(0.25f, 0.7f, 0.69f));
        indices.Add(vertices.Count - 1);
        indices.Add(vertices.Count - 1);
        colors.Add(Color.green);
        uv.Add(new Vector2(1f, 1f));


        vertices.Add(new Vector3(0.81f, 0.56f, 0.649f));
        indices.Add(vertices.Count - 1);
        indices.Add(vertices.Count - 1);
        colors.Add(Color.green);
        uv.Add(new Vector2(2f, 2f));


        vertices.Add(new Vector3(0.41f, 0.56f, 0.6779f));
        indices.Add(vertices.Count - 1);
        indices.Add(vertices.Count - 1);
        colors.Add(Color.green);
        uv.Add(new Vector2(3f, 3f));

        vertices.Add(new Vector3(0.43f, 0.46f, 0.9879f));
        indices.Add(vertices.Count - 1);
        indices.Add(vertices.Count - 1);
        colors.Add(Color.green);
        uv.Add(new Vector2(4f, 4f));



        vertices.Add(new Vector3(0.01f, 0.5f, 1f));
        indices.Add(vertices.Count - 1);
        //indices.Add(vertices.Count - 1);
        colors.Add(Color.green);
        uv.Add(new Vector2(5f, 5f));

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
        mesh.colors = colors.ToArray();
        mesh.uv = uv.ToArray();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshFilter>();

        Material material = new Material(Shader.Find("Unlit/NewUnlitShader"));
        GetComponent<MeshRenderer>().material = material;

    }
}
