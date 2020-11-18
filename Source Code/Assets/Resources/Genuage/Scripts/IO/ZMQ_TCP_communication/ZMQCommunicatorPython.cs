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


using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

namespace IO
{

    public class ZMQCommunicatorPython : ThreadCommunicator
    {
        protected NetMQSocket socket;
        List<byte[]> byte_dataList;
        //public int timeLimit = 120;     
        //Add option to modify this in AppOptions

        protected override void Run()
        {
            ForceDotNet.Force();
            //socket = new RequestSocket();
            //socket.Connect("tcp://localhost:5555");
            Debug.Log("Communication started");

            if (option == CommunicatorOption.RECEIVE_DATA)
            {
                socket = new RequestSocket();
                socket.Connect(address);
                //Debug.Log("Communication started");

                dataList = new List<float[]>();

                ReceiveOnePointData();
            }
            else if (option == CommunicatorOption.SEND_DATA)
            {
                socket = new ResponseSocket();
                socket.Bind(address);
                //Debug.Log("Communication started");

                CreateByteData();
                SendOnePointData();
            }
            //byte_dataList


        }

        private void CreateByteData()
        {
            byte_dataList = new List<byte[]>();
            foreach( float[] array in dataList)
            {
                byte[] byteArray = ConvertFloatArrayToBytes(array);
                string s = BitConverter.ToString(byteArray);
                //Debug.Log(s);
                byte_dataList.Add(byteArray);
            }
        }

        private byte[] ConvertFloatArrayToBytes(float[] floatArray)
        {
            var byteArray = new byte[floatArray.Length * sizeof(float)];
            Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
            /**
            foreach(float i in floatArray)
            {
                Debug.Log(i);
                byte[] b = BitConverter.GetBytes(i);
                string s = BitConverter.ToString(b);
                Debug.Log(s);
            }
            **/
            return byteArray;
        }

        protected override ReceiveStatus SendPointValues()
        {
            byte[] message = null;
            bool recievedmessage = false;
            DateTime lastMessageTime = DateTime.Now;
            int index = 0;
            while (true)
            {
                recievedmessage = socket.TryReceiveFrameBytes(out message);
                if (recievedmessage)
                {
                    lastMessageTime = DateTime.Now;
                    Debug.Log("Message Received");
                    //Debug.Log(BitConverter.ToString(message));
                    if(index >= byte_dataList.Count)
                    {
                        Debug.Log("Transfer Finished");
                        socket.SendFrame("PointData Finished");
                        return ReceiveStatus.SUCCESS;
                    }
                    string convert = System.Text.Encoding.UTF8.GetString(message, 0, message.Length);
                    if (convert == "PointCollumn Echo")
                    {
                        Debug.Log("length of " + index + " : " + dataList[index].Length);

                        Debug.Log("length of " + index + " : " + byte_dataList[index].Length);
                        socket.SendFrame(byte_dataList[index]);
                        lastMessageTime = DateTime.Now;
                        index++;
                    }
                }
                DateTime currentTime = DateTime.Now;
                DateTime stopTime = lastMessageTime.AddSeconds(TimeLimit);
                if (stopTime < currentTime)
                {
                    return ReceiveStatus.TIMEOUT;
                }
            }
        }
        protected override ReceiveStatus ReceivePointValues()
        {
            //float timeSinceLastMessage;
            DateTime lastMessageTime = DateTime.Now;
            while (true)
            {
                socket.SendFrame("PointCollumn Echo");
                Debug.Log("PointCollumn Echo Sent");
                byte[] message = null;
                bool recievedmessage = false;
                lastMessageTime = DateTime.Now;
                bool stopwhile = false;
                while (stopwhile == false)
                {
                    recievedmessage = socket.TryReceiveFrameBytes(out message);
                    if (recievedmessage)
                    {
                        lastMessageTime = DateTime.Now;
                        Debug.Log("Message Received");
                        //Debug.Log(message.ToString());
                        string convert = System.Text.Encoding.UTF8.GetString(message, 0, message.Length);
                        if (convert == "PointData Finished")
                        {
                            if (dataList.Count < 1)
                            {
                                return ReceiveStatus.INVALID_FORMAT;
                            }
                            else
                            {
                                return ReceiveStatus.SUCCESS;
                            }
                        }
                        if (message.Length < 1)
                        {
                            return ReceiveStatus.INVALID_FORMAT;
                        }

                        var array = new float[message.Length / sizeof(float)];
                        try
                        {
                            Buffer.BlockCopy(message, 0, array, 0, message.Length);
                        }
                        catch
                        {
                            return ReceiveStatus.BYTE_CONVERSION_ERROR;
                        }
                        if (dataList.Count != 0 && array.Length != dataList[dataList.Count - 1].Length)
                        {
                            return ReceiveStatus.INVALID_FORMAT;
                        }

                        dataList.Add(array);
                        stopwhile = true;
                    }
                    //else
                    //{
                    //lastMessageTime += Time.deltaTime;
                    //}
                    DateTime currentTime = DateTime.Now;
                    DateTime stopTime = lastMessageTime.AddSeconds(TimeLimit);
                    if (stopTime < currentTime)
                    {
                        return ReceiveStatus.TIMEOUT;
                    }


                }
            }
        }

        protected override void StopConnection()
        {
            socket.Disconnect("tcp://localhost:5555");

            Debug.Log("Stopping Connection");

            socket.Dispose();
            NetMQConfig.Cleanup();
            isRunning = false;

        }
    }
}