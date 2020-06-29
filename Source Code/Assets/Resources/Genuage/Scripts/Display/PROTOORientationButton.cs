/**
Copyright (c) 2020, 	Institut Curie, Institut Pasteur and CNRS
			Thomas BLanc, Mohamed El Beheiry, Jean Baptiste Masson and Bassam Hajj
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{


    public class PROTOORientationButton : IButtonScript
    {
        public bool orientationModeON = false;

        public InputField thetainput;
        public InputField rhoinput;




        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
        }
        public override void Execute()
        {
            int thetaindex;
            int rhoindex;
            if (orientationModeON == true)
            {
                CloudData data = CloudUpdater.instance.LoadCurrentStatus();
                data.orientationObject.GetComponent<MeshRenderer>().enabled = false;
                orientationModeON = false;
            }
            else
            {
                

                        

                        CloudData data = CloudUpdater.instance.LoadCurrentStatus();

                        if (data.orientationObject)
                        {
                            GameObject obj = data.orientationObject;
                            data.orientationObject = null;
                            Destroy(obj);
                        }

                        List<float> xvalues = new List<float>();
                        List<float> yvalues = new List<float>();
                        List<float> zvalues = new List<float>();

                        List<Color> color = new List<Color>();


                        float[] xvalue = data.columnData[data.columnData.Count - 3];
                        float[] yvalue = data.columnData[data.columnData.Count - 2];
                        float[] zvalue = data.columnData[data.columnData.Count - 1];


                        foreach (var kvp in data.pointDataTable)
                        {
                            float theta = kvp.Value.theta_angle;
                            float phi = kvp.Value.phi_angle;
                            xvalues.Add(0.005f * Mathf.Sin(theta) * Mathf.Cos(phi));
                            yvalues.Add(0.005f * Mathf.Sin(theta) * Mathf.Sin(phi));
                            zvalues.Add(0.005f * Mathf.Cos(theta));

                            color.Add(Color.HSVToRGB(phi / 360f, 0.75f, 0.55f));
                            color.Add(Color.HSVToRGB(phi / 360f, 0.75f, 0.55f));
                         }

                        

                        Mesh mesh = new Mesh();
                        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

                        List<Vector3> vertices = new List<Vector3>();
                        List<int> indices = new List<int>();
                        List<Vector2> UV3List = new List<Vector2>();
                        int index = 0;
                        /**
                        vertices.Add(new Vector3(xvalue[0], yvalue[0], zvalue[0]));
                        color.Add(Color.blue);
                **/
                        for (int i = 0; i < xvalues.Count; i++)
                        {
                            //vertices.Add(data.pointDataTable[i].normed_position - (new Vector3(xvalue[i], yvalue[i], zvalue[i]).normalized * 0.001f));

                            vertices.Add(data.pointDataTable[i].normed_position - (new Vector3(xvalues[i], yvalues[i], zvalues[i])));
                            indices.Add(index);
                            UV3List.Add(new Vector2(data.pointDataTable[i].trajectory, data.pointDataTable[i].frame));

                            index++;

                            //vertices.Add(data.pointDataTable[i].normed_position + (new Vector3(xvalue[i], yvalue[i], zvalue[i]).normalized * 0.001f));

                            vertices.Add(data.pointDataTable[i].normed_position + (new Vector3(xvalues[i], yvalues[i], zvalues[i])));
                            indices.Add(index);
                            UV3List.Add(new Vector2(data.pointDataTable[i].trajectory, data.pointDataTable[i].frame));

                            index++;
                        }
                        mesh.vertices = vertices.ToArray();
                        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
                        mesh.colors = color.ToArray();
                        mesh.uv4 = UV3List.ToArray();
                        GameObject orientationChild = new GameObject();
                        orientationChild.transform.SetParent(data.transform, false);
                        orientationChild.AddComponent<MeshFilter>();
                        orientationChild.AddComponent<MeshRenderer>();
                        orientationChild.GetComponent<MeshFilter>().mesh = mesh;
                        Material material = new Material(Shader.Find("Genuage/UnlitLineShader"));
                        material.SetFloat("_UpperTimeLimit", data.globalMetaData.timeList.Count - 1);
                        material.SetFloat("_LowerTimeLimit", 0f);

                        orientationChild.GetComponent<MeshRenderer>().material = material;

                        data.orientationObject = orientationChild;
                        
                        orientationModeON = true;

                    
                
            }
        }
    }
}