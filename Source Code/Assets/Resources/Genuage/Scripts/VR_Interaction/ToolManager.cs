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
using Display;

namespace VR_Interaction
{

    public struct ToolState
    {
        public string id;
        public IControllerTool component;
        public List<string> incompatibleToolsList;
        public bool isactivated;

        public ToolState(string id, IControllerTool component, List<string> incompatibleToolsList, bool isactivated)
        {
            this.id = id;
            this.component = component;
            this.incompatibleToolsList = incompatibleToolsList;
            this.isactivated = isactivated;
        }

    }
    /// <summary>
    /// Script to put on the GO with the VRTK_controllerEvents. handle tool change
    /// </summary>
    public class ToolManager : MonoBehaviour
    {
        
        public IControllerTool selectionPointer;
        public IControllerTool selectionSphere;
        public IControllerTool selectionConvexHull;
        public IControllerTool freeselection;
        public IControllerTool counter;
        public IControllerTool measureDistance;
        public IControllerTool clippingPlane;
        public IControllerTool multiclippingPlane;
        public IControllerTool anglemeasure;
        public IControllerTool histogram;

        public Dictionary<string,ToolState> toolsDict; // available tools on the controller
        public List<string> _current_tools; // the tool currently selected
        VRTK_ControllerEvents _controller;
        public GameObject toolMenuPrefab;
        private GameObject menu = null;

        public void Awake()
        {
            _controller = GetComponent<VRTK_ControllerEvents>();
            _controller.ButtonTwoPressed += OnMenuClicked; // toggle state using the MenuButton
            //_controller.GripClicked += ActivateMatrix;

            if (!selectionSphere)
            {
                //selectionSphere = this.gameObject.AddComponent<SphereSelectionCreator>();
            }
            if (!selectionConvexHull)
            {
                //selectionConvexHull = this.gameObject.AddComponent<ConvexHullCreator>();
            }
            if (!counter)
            {
                //counter = this.gameObject.AddComponent<CounterCreator>();
            }
            if (!measureDistance)
            {
                //measureDistance = this.gameObject.AddComponent<MeasureDistance>();
            }
            if (!clippingPlane)
            {
                clippingPlane = this.gameObject.AddComponent<ClippingPlane>();
            }
            

            toolsDict = new Dictionary<string, ToolState>();
            toolsDict.Add("Pointer", new ToolState("Pointer", 
                                            selectionPointer, 
                                            new List<string>(),
                                            true));

            toolsDict.Add("Selection Sphere", new ToolState("Selection Sphere",
                                                                selectionSphere,
                                                                new List<string>() { "Selection Convex Hull", "Counter", "Measure Distance", "Angle Measure", "Histogram", "MultiClipping Plane", "Free Selection" },
                                                                false));
            toolsDict.Add("Selection Convex Hull", new ToolState("Selection Convex Hull",
                                                                selectionConvexHull,
                                                                new List<string>() { "Selection Sphere", "Counter", "Measure Distance", "Angle Measure", "Histogram", "MultiClipping Plane", "Free Selection" },
                                                                false));

            toolsDict.Add("Free Selection", new ToolState("Free Selection",
                                                    freeselection,
                                                    new List<string>() { "Selection Convex Hull", "Selection Sphere", "Counter", "Measure Distance", "Angle Measure", "Histogram", "MultiClipping Plane" },
                                                    false));

            toolsDict.Add("Counter", new ToolState("Counter",
                                                     counter,
                                                     new List<string>() { "Selection Convex Hull", "Selection Sphere", "Measure Distance", "Angle Measure", "Histogram", "MultiClipping Plane", "Free Selection" },
                                                     false));

            toolsDict.Add("Measure Distance", new ToolState("Measure Distance",
                                                     measureDistance,
                                                     new List<string>() { "Selection Convex Hull", "Counter", "Selection Sphere", "Angle Measure", "Histogram", "MultiClipping Plane", "Free Selection" },
                                                     false));

            toolsDict.Add("Angle Measure", new ToolState("Angle Measure",
                                         anglemeasure,
                                         new List<string>() { "Selection Convex Hull", "Counter", "Selection Sphere", "Measure Distance", "Histogram", "MultiClipping Plane", "Free Selection" },
                                         false));

            toolsDict.Add("Histogram", new ToolState("Histogram",
                             histogram,
                             new List<string>() { "Selection Convex Hull", "Counter", "Selection Sphere", "Measure Distance", "Angle Measure", "MultiClipping Plane", "Free Selection" },
                             false));



            toolsDict.Add("Clipping Plane", new ToolState("Clipping Plane",
                                         clippingPlane,
                                         new List<string>() { "MultiClipping Plane" } ,
                                         false));

            toolsDict.Add("MultiClipping Plane", new ToolState("MultiClipping Plane",
                             multiclippingPlane,
                             new List<string>() { "Clipping Plane", "Selection Convex Hull", "Counter", "Selection Sphere", "Measure Distance", "Angle Measure", "Histogram", "Free Selection" },
                             false));




            VRTK_EventSystem.current.sendNavigationEvents = false;
        }


        private void ActivatePointer(ControllerInteractionEventArgs e)
        {
            GetComponent<VRTK_Pointer>().DoActivationButtonPressed(_controller, e);
            GetComponent<VRTK_Pointer>().DoActivationButtonReleased(_controller, e);
            GetComponent<VRTK_InteractGrab>().grabButton = VRTK_ControllerEvents.ButtonAlias.TriggerClick;
        }

        /// <summary>
        /// Change the toggle tool state and switch tool accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMenuClicked(object sender, ControllerInteractionEventArgs e)
        {
            
            if (menu == null)
            {
                menu = Instantiate(toolMenuPrefab) as GameObject;
                menu.GetComponentInChildren<VRToolMenu>().manager = this;
                menu.transform.position = ((CameraManager.instance.vr_camera.transform.forward * 2) + (CameraManager.instance.vr_camera.transform.up/2));
            }
            else
            {
                menu.SetActive(!menu.activeInHierarchy);
                //Destroy(menu);
            }
            
        }

        /// <summary>
        /// Method to change the tool using toggle button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SwitchTool(string id, bool state)
        {
            if (toolsDict.ContainsKey(id))
            {
                ToolState currenttoolState = toolsDict[id];
                //Debug.Log(currenttoolState.isactivated);
                //Debug.Log(state);
                if (currenttoolState.isactivated == state)
                {
                    Debug.Log("Same state");
                    return;
                }
                if (state == true)
                {
                    if (menu)
                    {
                        menu.GetComponentInChildren<VRToolMenu>().DeactivateIncompatibleButtons(currenttoolState.incompatibleToolsList.ToArray());
                    }
                    foreach (string str in currenttoolState.incompatibleToolsList)
                    {
                        SwitchTool(str, false);
                    }
                    currenttoolState.component.OnToolActivated();
                    currenttoolState.component.enabled = true;
                    
                    

                }
                else
                {
                    currenttoolState.component.OnToolDeactivated();
                    currenttoolState.component.enabled = false;
                }
                currenttoolState.isactivated = state;
                toolsDict[id] = currenttoolState;
                Debug.Log("changed state of " + currenttoolState.id + " to " + currenttoolState.isactivated);
            }
        }

            /// <summary>
            /// Method to disable all tools
            /// </summary>
        public void DeactivateAllTools()
        {
            MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour c in comps)
            {
                c.enabled = false;
            }
            GetComponent<ToolManager>().enabled = true;
            GetComponent<VRTK_ControllerEvents>().enabled = true;
            GetComponent<VRTK_Pointer>().enabled = true;
        }

        
    }
}