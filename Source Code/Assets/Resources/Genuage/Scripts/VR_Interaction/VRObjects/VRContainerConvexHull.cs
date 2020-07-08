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
using VR_Interaction.Convex_Hull;
using Data;

namespace VR_Interaction
{


    public class VRContainerConvexHull : VRContainer
    {
        List<Vector3> localPositionList;
        List<Vector3> positionList;
        List<GameObject> pointList;
        public ConvexHull convexhull;


        public override void AddVRObject(VRObject obj, int id = -1)
        {
            base.AddVRObject(obj, id);
            GenerateHull();
        }

        public override void DeleteVRObject(int id)
        {
            base.DeleteVRObject(id);
            GenerateHull();
        }
        public void GenerateHull()
        {
            localPositionList = new List<Vector3>();
            positionList = new List<Vector3>();

            pointList = new List<GameObject>();
            List<int> indices = new List<int>();

            foreach (var kvp in VRObjectsDict)
            {
                pointList.Add(kvp.Value.gameObject);
                localPositionList.Add(kvp.Value.transform.localPosition);
                positionList.Add(kvp.Value.transform.position);
            }

            if (VRObjectsDict.Count == 1)
            {
                if (!gameObject.GetComponent<CloudChildConvexHull>())
                {
                    gameObject.AddComponent<CloudChildConvexHull>();

                }
                gameObject.GetComponent<CloudChildConvexHull>().id = id;
                Debug.Log(gameObject.GetComponent<CloudChildConvexHull>().id);
                gameObject.AddComponent<BoxCollider>();


            }

            if (VRObjectsDict.Count == 3)
            {
                GetComponent<BoxCollider>().enabled = false;

                mesh = new Mesh();
                foreach (var kvp in VRObjectsDict)
                {
                    indices.Add(kvp.Key);
                    foreach (var kvp2 in VRObjectsDict)
                    {
                        if (kvp2.Key != kvp.Key)
                        {
                            indices.Add(kvp2.Key);
                        }
                    }
                }
                meshrenderer.material = new Material(Shader.Find("Standard"));
                mesh.vertices = localPositionList.ToArray();
                mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

                meshfilter.mesh = mesh;

            }
            if (VRObjectsDict.Count >= 4)
            {
                convexhull = new ConvexHull();
                convexhull.pointList = pointList;
                convexhull.positionList = localPositionList;
                Mesh mesh = convexhull.CreateMesh();
                meshfilter.mesh = mesh;

                if (!GetComponent<PointSelectorConvexHull>())
                {
                    gameObject.AddComponent<PointSelectorConvexHull>();
                }
                //GetComponent<PointSelectorConvexHull>().creator = this;

                if (!GetComponent<MeshCollider>())
                {
                    gameObject.AddComponent<MeshCollider>();
                    //selector.GetComponent<MeshCollider>().convex = true;
                    //selector.GetComponent<MeshCollider>().isTrigger = true;
                }

                GetComponent<PointSelectorConvexHull>().hullFaces = convexhull.faces;
                GetComponent<PointSelectorConvexHull>().hullPointList = pointList;
                GetComponent<PointSelectorConvexHull>().hullPositionList = positionList;
                GetComponent<PointSelectorConvexHull>().findPointsInsideHull();


            }
        }

        private void OnDisable()
        {
            CloudUpdater.instance.UpdatePointSelection();
        }

        private void OnEnable()
        {
            CloudUpdater.instance.UpdatePointSelection();

        }

        protected override void OnDestroy()
        {
            VRObjectsManager.instance.ContainerDeleted(id, "ConvexHull");
        }

    }
}