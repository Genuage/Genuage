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
                    writer.WriteLine(currcloud.pointDataTable[i].position.x.ToString(CultureInfo.InvariantCulture) + "\t" +
                                     currcloud.pointDataTable[i].position.y.ToString(CultureInfo.InvariantCulture) + "\t" +
                                     currcloud.pointDataTable[i].position.z.ToString(CultureInfo.InvariantCulture) + "\t" +
                                     currcloud.pointDataTable[i].intensity.ToString(CultureInfo.InvariantCulture) + "\t" +
                                     currcloud.pointDataTable[i].trajectory.ToString(CultureInfo.InvariantCulture));
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
                        writer.WriteLine(currcloud.pointDataTable[selectedPointsList[i]].position.x.ToString(CultureInfo.InvariantCulture) + "\t" +
                                         currcloud.pointDataTable[selectedPointsList[i]].position.y.ToString(CultureInfo.InvariantCulture) + "\t" +
                                         currcloud.pointDataTable[selectedPointsList[i]].position.z.ToString(CultureInfo.InvariantCulture) + "\t" +
                                         currcloud.pointDataTable[selectedPointsList[i]].intensity.ToString(CultureInfo.InvariantCulture) + "\t" +
                                         currcloud.pointDataTable[selectedPointsList[i]].trajectory.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
        }
    


    }
}
