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

public class Test : MonoBehaviour
{
    public class ZMQCommunicator23 : RunnableThread
    {
        StreamSocket client;
        long timeLimit = 5000000;

        protected override void Run()
        {
            ForceDotNet.Force();


            client = new StreamSocket();
            //client.Options.RouterRawSocket = true;
            client.Bind("tcp://localhost:5555");
            Debug.Log("Communication started");

            long timeSinceLastMessage = 0;


            while (true)
            {
                List<byte[]> message = null;
                bool receivedmessage = false;
                receivedmessage = client.TryReceiveMultipartBytes(ref message,2);

                if (receivedmessage)
                {
                    timeSinceLastMessage = 0;

                    Debug.Log("Message Received");
                    Char[] charconvert = System.Text.Encoding.UTF8.GetChars(message[1], 0, message[1].Length);
                    if (string.Join("", charconvert) == "PointData Finished" || System.Text.Encoding.UTF8.GetString(message[1], 0, message[1].Length) == "PointData Finished")
                    {
                        Debug.Log("Transfer finished");
                        break;
                    }

                    Debug.Log("Recieved bytes " + string.Join("", message[1]));

                    float convert = BitConverter.ToSingle(message[1], 0);
                    Debug.Log("Recieved : " + convert);

                    Array.Reverse(message[1]);

                    float convert2 = BitConverter.ToSingle(message[1], 0);
                    Debug.Log("Reversed : " + convert2);

                }


                //Char[] convert2 = System.Text.Encoding.ASCII.GetChars(message, 0, message.Length);
                //Debug.Log("Recieved : " + string.Join("",convert2));



                timeSinceLastMessage++;


                if (timeSinceLastMessage > timeLimit)
                {
                    Debug.Log("Timeout");

                    break;
                }

            }

            client.Disconnect("tcp://localhost:5555");

            Debug.Log("Stopping Connection");

            client.Dispose();
            NetMQConfig.Cleanup();
            isRunning = false;

        }
    }
}
