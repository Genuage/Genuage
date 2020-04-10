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
using Display;

namespace VR_Interaction
{


    public class VRContainerAngleMeasurement : VRContainer
    {
        public List<Vector3> vector1points = new List<Vector3>();
        public List<Vector3> vector2points = new List<Vector3>();
        GameObject textobject;
        public override void AddVRObject(VRObject obj, int id)
        {
            base.AddVRObject(obj, id);
            ReloadMesh(obj,id);
        }

        public void ReloadMesh(VRObject obj, int id)
        {
            if(vector1points.Count < 2)
            {
                vector1points.Add(obj.transform.localPosition);
            }
            else if(vector2points.Count < 2)
            {
                vector2points.Add(obj.transform.localPosition);
            }


            List<int> indices = new List<int>();
            List<Vector3> vertices = new List<Vector3>();

            if(vector1points.Count == 2)
            {
                vertices.Add(vector1points[0]);
                indices.Add(vertices.Count - 1);
                vertices.Add(vector1points[1]);
                indices.Add(vertices.Count - 1);
                
            }

            if (vector2points.Count == 2)
            {
                vertices.Add(vector2points[0]);
                indices.Add(vertices.Count - 1);
                vertices.Add(vector2points[1]);
                indices.Add(vertices.Count - 1);
            }

            meshrenderer.material = new Material(Shader.Find("Standard"));

            mesh.vertices = vertices.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            meshfilter.mesh = mesh;
            if(vector1points.Count == 2 && vector2points.Count == 2)
            {
                CalculateAngle();
                vector1points.Clear();
                vector2points.Clear();
            }
            else if (textobject)
            {
                textobject.GetComponent<TextMesh>().text = "";
            }
            
        }

        public void CalculateAngle()
        {
            Vector3 v1 = vector1points[1] - vector1points[0];
            Vector3 v2 = vector2points[1] - vector2points[0];
            float angle = Vector3.Angle(v1, v2);

            if (!textobject)
            {
                textobject = new GameObject("text");
                textobject.transform.SetParent(VRObjectsDict[0].transform);
                textobject.AddComponent<MeshRenderer>();
                //text_object.GetComponent<MeshRenderer>();
                textobject.AddComponent<TextMesh>();
                textobject.AddComponent<StaringLabel>();
                textobject.transform.localPosition = Vector3.up;
                textobject.transform.localScale = Vector3.one ;

            }

            textobject.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
            textobject.GetComponent<TextMesh>().color = color;
            textobject.GetComponent<TextMesh>().text = angle.ToString();
        }

        public override void DeleteVRObject(int id)
        {
            base.DeleteVRObject(id);
        }

        protected override void OnDestroy()
        {
            VRObjectsManager.instance.ContainerDeleted(id, "AngleMeasure");
        }


    }
}