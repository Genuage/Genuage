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
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{


    /// <summary>
    /// Script to be attached on ColorMapChoice button in a ColorMapScrollView. Handle changing the colormap name of the Cloud Status and if double click, show again the current colormap button
    /// </summary>
    public class ColorMapButton : IButtonScript, IPointerClickHandler
    {

        public GameObject _colormapdisplay;
        public Texture2D texture;
        public Texture2D reversedTexture;
        public string _color_map_name;
        public bool ActiveButton = true;
        public void Initialize()
        {
            //_colormapdisplay = GameObject.Find("ColorMapButton");
            button = gameObject.AddComponent<Button>();
            initializeClickEvent();

            _color_map_name = gameObject.name;
            //texture = GetComponent<RawImage>().texture as Texture2D;
            texture = ColorMapManager.instance.colormapDict[_color_map_name].texture;
            GetComponent<RawImage>().texture = texture;
        }


        public override void Execute()
        {
            if (ActiveButton)
            {
                CloudUpdater.instance.ChangeCurrentColorMap(_color_map_name);

            }


        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (ActiveButton)
            {

                if (eventData.clickCount == 1)
                {
                    bool reverse = false;

                    if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        reverse = true;
                    }
                    CloudUpdater.instance.ChangeCurrentColorMap(_color_map_name, reverse);
                }
            }
            _colormapdisplay.GetComponent<ColorMapLoader>().ChangeColorMap(_color_map_name);
            _colormapdisplay.SetActive(true);
            transform.parent.parent.parent.gameObject.SetActive(false);
        }


    }
}