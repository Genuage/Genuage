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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Data
{

    public class ColorMapManager : MonoBehaviour
    {
        public RawImage image;
        public static ColorMapManager instance = null;
        public Dictionary<string,ColorMap> colormapDict;
        public Dictionary<string, ColorMap> circularcolormapDict;



        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
                InitializeColorMaps();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public delegate void OnColorMapsInitializedEvent();
        public event OnColorMapsInitializedEvent OnColorMapsInitialized;

        private void InitializeColorMaps()
        {
            colormapDict = new Dictionary<string, ColorMap>();
            circularcolormapDict = new Dictionary<string, ColorMap>();
            ColorMap c1 = GenerateColorMap("autumn",new List<Color>() {Color.red, Color.yellow});
            ColorMap c2 = GenerateColorMap("spring", new List<Color>() {Color.magenta, Color.yellow });
            ColorMap c3 = GenerateColorMap("winter", new List<Color>() {Color.blue, Color.green });
            ColorMap c4 = GenerateColorMap("greenred", new List<Color>() {Color.green, Color.red});
            ColorMap c5 = GenerateColorMap("Blues", new List<Color>() {Color.blue, Color.white });
            ColorMap c6 = GenerateColorMap("Greens", new List<Color>() {Color.green, Color.white });
            ColorMap c7 = GenerateColorMap("hot", new List<Color>() {Color.black, Color.red, Color.yellow, Color.white });
            ColorMap c8 = GenerateColorMap("jet", new List<Color>() {Color.blue, Color.cyan, Color.green, Color.yellow, Color.red});
            ColorMap c9 = GenerateColorMap("hsv", new List<Color>() {Color.red, Color.yellow, Color.green,Color.cyan, Color.blue, Color.magenta, Color.red }); // circular
            ColorMap c10 = GenerateColorMap("colorblind_contrast", new List<Color>() { Color.cyan, Color.red});
            ColorMap c11 = GenerateColorMap("colorblind_contrast_2", new List<Color>() { Color.cyan, new Color(1,0.27f,0) });
            ColorMap c12 = GenerateColorMap("colorblind_contrast_3", new List<Color>() { new Color(0.5f,0,0.5f), new Color(1, 1, 0) });
            ColorMap circularc2 = GenerateColorMap("circular2", new List<Color>() { Color.red, Color.yellow, Color.red });
            ColorMap circularc3 = GenerateColorMap("circular3", new List<Color>() { Color.blue, Color.green, Color.blue });
            ColorMap circularc4 = GenerateColorMap("circular4", new List<Color>() { Color.blue, Color.green, Color.yellow, Color.red, Color.blue });


            colormapDict.Add(c1.name, c1);
            colormapDict.Add(c2.name, c2);
            colormapDict.Add(c3.name, c3);
            colormapDict.Add(c4.name, c4);
            colormapDict.Add(c5.name, c5);
            colormapDict.Add(c6.name, c6);
            colormapDict.Add(c7.name, c7);
            colormapDict.Add(c8.name, c8);
            colormapDict.Add(c9.name, c9);
            colormapDict.Add(c10.name, c10);
            colormapDict.Add(c11.name, c11);
            colormapDict.Add(c12.name, c12);

            colormapDict.Add(circularc2.name, circularc2);
            colormapDict.Add(circularc3.name, circularc3);
            colormapDict.Add(circularc4.name, circularc4);

            circularcolormapDict.Add(c9.name, c9);
            circularcolormapDict.Add(circularc2.name, circularc2);
            circularcolormapDict.Add(circularc3.name, circularc3);
            circularcolormapDict.Add(circularc4.name, circularc4);
            circularcolormapDict.Add(c8.name, c8);

        }

        private ColorMap GenerateColorMap(string name, List<Color> colors)
        {
            ColorMap colormap = new ColorMap(name, colors.ToArray());
            return colormap;
        }

        public ColorMap GetColorMap(string name)
        {
            return colormapDict[name];
        }

        public Dictionary<string,ColorMap>.KeyCollection GetAllColormapNames()
        {
            return colormapDict.Keys;
        }

        public Dictionary<string, ColorMap>.KeyCollection GetAllCircularColormapNames()
        {
            return circularcolormapDict.Keys;
        }
    }
}