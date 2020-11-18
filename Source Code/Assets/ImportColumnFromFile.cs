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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using Data;

namespace DesktopInterface
{


    public class ImportColumnFromFile : IButtonScript
    {
        //Made to load a column from a ASCII file with a 2 column format
        //format : ID   NEWVALUE
        //TODO : Generalize system
        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();

        }

        public override void Execute()
        {

            var extensions = new[]
{
                new ExtensionFilter("3D format", "3d"),
                new ExtensionFilter("text format", "txt"),
                new ExtensionFilter("All Files", "*" )
            };
            StandaloneFileBrowser.OpenFilePanelAsync("Open File", "" ,  extensions, true, (string[] paths) => { LoadColumns(paths); });

        }

        private void LoadColumns(string[] paths)
        {
            foreach(string s in paths)
            {
                LoadColumn(s);
            }
        }

        private void LoadColumn(string path)
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();

            string[] lines = File.ReadAllLines(path);
            int N = lines.Length;
            if (data.pointDataTable.Count == N)
            {
                //float[] idcolumn = new float[N];
                float[] newColumn = new float[N];

                for(int i = 0; i < N; i++)
                {
                    string[] entries = lines[i].Split('\t');

                    //idcolumn[i] = Single.Parse(entries[0], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
                    newColumn[i] = Single.Parse(entries[1], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);

                }

                float max = Mathf.NegativeInfinity;
                float min = Mathf.Infinity;

                for (int i = 0; i < newColumn.Length; i++)
                {
                    if (newColumn[i] <= min)
                    {
                        min = newColumn[i];
                    }

                    if (newColumn[i] >= max)
                    {
                        max = newColumn[i];
                    }
                }
                data.columnData.Add(newColumn);
                ColumnMetadata metadata = new ColumnMetadata();
                metadata.ColumnID = data.columnData.Count - 1;
                metadata.MaxValue = max;
                metadata.MinValue = min;
                metadata.MinThreshold = min;
                metadata.MaxThreshold = max;
                metadata.Range = max - min;
                data.globalMetaData.columnMetaDataList.Add(metadata);

            }
            else
            {
                Debug.Log("ERROR, File didn't have the same amount of lines as current cloud");
            }
        }
    }
}