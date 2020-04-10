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


using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IO
{


    public class TCPIPCommunicator : ThreadCommunicator
    {
        //long timeLimit = 50;
        //public List<float[]> dataList;
        TcpListener server;
        DateTime firstTime;
        DateTime timeLimit;

        protected override void Run()
        {
            dataList = new List<float[]>();
            server = new TcpListener(IPAddress.Any, 5555);
            ReceiveOnePointData();
            DateTime firstTime = DateTime.Now;
            DateTime timeLimit = firstTime.AddMinutes(1.0);

        }

        protected override ReceiveStatus ReceivePointValues()
        {

            server.Start();

            while (true)
            {

                TcpClient client = server.AcceptTcpClient();  //if a connection exists, the server will accept it

                NetworkStream ns = client.GetStream();


                byte[] initialmessage = Encoding.Default.GetBytes("PointCollumn Echo");   

                ns.Write(initialmessage, 0, initialmessage.Length);     
                Debug.Log("Writing Message");
                ns.Flush();



                while (client.Connected)  
                {
                    Debug.Log(client.Connected);
                    ns.Flush();
                    int timeOut = DateTime.Compare(firstTime, timeLimit);
                    if(timeOut > 0)
                    {
                        return ReceiveStatus.TIMEOUT;
                    }
                    else
                    {
                        Debug.Log("test");
                    }
                    byte[] sizebytearray = new byte[4];
                    try
                    {
                        ns.Read(sizebytearray, 0, sizebytearray.Length);
                    }
                    catch
                    {
                        return ReceiveStatus.READ_ERROR;
                    }
                    
                    Array.Reverse(sizebytearray);
                    int sizebyte = BitConverter.ToInt32(sizebytearray, 0);
                    ns.Flush();
                    Debug.Log(sizebyte + " size");


                    //Debug.Log("Client Connected");
                    byte[] message;

                    Debug.Log("Reading From Stream");
                    try
                    {


                        using (MemoryStream ms = new MemoryStream())
                        {
                            ns.CopyTo(ms);
                            ns.Flush();
                            message = ms.ToArray();
                        }
                    }
                    catch
                    {
                        return ReceiveStatus.READ_ERROR;
                    }
                    Debug.Log("Finished Reading");

                    Array.Reverse(message);
                    Debug.Log("Received " + message.Length + "bytes");

                    int columnNumber = message.Length / sizebyte;
                    int pointNumber = sizebyte / sizeof(float);

                    for(int j = 0; j < columnNumber; j++)
                    {
                        dataList.Add(new float[pointNumber]);
                    }


                    if (sizebyte * columnNumber != message.Length)
                    {
                        return ReceiveStatus.INVALID_FORMAT;
                    }

                    for (int i = 0; i < columnNumber; i++)
                    {
                        Debug.Log("Translating bytes");
                        float[] array = new float[sizebyte / sizeof(float)];
                        try
                        {
                            Buffer.BlockCopy(message, i * sizebyte, array, 0, sizebyte);
                        }
                        catch
                        {
                            return ReceiveStatus.BYTE_CONVERSION_ERROR;
                        }
                        /**
                        foreach (var a in array)
                        {
                            Debug.Log(a);
                        }
                        **/
                        
                        Debug.Log(array.Length + " numbers translated.");
                        dataList[dataList.Count-1-i] = array;



                    }

                    return ReceiveStatus.SUCCESS;

                }
                

            }
        }

        protected override void StopConnection()
        {
            server.Stop();
            isRunning = false;

        }
    }
}