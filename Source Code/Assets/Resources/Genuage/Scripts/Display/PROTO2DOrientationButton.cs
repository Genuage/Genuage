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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{


    public class PROTO2DOrientationButton : IButtonScript
    {
        bool orientation2DmodeOn = false;
        public InputField input;
        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
        }
        public override void Execute()
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();

            //int thetaindex;
            if (!orientation2DmodeOn)
            {


                if (data.orientationObject)
                {
                    if (data.orientationObject)
                    {
                        GameObject obj = data.orientationObject;
                        data.orientationObject = null;
                        Destroy(obj);
                    }
                }

                


                    List<float> xvalues = new List<float>();
                    List<float> yvalues = new List<float>();
                    List<float> zvalues = new List<float>();


                    List<Color> color = new List<Color>();

                List<Vector2> UV1List = new List<Vector2>();
                List<Vector2> UV2List = new List<Vector2>();

                List<Vector2> UV3List = new List<Vector2>();
                List<float> coloruv = new List<float>();

                foreach (var kvp in data.pointDataTable)
                    {
                    float theta = kvp.Value.theta_angle;
                        xvalues.Add(0.0035f * Mathf.Cos(  theta));
                        yvalues.Add(0.0035f * Mathf.Sin( theta));
                        zvalues.Add(0f);
                        color.Add(Color.HSVToRGB( theta / 360f, 0.75f, 0.55f));
                        color.Add(Color.HSVToRGB(theta / 360f, 0.75f, 0.55f));
                    coloruv.Add((Mathf.Rad2Deg * theta) / 360f);

                }


                Mesh mesh = new Mesh();
                    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

                    List<Vector3> vertices = new List<Vector3>();
                    List<int> indices = new List<int>();

                    int index = 0;
                float hidden = 0f;
                float selected = 0f;

                for (int i = 0; i < xvalues.Count; i++)
                    {
                        if (!data.pointMetaDataTable[i].isHidden)
                        {
                        hidden = 0f;
                        selected = 0f;
                        if (data.pointMetaDataTable[i].isHidden)
                        {
                            hidden = 1f;
                        }
                        if (data.pointMetaDataTable[i].isSelected)
                        {
                            selected = 1f;
                        }

                        vertices.Add(data.pointDataTable[i].normed_position - (new Vector3(xvalues[i], yvalues[i], zvalues[i])));
                                //color.Add(Color.blue);
                            indices.Add(index);
                        UV3List.Add(new Vector2(data.pointDataTable[i].trajectory, data.pointDataTable[i].frame));
                        UV1List.Add(new Vector2(coloruv[i], data.pointDataTable[i].pointID));
                        UV2List.Add(new Vector2(selected, hidden));

                        index++;
                            vertices.Add(data.pointDataTable[i].normed_position + (new Vector3(xvalues[i], yvalues[i], zvalues[i])));
                            //color.Add(Color.blue);
                            indices.Add(index);
                        UV3List.Add(new Vector2(data.pointDataTable[i].trajectory, data.pointDataTable[i].frame));
                        UV1List.Add(new Vector2(coloruv[i], data.pointDataTable[i].pointID));
                        UV2List.Add(new Vector2(selected, hidden));

                        index++;
                        }
                    }
                    mesh.vertices = vertices.ToArray();
                    mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
                mesh.uv2 = UV1List.ToArray();
                mesh.uv3 = UV2List.ToArray();
                mesh.uv4 = UV3List.ToArray();

                GameObject child = new GameObject();
                    child.transform.SetParent(data.transform, false);
                    child.AddComponent<MeshFilter>();
                    child.AddComponent<MeshRenderer>();
                    child.GetComponent<MeshFilter>().mesh = mesh;
                    Material material = new Material(Shader.Find("Genuage/UnlitLineShader"));
                material.SetTexture("_ColorTex", ColorMapManager.instance.GetColorMap("jet").texture);

                material.SetFloat("_UpperTimeLimit", data.globalMetaData.timeList.Count - 1);
                material.SetFloat("_LowerTimeLimit", 0f);


                child.GetComponent<MeshRenderer>().material = material;
                    data.orientationObject = child;
                    orientation2DmodeOn = true;
                }

            
               
        
            else
            {
                data.orientationObject.GetComponent<MeshRenderer>().enabled = false;
                orientation2DmodeOn = false;
            }
        }
    }
    
}