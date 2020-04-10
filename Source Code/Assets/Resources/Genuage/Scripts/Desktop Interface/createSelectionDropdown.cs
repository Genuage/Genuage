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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{


    public class CreateSelectionDropdown : MonoBehaviour
    {
        //public GameObject _selectionOptionPrefab;
        //public Transform _selftransform;
        public Dropdown dropdown;
        public Dictionary<int, Dropdown.OptionData> _selectionOptionList;
        public int _selected_id;
        // Start is called before the first frame update
        void Start()
        {
            _selected_id = 0;
            dropdown = GetComponent<Dropdown>();
            _selectionOptionList = new Dictionary<int, Dropdown.OptionData>();
            //_selftransform = GetComponent<Transform>();
            dropdown.onValueChanged.AddListener(delegate { Update_id(dropdown); });
            CloudStorage.instance.OnCloudCreated += CreateNewOption;
            CloudStorage.instance.OnCloudDeleted += DestroyOption;
            //CloudSelector.instance.OnSelectionChange += changeOptionColors;

        }

        public void Update_id(Dropdown change)
        {

            int temp = 1;
            if (int.TryParse(change.captionText.text, out temp))
            {
                Debug.Log("ok");
                _selected_id = temp;
            }

        }

        public void CreateNewOption(int id)
        {
            Dropdown.OptionData newData = new Dropdown.OptionData();
            newData.text = "" + id;
            _selectionOptionList.Add(id, newData);
            dropdown.options.Add(newData);
            /**
            GameObject son = Instantiate(_selectionOptionPrefab) as GameObject;
            //son.GetComponent<CloudSelectionOption>().id = id;
            son.name = id.ToString();
            son.GetComponent<Transform>().SetParent(_selftransform, false);
            son.GetComponentInChildren<Text>().text = id.ToString();
            _selectionOptionList.Add(id, son);
            **/
        }

        public void DestroyOption(int id)
        {

            if (CheckId(id))
            {
                _selectionOptionList.Remove(id);
                dropdown.ClearOptions();
                foreach (KeyValuePair<int, Dropdown.OptionData> item in _selectionOptionList)
                {
                    dropdown.options.Add(item.Value);
                }
            }

        }

        public bool CheckId(int id)
        {
            return _selectionOptionList.ContainsKey(id);
        }

        /**
        public void changeOptionColors(int selected_id)
        {

            foreach (KeyValuePair<int, GameObject> item in _selectionOptionList)
            {
                Button current = item.Value.GetComponent<Button>();
                if (current != null)
                {
                    if (item.Key == selected_id)
                    {
                        ColorBlock cb = current.colors;
                        cb.normalColor = Color.white;
                        cb.highlightedColor = Color.white;
                        current.colors = cb;
                    }
                    else
                    {
                        ColorBlock cb = current.colors;
                        cb.normalColor = Color.gray;
                        cb.highlightedColor = Color.gray;
                        current.colors = cb;
                    }
                }
            }
        }
        **/
    }
}