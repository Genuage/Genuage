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
using System.Threading;
using UnityEngine;
using UnityEngine.Animations;
using VRTK;
using VRTK.GrabAttachMechanics;
using Data;
using UnityEngine.Experimental.PlayerLoop;

namespace VR_Interaction
{


    public class PointSelectorSphere : MonoBehaviour
    {
        // Script to be used on a sphere GameObject that detects points from clouds by collision with a trigger collider.

        public float radius;
        public bool taskOn = false; //When true, the script tries to find points that are located inside the sphere radius.
        public Queue<SphereSelectionThreadHandler> ThreadList; //Queue used to store currently running threads. 
        public int maxThreadsNumber = 1;
        public CloudData data;
        public HashSet<int> selectedPoints = new HashSet<int>();

        bool unitaryTaskOn = false;
        SphereSelectionThreadHandler Thread;

        void Start()
        {
            ThreadList = new Queue<SphereSelectionThreadHandler>();
        }

        // When the sphere touches the trigger collider of a cloud box, this function link the sphere to the box so it can follow its movement and rotation.
        /**
        private void OnTriggerEnter(Collider cloud_box)
        {
            if (taskOn)
            {
                if (cloud_box.tag == "PointCloud")
                {

                    ConstraintSource source = new ConstraintSource();
                    source.sourceTransform = cloud_box.transform;
                    source.weight = 1;
                    this.gameObject.GetComponent<ParentConstraint>().AddSource(source);
                    Vector3 offsetVector = this.transform.position - cloud_box.transform.position;
                    this.gameObject.GetComponent<ParentConstraint>().SetTranslationOffset(0, Quaternion.Inverse(cloud_box.transform.rotation) * offsetVector);
                    this.gameObject.GetComponent<ParentConstraint>().locked = true;
                    this.gameObject.GetComponent<ParentConstraint>().constraintActive = true;

                }
            }
        }
        //If the sphere exits the trigger collider, remove the parent constraint's effects.
        private void OnTriggerExit(Collider cloud_box)
        {
            if (taskOn)
            {
                if (cloud_box.tag == "PointCloud")
                {
                    this.gameObject.GetComponent<ParentConstraint>().constraintActive = false;
                    this.gameObject.GetComponent<ParentConstraint>().RemoveSource(0);
                }
            }
        }
    **/

        //

        private void OnTriggerEnter(Collider cloud_box)
        {
            if (cloud_box.tag == "PointCloud")
            {
                data = cloud_box.transform.parent.GetComponentInChildren<CloudData>();
            }
        }
        private void OnTriggerStay(Collider cloud_box)
        {
            if (taskOn)
            {

                if (cloud_box.tag == "PointCloud")
                {
                    data = cloud_box.transform.parent.GetComponentInChildren<CloudData>();

                    if (ThreadList.Count != 0)
                    {
                        SphereSelectionThreadHandler latestThread = ThreadList.Peek();
                        if (latestThread.isFinished == true)
                        {
                            latestThread = ThreadList.Dequeue();
                            selectedPoints = latestThread.pointSelectionList;

                            latestThread.thread.Join();

                        }
                    }



                    Matrix4x4 world_to_local = data.gameObject.transform.worldToLocalMatrix;
                    Vector3 selfworldpos = transform.position;
                    Vector3 selflocalpos = world_to_local.MultiplyPoint3x4(selfworldpos);

                    Mesh mesh = data.gameObject.GetComponent<MeshFilter>().mesh;
                    Vector3[] points = mesh.vertices;
                    Color[] colors = new Color[points.Length];

                    if (ThreadList.Count < maxThreadsNumber)
                    {
                        SphereSelectionThreadHandler newHandler = new SphereSelectionThreadHandler(radius, data, selflocalpos, colors);
                        Thread newThread = new Thread(new ThreadStart(newHandler.ThreadLoop));
                        newHandler.thread = newThread;
                        newThread.Start();
                        ThreadList.Enqueue(newHandler);
                    }
                }
            }
        }

        public void UpdateSelection()
        {
            CloudUpdater.instance.UpdatePointSelection();

        }

        public void DoSelectionOnce()
        {
            

            Matrix4x4 world_to_local = data.gameObject.transform.worldToLocalMatrix;
            Vector3 selfworldpos = transform.position;
            Vector3 selflocalpos = world_to_local.MultiplyPoint3x4(selfworldpos);

            Mesh mesh = data.gameObject.GetComponent<MeshFilter>().mesh;
            Vector3[] points = mesh.vertices;
            Color[] colors = new Color[points.Length];

            SphereSelectionThreadHandler newHandler = new SphereSelectionThreadHandler(radius, data, selflocalpos, colors);
            Thread newThread = new Thread(new ThreadStart(newHandler.ThreadLoop));
            newHandler.thread = newThread;
            newThread.Start();
            Thread = newHandler;
            unitaryTaskOn = true;
        }

        private void Update()
        {
            if (unitaryTaskOn)
            {
                if (Thread.isFinished == true)
                {
                    unitaryTaskOn = false;

                    selectedPoints = Thread.pointSelectionList;
                    Thread.thread.Join();
                    UpdateSelection();
                    Debug.Log(selectedPoints);
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
            UpdateSelection();
        }
    }

    public class SphereSelectionThreadHandler
    //This thread class is used to find all the points located inside the sphere and assign a new color to them. 
    {
        public float radius;
        
        public CloudData data;
        public Vector3 selflocalpos;
        public bool isFinished = false;
        public Color[] colors;
        public HashSet<int> pointSelectionList;
        public Thread thread;

        public SphereSelectionThreadHandler(float radius, CloudData cloud, Vector3 selflocalpos, Color[] colors)
        {
            this.radius = radius;
            this.data = cloud;
            this.selflocalpos = selflocalpos;
            this.colors = colors;
            this.pointSelectionList = new HashSet<int>();
        }

        public void ThreadLoop()
        {
            isFinished = false;
            foreach (KeyValuePair<int, PointData> item in data.pointDataTable)
            {
                float distance = Vector3.Distance(selflocalpos, item.Value.normed_position);
                if (distance <= radius / data.globalMetaData.scale.x && distance <= radius / data.globalMetaData.scale.y && distance <= radius / data.globalMetaData.scale.z)
                {
                    //colors[item.Key] = Color.green;
                    //item.Value.color = Color.green;
                    
                    pointSelectionList.Add(item.Key);
                }
               

            }
            //Debug.Log(pointSelectionList.Count + "Point selected (SphereSelector");
            isFinished = true;

        }
    }
}

