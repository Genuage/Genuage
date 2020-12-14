﻿/**
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

using Data;

namespace DesktopInterface
{


    public class ScaleSliders : MonoBehaviour
    {
        public Slider GlobalSlider;
        public Slider XSlider;
        public Slider YSlider;
        public Slider ZSlider;

        public Vector3 scale;

        private void Awake()
        {
            GlobalSlider.onValueChanged.AddListener(ChangeGlobalScale);
            XSlider.onValueChanged.AddListener(ChangeXScale);
            YSlider.onValueChanged.AddListener(ChangeYScale);
            ZSlider.onValueChanged.AddListener(ChangeZScale);
            scale = Vector3.one;
            CloudSelector.instance.OnSelectionChange += OnSelectionChange;
        }

        public void ChangeGlobalScale(float value)
        {
            scale = new Vector3(value, value, value);
            UpdateScale();
            XSlider.value = scale.x;
            YSlider.value = scale.y;
            ZSlider.value = scale.z;
        }

        public void ChangeXScale(float value)
        {
            scale.x = value;
            UpdateScale();
        }

        public void ChangeYScale(float value)
        {
            scale.y = value;
            UpdateScale();
        }

        public void ChangeZScale(float value)
        {
            scale.z = value;
            UpdateScale();
        }

        public void UpdateScale()
        {
            CloudUpdater.instance.ChangeCloudScale(scale);

        }

        public void OnSelectionChange(int id)
        {
            scale = CloudUpdater.instance.LoadCurrentStatus().globalMetaData.scale;
            XSlider.value = scale.x;
            YSlider.value = scale.y;
            ZSlider.value = scale.z;
        }
    }
}