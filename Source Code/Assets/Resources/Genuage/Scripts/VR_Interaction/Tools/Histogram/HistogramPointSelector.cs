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

using Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VR_Interaction
{


    public class HistogramPointSelector : MonoBehaviour
    {
        CloudData currcloud;

        public int sectionsNumber;
        public List<GameObject> circleList = new List<GameObject>();
        public List<Vector3> circlePositionsList = new List<Vector3>();
        public float radius;
        public float length;
        public List<Color> colorList;
        public GameObject canvasPrefab;
        public GameObject canvascontainer = null;
        public GameObject canvas;
        public List<GameObject> histogramBarsList = new List<GameObject>();
        List<int> pointCountsbySections;

        public List<float> xValues;
        public List<int> yValues;


        List<int> selectedPoints = new List<int>();
        List<Color> selectedPointsColors = new List<Color>();

        private Queue<HistogramPointSelectionThreadHandler> SelectionThreadQueue;
        bool pointSelectionJobON = false;

        public void Awake()
        {
            if (!canvasPrefab)
            {
                canvasPrefab = VRWindowsManager.instance.VRHistogramCanvasPrefab;
            }
            colorList = new List<Color>();
            colorList.Add(Color.red);
            colorList.Add(Color.magenta);
            colorList.Add(Color.blue);
            colorList.Add(Color.grey);
            colorList.Add(Color.green);
            colorList.Add(Color.yellow);
            colorList.Add(Color.cyan);

            SelectionThreadQueue = new Queue<HistogramPointSelectionThreadHandler>();

        }


        public void FindPointsProto(List<GameObject> circleList, List<Vector3> circlePositionsList)
        {

            currcloud = CloudUpdater.instance.LoadCurrentStatus();

            List<Vector3> CircleLocalPositionsList = new List<Vector3>();
            foreach(var v in circlePositionsList)
            {
                CircleLocalPositionsList.Add(currcloud.transform.worldToLocalMatrix.MultiplyPoint3x4(v));
            }

            HistogramPointSelectionThreadHandler ThreadHandle = new HistogramPointSelectionThreadHandler();
            ThreadHandle.circlePositionsList = circlePositionsList;
            ThreadHandle.circleLocalPositionsList = CircleLocalPositionsList;
            ThreadHandle.radius = radius;
            ThreadHandle.colorList = colorList;
            ThreadHandle.currcloud = currcloud;
            ThreadHandle.StartThread();
            SelectionThreadQueue.Enqueue(ThreadHandle);
            pointSelectionJobON = true;

            this.circleList = circleList;
            this.circlePositionsList = circlePositionsList;
            /**
            Mesh newmesh = currcloud.gameObject.GetComponent<MeshFilter>().mesh;
            newmesh.colors = colors;
            currcloud.gameObject.GetComponent<MeshFilter>().mesh = newmesh;
            CreateCanvas(pointCountsbySections);
            yValues = pointCountsbySections;
            **/
        }

        public void CreateCanvas(List<int> yValues)
        {
            int j = 0;

            int max = Mathf.Max(yValues.ToArray());
            if (max != 0f)
            {

                if (!canvascontainer)
                {


                    canvascontainer = Instantiate(canvasPrefab) as GameObject;
                    canvascontainer.transform.position = transform.forward + transform.up;
                    canvas = canvascontainer.transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
                    histogramBarsList = new List<GameObject>();
                }
                else
                {
                    for(int i = 0; i < histogramBarsList.Count; i++)
                    {
                        Destroy(histogramBarsList[i].transform.parent.gameObject);
                        //histogramBarsList.RemoveAt(i);
                    }
                    histogramBarsList.Clear();
                }
                /**
                GameObject poscontainer = new GameObject();
                poscontainer.AddComponent<RectTransform>();
                poscontainer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                poscontainer.GetComponent<RectTransform>().anchorMin = Vector2.zero;
                poscontainer.GetComponent<RectTransform>().anchorMax = Vector2.zero;
                poscontainer.transform.SetParent(canvas.transform, false);

                GameObject Xtextpos = new GameObject("text", typeof(Text));
                Xtextpos.transform.SetParent(poscontainer.transform, false);
                RectTransform xrtctpos = Xtextpos.GetComponent<RectTransform>();
                xrtctpos.sizeDelta = new Vector2(70f, 25f);
                xrtctpos.localScale = new Vector3(0.005f, 0.005f, 1f);
                xrtctpos.anchoredPosition = new Vector2(0f, -0.25f);
                xrtctpos.anchorMin = Vector2.zero;
                xrtctpos.anchorMax = Vector2.zero;

                xrtctpos.Rotate(new Vector3(0f, 0f, 90f));
                Xtextpos.GetComponent<Text>().alignment = TextAnchor.UpperCenter;
                Xtextpos.GetComponent<Text>().color = Color.white;
                Xtextpos.GetComponent<Text>().text = "positions";
                Xtextpos.GetComponent<Text>().font = DesktopApplication.instance.defaultFont;
                histogramBarsList.Add(Xtextpos);
                **/

                for (int i = 0; i < yValues.Count; i++)
                {
                    GameObject container = new GameObject();
                    container.AddComponent<RectTransform>();
                    container.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    container.GetComponent<RectTransform>().anchorMin = Vector2.zero;
                    container.GetComponent<RectTransform>().anchorMax = Vector2.zero;
                    container.transform.SetParent(canvas.transform, false);

                    GameObject Xtext = new GameObject("text", typeof(Text));
                    Xtext.transform.SetParent(container.transform, false);
                    RectTransform xrtct = Xtext.GetComponent<RectTransform>();
                    xrtct.sizeDelta = new Vector2(70f, 17f);
                    xrtct.localScale = new Vector3(0.005f, 0.005f, 1f);
                    xrtct.anchoredPosition = new Vector2(0f, -0.25f);
                    xrtct.anchorMin = Vector2.zero;
                    xrtct.anchorMax = Vector2.zero;
                    
                    xrtct.Rotate(new Vector3(0f, 0f, 90f));
                    Xtext.GetComponent<Text>().alignment = TextAnchor.UpperCenter;
                    Xtext.GetComponent<Text>().color = Color.white;
                    Xtext.GetComponent<Text>().text = System.Math.Round(xValues[i],1).ToString();
                    Xtext.GetComponent<Text>().font = DesktopApplication.instance.defaultFont;


                    GameObject bar = new GameObject("bar", typeof(Image));
                    bar.transform.SetParent(container.transform, false);
                    RectTransform rtc = bar.GetComponent<RectTransform>();
                    rtc.sizeDelta = new Vector2(2.85f, 1f);
                    rtc.anchoredPosition = Vector2.zero;
                    rtc.anchorMin = Vector2.zero;
                    rtc.anchorMax = Vector2.zero;
                    rtc.pivot = new Vector2(.5f, 0f);
                    float scale = (float)yValues[i] / max;
                    //Debug.Log("y : "+yValues[i]+"max : " + max + "Calcul : " + scale);
                    rtc.localScale = new Vector3(1f / yValues.Count, scale, 1f);
                    rtc.GetComponent<Image>().color = new Color(0.8f, 0.8f, 1f); ;
                    

                    GameObject text = new GameObject("text", typeof(Text));
                    text.transform.SetParent(container.transform, false);
                    RectTransform rtct = text.GetComponent<RectTransform>();
                    rtct.sizeDelta = new Vector2(70f, 17f);
                    rtct.localScale = new Vector3(0.005f, 0.005f, 1f);
                    rtct.anchoredPosition = new Vector2(0f,1.1f);
                    rtct.anchorMin = Vector2.zero;
                    rtct.anchorMax = Vector2.zero;
                    //rtct.rotation.z = 90f;
                    rtct.Rotate(new Vector3(0f, 0f, 90f));
                    text.GetComponent<Text>().alignment = TextAnchor.UpperCenter;
                    text.GetComponent<Text>().color = Color.white;
                    text.GetComponent<Text>().text = pointCountsbySections[i].ToString();
                    text.GetComponent<Text>().font = DesktopApplication.instance.defaultFont;
                    histogramBarsList.Add(bar);

                    



                    j++;
                    if (j > colorList.Count - 1)
                    {
                        j = 0;
                    }

                }
            }
        }
        private void OnDestroy()
        {
            Destroy(canvascontainer);
        }

        private void OnDisable()
        {
            if (canvas)
            {
                canvascontainer.SetActive(false);
                //CloudData data = CloudUpdater.instance.LoadCurrentStatus();
                //CloudUpdater.instance.ChangeCurrentColorMap(data.globalMetaData.colormapName, data.globalMetaData.colormapReversed);

            }
        }

        public void DisableHistogramCanvas()
        {
            canvascontainer.SetActive(false);
        }

        public void DestroyHistogramCanvas()
        {
            Destroy(canvascontainer);
        }

        private void OnEnable()
        {
            if (canvas)
            {
                
                canvascontainer.SetActive(true);

                circlePositionsList.Clear();
                foreach (GameObject go in circleList)
                {
                    circlePositionsList.Add(go.transform.position);
                }

                FindPointsProto(circleList, circlePositionsList);
            }
        }

        private void Update()
        {
            if (pointSelectionJobON)
            {
                if(SelectionThreadQueue.Count != 0)
                {
                    HistogramPointSelectionThreadHandler ThreadHandle = SelectionThreadQueue.Peek();
                    if (ThreadHandle.isRunning == false)
                    {
                        ThreadHandle = SelectionThreadQueue.Dequeue();
                        Mesh newmesh = currcloud.gameObject.GetComponent<MeshFilter>().mesh;
                        newmesh.colors = ThreadHandle.colors;
                        currcloud.gameObject.GetComponent<MeshFilter>().mesh = newmesh;
                        pointCountsbySections = ThreadHandle.pointCountsbySections;
                        xValues = ThreadHandle.xValues;
                        yValues = pointCountsbySections;
                        selectedPoints = ThreadHandle.SelectedPointsList;
                        selectedPointsColors = ThreadHandle.SelectedPointsColorList;
                        ThreadHandle.StopThread();
                        CreateCanvas(pointCountsbySections);

                    }

                }
                else
                {
                    pointSelectionJobON = false;

                }
            }
        }

    }

    public class HistogramPointSelectionThreadHandler : RunnableThread
    {
        public List<Vector3> circlePositionsList;
        public List<Vector3> circleLocalPositionsList;

        public float radius;
        public List<Color> colorList;
        public CloudData currcloud;

        public List<int> pointCountsbySections;
        public List<float> xValues;
        public Color[] colors;

        public List<int> SelectedPointsList;
        public List<Color> SelectedPointsColorList;

        protected override void Run()
        {
            Debug.Log("positionLIst : " + circlePositionsList.Count);
            Debug.Log("circleLocalPositionsList : " + circleLocalPositionsList.Count);


            SelectedPointsList = new List<int>();
            SelectedPointsColorList = new List<Color>();
            pointCountsbySections = new List<int>();
            xValues = new List<float>();

            int j = 0;

            int[] pointschecked = new int[currcloud.pointDataTable.Count]; // Used to remember which points have been checked and which haven't.
            for (int u = 0; u < pointschecked.Length; u++)
            {
                pointschecked[u] = 0;
            }
            colors = new Color[currcloud.pointDataTable.Count];

            int counter = 0;

            for (int i = 0; i < circlePositionsList.Count - 1; i++)
            {
                counter = 0;
                Vector3 middlepoint = (circlePositionsList[i + 1] + circlePositionsList[i]) / 2;
                float distance = Vector3.Distance(circlePositionsList[0], middlepoint) * currcloud.globalMetaData.maxRange;
                xValues.Add(distance);
                foreach (var kvp in currcloud.pointDataTable)
                {
                    if (!currcloud.pointMetaDataTable[kvp.Key].isHidden)
                    {


                        Vector3 circle1LocalPosition = circleLocalPositionsList[i];
                        Vector3 circle2LocalPosition = circleLocalPositionsList[i + 1];
                        float test1 = Vector3.Dot(kvp.Value.normed_position - circle1LocalPosition, circle2LocalPosition - circle1LocalPosition);
                        float test2 = Vector3.Dot(kvp.Value.normed_position - circle2LocalPosition, circle2LocalPosition - circle1LocalPosition);
                        if (pointschecked[kvp.Key] == 0)
                        {
                            if (test1 >= 0 && test2 <= 0) //find if the point is between the two circles
                            {
                                float test3 = Vector3.Magnitude(Vector3.Cross(kvp.Value.normed_position - circle1LocalPosition, circle2LocalPosition
                                                      - circle1LocalPosition)) / Vector3.Magnitude(circle2LocalPosition - circle1LocalPosition);
                                if (test3 <= radius) // find if the point is inside the cylinder
                                {
                                    counter++;
                                    pointschecked[kvp.Key] = 1;
                                    SelectedPointsList.Add(kvp.Key);
                                    SelectedPointsColorList.Add(colorList[j]);
                                }


                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {
                        pointschecked[kvp.Key] = 1;

                    }
                }
                //Debug.Log(i);
                pointCountsbySections.Add(counter);
                //Debug.Log("point found in section : "+(i)+" / "+pointCountsbySections[i]);
                //Debug.Log(radius);
                j++;
                if (j > colorList.Count - 1)
                {
                    j = 0;
                }

            }

            isRunning = false;

        }

    }
}