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

using Data;
using IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace DesktopInterface
{


    public class PythonSendDataButton : ThreadCommunicationButton
    {

        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
            defaultText = "Communication Python";
        }
        protected override void CreateThread()
        {
            thread = new ZMQCommunicatorPython();
            List<float[]> DataList = PrepareData();
            thread.dataList = DataList;
            thread.option = ThreadCommunicator.CommunicatorOption.SEND_DATA;

        }

        private List<float[]> PrepareData()
        {
            List<float[]> list = new List<float[]>();
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();
            float[] ids = new float[data.globalMetaData.SelectedPointsList.Count];
            float[] xvalues = new float[data.globalMetaData.SelectedPointsList.Count];
            float[] yvalues = new float[data.globalMetaData.SelectedPointsList.Count];
            float[] zvalues = new float[data.globalMetaData.SelectedPointsList.Count];
            float[] colorvalues = new float[data.globalMetaData.SelectedPointsList.Count];
            float[] timevalues = new float[data.globalMetaData.SelectedPointsList.Count];
            float[] trajectoryvalues = new float[data.globalMetaData.SelectedPointsList.Count];
            float[] phivalues = new float[data.globalMetaData.SelectedPointsList.Count];
            float[] thetavalues = new float[data.globalMetaData.SelectedPointsList.Count];
            float[] sizevalues = new float[data.globalMetaData.SelectedPointsList.Count];
            int index = 0;
            foreach (int i in data.globalMetaData.SelectedPointsList)
            {
                ids[index] = i;
                xvalues[index] = data.pointDataTable[i].position.x;
                yvalues[index] = data.pointDataTable[i].position.y;
                zvalues[index] = data.pointDataTable[i].position.z;
                colorvalues[index] = data.pointDataTable[i].intensity;
                timevalues[index] = data.pointDataTable[i].time;
                trajectoryvalues[index] = data.pointDataTable[i].trajectory;
                phivalues[index] = data.pointDataTable[i].phi_angle;
                thetavalues[index] = data.pointDataTable[i].theta_angle;
                sizevalues[index] = data.pointDataTable[i].size;
                index++;
            }
            list.Add(ids);
            list.Add(xvalues);
            list.Add(yvalues);
            list.Add(zvalues);
            list.Add(colorvalues);
            list.Add(timevalues);
            list.Add(trajectoryvalues);
            list.Add(phivalues);
            list.Add(thetavalues);
            list.Add(sizevalues);

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
        protected override void ThreadSuccess()
        {
            CloudLoader.instance.LoadFromConnection(thread.dataList);
            //ProcessReceivedData(thread.dataList);
        }


    }
}
