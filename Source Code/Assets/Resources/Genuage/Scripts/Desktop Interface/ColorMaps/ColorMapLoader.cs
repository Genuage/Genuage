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



using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{


    /// <summary>
    /// Script to put on the button that display the current color map. Handle showing the current color map and opening the ColorMapScrollView to choose a new colormap
    /// </summary>
    public class ColorMapLoader : IButtonScript
    {
        public string _current_colormap_name = "autumn";
        public GameObject _colormap_selector;
        public bool _colormap_reversed;
        //public CloudStatus _cloud_status;


        public void Awake()
        {
            button = gameObject.GetComponent<Button>();
            initializeClickEvent();
            //gameObject.AddComponent<RawImage>();
            //_cloud_status.OnColorMapChange += ChangeColorMap;
            CloudSelector.instance.OnSelectionChange += ChangeColorMap;
            CloudUpdater.instance.OnColorMapSaturationChange += GenerateNewTexture;
            CloudUpdater.instance.OnColorMapReversed += ReverseColorMap;
            CloudUpdater.instance.OnColorMapChange += ChangeColorMap;

            ChangeColorMap("autumn");
        }

        public override void Execute()
        {
            if (_colormap_selector)
            {
                _colormap_selector.SetActive(this.gameObject.activeSelf);
                this.gameObject.SetActive(!this.gameObject.activeSelf);
            }
        }

        /// <summary>
        /// Update color map to be displayed by its name
        /// </summary>
        /// <param name="name">name of the color map to be currently displayed</param>
        public void ChangeColorMap(string name)
        {
            //GetComponent<RawImage>().texture = Resources.Load(UILocation._large_color_rotated + name) as Texture;
            Texture2D newtex = ColorMapManager.instance.colormapDict[name].texture;
            GetComponent<RawImage>().texture = newtex;
           _current_colormap_name = name;
        }

        public void ChangeColorMap(int id)
        {
            string currentMap = CloudStorage.instance.table[id].globalMetaData.colormapName;

            Texture2D newtex = ColorMapManager.instance.colormapDict[currentMap].texture;
            GetComponent<RawImage>().texture = newtex;

            _current_colormap_name = currentMap;
        }

        public void GenerateNewTexture(float value1, float value2)
        {

            ChangeColorMap(_current_colormap_name);
            /**
            Texture2D oldtexture = Resources.Load(UILocation._large_color_rotated + _current_colormap_name) as Texture2D;
            Texture2D colortexture = Resources.Load(UILocation._thin_color_maps + _current_colormap_name) as Texture2D;
    **/
            /**
            Texture2D oldtexture = ColorMapManager.instance.colormapDict[_current_colormap_name].texture;
            Color[] colormap = oldtexture.GetPixels();
            Array.Reverse(colormap);
            float step = 1f / oldtexture.height;
            Texture2D newtexture = new Texture2D(oldtexture.width, oldtexture.height, TextureFormat.ARGB32, false);
            List<Color> newcolormap = new List<Color>();

            int floor;
            int roof;
            //set to 0
            if (value1 < value2)
            {
                roof = Mathf.RoundToInt(255 - (Mathf.Abs(value1) * 255));
                floor = Mathf.RoundToInt(255 * value2);
            }
            else
            {
                roof = Mathf.RoundToInt(255 - (value2 * 255));
                floor = Mathf.RoundToInt(255 * value1);
            }

            
            for (int i = 0; i < newtexture.height; i++)
            {
                for (int j = 0; j < newtexture.width; j++)
                {
                    float newvalue = step;

                    if (i < roof && i >= floor)
                    {
                        newtexture.SetPixel(j, i, Color.Lerp(colormap[0], colormap[colormap.Length - 1], newvalue));
                    }
                    else if (i >= roof)
                    {
                        newtexture.SetPixel(j, i, colormap[colormap.Length - 1]);
                    }
                    else if (i < floor)
                    {
                        newtexture.SetPixel(j, i, colormap[0]);
                    }
                }
                step = step + (1f / newtexture.height);

            }
            
            newtexture.Apply();

            GetComponent<RawImage>().texture = newtexture;
            **/
        }

        private void ReverseColorMap(bool reversed)
        {
            Texture2D oldtexture = ColorMapManager.instance.colormapDict[_current_colormap_name].texture;

            if (reversed == true)
            {


                Color[] colormap = oldtexture.GetPixels();

                Texture2D newtexture = new Texture2D(oldtexture.width, oldtexture.height, TextureFormat.ARGB32, false);

                Color[] pixels = oldtexture.GetPixels();
                Color[] pixelsFlipped = new Color[pixels.Length];

                for (int i = 0; i < oldtexture.height; i++)
                {
                    Array.Copy(pixels, i * oldtexture.width, pixelsFlipped, (oldtexture.height - i - 1) * oldtexture.width, oldtexture.width);
                }

                newtexture.SetPixels(pixelsFlipped);
                
                newtexture.Apply();

                GetComponent<RawImage>().texture = newtexture as Texture;

                _colormap_reversed = true;
            }
            else
            {
                GetComponent<RawImage>().texture = oldtexture;

            }
        }

        private void OnDestroy()
        {
            CloudSelector.instance.OnSelectionChange -= ChangeColorMap;
            CloudUpdater.instance.OnColorMapSaturationChange -= GenerateNewTexture;
            CloudUpdater.instance.OnColorMapReversed -= ReverseColorMap;

        }
    }
}