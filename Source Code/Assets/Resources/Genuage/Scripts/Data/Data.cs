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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{

    public abstract class ICloudStorage : MonoBehaviour
    {

        public delegate void OnCloudCreatedEvent(int id);
        public event OnCloudCreatedEvent OnCloudCreated;
        protected void CallOnCloudCreated(int id)
        {
            OnCloudCreated(id);
        }

        public abstract void AddCloud(GameObject cloudpoint);

        public delegate void OnCloudDeletedEvent(int id);
        public event OnCloudDeletedEvent OnCloudDeleted;
        protected void CallOnCloudDeleted(int id)
        {
            OnCloudDeleted(id);
        }

        public abstract void RemoveCloud(int id);

        public abstract bool CheckID(int id);
    }

    public abstract class ICloudUpdater : MonoBehaviour
    {
        public abstract int GetSelection();

        public abstract CloudData LoadStatus(int id);

        #region display
        public abstract void SetCloudActive(int id, bool setting);
        public abstract void ChangeCloudScale(Vector3 new_scale);
        #endregion

        #region colormap
        public delegate void OnColorMapChangeEvent();
        public event OnColorMapChangeEvent OnColorMapChange;

        public abstract void ChangeColorMap(int id, string newMapName, bool reverse);

        public delegate void OnColorMapSaturationChangeEvent(float value);
        public event OnColorMapSaturationChangeEvent OnColorMapSaturationChange;

        public abstract void ChangeColorMapSaturation(float value1,float value2,string id);
        #endregion

        #region point_size
        public abstract void ChangePointSize(float value);
        #endregion

        #region link_clouds
        public delegate void OnCloudLinkedEvent();
        public event OnCloudLinkedEvent OnCloudLinked;
        public abstract void LinkClouds(List<int> cloudIDList);

        public delegate void OnCloudUnlinkedEvent();
        public OnCloudUnlinkedEvent OnCloudUnlinked;
        public abstract void UnlinkClouds(List<int> cloudIDList);


        public abstract void DisplayLinkedClouds(List<int> cloudIDList);
        #endregion

        #region calculate_density
        public abstract void CalculateDensity(float radius);
        #endregion
    }
}