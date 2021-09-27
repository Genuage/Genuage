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

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class GenerateArrow : MonoBehaviour
{
    MeshRenderer mrenderer;
    public Material material;
    MeshFilter filter;
    Mesh mesh;

    List<Vector3> vertices;
    List<int> indices;
    private void Awake()
    {
        mrenderer = gameObject.AddComponent<MeshRenderer>();
        filter = gameObject.AddComponent<MeshFilter>();
        mrenderer.material = new Material(Shader.Find("Standard"));

        //GenerateArrowMesh(new Vector3(1, 1, 1));
        //GenerateTriangularArrowMesh(new Vector3(1, 0, 0));
    }

    public void GenerateArrowMesh(Vector3 direction)
    {
        vertices = new List<Vector3>();
        indices = new List<int>();
                
        vertices.Add(Vector3.zero);
        indices.Add(0);

        Vector3 arrowheadposition = direction - direction / 5f;

        vertices.Add(arrowheadposition);
        indices.Add(1);

        Vector3 left = Vector3.Cross(arrowheadposition, Vector3.up).normalized;
        Vector3 right = -left;
        Vector3 up = Vector3.Cross(arrowheadposition, Vector3.forward).normalized;
        Vector3 down = -up;

        Debug.Log("up" + up);
        Debug.Log("down" + down);
        Debug.Log("left" + left);
        Debug.Log("right" + right);

        vertices.Add(arrowheadposition + left /10);
        indices.Add(2);

        vertices.Add(arrowheadposition + up /10);
        indices.Add(3);
        indices.Add(3);

        vertices.Add(arrowheadposition + right /10);
        indices.Add(4);
        indices.Add(4);

        vertices.Add(arrowheadposition + down /10);
        indices.Add(5);
        indices.Add(5);
        indices.Add(2);

        vertices.Add(direction);
        indices.Add(6);
        indices.Add(2);
        indices.Add(6);
        indices.Add(3);
        indices.Add(6);
        indices.Add(4);
        indices.Add(6);
        indices.Add(5);

        indices.Add(1);
        indices.Add(2);
        indices.Add(1);
        indices.Add(3);
        indices.Add(1);
        indices.Add(4);
        indices.Add(1);
        indices.Add(5);


        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

        filter.mesh = mesh;

    }

    public void GenerateTriangularArrowMesh(Vector3 direction)
    {

        //Vector3 direction = ndirection.normalized / 2;
        vertices = new List<Vector3>();
        indices = new List<int>();

        Vector3 arrowheadposition = direction - direction / 5f;
        Vector3 left = Vector3.Cross(arrowheadposition, Vector3.up).normalized;
        Vector3 right = -left;
        Vector3 up = Vector3.Cross(arrowheadposition, Vector3.forward).normalized;
        Vector3 down = -up;


        vertices.Add(Vector3.zero + left / 20); //0
        vertices.Add(Vector3.zero + up / 20); //1
        vertices.Add(Vector3.zero + right / 20); //2
        vertices.Add(Vector3.zero + down / 20); //3

        indices.Add(0);
        indices.Add(1);
        indices.Add(2);

        indices.Add(2);
        indices.Add(3);
        indices.Add(0);

        vertices.Add(arrowheadposition + left / 20); //4
        vertices.Add(arrowheadposition + up / 20); //5
        vertices.Add(arrowheadposition + right / 20); //6
        vertices.Add(arrowheadposition + down / 20); //7

        indices.Add(0);
        indices.Add(4);
        indices.Add(5);

        indices.Add(5);
        indices.Add(1);
        indices.Add(0);


        indices.Add(1);
        indices.Add(5);
        indices.Add(6);

        indices.Add(6);
        indices.Add(2);
        indices.Add(1);


        indices.Add(2);
        indices.Add(6);
        indices.Add(7);

        indices.Add(7);
        indices.Add(3);
        indices.Add(2);


        indices.Add(3);
        indices.Add(7);
        indices.Add(4);

        indices.Add(4);
        indices.Add(0);
        indices.Add(3);


        vertices.Add(arrowheadposition + left / 10); //8
        vertices.Add(arrowheadposition + up / 10); //9
        vertices.Add(arrowheadposition + right / 10); //10
        vertices.Add(arrowheadposition + down / 10); //11

        indices.Add(4);
        indices.Add(8);
        indices.Add(9);

        indices.Add(9);
        indices.Add(5);
        indices.Add(4);


        indices.Add(5);
        indices.Add(9);
        indices.Add(10);

        indices.Add(10);
        indices.Add(6);
        indices.Add(5);


        indices.Add(6);
        indices.Add(10);
        indices.Add(11);

        indices.Add(11);
        indices.Add(7);
        indices.Add(6);

        indices.Add(7);
        indices.Add(11);
        indices.Add(8);

        indices.Add(8);
        indices.Add(4);
        indices.Add(7);

        vertices.Add(direction); //12

        indices.Add(8);
        indices.Add(12);
        indices.Add(9);

        indices.Add(9);
        indices.Add(12);
        indices.Add(10);

        indices.Add(10);
        indices.Add(12);
        indices.Add(11);

        indices.Add(11);
        indices.Add(12);
        indices.Add(8);

        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

        filter.mesh = mesh;

        transform.localScale = Vector3.one / 5;

    }

    public void GenerateSquarePyramidMesh(Vector3 direction)
    {
        direction = direction / 3;
        vertices = new List<Vector3>();
        indices = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        //Vector3 arrowheadposition = direction - direction / 5f;
        Vector3 left = Vector3.Cross(direction, Vector3.left).normalized;
        Vector3 right = -left;
        Vector3 up = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 down = -up;

        Vector3 v1 = ((Vector3.zero - (direction / 2)) + left / 10);
        Vector3 v2 = ((Vector3.zero - (direction / 2)) + up / 10);
        Vector3 v3 = ((Vector3.zero - (direction / 2)) + right / 10);
        Vector3 v4 = ((Vector3.zero - (direction / 2)) + down / 10);

        vertices.Add(v1); //0
        vertices.Add(v2); //1
        vertices.Add(v3); //2
        vertices.Add(v4); //3
        //vertices.Add(Vector3.zero); //4

        indices.Add(0);
        indices.Add(1);
        indices.Add(2);

        Vector3 normal1 = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[1]).normalized;
        if(Vector3.Cross(normal1, direction).magnitude == 0 && Vector3.Dot(normal1, direction) > 0) 
            //if the normal is facing towards the tip of the pyramid
        {
            normal1 = -normal1;
        }
        normals.Add(normal1);
        normals.Add(normal1);
        normals.Add(normal1);
        normals.Add(normal1);

        indices.Add(0);
        indices.Add(2);
        indices.Add(3);

        vertices.Add(v1); //0 
        vertices.Add(direction / 2); //4
        vertices.Add(v2); //1 
                
        indices.Add(4);
        indices.Add(5); //OK
        indices.Add(6);

        Vector3 normal2 = Vector3.Cross(vertices[5] - vertices[4], vertices[6] - vertices[5]);
        normals.Add(normal2);
        normals.Add(normal2);
        normals.Add(normal2);


        vertices.Add(v2); //1 
        vertices.Add(direction / 2); //4 
        vertices.Add(v3); //2 

        indices.Add(7);
        indices.Add(8); //OK
        indices.Add(9);

        Vector3 normal3 = Vector3.Cross(vertices[8] - vertices[7], vertices[9] - vertices[8]);
        normals.Add(normal3);
        normals.Add(normal3);
        normals.Add(normal3);


        vertices.Add(v3); //2 
        vertices.Add(direction / 2); //4 
        vertices.Add(v4); //3 

        indices.Add(10);
        indices.Add(11); //OK
        indices.Add(12);

        Vector3 normal4 = Vector3.Cross(vertices[11] - vertices[10], vertices[12] - vertices[11]);
        normals.Add(normal3);
        normals.Add(normal3);
        normals.Add(normal3);

        vertices.Add(v4); //3 
        vertices.Add(direction / 2); //4 
        vertices.Add(v1); //0 

        indices.Add(13);
        indices.Add(14); //OK
        indices.Add(15);

        Vector3 normal5 = Vector3.Cross(vertices[14] - vertices[13], vertices[15] - vertices[14]);
        normals.Add(normal5);
        normals.Add(normal5);
        normals.Add(normal5);

        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(indices.ToArray(),0);
        mesh.SetNormals(normals);
        //mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        //mesh.();
        filter.mesh = mesh;

        transform.localScale = Vector3.one / 5;


    }
}


