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


    public class CreateSelectionButtons : MonoBehaviour
    {
        public GameObject _selectionButtonContainerPrefab;
        public Transform _selftransform;
        public Dictionary<int, GameObject> _selectionButtonList;
        public bool VR = false;

        void Awake()
        {
            _selectionButtonList = new Dictionary<int, GameObject>();
            _selftransform = GetComponent<Transform>();
            CloudStorage.instance.OnCloudCreated += CreateNewButton;
            CloudStorage.instance.OnCloudDeleted += DestroyButton;
            CloudSelector.instance.OnSelectionChange += ChangeButtonColors;
            CloudUpdater.instance.OnCloudLinked += LockButtons;
            CloudUpdater.instance.OnCloudUnlinked += UnlockButtons;
            if (VR && !CloudSelector.instance.noSelection)
            {
                List<int> idlist = CloudSelector.instance.GetAllIDs();

                foreach (int i in idlist)
                {
                    CreateNewButton(i);

                }
            }
        }

        public void CreateNewButton(int id)
        {
            CloudData currcloud = CloudUpdater.instance.LoadStatus(id);
            GameObject son = Instantiate(_selectionButtonContainerPrefab) as GameObject;
            son.GetComponent<Transform>().SetParent(_selftransform, false);

            GameObject filenamebutton = son.GetComponent<SelectionButtonContainerRefference>().filenamebutton;
            filenamebutton.GetComponent<CloudSelectionButton>().id = id;
            filenamebutton.name = currcloud.globalMetaData.fileName;
            filenamebutton.GetComponentInChildren<Text>().text = currcloud.globalMetaData.fileName;

            GameObject closebutton = son.GetComponent<SelectionButtonContainerRefference>().closebutton;
            closebutton.GetComponent<CloseCloudByIDButton>().id = id;

            _selectionButtonList.Add(id, son);
            if (VR)
            {
                UIManager.instance.VRselectionButtonsDict.Add(id, filenamebutton.GetComponent<Button>());
                UIManager.instance.VRcloseButtonsDict.Add(id, closebutton.GetComponent<Button>());

            }
            else
            {
                UIManager.instance.selectionButtonsDict.Add(id, filenamebutton.GetComponent<Button>());
                UIManager.instance.closeButtonsDict.Add(id, closebutton.GetComponent<Button>());

            }
        }

        public void DestroyButton(int id)
        {
            if (CheckId(id))
            {
                GameObject obj = _selectionButtonList[id];
                _selectionButtonList.Remove(id);
                UIManager.instance.selectionButtonsDict.Remove(id);
                UIManager.instance.closeButtonsDict.Remove(id);


                obj.SetActive(false);
            }
        }

        public bool CheckId(int id)
        {
            return _selectionButtonList.ContainsKey(id);
        }

        public void ChangeButtonColors(int selected_id)
        {
            foreach (KeyValuePair<int, GameObject> item in _selectionButtonList)
            {
                Button current = item.Value.GetComponent<SelectionButtonContainerRefference>().filenamebutton.GetComponent<Button>();
                if (current != null)
                {
                    if (item.Key == selected_id)
                    {
                        ColorBlock cb = current.colors;
                        Color textcolor = current.GetComponentInChildren<Text>().color;
                        cb.normalColor = Color.clear;
                        cb.highlightedColor = Color.grey;
                        textcolor = Color.white;
                        current.colors = cb;
                        current.GetComponentInChildren<Text>().color = textcolor;
                    }
                    else
                    {
                        ColorBlock cb = current.colors;
                        Color textcolor = current.GetComponentInChildren<Text>().color;
                        cb.normalColor = Color.clear;
                        cb.highlightedColor = Color.gray;
                        current.colors = cb;
                        textcolor = Color.grey;

                        current.GetComponentInChildren<Text>().color = textcolor;

                    }
                }
            }
        }

        public List<int> GetIdsToggled()
        {
            List<int> list = new List<int>();
            foreach (KeyValuePair<int, GameObject> item in _selectionButtonList)
            {
                if (item.Value.GetComponent<SelectionButtonContainerRefference>().toggle.GetComponent<Toggle>().isOn)
                {
                    list.Add(item.Key);
                }

            }
            return list;
        }

        public void LockButtons()
        {

            foreach (KeyValuePair<int, GameObject> item in _selectionButtonList)
            {
                item.Value.GetComponent<SelectionButtonContainerRefference>().filenamebutton.GetComponent<Button>().interactable = false;
                item.Value.GetComponent<SelectionButtonContainerRefference>().closebutton.GetComponent<Button>().interactable = false;
                item.Value.GetComponent<SelectionButtonContainerRefference>().toggle.GetComponent<Toggle>().interactable = false;
            }
        }

        public void UnlockButtons()
        {
            foreach (KeyValuePair<int, GameObject> item in _selectionButtonList)
            {
                item.Value.GetComponent<SelectionButtonContainerRefference>().filenamebutton.GetComponent<Button>().interactable = true;
                item.Value.GetComponent<SelectionButtonContainerRefference>().closebutton.GetComponent<Button>().interactable = true;
                item.Value.GetComponent<SelectionButtonContainerRefference>().toggle.GetComponent<Toggle>().interactable = true;

            }
        }

        public void OnDestroy()
        {
            CloudStorage.instance.OnCloudCreated -= CreateNewButton;
            CloudStorage.instance.OnCloudDeleted -= DestroyButton;
            CloudSelector.instance.OnSelectionChange -= ChangeButtonColors;
            CloudUpdater.instance.OnCloudLinked -= LockButtons;
            CloudUpdater.instance.OnCloudUnlinked -= UnlockButtons;

            if (VR)
            {
                UIManager.instance.VRselectionButtonsDict.Clear();
                UIManager.instance.VRcloseButtonsDict.Clear();

            }
        }
    }
}