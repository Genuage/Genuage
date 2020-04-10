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
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

namespace IO
{
    public class ZMQCommunicatorPython : ThreadCommunicator
    {
        protected NetMQSocket socket;

        long timeLimit = 5000000;

        protected override void Run()
        {
            ForceDotNet.Force();

            dataList = new List<float[]>();

            socket = new RequestSocket();
            socket.Connect("tcp://localhost:5555");
            Debug.Log("Communication started");

            ReceiveOnePointData();
        }

        protected override ReceiveStatus ReceivePointValues()
        {
            long timeSinceLastMessage;
            while (true)
            {
                socket.SendFrame("PointCollumn Echo");
                byte[] message = null;
                bool recievedmessage = false;
                timeSinceLastMessage = 0;

                while (true)
                {
                    recievedmessage = socket.TryReceiveFrameBytes(out message);
                    if (recievedmessage)
                    {
                        timeSinceLastMessage = 0;
                        Debug.Log("Message Received");
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
                        break;

                    }
                    else
                    {
                        timeSinceLastMessage++;
                    }

                    if (timeSinceLastMessage > timeLimit)
                    {
                        //Doesn't seem very efficient, should probably find another way later.
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