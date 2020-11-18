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
using System.Globalization;
using System.IO;

using UnityEngine;
using SFB;
using Data;

namespace IO
{



    public class CloudSaver : MonoBehaviour
    {

        public static CloudSaver instance = null;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SaveJSON(string path)
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();
            CloudSaveableData savedata = new CloudSaveableData(data);
            string JSON = JsonUtility.ToJson(savedata);
            string directory = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(directory +Path.DirectorySeparatorChar+ filename + ".JSON"))
            {
                writer.WriteLine(JSON);
            }
        }

        public void WriteSaveFile(string path)
        {
            CloudData currcloud = CloudUpdater.instance.LoadCurrentStatus();
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path))
            {
                for (int i = 0; i < currcloud.pointDataTable.Count; i++)
                {
                    string s = "";
                    for (int j = 0; j < currcloud.columnData.Count; j++)
                    {
                        s = s + currcloud.columnData[j][i].ToString(CultureInfo.InvariantCulture);
                        if (j < currcloud.columnData.Count - 1)
                        {
                            s = s + "\t";
                        }
                    }
                    writer.WriteLine(s);
                }
            }

            SaveJSON(path);

        }


        
        public void SaveSelection(string path)
        {
            CloudData currcloud = CloudUpdater.instance.LoadCurrentStatus();
            int[] selectedPointsList = new int[currcloud.globalMetaData.SelectedPointsList.Count];
            currcloud.globalMetaData.SelectedPointsList.CopyTo(selectedPointsList);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path))
            {
                for (int i = 0; i < selectedPointsList.Length; i++)
                {
                    if (currcloud.pointMetaDataTable[selectedPointsList[i]].isHidden == false)
                    {
                        string s = "";
                        for (int j = 0; j < currcloud.columnData.Count; j++)
                        {
                            s = s + currcloud.columnData[j][selectedPointsList[i]].ToString(CultureInfo.InvariantCulture);
                            if (j < currcloud.columnData.Count - 1)
                            {
                                s = s + "\t";
                            }
                        }
                        writer.WriteLine(s);

                    }
                }
            }
        }

        public void savePoints(List<int> IDList, string path)
        {
            CloudData currcloud = CloudUpdater.instance.LoadCurrentStatus();
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path))
            {
                for (int i = 0; i < IDList.Count; i++)
                {
                    if (currcloud.pointMetaDataTable[IDList[i]].isHidden == false)
                    {
                        string s = "";
                        for(int j = 0; j < currcloud.columnData.Count; j++)
                        {
                            s = s + currcloud.columnData[j][IDList[i]].ToString(CultureInfo.InvariantCulture);
                            if (j < currcloud.columnData.Count - 1)
                            {
                                s = s + "\t";
                            }
                        }
                        writer.WriteLine(s);
                    }
                }
            }

        }

        public void SaveSelectionCSV(string path)
        {
            string separator = "; ";

            CloudData currcloud = CloudUpdater.instance.LoadCurrentStatus();


            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path))
            {
                writer.WriteLine("id" + separator + "x"+separator + "y" + separator + "z" + separator + 
                                 "color" + separator + "time" + separator + "trajectory" + separator + 
                                 "phi" + separator + "theta" + separator + "size" + "\t");
                foreach (var i in currcloud.pointDataTable)
                {
                    //if (currcloud.pointMetaDataTable[i].isHidden == false)
                    //{
                        string s = currcloud.pointDataTable[i.Key].pointID + separator + 
                                   currcloud.pointDataTable[i.Key].position.x + separator +
                                   currcloud.pointDataTable[i.Key].position.y + separator +
                                   currcloud.pointDataTable[i.Key].position.z + separator +
                                   currcloud.pointDataTable[i.Key].intensity + separator +
                                   currcloud.pointDataTable[i.Key].time + separator +
                                   currcloud.pointDataTable[i.Key].trajectory + separator +
                                   currcloud.pointDataTable[i.Key].phi_angle + separator +
                                   currcloud.pointDataTable[i.Key].theta_angle + separator +
                                   currcloud.pointDataTable[i.Key].size + "\t";

                        writer.WriteLine(s);

                    //}
                }
            }

        }



    }
}
