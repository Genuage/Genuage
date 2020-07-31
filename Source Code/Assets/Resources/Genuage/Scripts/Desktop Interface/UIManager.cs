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


namespace DesktopInterface
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;
        public List<Button> buttonsList;
        public List<Button> threadcommunicationButtonsList;
        public List<Slider> slidersList;
        public List<InputField> inputfieldList;
        public Dictionary<int,Button> selectionButtonsDict;
        public Dictionary<int, Button> VRselectionButtonsDict;

        public Dictionary<int, Button> closeButtonsDict;
        public Dictionary<int, Button> VRcloseButtonsDict;

        public Text StatusText;
        // Start is called before the first frame update

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(gameObject);
                selectionButtonsDict = new Dictionary<int, Button>();
                VRselectionButtonsDict = new Dictionary<int, Button>();
                closeButtonsDict = new Dictionary<int, Button>();
                VRcloseButtonsDict = new Dictionary<int, Button>();

            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void DeactivateUI()
        {
            foreach (Button b in buttonsList)
            {
                b.interactable = false;
            }

            foreach (Slider s in slidersList)
            {
                s.interactable = false;
            }

            foreach(InputField i in inputfieldList)
            {
                i.interactable = false;
            }

            foreach (KeyValuePair<int, Button> item in VRselectionButtonsDict)
            {
                item.Value.interactable = false;
            }


            foreach (KeyValuePair<int,Button> item in selectionButtonsDict)
            {
                item.Value.interactable = false;
            }

            foreach (KeyValuePair<int, Button> item in closeButtonsDict)
            {
                item.Value.interactable = false;
            }

            foreach (KeyValuePair<int, Button> item in VRcloseButtonsDict)
            {
                item.Value.interactable = false;
            }


        }

        public void ActivateUI()
        {
            foreach (Button b in buttonsList)
            {
                b.interactable = true;
            }

            foreach (Slider s in slidersList)
            {
                s.interactable = true;
            }

            foreach (InputField i in inputfieldList)
            {
                i.interactable = true;
            }

            foreach (KeyValuePair<int, Button> item in selectionButtonsDict)
            {
                item.Value.interactable = true;
            }

            foreach (KeyValuePair<int, Button> item in closeButtonsDict)
            {
                item.Value.interactable = true;
            }

            foreach (KeyValuePair<int, Button> item in VRselectionButtonsDict)
            {
                item.Value.interactable = true;
            }

            foreach (KeyValuePair<int, Button> item in VRcloseButtonsDict)
            {
                item.Value.interactable = true;
            }

        }

        public void DeactivateSelectionButtons()
        {
            foreach (KeyValuePair<int, Button> item in selectionButtonsDict)
            {
                item.Value.interactable = false;
            }

            foreach (KeyValuePair<int, Button> item in closeButtonsDict)
            {
                item.Value.interactable = false;
            }

            foreach (KeyValuePair<int, Button> item in VRselectionButtonsDict)
            {
                item.Value.interactable = false;
            }

            foreach (KeyValuePair<int, Button> item in VRcloseButtonsDict)
            {
                item.Value.interactable = false;
            }
        }

        public void ActivateSelectionButtons()
        {
            foreach (KeyValuePair<int, Button> item in selectionButtonsDict)
            {
                item.Value.interactable = true;
            }

            foreach (KeyValuePair<int, Button> item in closeButtonsDict)
            {
                item.Value.interactable = true;
            }
            foreach (KeyValuePair<int, Button> item in VRselectionButtonsDict)
            {
                item.Value.interactable = true;
            }

            foreach (KeyValuePair<int, Button> item in VRcloseButtonsDict)
            {
                item.Value.interactable = true;
            }

        }

        public void ChangeStatusText(string newtext)
        {
            StatusText.text = newtext;
        }

        public void ResetStatusText()
        {
            StatusText.text = "Idle";
        }
    }
}