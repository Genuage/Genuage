using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DesktopInterface;
using Data;
//using System.Windows.Forms;

namespace IO
{


    public class ZMQSynchronizedCommunicator : MonoBehaviour
    {
        public ThreadCommunicator sendSocket;
        public bool sendSocketON = false;
        public ThreadCommunicator receiveSocket;
        public bool receiveSocketON = false;
        private Process proc;
        private int defaulyTimeLimit = 3600;
        private Vector2[] uv1save;
        private Vector2[] trajuv1save;

        private int defaultAlphaValue = 0;
        private bool sendAllTrajectories = true;

        public bool InferenceModeActive = false;



        private DateTime StartTime;
        private DateTime StopTime;

        private HashSet<int> InferredPointsIDSet; //Stores ids of the points already treated by the system.

        public static ZMQSynchronizedCommunicator instance = null;
        Button button;

        private void Awake()
        {
            /**
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
            **/
            //button = GetComponent<Button>();
            //button.onClick.AddListener(SwitchModes);
            //RunPythonScript();
        }

        public void Activate()
        {
            InferenceModeActive = true;
            ActivateInferenceMode();
        }

        public void Deactivate()
        {
            InferenceModeActive = false;
            DeactivateInferenceMode();
        }
        /**
        public void SwitchModes()
        {
            InferenceModeActive = !InferenceModeActive;
            if (InferenceModeActive)
            {
                InitiateInferenceMode();
            }
            else
            {
                DeactivateInferenceMode();
            }
        }
        **/
        private void ActivateInferenceMode()
        {
            InferredPointsIDSet = new HashSet<int>();
            if (!CloudSelector.instance.noSelection)
            {

                CloudData data = CloudUpdater.instance.LoadCurrentStatus();

                CloudUpdater.instance.OnSelectionUpdate += SendData;
                NullifyPointColors(data);
                UnityEngine.Debug.Log("Real Time Inference activated");
                UIManager.instance.ChangeStatusText("Real Time Inference Mode Activated");
                UIManager.instance.DeactivateSelectionButtons();

                if (data.globalMetaData.alphacolumnExists)
                {
                    //Fill hashset ??
                }
            }
        }

        private void DeactivateInferenceMode()
        {
            InferredPointsIDSet.Clear();
            if (!CloudSelector.instance.noSelection)
            {
                
                CloudData data = CloudUpdater.instance.LoadCurrentStatus();

                CloudUpdater.instance.OnSelectionUpdate -= SendData;
                RestorePointColors(data);
                if (sendSocketON)
                {
                    sendSocket.AbortThread();
                }
                if (receiveSocketON)
                {
                    receiveSocket.AbortThread();
                }
                UnityEngine.Debug.Log("Real Time Inference deactivated");
                UIManager.instance.ChangeStatusText("Real Time Inference Mode Deactivated");
                UIManager.instance.ActivateSelectionButtons();
            }

        }

        public void ChangeDefaultAlphaValue(int newvalue)
        {
            if(newvalue <=2 && newvalue >= 0)
            {
                defaultAlphaValue = newvalue;
            }
        }

        public void SwitchTrajectoryParameter(bool value)
        {
            sendAllTrajectories = value;
        }

        private void RunPythonScript()
        {
            int x = 1;
            int y = 2;
            string progToRun = "C:/Users/Thomas/Desktop/test/basic_script.py";
            //char[] splitter = { '\r' };
            proc = new Process();

            //Process proc = new Process();
            proc.StartInfo.FileName = "python.exe";
            //proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            // call hello.py to concatenate passed parameters
            proc.StartInfo.Arguments = progToRun;

            //proc.StartInfo.Arguments = string.Concat(progToRun, " ", x.ToString(), " ", y.ToString());
            proc.Start();


            //proc.WaitForExit();
        }


        private void SendData()
        {
            if (receiveSocketON == false && sendSocketON == false)
            {
                CloudData data = CloudUpdater.instance.LoadCurrentStatus();
                if (data.globalMetaData.SelectedPointsList.Count > 0)
                {
                    StartTime = DateTime.Now;
                    sendSocket = new ZMQCommunicatorPython();
                    sendSocket.TimeLimit = defaulyTimeLimit;
                    UnityEngine.Debug.Log("Preparing data to send");
                    List<float[]> list = PrepareData();
                    UnityEngine.Debug.Log("Data prepared");
                    sendSocket.dataList = list;
                    sendSocket.option = ThreadCommunicator.CommunicatorOption.SEND_DATA;
                    sendSocket.StartThread();
                    sendSocketON = true;
                    UIManager.instance.ChangeStatusText("Sending selection data through ZMQ");
                    UnityEngine.Debug.Log("SENDSOCKET ON");
                }
            }
        }

        private void ReceiveData()
        {
            receiveSocket = new ZMQCommunicatorPython();
            receiveSocket.TimeLimit = 1200;
            receiveSocket.option = ThreadCommunicator.CommunicatorOption.RECEIVE_DATA;
            receiveSocket.StartThread();
            receiveSocketON = true;
            UnityEngine.Debug.Log("ReceiveSocket ON");
            UIManager.instance.ChangeStatusText("Waiting for inference data");

        }

        private void Update()
        {
            if (sendSocketON)
            {
                if (!sendSocket.isRunning)
                {
                    UnityEngine.Debug.Log("SENDSOCKET has finished job");
                    sendSocket.StopThread();
                    if (sendSocket.receive_status == ThreadCommunicator.ReceiveStatus.SUCCESS)
                    {
                        //CloudLoader.instance.LoadFromConnection(thread.dataList);
                        UIManager.instance.ChangeStatusText("Selection data sent !");
                        ReceiveData();
                    }

                    else if (sendSocket.receive_status == ThreadCommunicator.ReceiveStatus.TIMEOUT)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : Timeout");
                    }

                    else if (sendSocket.receive_status == ThreadCommunicator.ReceiveStatus.INVALID_FORMAT)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : Invalid Format");
                    }

                    else if (sendSocket.receive_status == ThreadCommunicator.ReceiveStatus.BYTE_CONVERSION_ERROR)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : Can't convert Bytes into floats");
                    }

                    else if (sendSocket.receive_status == ThreadCommunicator.ReceiveStatus.NO_BYTES)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : No Bytes were sent");
                    }

                    else if (sendSocket.receive_status == ThreadCommunicator.ReceiveStatus.COLLUMN_SIZE_DISCREPANCY)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : All collumns do not have the same size");
                    }
                    sendSocketON = false;

                }
            }
            if (receiveSocketON)
            {
                if (!receiveSocket.isRunning)
                {
                    receiveSocket.StopThread();
                    if (receiveSocket.receive_status == ThreadCommunicator.ReceiveStatus.SUCCESS)
                    {
                        UIManager.instance.ChangeStatusText("Inference data received, updating the cloud...");
                        //CloudLoader.instance.LoadFromConnection(thread.dataList);
                        ProcessReceivedData(receiveSocket.dataList);

                    }

                    if (receiveSocket.receive_status == ThreadCommunicator.ReceiveStatus.TIMEOUT)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : Timeout");
                    }

                    if (receiveSocket.receive_status == ThreadCommunicator.ReceiveStatus.INVALID_FORMAT)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : Invalid Format");
                    }

                    if (receiveSocket.receive_status == ThreadCommunicator.ReceiveStatus.BYTE_CONVERSION_ERROR)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : Can't convert Bytes into floats");
                    }

                    if (receiveSocket.receive_status == ThreadCommunicator.ReceiveStatus.NO_BYTES)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : No Bytes were sent");
                    }

                    if (receiveSocket.receive_status == ThreadCommunicator.ReceiveStatus.COLLUMN_SIZE_DISCREPANCY)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : All collumns do not have the same size");
                    }
                    receiveSocketON = false;
                }
            }
        }

        private List<float[]> PrepareData()
        {
            List<float[]> list = new List<float[]>();

            CloudData data = CloudUpdater.instance.LoadCurrentStatus();
            HashSet<int> selectedpointsSet = data.globalMetaData.SelectedPointsList;
            HashSet<float> selectedtrajectorySet = data.globalMetaData.SelectedTrajectories;

            List<float> ids = new List<float>();
            List<float> xvalues = new List<float>();
            List<float> yvalues = new List<float>();
            List<float> zvalues = new List<float>();
            List<float> colorvalues = new List<float>();
            List<float> timevalues = new List<float>();
            List<float> trajectoryvalues = new List<float>();
            List<float> phivalues = new List<float>();
            List<float> thetavalues = new List<float>();
            List<float> sizevalues = new List<float>();
            //int index = 0;
            if (sendAllTrajectories == true)
            {
                foreach (float i in selectedtrajectorySet)
                {
                    foreach (int j in data.pointTrajectoriesTable[i].pointsIDList)
                    {
                        if (!InferredPointsIDSet.Contains(j))
                        {
                            ids.Add(j);
                            xvalues.Add(data.pointDataTable[j].position.x);
                            yvalues.Add(data.pointDataTable[j].position.y);
                            zvalues.Add(data.pointDataTable[j].position.z);
                            colorvalues.Add(data.pointDataTable[j].intensity);
                            timevalues.Add(data.pointDataTable[j].time);
                            trajectoryvalues.Add(data.pointDataTable[j].trajectory);
                            phivalues.Add(data.pointDataTable[j].phi_angle);
                            thetavalues.Add(data.pointDataTable[j].theta_angle);
                            sizevalues.Add(data.pointDataTable[j].size);

                        }
                        //index++;
                    }
                }
            }
            else
            {
                foreach (int i in selectedpointsSet)
                {
                    if (!InferredPointsIDSet.Contains(i))
                    {

                        ids.Add(i);
                        xvalues.Add(data.pointDataTable[i].position.x);
                        yvalues.Add(data.pointDataTable[i].position.y);
                        zvalues.Add(data.pointDataTable[i].position.z);
                        colorvalues.Add(data.pointDataTable[i].intensity);
                        timevalues.Add(data.pointDataTable[i].time);
                        trajectoryvalues.Add(data.pointDataTable[i].trajectory);
                        phivalues.Add(data.pointDataTable[i].phi_angle);
                        thetavalues.Add(data.pointDataTable[i].theta_angle);
                        sizevalues.Add(data.pointDataTable[i].size);
                    }
                }
            }
            UnityEngine.Debug.Log("selectedtrajs length " + data.globalMetaData.SelectedTrajectories.Count);
            UnityEngine.Debug.Log("id length " + ids.Count);
            list.Add(ids.ToArray());
            list.Add(xvalues.ToArray());
            list.Add(yvalues.ToArray());
            list.Add(zvalues.ToArray());
            list.Add(colorvalues.ToArray());
            list.Add(timevalues.ToArray());
            list.Add(trajectoryvalues.ToArray());
            list.Add(phivalues.ToArray());
            list.Add(thetavalues.ToArray());
            list.Add(sizevalues.ToArray());

            return list;

            /**
            foreach(int i in data.globalMetaData.displayCollumnsConfiguration)
            {
                float[] array = new float[data.pointDataTable.Count]; 
                data.columnData[i].CopyTo(array,0); //ID X Y Z C T TR PH TH SZ
                list.Add(array);
            }
            **/
        }


        private void ProcessReceivedData(List<float[]> SelectedAlphaList)
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();
            float[] alphacolumn = new float[data.pointDataTable.Count];
            for (int i = 0; i < alphacolumn.Length; i++)
            {
                alphacolumn[i] = defaultAlphaValue;
            }

            if (!data.globalMetaData.alphacolumnExists)
            {
                CreateAlphaColumn(data, alphacolumn);
            }

            foreach (int i in InferredPointsIDSet)
            {
                alphacolumn[i] = data.columnData[data.globalMetaData.alphacolumnIndex][i];

            }

            for (int i = 0; i < SelectedAlphaList[0].Length; i++)
            {
                alphacolumn[(int)SelectedAlphaList[0][i]] = SelectedAlphaList[1][i];
                InferredPointsIDSet.Add((int)SelectedAlphaList[0][i]);
            }

            
            //float[]
            //for(int i = 0; i < SelectedAlphaList.Count; i++)
            //{
                //alphacolumn[(int)SelectedAlphaList[0][i]] = SelectedAlphaList[1][i];
            //}

            data.columnData[data.globalMetaData.alphacolumnIndex] = alphacolumn;

            float max = Mathf.NegativeInfinity;
            float min = Mathf.Infinity;
            
            for (int i = 0; i < alphacolumn.Length; i++)
            {
                if (alphacolumn[i] <= min)
                {
                    min = alphacolumn[i];
                }

                if (alphacolumn[i] >= max)
                {
                    max = alphacolumn[i];
                }
            }
            
            ColumnMetadata metadata = new ColumnMetadata();
            metadata.ColumnID = data.globalMetaData.alphacolumnIndex;
            metadata.MaxValue = min;
            metadata.MinValue = max;
            metadata.MinThreshold = min;
            metadata.MaxThreshold = max;
            metadata.Range = max - min;

            data.globalMetaData.columnMetaDataList[data.globalMetaData.alphacolumnIndex] = metadata;
            //ModalWindowManager.instance.CreateModalWindow("Column "+ (data.globalMetaData.alphacolumnIndex+1) + 
            //                                              " has been modified." + "\n");
            UpdatePointColor(data, alphacolumn);
            UIManager.instance.ChangeStatusText("Cloud data has been updated !");
            StopTime = DateTime.Now;
            EvaluateTimespan();
        }

        private void CreateAlphaColumn(CloudData data, float[] array)
        {
            data.columnData.Add(array);
            ColumnMetadata metadata = new ColumnMetadata();
            metadata.ColumnID = data.columnData.Count - 1;
            metadata.MaxValue = defaultAlphaValue;
            metadata.MinValue = defaultAlphaValue;
            metadata.MinThreshold = defaultAlphaValue;
            metadata.MaxThreshold = defaultAlphaValue;
            metadata.Range = defaultAlphaValue - defaultAlphaValue;
            data.globalMetaData.columnMetaDataList.Add(metadata);
            data.globalMetaData.alphacolumnExists = true;
            data.globalMetaData.alphacolumnIndex = metadata.ColumnID;

            ModalWindowManager.instance.CreateModalWindow("Column " + (data.globalMetaData.alphacolumnIndex + 1) + " has been added to the data." + "\n");
        }

        private void NullifyPointColors(CloudData data)
        {
            Mesh mesh = data.GetComponent<MeshFilter>().mesh;
            Vector2[] uv1 = mesh.uv2;
            uv1save = new Vector2[uv1.Length];
            uv1.CopyTo(uv1save, 0);
            for (int i = 0; i < uv1.Length; i++)
            {
                uv1[i].x = 0;
            }
            mesh.uv2 = uv1;
            data.GetComponent<MeshFilter>().mesh = mesh;

            if (data.trajectoryObject)
            {
                Mesh trajmesh = data.trajectoryObject.GetComponent<MeshFilter>().mesh;
                
                Vector2[] trajuv1 = trajmesh.uv2;
                trajuv1save = new Vector2[trajuv1.Length];

                trajuv1.CopyTo(trajuv1save, 0);
                for (int i = 0; i < trajuv1.Length; i++)
                {
                    trajuv1[i].x = 0;
                }
                trajmesh.uv2 = trajuv1;
                data.trajectoryObject.GetComponent<MeshFilter>().mesh = trajmesh;

            }
        }

        private void UpdatePointColor(CloudData data, float[] alphacolumn)
        {
            //TODO : Incorporate this in CloudUpdater
            Shader.EnableKeyword("COLOR_OVERRIDE");
            //Shader.DisableKeyword("FREE_SELECTION");
            Mesh mesh = data.GetComponent<MeshFilter>().mesh;
            Vector2[] uv1 = mesh.uv2;
            for (int i = 0; i < uv1.Length; i++)
            {
                uv1[i].x = alphacolumn[i]/2;
            }
            mesh.uv2 = uv1;

            //mesh.uv2 = uv1save;
            data.GetComponent<MeshFilter>().mesh = mesh;

            if (data.trajectoryObject)
            {
                Mesh trajmesh = data.trajectoryObject.GetComponent<MeshFilter>().mesh;
                Vector2[] trajuv1 = trajmesh.uv2;
                for (int i = 0; i < trajuv1.Length; i++)
                {
                    trajuv1[i].x = alphacolumn[i] / 2;
                }
                trajmesh.uv2 = trajuv1;
                data.trajectoryObject.GetComponent<MeshFilter>().mesh = trajmesh;

            }

        }

        private void RestorePointColors(CloudData data)
        {
            Mesh mesh = data.GetComponent<MeshFilter>().mesh;
            mesh.uv2 = uv1save;
            data.GetComponent<MeshFilter>().mesh = mesh;

            if (data.trajectoryObject)
            {
                Mesh trajmesh = data.trajectoryObject.GetComponent<MeshFilter>().mesh;
                trajmesh.uv2 = trajuv1save;
                data.trajectoryObject.GetComponent<MeshFilter>().mesh = trajmesh;

            }

        }

        private void EvaluateTimespan()
        {
            TimeSpan ts = StopTime - StartTime;
            double seconds = ts.TotalSeconds;
            double miliseconds = ts.TotalMilliseconds;
            UIManager.instance.ChangeStatusText("Cloud data has been updated ! / Timespan : " + miliseconds.ToString() + " ms");

        }

        private void OnDestroy()
        {
            if(InferenceModeActive == true)
            {
                DeactivateInferenceMode();
            }
        }
    }
}