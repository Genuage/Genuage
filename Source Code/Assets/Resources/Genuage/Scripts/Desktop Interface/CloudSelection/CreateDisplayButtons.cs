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
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{


    public class CreateDisplayButtons : MonoBehaviour
    {
        public GameObject _selectionButtonPrefab;
        public Transform _selftransform;
        public Dictionary<int, GameObject> _selectionButtonList;
        // Start is called before the first frame update
        void Start()
        {
            _selectionButtonList = new Dictionary<int, GameObject>();
            _selftransform = GetComponent<Transform>();
            CloudStorage.instance.OnCloudCreated += CreateNewButton;
            CloudStorage.instance.OnCloudDeleted += DestroyButton;

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void CreateNewButton(int id)
        {
            GameObject son = Instantiate(_selectionButtonPrefab) as GameObject;
            son.GetComponent<DisplaySelectorButton>()._id = id;
            son.name = id.ToString();
            son.GetComponent<Transform>().SetParent(_selftransform, false);
            son.GetComponentInChildren<Text>().text = id.ToString();
            _selectionButtonList.Add(id, son);

        }

        public void DestroyButton(int id)
        {
            if (CheckId(id))
            {
                Destroy(_selectionButtonList[id]);
                _selectionButtonList.Remove(id);
            }
        }

        public bool CheckId(int id)
        {
            return _selectionButtonList.ContainsKey(id);
        }

    }
}