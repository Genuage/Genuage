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
using IO;
namespace DesktopInterface
{

    public abstract class ThreadCommunicationButton : IButtonScript
    {
        protected ThreadCommunicator thread;
        private bool threadON;
        public GameObject abortButton;

        protected abstract void CreateThread();

        protected GameObject window;

        protected string defaultText;

        

      
        // Update is called once per frame
        public override void Execute()
        {
            if (!threadON)
            {
                window = ModalWindowManager.instance.CreateModalWindow("Waiting For Messages...", false);
                CreateThread();
                //GetComponentInChildren<Text>().text = "Cancel Communication";
                thread.StartThread();
                threadON = true;
                abortButton.SetActive(true);
                abortButton.GetComponent<AbortThread>().thread = this.thread;
                abortButton.GetComponent<AbortThread>().modalwindow = window;
                abortButton.GetComponent<AbortThread>().OnThreadAborted += OnThreadAborted; 

            }
            /**
            else
            {
                Destroy(window);
                threadON = false;
                thread.AbortThread();
                ModalWindowManager.instance.CreateModalWindow("Communication Cancelled");
                GetComponentInChildren<Text>().text = defaultText;
            }
            **/
        }

        private void OnThreadAborted()
        {
            threadON = false;
            abortButton.GetComponent<AbortThread>().OnThreadAborted -= OnThreadAborted;

        }


        private void Update()
        {
            if (threadON)
            {
                if (!thread.isRunning)
                {
                    Destroy(window);
                    thread.StopThread();
                    threadON = false;

                    if (thread.receive_status == ThreadCommunicator.ReceiveStatus.SUCCESS)
                    {
                        CloudLoader.instance.LoadFromConnection(thread.dataList);
                    }

                    if (thread.receive_status == ThreadCommunicator.ReceiveStatus.TIMEOUT)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : Timeout");
                    }

                    if (thread.receive_status == ThreadCommunicator.ReceiveStatus.INVALID_FORMAT)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : Invalid Format");
                    }

                    if (thread.receive_status == ThreadCommunicator.ReceiveStatus.BYTE_CONVERSION_ERROR)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : Can't convert Bytes into floats");
                    }

                    if (thread.receive_status == ThreadCommunicator.ReceiveStatus.NO_BYTES)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : No Bytes were sent");
                    }

                    if (thread.receive_status == ThreadCommunicator.ReceiveStatus.COLLUMN_SIZE_DISCREPANCY)
                    {
                        ModalWindowManager.instance.CreateModalWindow("ERROR : All collumns do not have the same size");
                    }
                    GetComponentInChildren<Text>().text = defaultText;
                }
            }
        }
    }

}
