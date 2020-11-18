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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IO;

namespace DesktopInterface
{


    public class PythonThreadCommunicationButton : ThreadCommunicationButton
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
            thread.option = ThreadCommunicator.CommunicatorOption.RECEIVE_DATA;
        }

        protected override void ThreadSuccess()
        {
            CloudLoader.instance.LoadFromConnection(thread.dataList);
            //ProcessReceivedData(thread.dataList);
        }

        private void ProcessReceivedData(List<float[]> SelectedAlphaList)
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();
            float[] alphacolumn = new float[data.pointDataTable.Count];
            for (int i = 0; i < alphacolumn.Length; i++)
            {
                alphacolumn[i] = 0f;
            }

            if (!data.globalMetaData.alphacolumnExists)
            {
                CreateAlphaColumn(data, alphacolumn);
            }

            //float[]
            for (int i = 0; i < SelectedAlphaList.Count; i++)
            {
                alphacolumn[(int)SelectedAlphaList[0][i]] = SelectedAlphaList[1][i];
            }

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
            metadata.MaxValue = max;
            metadata.MinValue = min;
            metadata.MinThreshold = min;
            metadata.MaxThreshold = max;
            metadata.Range = max - min;

            data.globalMetaData.columnMetaDataList[data.globalMetaData.alphacolumnIndex] = metadata;

        }

        private void CreateAlphaColumn(CloudData data, float[] array)
        {
            data.columnData.Add(array);
            ColumnMetadata metadata = new ColumnMetadata();
            metadata.ColumnID = data.columnData.Count - 1;
            metadata.MaxValue = 0;
            metadata.MinValue = 0;
            metadata.MinThreshold = 0;
            metadata.MaxThreshold = 0;
            metadata.Range = 0 - 0;
            data.globalMetaData.columnMetaDataList.Add(metadata);
            data.globalMetaData.alphacolumnExists = true;
            data.globalMetaData.alphacolumnIndex = metadata.ColumnID;

            ModalWindowManager.instance.CreateModalWindow("Column has been added to the data." + "\n");
        }

    }
}