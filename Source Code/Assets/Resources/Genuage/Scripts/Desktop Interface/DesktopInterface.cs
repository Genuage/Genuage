﻿/**
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopInterface
{
    public abstract class IButtonScript : MonoBehaviour
    {
        //Abstract class used to put listener on a button at startup. Use this if you want a button to execute a specific function.

        public Button button;

        protected void initializeClickEvent()
        {
            button.onClick.AddListener(Execute);
        }

        public abstract void Execute();

    }

    public class ISliderScript : MonoBehaviour
    {
        public Slider slider;
        public InputField _field;

        protected void InitializeSliderEvent()
        {
            slider.onValueChanged.AddListener(Execute);
            if (_field)
            {
                _field.onEndEdit.AddListener(InputLabelChanged);
            }
        }

        /// <summary>
        /// Handle NewInput String
        /// </summary>
        /// <param name="value"></param>
        public virtual void InputLabelChanged(string value)
        {
            float new_value = Single.Parse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);

            slider.value = new_value;
        }

        public virtual void Execute(float value)
        {

        }
    }

    public abstract class IDropdownScript : MonoBehaviour
    {
        public Dropdown dropdown;

        protected void InitialiseClickEvent()
        {
            dropdown.onValueChanged.AddListener(Execute);
        }

        public abstract void Execute(int value);

    }
}