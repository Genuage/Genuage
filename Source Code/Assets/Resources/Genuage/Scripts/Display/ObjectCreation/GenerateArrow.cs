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
}
