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
using UnityEngine.UI;
using Display;
using Data;

namespace DesktopInterface
{


    /// <summary>
    /// Script to put on the button that can switch between VR camera and Desktop Camera
    /// </summary>
    public class MenuVRButton : IButtonScript
    {

        public Camera _desktop_camera;
        //public Camera _display_camera;
        public Camera _VR_Camera;
        private Text _label_vr;

        public bool _vr_enabled;

        bool scriptcheck = false;

        public void Start()
        {
            _desktop_camera = CameraManager.instance.desktop_camera;
            _VR_Camera = CameraManager.instance.vr_camera;
            button = GetComponent<Button>();
            initializeClickEvent();
            _vr_enabled = false;
            _label_vr = transform.GetComponentInChildren<Text>();
            _VR_Camera.enabled = false;
            _desktop_camera.enabled = true;
            _desktop_camera.gameObject.SetActive(true);
        }

        public void SwitchVR()
        {
            _vr_enabled = !_vr_enabled;

            _desktop_camera.enabled = !_vr_enabled;
            _desktop_camera.gameObject.SetActive(!_vr_enabled);
            _VR_Camera.enabled = _vr_enabled;
            _VR_Camera.gameObject.SetActive(_vr_enabled);
            _desktop_camera.GetComponent<DragMouseOrbit>().enabled = !_vr_enabled;
            foreach (KeyValuePair<int,CloudData> obj in CloudStorage.instance.table)
            {
                obj.Value.transform.parent.gameObject.GetComponentInChildren<DragMouse>().enabled = !_vr_enabled;
            }
            if (_vr_enabled) // Let the user know the button state
            {
                _label_vr.text = "Desktop";
            }
            else
            {
                _label_vr.text = "VR";
            }

            if (!scriptcheck)
            {
//DO CONTROLLER CHANGE
            }
            DesktopApplication.instance.VR_Enabled = _vr_enabled;
        }

        public override void Execute()
        {
            SwitchVR();
        }
    }
}