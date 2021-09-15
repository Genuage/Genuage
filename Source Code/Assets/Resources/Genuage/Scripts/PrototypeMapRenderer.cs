using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

[RequireComponent(typeof(MeshFilter))]

public class VoxelData
{
    private int[,] VoxelArray =  new int[,] {{0,1,1},{1,1,1},{1,1,0}};


    public VoxelData()
    {

    }

    public int Width()
    {
        return VoxelArray.GetLength(0);
    }

    public int Depth()
    {
        return VoxelArray.GetLength(1);
    }

    public int Height()
    {
        return VoxelArray.GetLength(2);
    }

    public int GetCell(int x,int y)
    {
        return VoxelArray[x, y];
    }
}

public class PrototypeMapRenderer : MonoBehaviour
{
    MeshFilter Mfilter;
    MeshRenderer Mrenderer;
    List<Vector3> vertices;
    List<int> indices;
    List<Vector2> uvList;
    Mesh mesh;
    // Start is called before the first frame update
    void Awake()
    {
        Mfilter = gameObject.GetComponent<MeshFilter>();
        Mrenderer = gameObject.GetComponent<MeshRenderer>();

        mesh = Mfilter.mesh;
        CreateMeshData();
    }

    private void CreateMeshData()
    {
        vertices = new List<Vector3>();
        indices = new List<int>();
        uvList = new List<Vector2>();
        /**
        CreateCube(new Vector3(-0.5f, 0, 0), 0.2f);
        CreateCube(new Vector3(0, 0, 0), 0.2f);
        CreateCube(new Vector3(0.6f, 0, 0), 0.2f);
        CreateCube(new Vector3(0.3f, 0, 0), 0.2f);
        **/
        VoxelData data = new VoxelData();
        GenerateVoxelMesh(data);
        //CreateQuad();
        Material material = new Material(Shader.Find("Genuage/UnlitLineShader"));
        material.SetTexture("_ColorTex", ColorMapManager.instance.GetColorMap("jet").texture);
        Mrenderer.material = material;

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.uv = uvList.ToArray();
        mesh.RecalculateNormals();
        


        gameObject.AddComponent<BoxCollider>();

    }

    private void GenerateVoxelMesh(VoxelData data)
    {
        for (int y = 0; y < data.Depth(); y++)
        {
            for(int x = 0; x < data.Width(); x++)
            {
                if (data.GetCell(x, y) == 0)
                {
                    continue;
                }
                CreateCube(new Vector3((float)x * 0.2f, 0, (float)y * 0.2f), 0.2f);
            }
        }
    }

    private void CreateCube(Vector3 Center, float edgeSize)
    {
        float adjustedSize = edgeSize * 0.5f;
        Vector3[] cornerArray = new Vector3[]
        {
            Center + adjustedSize * new Vector3(1, 1, 1), //0
            Center + adjustedSize * new Vector3(-1, 1, 1), //1
            Center + adjustedSize * new Vector3(-1, -1, 1), //2
            Center + adjustedSize * new Vector3(1, -1, 1), //3
            Center + adjustedSize * new Vector3(-1, 1, -1), //4
            Center + adjustedSize * new Vector3(1, 1, -1),//5
            Center + adjustedSize * new Vector3(1, -1, -1), //6
            Center + adjustedSize * new Vector3(-1, -1, -1), //7 
        };

        int[] north = new int[] { 0, 1, 2, 3 };
        int[] east = new int[] { 5, 0, 3, 6 };
        int[] west = new int[] { 1, 4, 7, 2 };
        int[] south = new int[] { 4, 5, 6, 7 };
        int[] up = new int[] { 5, 4, 1, 0 };
        int[] down = new int[] { 3, 2, 7, 6 };

        CreateFace(north, cornerArray);
        CreateFace(east, cornerArray);
        CreateFace(south, cornerArray);
        CreateFace(west, cornerArray);
        CreateFace(up, cornerArray);
        CreateFace(down, cornerArray);
    }

    private void CreateFace(int[] faceindices, Vector3[] positionArray)
    {
        float colornumber = Random.Range(0, 1);
        foreach (int i in faceindices)
        {
            vertices.Add(positionArray[i]);
            uvList.Add(new Vector2( colornumber, 0));

        }
        int vCount = vertices.Count;
        //First triangle
        indices.Add(vCount - 4);
        indices.Add(vCount - 3);
        indices.Add(vCount - 2);
        /**
        indices.Add(vCount - 2);
        indices.Add(vCount - 3);
        indices.Add(vCount - 4);
        **/
        //Second triangle

        indices.Add(vCount - 4);
        indices.Add(vCount - 2);
        indices.Add(vCount - 1);
        /**
        indices.Add(vCount - 1);
        indices.Add(vCount - 2);
        indices.Add(vCount - 4);
        **/
    }

    private void CreateQuad()
    {
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(0, 0, 1));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(1, 0, 1));
        //First triangle
        indices.Add(0);
        indices.Add(1);
        indices.Add(2);
        indices.Add(2);
        indices.Add(1);
        indices.Add(0);
        //Second triangle
        indices.Add(2);
        indices.Add(1);
        indices.Add(3);
        indices.Add(3);
        indices.Add(1);
        indices.Add(2);
    }
}
