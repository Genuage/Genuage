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
using UnityEngine.Animations;
using VR_Interaction.Convex_Hull;
using VRTK;
using Data;

namespace VR_Interaction
{


    public class PointSelectorConvexHull : MonoBehaviour
    {
        public CloudData cloud_data;
        public MeshCollider m_collider;
        //public ConvexHullCreator creator;
        public Dictionary<int, Face> hullFaces;
        public List<GameObject> hullPointList;
        public List<Vector3> hullPositionList;

        public bool parent_activated;
        public bool threadON;
        private PointSelectorConvexHullThread thread;

        public HashSet<int> selectedPoints = new HashSet<int>();

        //public Transform target;

        public bool selectionNotDone;
       
        // Start is called before the first frame update
        void Start()
        {
            selectionNotDone = true;
            parent_activated = false;
            threadON = false;
        }

        private void OnTriggerEnter(Collider cloud_box)
        {
            if (cloud_box.tag == "PointCloud")
            {
                /**

                if (!parent_activated)


                {
                    
                    GameObject container = new GameObject("container");
                    container.transform.position = this.transform.position;
                    container.transform.SetParent(cloud_box.transform,true);
                    gameObject.AddComponent<VRTK_TransformFollow>();
                    gameObject.GetComponent<VRTK_TransformFollow>().followsScale = false;

                    gameObject.GetComponent<VRTK_TransformFollow>().gameObjectToChange = this.gameObject;
                    gameObject.GetComponent<VRTK_TransformFollow>().gameObjectToFollow = container;
                    gameObject.GetComponent<VRTK_TransformFollow>().moment = VRTK_TransformFollow.FollowMoment.OnLateUpdate;

                }
                **/
                cloud_data = cloud_box.transform.parent.GetComponentInChildren<CloudData>();
                if (selectionNotDone)
                {
                    findPointsInsideHull();
                    selectionNotDone = false;
                }
                
            }
        }
        /**
        private void Update()
        {
            if (parent_activated)
            {
                //transform.LookAt(target.position);

                Vector3 offsetVector =  target.transform.position - this.transform.position ;
                transform.rotation = target.rotation;
                if ((transform.position - target.position).magnitude > 0.01f)
                {
                    transform.position+=offsetVector;
                }
            }
        }
        **/

        public void findPointsInsideHull()
        {
            if (cloud_data == null)
            {

                return;
            }
            /**
            for (int i = 0; i<hullPositionList.Count;i++)
            {

                hullPositionList[i] = hullPointList[i].transform.position;
            }
    **/
    
            Color[] colors = new Color[cloud_data.pointDataTable.Count];
           
            Matrix4x4 local_to_world = cloud_data.transform.localToWorldMatrix;
            Matrix4x4 world_to_local = cloud_data.transform.worldToLocalMatrix;


            thread = new PointSelectorConvexHullThread();
            thread.data = cloud_data;
            thread.local_to_world = local_to_world;
            thread.world_to_local = world_to_local;
            thread.rotation = transform.rotation;
            thread.colors = colors;
            thread.hullFaces = hullFaces;
            thread.hullPointList = hullPointList;
            thread.hullPositionList = hullPositionList;
            thread.StartThread();
            threadON = true;

        }

        private void Update()
        {
            if (threadON)
            {
                if (thread.isRunning == false)
                {
                    threadON = false;
                    //Mesh newmesh = cloud_data.gameObject.GetComponent<MeshFilter>().mesh;
                    //newmesh.colors = thread.colors;
                    selectedPoints = thread.pointSelectionList;
                    thread.StopThread();

                    CloudUpdater.instance.UpdatePointSelection();
                    

                    //CloudSelector.instance.selectedPointList = thread.pointSelectionList;
                    //CloudSelector.instance.pointSelectionID = cloud_data.globalMetaData.cloud_id;
                    //cloud_data.gameObject.GetComponent<MeshFilter>().mesh = newmesh;
                }
            }
        }

        private void OnEnable()
        {
            //CloudUpdater.instance.ColorOverride(selectedPoints, false);

        }

        private void OnDisable()
        {
            //CloudUpdater.instance.ColorOverride(selectedPoints, true);
        }

        private void OnDestroy()
        {
            CloudUpdater.instance.UpdatePointSelection();

        }
    }

    public class PointSelectorConvexHullThread : RunnableThread
    {
        public CloudData data;
        public Dictionary<int, Face> hullFaces;
        public List<GameObject> hullPointList;
        public List<Vector3> hullPositionList;
        public Matrix4x4 local_to_world;
        public Matrix4x4 world_to_local;

        public Quaternion rotation;

        public Color[] colors;
        public HashSet<int> pointSelectionList;


        protected override void Run()
        {
            //int _inside = 0;
            //int _outside = 0;
            pointSelectionList = new HashSet<int>();

            

            foreach (KeyValuePair<int, PointData> item in data.pointDataTable)
            {
                Vector3 pointWorldPosition = local_to_world.MultiplyPoint3x4(item.Value.normed_position);
                bool outside = false;

                foreach (KeyValuePair<int, Face> kvp in hullFaces)
                {
                    Vector3 hullLocalPosition = world_to_local.MultiplyPoint3x4(hullPositionList[kvp.Value.Vertex0]);

                    float dist = Vector3.Dot(kvp.Value.normal, item.Value.normed_position - hullLocalPosition);

                    //float dist = Vector3.Dot(kvp.Value.normal, pointWorldPosition - (Quaternion.Inverse(rotation) * hullPositionList[kvp.Value.Vertex0] ));

                    //float dist = Vector3.Dot(kvp.Value.normal, pointWorldPosition - (Quaternion.Inverse(rotation) * hullPositionList[kvp.Value.Vertex0]));

                    if (dist >= 0f)
                    {
                        //Debug.Log(dist);
                        outside = true;
                        //colors[item.Key] = item.Value.color;
                        //_outside++;
                        break;
                    }
                }

                if (!outside)
                {
                    //colors[item.Key] = Color.green;
                    //item.Value.color = Color.green;
                    pointSelectionList.Add(item.Key);
                    //_inside++;
                }
            }

            //Debug.Log(_inside + " points inside");
            //Debug.Log(_outside + " points outside");
            isRunning = false;
        }
    }
}