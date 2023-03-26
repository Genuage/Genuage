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


using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Collections;
using System.Collections.Generic;

namespace Display
{


    /// <summary>
    /// Script to put on a camera. Allow user to move the camera using the mouse around a "target" point
    /// </summary>
    public class DragMouseOrbit : MonoBehaviour
    {
        public Transform target;
        public float distance = 0f;
        public float sensitivityDistance = 1.5f;
        public float distanceMin = -1f;
        public float distanceMax = 10f;
        //public float smoothTime = 8f;
        

        bool _pressed;

        bool _state;
        bool interacting = false;
        public bool State
        {
            get { return _state; }
            set
            {
                if (_state == value) return;
                _state = value;
                if (OnStateChange != null)
                    OnStateChange(_state);
            }
        }
        public delegate void OnStateChangeEvent(bool newVal);
        public event OnStateChangeEvent OnStateChange;

        public Vector3 cameraInitialPosition;
        public float timesinceClick;
        public float catchtime = 0.25f;
        public bool reset = false;


        private void OnDisable()
        {
            State = false;
        }

        private void OnEnable()
        {
            State = true;
        }


        void Start()
        {
            transform.position = new Vector3(0.0f, 0.0f, -distance);
            cameraInitialPosition = transform.position;

            Vector3 angles = transform.eulerAngles;
            // Make the rigid body not change rotation
            if (GetComponent<Rigidbody>())
            {
                GetComponent<Rigidbody>().freezeRotation = true;
            }

            _pressed = false;

        }

        public void Update()
        {
            if (reset)
            {
                reset = false;
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time - timesinceClick < catchtime)
                {
                    transform.position = cameraInitialPosition;
                    distance = 2.0f;
                    reset = true;
                }
                else
                {
                    timesinceClick = Time.time;
                    reset = false;
                }
            }
        }

        void LateUpdate()
        {
            if(reset) { return; }
            interacting = false;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;
                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);

                if (raycastResults.Count > 0 && raycastResults[0].gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                    return;
                }
            }
            if (Input.GetMouseButton(0))
            {

                interacting = true;


                if (_pressed || (!(GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition).y > 1 || GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition).x > 1)))
                {
                    _pressed = true;
                }
            }
            else
            {
                _pressed = false;
            }
            if ((!(GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition).y > 1 || GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition).x > 1)))
            {
                distance -= Input.GetAxis("Mouse ScrollWheel") * sensitivityDistance;
                distance = Mathf.Clamp(distance, distanceMin, distanceMax);
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = negDistance;

            //GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, distance, Time.deltaTime * 5f);
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 5f); ;

        }

        
    }
}