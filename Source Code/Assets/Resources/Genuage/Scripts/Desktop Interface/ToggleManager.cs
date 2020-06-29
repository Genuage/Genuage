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
using System.ComponentModel;

namespace DesktopInterface
{


    public class ToggleManager : MonoBehaviour
    {
        public bool created = false;
        public int collumnnbr;
        public GameObject togglePrefab;
        public GameObject textPrefab;
        public GameObject buttonPrefab;
        public List<List<GameObject>> toggleList;
        public List<GameObject> headerList;
        public Button applyButton;
        public List<int> selectionList;

        public Dropdown ThresholdDropdown;

        public int VARIABLENUMBER = 9;

        void Delete(int id =0)
        {
            if (toggleList != null)
            {
                foreach (List<GameObject> list in toggleList)
                {
                    foreach (GameObject go in list)
                    {
                        Destroy(go);
                    }
                }
            }

            if (headerList != null)
            {
                foreach (GameObject go in headerList)
                {
                    Destroy(go);
                }
            }
            toggleList = null;
            selectionList = null;
            headerList = null;
            collumnnbr = 0;
            created = false;
        }

        public void Create()
        {
            if (!CloudSelector.instance.noSelection)
            {


                if (created) { Delete(); }

                CloudData selectedCloud = CloudUpdater.instance.LoadCurrentStatus();

                toggleList = new List<List<GameObject>>();
                headerList = new List<GameObject>();
                selectionList = new List<int>();
                collumnnbr = selectedCloud.columnData.Count;
                for (int i = 0; i < VARIABLENUMBER; i++)
                {
                    toggleList.Add(new List<GameObject>());
                    selectionList.Add(selectedCloud.globalMetaData.displayCollumnsConfiguration[i]);

                }

                CreateToggles(selectedCloud);
                applyButton.onClick.AddListener(ApplySelection);
                created = true;
            }
           
        }

        public void CreateToggles(CloudData data)
        {
            int i = 0;
            List<string> OptionList = new List<string>();
            foreach (Transform child in this.transform)
            {
                Transform layout = child.GetChild(0);
                if (child.gameObject.name == "header")
                {
                    for (int k = 1; k <= collumnnbr; k++)
                    {
                        GameObject go = Instantiate(textPrefab) as GameObject;
                        go.transform.SetParent(layout);
                        go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0f);
                        go.transform.localScale = Vector3.one;
                        go.transform.rotation = this.transform.rotation;
                        Text newtext = go.GetComponent<Text>();
                        newtext.text = k.ToString();

                        //GameObject button = Instantiate(buttonPrefab) as GameObject;
                        //button.transform.SetParent(go.transform,true);
                        //button.transform.localPosition = Vector3.zero;
                        
                        headerList.Add(go);
                        OptionList.Add(k.ToString());
                        //newtext.font = (Font)Resources.Load("Menlo-Regular");
                    }
                    if (data.globalMetaData.densityCalculated)
                    {
                        headerList[headerList.Count-1].GetComponent<Text>().text = "d";
                        OptionList[OptionList.Count - 1] = "d";

                    }

                    ThresholdDropdown.AddOptions(OptionList);

                    /**
                    if (data.globalMetaData.densityCalculated)
                    {
                        GameObject go = Instantiate(textPrefab) as GameObject;
                        go.transform.SetParent(layout);
                        go.transform.localScale = Vector3.one;
                        Text newtext = go.GetComponent<Text>();
                        newtext.text = "d";
                        headerList.Add(go);

                    }
                    **/
                }
                else if (i < VARIABLENUMBER)
                {
                    for (int j = 0; j < collumnnbr; j++)
                    {
                        GameObject newtoggle = Instantiate(togglePrefab) as GameObject;
                        newtoggle.transform.SetParent(layout);
                        newtoggle.transform.localPosition = new Vector3(newtoggle.transform.localPosition.x, newtoggle.transform.localPosition.y, 0f);
                        newtoggle.transform.localScale = Vector3.one;
                        newtoggle.transform.rotation = this.transform.rotation;
                        newtoggle.gameObject.GetComponent<Toggle>().group = layout.GetComponent<ToggleGroup>();
                        newtoggle.gameObject.GetComponent<Toggle>().isOn = false;
                        newtoggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate { CheckCollumnOnValueChanged(newtoggle.GetComponent<Toggle>()); });
                        toggleList[i].Add(newtoggle);

                    }
                    /**
                    if (data.globalMetaData.densityCalculated)
                    {
                        GameObject newtoggle = Instantiate(togglePrefab) as GameObject;
                        newtoggle.transform.SetParent(layout);
                        newtoggle.gameObject.GetComponent<Toggle>().group = layout.GetComponent<ToggleGroup>();
                        newtoggle.gameObject.GetComponent<Toggle>().isOn = false;
                        newtoggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate { CheckCollumnOnValueChanged(newtoggle.GetComponent<Toggle>()); });
                        toggleList[i].Add(newtoggle);


                    }
                    **/
                    i++;
                }
            }
            InitializeToggles();
        }

        public void InitializeToggles()
        {
            int i = 0;
            foreach (List<GameObject> row in toggleList)
            {
                

                row[selectionList[i]].GetComponent<Toggle>().isOn = true;
                i++;
            }
        }

        public void CheckCollumnOnValueChanged(Toggle change)
        {
            /**
            if (change.isOn)
            {
                int row = 0;
                int collumn = 0;
                for (int i = 0; i < toggleList.Count; i++)
                {
                    for (int j = 0; j < toggleList[i].Count; j++)
                    {
                        if (toggleList[i][j] == change.gameObject)
                        {
                            row = i;
                            collumn = j;
                        }
                    }
                }
                for (int k = 0; k < toggleList.Count; k++)
                {
                    if (toggleList[k][collumn].GetComponent<Toggle>().isOn == true && k != row)
                    {
                        toggleList[k][collumn].GetComponent<Toggle>().isOn = false;
                    }
                }
            }
            **/
            UpdateSelection();

        }

        public void UpdateSelection()
        {
            for (int i = 0; i < toggleList.Count; i++)
            {
                for (int j = 0; j < toggleList[i].Count; j++)
                {
                    if (toggleList[i][j].GetComponent<Toggle>().isOn)
                    {
                        selectionList[i] = j;
                    }
                }
            }
        }

        public void ClearList()
        {
            foreach (List<GameObject> list in toggleList)
            {
                foreach (GameObject obj in list)
                {
                    Destroy(obj);
                }
                list.Clear();
            }
        }

        public void ApplySelection()
        {
            UIManager.instance.ActivateUI();
            CloudUpdater.instance.ChangeCollumnSelection(selectionList);
            
            Destroy(transform.parent.gameObject);
        }
    }
}