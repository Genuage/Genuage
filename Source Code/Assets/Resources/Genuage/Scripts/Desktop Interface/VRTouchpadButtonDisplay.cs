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

using VRTK;

namespace VR_Interaction
{


    public class VRTouchpadButtonDisplay : MonoBehaviour
    {
        public Image imgup;
        public Image imgdown;
        public Image imgleft;
        public Image imgright;

        public Text textup;
        public Text textdown;
        public Text textleft;
        public Text textright;


        public VRTK_ControllerEvents controller;

        // Start is called before the first frame update
        void Awake()
        {
            //controller = GetComponent<VRTK_ControllerEvents>();
            controller.TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
            controller.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
            controller.TouchpadTouchEnd += new ControllerInteractionEventHandler(ReinitializeImages);
        }

        private void ReinitializeImages(object sender, ControllerInteractionEventArgs e)
        {
            imgup.color = Color.white;
            imgdown.color = Color.white;
            imgleft.color = Color.white;
            imgright.color = Color.white;
            textup.gameObject.SetActive(false);
            textdown.gameObject.SetActive(false);
            textleft.gameObject.SetActive(false);
            textright.gameObject.SetActive(false);


        }

        private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            //Debug.Log("x " + e.touchpadAxis.x);
            //Debug.Log("y " + e.touchpadAxis.y);
            imgdown.color = Color.white;
            imgup.color = Color.white;
            imgleft.color = Color.white;
            imgright.color = Color.white;
            textup.gameObject.SetActive(false);
            textdown.gameObject.SetActive(false);
            textleft.gameObject.SetActive(false);
            textright.gameObject.SetActive(false);

            if (e.touchpadAxis.y >= 0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
            {
                imgup.color = Color.green;
                textup.gameObject.SetActive(true);
            }
            else if (e.touchpadAxis.y <= -0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
            {
                imgdown.color = Color.green;
                textdown.gameObject.SetActive(true);
            }
            else if (e.touchpadAxis.x <= -0.15 && e.touchpadAxis.y < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y > (-Mathf.Sqrt(2f) / 2))
            {
                imgleft.color = Color.green;
                textleft.gameObject.SetActive(true);
            }
            else if (e.touchpadAxis.x >= 0.15 && e.touchpadAxis.y < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.y > (-Mathf.Sqrt(2f) / 2))
            {
                imgright.color = Color.green;
                textright.gameObject.SetActive(true);
            }


        }

        private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            /**
            if (e.touchpadAxis.y > 0)
            {
                Debug.Log("Option UP !");
            }

            else
            {
                Debug.Log("Option DOWN !");
            }
            **/
        }
    }
}