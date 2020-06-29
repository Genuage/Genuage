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
using VRTK;
using Data;

namespace VR_Interaction
{
    public class MultiClippingPlane : IControllerTool
    {
        protected VRTK_ControllerEvents _controller;

        GameObject square;

        public MultiClippingPlane(VRTK_ControllerEvents controller)
        {
        }

        public override void OnToolActivated()
        {
            _controller = GetComponent<VRTK_ControllerEvents>();

            _controller.TouchpadPressed += OnTouchpadPressed;

            Debug.Log("multi clipping plane activated");
            Shader.EnableKeyword("CLIPPING_PLANE");
            GetComponent<SendMatrixController>().ActivateClippingPlane();
            CloudSelector.instance.OnSelectionChange += ReloadBox;
            //Debug.Log("check");
            if (!CloudSelector.instance.noSelection)
            {
                //Debug.Log("check");
                GetComponent<SendMatrixController>().box = CloudUpdater.instance.LoadCurrentStatus().transform.parent.GetComponent<CloudObjectRefference>().box.transform;

                GetComponent<SendMatrixController>().cloud = CloudUpdater.instance.LoadCurrentStatus().transform.parent.GetComponent<CloudObjectRefference>().cloud.transform;
                CreateWireSquare();
                GetComponent<SendMatrixController>().square = square.transform;

            }
            else
            {
                //Debug.Log("negativecheck");
                OnToolDeactivated();
                return;
            }

        }

        public void CreateWireSquare()
        {
            if (square)
            {
                square.SetActive(true);

            }
            else
            {
                square = new GameObject();
                square.AddComponent<MeshFilter>();
                square.AddComponent<MeshRenderer>();
                square.transform.SetParent(this.transform, false);
                square.transform.localPosition = Vector3.forward * 0.05f;
                Mesh mesh = new Mesh();
                List<Vector3> vertices = new List<Vector3>();
                vertices.Add(new Vector3(-0.1f, -0.1f, 0));
                vertices.Add(new Vector3(-0.1f, 0.1f, 0));
                vertices.Add(new Vector3(0.1f, 0.1f, 0));
                vertices.Add(new Vector3(0.1f, -0.1f, 0));

                List<int> indices = new List<int>();
                indices.Add(0);
                indices.Add(1);
                indices.Add(1);
                indices.Add(2);
                indices.Add(2);
                indices.Add(3);
                indices.Add(3);
                indices.Add(0);
                mesh.vertices = vertices.ToArray();
                mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
                square.GetComponent<MeshFilter>().mesh = mesh;
                Material _material = new Material(Shader.Find("Unlit/Color"));
                _material.SetColor("_Color", Color.white);
                square.GetComponent<MeshRenderer>().material = _material;

            }

        }

        private void OnTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            GetComponent<SendMatrixController>().CreateFixedClippingPlane();
        }

        private void DestroyWireSquare()
        {
            if (square)
            {
                square.SetActive(false);
            }
        }

        public void ReloadBox(int id)
        {
            GetComponent<SendMatrixController>().box = CloudUpdater.instance.LoadCurrentStatus().transform.parent.GetComponent<CloudObjectRefference>().box.transform;

            GetComponent<SendMatrixController>().cloud = CloudUpdater.instance.LoadCurrentStatus().transform.parent.GetComponent<CloudObjectRefference>().cloud.transform;
            //GetComponent<SendMatrixController>().ReloadGoArray();
        }

        public override void OnToolDeactivated()
        {
            Shader.DisableKeyword("CLIPPING_PLANE");
            /**
            Shader.DisableKeyword("FIXED_CLIPPING_PLANE_1");
            Shader.DisableKeyword("FIXED_CLIPPING_PLANE_2");

            Shader.DisableKeyword("FIXED_CLIPPING_PLANE_3");

            Shader.DisableKeyword("FIXED_CLIPPING_PLANE_4");
            Shader.DisableKeyword("FIXED_CLIPPING_PLANE_5");
            Shader.DisableKeyword("FIXED_CLIPPING_PLANE_6");
            Shader.DisableKeyword("FIXED_CLIPPING_PLANE_7");
            Shader.DisableKeyword("FIXED_CLIPPING_PLANE_8");
            Shader.DisableKeyword("FIXED_CLIPPING_PLANE_9");
            Shader.DisableKeyword("FIXED_CLIPPING_PLANE_10");
            **/

            DestroyWireSquare();
            GetComponent<SendMatrixController>().DeactivateClippingPlane();
            CloudSelector.instance.OnSelectionChange -= ReloadBox;
            _controller.TouchpadPressed -= OnTouchpadPressed;


        }

        public override void OnDisabled()
        {
            CloudSelector.instance.OnSelectionChange -= ReloadBox;

            GetComponent<MultiClippingPlane>().enabled = false;

        }
    }
}