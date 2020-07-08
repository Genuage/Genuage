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
using Data;

namespace DesktopInterface
{


    public class ColormapSaturationSlider : SliderScript
    {
        public int multiplier;
        public Slider otherSlider;
        // Start is called before the first frame update
        void Start()
        {
            slider = GetComponent<Slider>();
            //slider.onValueChanged.AddListener(delegate { CloudUpdater.instance.changeColorMapSaturation(multiplier * slider.value); });
            CloudUpdater.instance.OnColorMapChange += ResetSlider;
            InitializeSliderEvent();
        }

        public override void Execute(float value)
        {
            CloudUpdater.instance.ChangeColorMapSaturation(value, otherSlider.value, gameObject.name);
        }

        public void ResetSlider(string name)
        {
            ColorMap map = ColorMapManager.instance.GetColorMap(name);
            switch (gameObject.name)
            {
                case "cmin":
                    slider.value = map.cminValue;
                    break;
                case "cmax":
                    slider.value = map.cmaxValue;
                    break;
                default:
                    Debug.Log("Saturation Slider Reset Error");
                    break;
            }
            if(map.colorArray.Length > 2)
            {
                //slider.interactable = false;
                //Debug.Log("falsecheck");
            }
            else
            {
                //Debug.Log("truecheck");
                slider.interactable = true;
            }
        }

        private void OnDestroy()
        {
            CloudUpdater.instance.OnColorMapChange -= ResetSlider;

        }
    }
}