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
using UnityEngine.EventSystems;


namespace Display
{


    public class MouseDraggableItem : MonoBehaviour
    {
        public float moveSpeed = 0.001f;

        Vector3 previousMousePosition;
        Vector3 objectInitialPosition;
        Vector3 deltaMousePosition;
        Camera camera;

        // Start is called before the first frame update
        void Start()
        {
            objectInitialPosition = this.transform.position;
            camera = DesktopApplication.instance.MainCamera.GetComponent<Camera>();
        }

        /**
        // Update is called once per frame
        void Update()
        {
            if (reset)
            {
                reset = false;
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time - timesinceClick < catchtime)
                {
                    transform.rotation = cloudInitialRotation;
                    transform.position = objectInitialPosition;
                    reset = true;
                }
                else
                {
                    timesinceClick = Time.time;
                    reset = false;
                }
            }
        }
        **/

        void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);

                if (raycastResults.Count > 0 && raycastResults[0].gameObject == this.gameObject)
                {
                    if (Input.GetMouseButton(0))
                    {
                        deltaMousePosition = Input.mousePosition - previousMousePosition;
                        //transform.Translate(deltaMousePosition * moveSpeed, Space.World);
                        transform.position = Input.mousePosition;
                    }

                    previousMousePosition = Input.mousePosition;
                }
            }
        }
    }
}