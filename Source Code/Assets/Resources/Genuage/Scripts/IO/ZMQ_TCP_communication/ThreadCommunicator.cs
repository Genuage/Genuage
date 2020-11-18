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


namespace IO
{


    public abstract class ThreadCommunicator : RunnableThread
    {
        public List<float[]> dataList;
        public ReceiveStatus receive_status;
        public CommunicatorOption option;
        public string address = "tcp://localhost:5555";
        public bool AutoStopConnection = true;
        public int TimeLimit = 120;
        public enum CommunicatorOption
        {
            SEND_DATA,
            RECEIVE_DATA
        }

        public enum SendStatus
        {
            SUCCESS,
            TIMEOUT
        }

        public enum ReceiveStatus
        {
            INVALID_FORMAT,
            SUCCESS,
            TIMEOUT,
            BYTE_CONVERSION_ERROR,
            NO_BYTES,
            COLLUMN_SIZE_DISCREPANCY,
            READ_ERROR
        };

        protected void SendOnePointData()
        {
            receive_status = SendPointValues();

            if (receive_status == ReceiveStatus.SUCCESS)
            {
                Debug.Log("SUCCESS");
            }

            if (receive_status == ReceiveStatus.TIMEOUT)
            {
                Debug.Log("ERROR : Timeout");
            }

            
            StopConnection();

            
        }

        protected void ReceiveOnePointData()
        {
            receive_status = ReceivePointValues();

            if (receive_status == ReceiveStatus.SUCCESS)
            {
                Debug.Log("SUCCESS");
            }

            if (receive_status == ReceiveStatus.TIMEOUT)
            {
                Debug.Log("ERROR : Timeout");
            }

            if (receive_status == ReceiveStatus.INVALID_FORMAT)
            {
                Debug.Log("ERROR : Invalid Format");
            }

            if (receive_status == ReceiveStatus.BYTE_CONVERSION_ERROR)
            {
                Debug.Log("ERROR : Can't convert Bytes into floats");
            }

            if (receive_status == ReceiveStatus.NO_BYTES)
            {
                Debug.Log("ERROR : No Bytes were sent");
            }

            if (receive_status == ReceiveStatus.COLLUMN_SIZE_DISCREPANCY)
            {
                Debug.Log("ERROR : All collumns do not have the same size");
            }

            
            StopConnection();

            
        }

        protected abstract ReceiveStatus ReceivePointValues();

        protected abstract ReceiveStatus SendPointValues();
        protected abstract void StopConnection();


    }
}