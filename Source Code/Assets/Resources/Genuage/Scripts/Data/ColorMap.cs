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
    public class ColorMap
    {
        public string name;
        public Color[] colorArray;
        public Texture2D texture;
        //public Texture2D largetexture;
        public Texture2D reversedtexture;

        public float cmaxValue;
        public float cminValue;

        public ColorMap(string name, Color[] colorArray)
        {
            this.name = name;
            this.colorArray = colorArray;
            CreateSecondaryTextures();
            cmaxValue = 1f;
            cminValue = 0f;
        }
        
        private void CreateSecondaryTextures()
        {
            texture = CreateColormap(colorArray);

        }


        private Texture2D CreateColormap(Color[] colors)
        {
            //Debug.Log(colors.Length);
            int width = 256;
            int spacing = width / (colors.Length);

            Texture2D colormap_texture = new Texture2D(width, 1, TextureFormat.RGBA32, true);
            colormap_texture.filterMode = FilterMode.Bilinear;
            colormap_texture.wrapMode = TextureWrapMode.Clamp;

            for (int i = 0; i < width; i++)
            {
                int j = Mathf.Min(i / spacing, colors.Length - 2);
                colormap_texture.SetPixel(i, 0, Color.Lerp(colors[j], colors[j + 1], (float)(i - (j * spacing)) / (float)spacing));
            }
            colormap_texture.Apply();

            return colormap_texture;
        }


    }
}