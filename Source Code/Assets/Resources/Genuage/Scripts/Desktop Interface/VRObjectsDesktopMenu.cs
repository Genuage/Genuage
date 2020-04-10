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
using UnityEngine.UI;
using VR_Interaction;
using Data;

namespace DesktopInterface
{


    public class VRObjectsDesktopMenu : MonoBehaviour
    {
        public GameObject ButtonPrefab;
        public GameObject CounterViewPort;
        public GameObject RulerViewPort;
        public GameObject AngleMeasureViewPort;
        public GameObject HistogramViewPort;
        public GameObject SphereViewPort;
        public GameObject ConvexHullViewPort;

        public Dictionary<string, Dictionary<int, GameObject>> ButtonDicts;
        public Dictionary<int, GameObject> CounterButtonsDict;
        public Dictionary<int, GameObject> RulerButtonsDict;
        public Dictionary<int, GameObject> AngleMeasureButtonsDict;
        public Dictionary<int, GameObject> HistogramButtonsDict;
        public Dictionary<int, GameObject> SphereButtonsDict;
        public Dictionary<int, GameObject> ConvexHullButtonsDict;

        public void Awake()
        {
            ButtonDicts = new Dictionary<string, Dictionary<int, GameObject>>();
            CounterButtonsDict = new Dictionary<int, GameObject>();
            RulerButtonsDict = new Dictionary<int, GameObject>();
            AngleMeasureButtonsDict = new Dictionary<int, GameObject>();
            HistogramButtonsDict = new Dictionary<int, GameObject>();
            SphereButtonsDict = new Dictionary<int, GameObject>();
            ConvexHullButtonsDict = new Dictionary<int, GameObject>();

            ButtonDicts.Add("Counter", CounterButtonsDict);
            ButtonDicts.Add("Ruler", RulerButtonsDict);
            ButtonDicts.Add("AngleMeasure", AngleMeasureButtonsDict);
            ButtonDicts.Add("Histogram", HistogramButtonsDict);
            ButtonDicts.Add("Sphere", SphereButtonsDict);
            ButtonDicts.Add("ConvexHull", ConvexHullButtonsDict);

            VRObjectsManager.instance.OnContainerCreated += ContainerCreated;
            VRObjectsManager.instance.OnContainerDeleted += ContainerDeleted;
            CloudSelector.instance.OnSelectionChange += SelectionChange;
            //Debug.Log("Event initialized");

            //gameObject.SetActive(false);
        }

        public void SelectionChange(int id)
        {
            DeleteAllButtons();
            CloudData data = CloudUpdater.instance.LoadStatus(id);
            foreach(KeyValuePair<int, GameObject> kvp in data.globalMetaData.counterPointsList)
            {
                ContainerCreated(kvp.Value.GetComponent<VRContainerCounter>().id, kvp.Value, "Counter");
            }
            foreach (KeyValuePair<int, GameObject> kvp in data.globalMetaData.rulerPointsList)
            {
                ContainerCreated(kvp.Value.GetComponent<VRContainerRuler>().id, kvp.Value, "Ruler");
            }
            foreach (KeyValuePair<int, GameObject> kvp in data.globalMetaData.angleMeasurementsList)
            {
                ContainerCreated(kvp.Value.GetComponent<VRContainerAngleMeasurement>().id, kvp.Value, "AngleMeasurement");
            }
            foreach (KeyValuePair<int, GameObject> kvp in data.globalMetaData.histogramList)
            {
                ContainerCreated(kvp.Value.GetComponent<VRContainerHistogram>().id, kvp.Value, "Histogram");
            }
            foreach (KeyValuePair<int, GameObject> kvp in data.globalMetaData.sphereList)
            {
                ContainerCreated(kvp.Value.GetComponent<VRContainerSelectionSphere>().id, kvp.Value, "Sphere");
            }
            foreach (KeyValuePair<int, GameObject> kvp in data.globalMetaData.convexHullsList)
            {
                ContainerCreated(kvp.Value.GetComponent<VRContainerConvexHull>().id, kvp.Value, "ConvexHull");
            }
        }

        private void DeleteAllButtons()
        {
            foreach(string s in ButtonDicts.Keys)
            {
                foreach(int i in ButtonDicts[s].Keys)
                {
                    Destroy(ButtonDicts[s][i]);
                }
                ButtonDicts[s].Clear();
            }

        }

        public void ContainerCreated(int id, GameObject container, string type)
        {
            GameObject newbutton = CreateButton(id, container);

            if (type == "Counter")
            {
                newbutton.transform.SetParent(CounterViewPort.transform);
                CounterButtonsDict.Add(id, newbutton);

            }
            if (type == "Ruler")
            {
                newbutton.transform.SetParent(RulerViewPort.transform);
                RulerButtonsDict.Add(id, newbutton);
            }
            if (type == "AngleMeasure")
            {
                newbutton.transform.SetParent(AngleMeasureViewPort.transform);
                AngleMeasureButtonsDict.Add(id, newbutton);

            }
            if (type == "Histogram")
            {
                newbutton.transform.SetParent(HistogramViewPort.transform);
                HistogramButtonsDict.Add(id, newbutton);

            }
            if (type == "Sphere")
            {
                newbutton.transform.SetParent(SphereViewPort.transform);
                SphereButtonsDict.Add(id, newbutton);

            }
            if (type == "ConvexHull")
            {
                newbutton.transform.SetParent(ConvexHullViewPort.transform);
                ConvexHullButtonsDict.Add(id, newbutton);

            }
        }

        private GameObject CreateButton(int id, GameObject container)
        {
            GameObject go = Instantiate(ButtonPrefab) as GameObject;
            go.transform.GetChild(0).GetComponent<Text>().text = id.ToString();
            go.AddComponent<GameObjectActivationButton>();
            go.GetComponent<GameObjectActivationButton>().obj = container;
            return go;
        }

        public void ContainerDeleted(int id, string type)
        {
            
                if (type == "Counter")
                {
                    if (CounterButtonsDict.ContainsKey(id))
                    {
                        Debug.Log("check");
                        GameObject go = CounterButtonsDict[id];
                        CounterButtonsDict.Remove(id);
                        Destroy(go);
                    }
                }

                if (type == "Ruler")
                {

                    if (RulerButtonsDict.ContainsKey(id))
                    {
                        Debug.Log("check");
                        GameObject go = RulerButtonsDict[id];
                        RulerButtonsDict.Remove(id);
                        Destroy(go);
                    }
                }

                if (type == "AngleMeasure")
                {
                    if (AngleMeasureButtonsDict.ContainsKey(id))
                    {
                        GameObject go = AngleMeasureButtonsDict[id];
                        AngleMeasureButtonsDict.Remove(id);
                        Destroy(go);
                    }
                }

                if (type == "Histogram")
                {
                    if (HistogramButtonsDict.ContainsKey(id))
                    {
                        GameObject go = HistogramButtonsDict[id];
                        HistogramButtonsDict.Remove(id);
                        Destroy(go);
                    }
                }

                if (type == "Sphere")
                {
                    if (SphereButtonsDict.ContainsKey(id))
                    {
                        GameObject go = SphereButtonsDict[id];
                        SphereButtonsDict.Remove(id);
                        Destroy(go);
                    }
                }

                if (type == "ConvexHull")
                {
                    if (ConvexHullButtonsDict.ContainsKey(id))
                    {
                        GameObject go = ConvexHullButtonsDict[id];
                        ConvexHullButtonsDict.Remove(id);
                        Destroy(go);
                    }
                }

            
        }

        private void OnDisable()
        {
            DeleteAllButtons();
        }

        private void OnEnable()
        {
            if (!CloudSelector.instance.noSelection)
            {
                SelectionChange(CloudSelector.instance._selectedID);
            }
        }
    }
}