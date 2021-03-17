/**
Copyright (c) 2020, 	Institut Curie, Institut Pasteur and CNRS
			Thomas BLanc, Mohamed El Beheiry, Jean Baptiste Masson, Bassam Hajj and Clement caporal
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
using System.IO;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesktopInterface;
using VR_Interaction;
using Data;

namespace IO
{
    //Script that can read a text file and output a list of waypoints and cloudstates to create a Video
    public class VideoScriptFileReader : MonoBehaviour
    {
        GameObject ManagerGo;
        public GameObject MenuPrefab;
        public char FunctionSeparator = '|';
        public char ParameterSeparator = ';';
        public char[] VectorCoordinatesSeparator;
        public string WaypointFunction = "WAYPOINT";
        public string PositionParameter = "POS";
        public string RotationParameter = "ROT";
        public string LookatFunction = "LOOKAT";
        public string TimeParameter = "TIME";
        public string CloudStateFunction = "STATE";
        public string ColorMapParameter = "COLOR";
        public string PointSizeParameter = "POINTSZ";

        public List<CameraWaypoint> WaypointList;

        //Vector Shortcuts
        public Dictionary<string, Vector3> VectorAliasDict = new Dictionary<string, Vector3>()
                                                             {
                                                                {"GLOBAL.FORWARD", Vector3.forward},
                                                                {"GLOBAL.BACK", Vector3.back},
                                                                {"GLOBAL.RIGHT", Vector3.right},
                                                                {"GLOBAL.LEFT", Vector3.left},
                                                                {"GLOBAL.UP", Vector3.right},
                                                                {"GLOBAL.DOWN", Vector3.left},

                                                                {"CLOUD.FRONT_UPPER_RIGHT_CORNER", new Vector3(0.5f,0.5f,-0.5f)},
                                                                {"CLOUD.FRONT_LOWER_RIGHT_CORNER", new Vector3(0.5f,-0.5f,-0.5f)},
                                                                {"CLOUD.BACK_UPPER_RIGHT_CORNER", new Vector3(0.5f,0.5f,0.5f)},
                                                                {"CLOUD.BACK_LOWER_RIGHT_CORNER", new Vector3(0.5f,-0.5f,0.5f)},

                                                                {"CLOUD.FRONT_UPPER_LEFT_CORNER", new Vector3(-0.5f,0.5f,-0.5f)},
                                                                {"CLOUD.FRONT_LOWER_LEFT_CORNER", new Vector3(-0.5f,-0.5f,-0.5f)},
                                                                {"CLOUD.BACK_UPPER_LEFT_CORNER", new Vector3(-0.5f,0.5f,0.5f)},
                                                                {"CLOUD.BACK_LOWER_LEFT_CORNER", new Vector3(-0.5f,-0.5f,0.5f)}
                                                             };

        //Parse a string in the form of x/y/z or check for related Alias.
        private Vector3 ParseNumbers(string numberstring)
        {
            Vector3 Position = Vector3.zero;

            string[] numbers = numberstring.Split(',');
            if(numbers.Length == 3)
            {
                float x;
                float y;
                float z;
                bool ParseSuccessX = Single.TryParse(numbers[0], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out x);
                if (!ParseSuccessX)
                {
                    UIManager.instance.ChangeStatusText("ERROR : Parsing Error, when trying to convert float from string");
                    return Vector3.zero;
                }
                bool ParseSuccessY = Single.TryParse(numbers[1], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out y);
                if (!ParseSuccessY)
                {
                    UIManager.instance.ChangeStatusText("ERROR : Parsing Error, when trying to convert float from string");
                    return Vector3.zero;
                }

                bool ParseSuccessZ = Single.TryParse(numbers[2], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out z);
                if (!ParseSuccessZ)
                {
                    UIManager.instance.ChangeStatusText("ERROR : Parsing Error, when trying to convert float from string");
                    return Vector3.zero;
                }

                Position = new Vector3(x, y, z);
            }
            else if (numbers.Length == 1)
            {
                //Check aliases
                foreach(KeyValuePair<string, Vector3> kvp in VectorAliasDict)
                {
                    if(numberstring == kvp.Key)
                    {
                        Position = kvp.Value;
                    }
                }
            }
            else
            {
                UIManager.instance.ChangeStatusText("ERROR : Parsing Error, when trying to convert float from string, or Alias could not be recognized");
                return Vector3.zero;
            }

            return Position;
        }

        private void InitializeVideoSystem()
        {
            
            ManagerGo = new GameObject();
            ManagerGo.AddComponent<VRCameraWaypointsManager>();
            VRCameraWaypointsManager.instance.MenuPrefab = this.MenuPrefab;
            VRCameraWaypointsManager.instance.CreateCamera();
            VRCameraWaypointsManager.instance.InitializeDesktop();
            //VRCameraWaypointsManager.instance.SetUIOnDesktop();

        }

        public void ReadFile(string path)
        {
            InitializeVideoSystem();
            WaypointList = new List<CameraWaypoint>();
            //UIManager.instance.ChangeStatusText("Loading Cloud...");
            string directory = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);

            string[] lines = File.ReadAllLines(path);
            int N = lines.Length;

            for (int j = 0; j < lines.Length; j++)
            {
                string[] entries = lines[j].Split(FunctionSeparator); // 0 should be the function name, 1 the list of parameters separated by ;
                                                                      
                //WAYPOINT FUNCTION
                if (entries[0] == WaypointFunction)
                {
                    float Time = 0f;
                    Vector3 position = Vector3.zero;
                    Vector3 rotation = Vector3.zero;

                    string[] parameters = entries[1].Split(ParameterSeparator);
                    if(parameters.Length != 3)
                    {
                        UIManager.instance.ChangeStatusText("ERROR : Parsing Error at line " + j + ", the file could not be loaded");

                        //error
                    }
                    else
                    {
                        //POSITION
                        string[] PositionParam = parameters[0].Split('=');
                        if (PositionParam[0] == PositionParameter)
                        {
                            /**
                            string[] LookatCheck = PositionParam[1].Split(':');
                            if (LookatCheck.Length == 2)
                            {
                                position = ParseNumbers(LookatCheck[1]);
                            }
                            **/
                            //else
                            //{
                                position = ParseNumbers(PositionParam[1]);
                            //}
                            //Extract numbers and save them
                        }
                        else
                        {
                            UIManager.instance.ChangeStatusText("ERROR : Parsing Error at line " + j + ", the file could not be loaded");
                        }

                        //ROTATION
                        string[] RotationParam = parameters[1].Split('=');
                        if (RotationParam[0] == RotationParameter)
                        {
                            string[] LookatCheck = RotationParam[1].Split(':');
                            if (LookatCheck.Length ==2)
                            {
                                Vector3 Lookat = ParseNumbers(LookatCheck[1]);
                                GameObject T = new GameObject();
                                T.transform.position = Lookat;
                                GameObject newGO = new GameObject();
                                newGO.transform.position = position;
                                newGO.transform.LookAt(T.transform);
                                rotation = newGO.transform.rotation.eulerAngles;
                                Destroy(T);
                                Destroy(newGO);
                                Debug.Log("Lookat rotation : " + rotation);
                                //Check if there is a lookout call
                                //Implement Lookat
                            }
                            else
                            {
                                rotation = ParseNumbers(RotationParam[1]);
                            }
                        }
                        else
                        {
                            //error
                        }

                        //TIME
                        string[] TimeParam = parameters[2].Split('=');
                        if (TimeParam[0] == TimeParameter)
                        {
                            bool ParseSuccess = Single.TryParse(TimeParam[1], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out Time);
                            if (!ParseSuccess)
                            {
                                UIManager.instance.ChangeStatusText("ERROR : Parsing Error at line " + j + ", the file could not be loaded");
                                return;
                            }
                            //Extract numbers and save them
                        }
                        else
                        {
                            UIManager.instance.ChangeStatusText("ERROR : Parsing Error at line " + j + ", the file could not be loaded");
                        }

                    }
                    Debug.Log("position : " + position + " rotation : " + rotation);
                    // calculate a vector3 of (0 , 90, 0)
                    Quaternion targetRotation = Quaternion.Euler(rotation);
                    //transform.rotation = targetRotation;
                    VRCameraWaypointsManager.instance.PlaceWaypoint(position, targetRotation, Time);
                    //Call to intermediary script that is able to create gameobjects
                    //CameraWaypoint waypoint = new CameraWaypoint(WaypointList.Count, Mathf.RoundToInt(Time));
                    //waypoint.
                }
            
                //CLOUDSTATE FUNCTION
                else if (entries[0] == CloudStateFunction)
                {
                    float StateTime = 0f;
                    float PointSize = 0f;
                    string colormapName = "NaN";

                    string[] parameters = entries[1].Split(ParameterSeparator);
                    if (parameters.Length != 3) //Or whatever number of parameters the function uses
                    {
                        //error
                    }
                    else
                    {
                        //COLORMAP
                        string[] ColorMapParam = parameters[0].Split('=');
                        if (ColorMapParam[0] == ColorMapParameter)
                        {
                            colormapName = ColorMapParam[1];
                            
                        }
                        else
                        {
                            //error
                        }
                        //POINTSIZE
                        string[] PointSizeParam = parameters[1].Split('=');
                        if (PointSizeParam[0] == PointSizeParameter)
                        {
                            bool ParseSuccess = Single.TryParse(PointSizeParam[1], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out PointSize);
                            if (!ParseSuccess)
                            {
                                UIManager.instance.ChangeStatusText("ERROR : Parsing Error at line " + j + ", the file could not be loaded");
                                return;
                            }
                                //float PointSize = PointSizeParam[1];
                            //Extract numbers and save them
                        }
                        else
                        {
                            //error
                        }
                        //TIME
                        string[] TimeParam = parameters[2].Split('=');
                        if (TimeParam[0] == TimeParameter)
                        {
                            float Time;
                            bool ParseSuccess = Single.TryParse(TimeParam[1], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out StateTime);
                            if (!ParseSuccess)
                            {
                                UIManager.instance.ChangeStatusText("ERROR : Parsing Error at line " + j + ", the file could not be loaded");
                                return;
                            }

                            //Extract numbers and save them
                        }
                        else
                        {
                            UIManager.instance.ChangeStatusText("ERROR : Parsing Error at line " + j + ", the file could not be loaded");
                        }
                        //CloudUpdater.instance.
                        Debug.Log("Colormap : " + colormapName + " PSize : " + PointSize + " Time : " + StateTime);
                    }
                }
                else //If the identifier isn't a known function
                {
                    UIManager.instance.ChangeStatusText("ERROR : The script isn't correctly formated at line "+ j + ", can't identify the function called.");
                }

            }
        }
    }
}