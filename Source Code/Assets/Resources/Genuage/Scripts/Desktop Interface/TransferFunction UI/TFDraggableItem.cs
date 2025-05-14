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


    public class TFDraggableItem : MonoBehaviour, IDragHandler
    {
        public float moveSpeed = 0.001f;

        Vector3 previousMousePosition;
        Vector3 objectInitialPosition;
        Vector3 deltaMousePosition;

        Vector3 Offset;

        [SerializeField]
        Vector2 startPoint;

        [SerializeField]
        Vector2 endPoint;

        [SerializeField]
        bool drag;


        public RectTransform DragTransform;
        private RectTransform rt;
        public float AreaWidth;
        public float AreaHeight;
        Vector3 LowerCorner;
        Vector3 HigherCorner;
        Vector2 localMousePosition;
        Rect interactRect;

        // Start is called before the first frame update
        void Start()
        {
            objectInitialPosition = this.transform.position;
        }

      



        public void OnDrag(PointerEventData eventData)
        {
            rt = transform.parent.GetComponent<RectTransform>();
            AreaWidth = rt.rect.width;
            AreaHeight = rt.rect.height;
            LowerCorner = new Vector3(-AreaWidth / 2, -AreaHeight / 2, 0);
            HigherCorner = new Vector3(AreaWidth / 2, AreaHeight / 2, 0);

            if (Bounded(Input.mousePosition, LowerCorner, HigherCorner))
            {
                Debug.Log("success");
                DragTransform.anchoredPosition += eventData.delta / DesktopApplication.instance.Canvas.GetComponent<Canvas>().scaleFactor;
            }

        }

        bool Bounded(Vector3 mousePosition, Vector3 LowerCorner, Vector3 HigherCorner)
        {


            if ((mousePosition.x > LowerCorner.x) && (mousePosition.y > LowerCorner.y))
            {
                if ((mousePosition.x < HigherCorner.x) && (mousePosition.y < HigherCorner.y))
                {

                    return true;
                }


            }
            return false;

        }

    }
}