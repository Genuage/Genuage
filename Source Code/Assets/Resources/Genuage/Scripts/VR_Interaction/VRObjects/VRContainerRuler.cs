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
using Data;

namespace VR_Interaction
{

    public class VRContainerRuler : VRContainer
    {
        public List<DistanceMap> distanceList;
        public float maxRange = -1f;

        protected override void Awake()
        {
            base.Awake();
            distanceList = new List<DistanceMap>();
        }

        public override void DeleteVRObject(int id)
        {
            int v = -1;
            base.DeleteVRObject(id);
            for (int i = 0; i < distanceList.Count; i++)
            {
                if(distanceList[i].pointID == id)
                {
                    v = i;
                    break;
                }
            }
            if(v != -1)
            {
                distanceList.RemoveAt(v);
            }

            List<DistanceMap> NewDistanceList = new List<DistanceMap>();

            foreach(DistanceMap d in distanceList)
            {
                NewDistanceList.Add(d);
            }
            distanceList = NewDistanceList;

            ReloadMesh();
            ReloadDistanceList();


        }

        public override void AddVRObject(VRObject obj, int id = -1)
        {
            base.AddVRObject(obj, id);
            if(id != -1)
            {
                CalculateDistance(id);

            }
            else
            {
                CalculateDistance(idToAssignNext - 1);
            }
            obj.transform.GetChild(0).gameObject.GetComponent<TextMesh>().color = color;

            obj.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = distanceList[distanceList.Count-1].distance.ToString();
            ReloadMesh();
        }

        public void ReloadDistanceList()
        {
            distanceList[0].distance = 0;
            for (int i = 1; i < distanceList.Count; i++)
            {
                distanceList[i].distance = distanceList[i - 1].distance + (Vector3.Distance(VRObjectsDict[distanceList[i - 1].pointID].transform.position, VRObjectsDict[distanceList[i].pointID].transform.position) * CloudUpdater.instance.LoadCurrentStatus().globalMetaData.maxRange);
                VRObjectsDict[distanceList[i].pointID].transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = distanceList[i].distance.ToString();
            }
        }

        public void CalculateDistance(int ptID)
        {
            float distance;
            if (distanceList.Count == 0)
            {
                distance = 0;
            }
            else
            {
                if (!CloudSelector.instance.noSelection && maxRange == -1)
                {
                    maxRange = CloudUpdater.instance.LoadCurrentStatus().globalMetaData.maxRange;
                }
                if (maxRange == -1f)
                {
                    distance = distanceList[distanceList.Count - 1].distance + Vector3.Distance(VRObjectsDict[distanceList[distanceList.Count - 1].pointID].transform.position, VRObjectsDict[ptID].transform.position);

                }
                else
                {
                    distance = distanceList[distanceList.Count - 1].distance + (Vector3.Distance(VRObjectsDict[distanceList[distanceList.Count - 1].pointID].transform.position, VRObjectsDict[ptID].transform.position)) * maxRange;
                    //Debug.Log(CloudUpdater.instance.LoadCurrentStatus().globalMetaData.maxRange);
                }

            }
            distanceList.Add(new DistanceMap(distance, ptID));

        }

        public void ReloadMesh()
        {
            mesh = new Mesh();
            Vector3[] positionArray = new Vector3[VRObjectsDict.Count];
            List<int> indices = new List<int>();

            for(int j = 0; j < distanceList.Count; j++)
            {
                positionArray[j] = VRObjectsDict[distanceList[j].pointID].transform.localPosition;
            }


            for( int i = 1; i < VRObjectsDict.Count; i++)
            {
                indices.Add(i - 1);
                indices.Add(i);

            }

            meshrenderer.material = new Material(Shader.Find("Standard"));
            
            mesh.vertices = positionArray;
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            meshfilter.mesh = mesh;

        }

        protected override void OnDestroy()
        {
            Debug.Log(id);
            VRObjectsManager.instance.ContainerDeleted(id, "Ruler");
        }

    }

    public class DistanceMap
    {
        public int pointID;
        public float distance;

        public DistanceMap(float dist, int id)
        {
            distance = dist;
            pointID = id;
        }
    }

}