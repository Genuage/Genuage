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
using DesktopInterface;

namespace VR_Interaction
{


    public class VRToolMenu : MonoBehaviour
    {
        public ToolState[] selectionArray;
        public List<GameObject> buttonsList;
        

        public ToolManager manager;

        private void Start()
        {
            
            InitializeButtons();
        }

        private void InitializeButtons()
        {

            foreach (var c in buttonsList)
            {
                if (manager.toolsDict.ContainsKey(c.name))
                {
                    if (manager.toolsDict[c.name].isactivated)
                    {
                        c.GetComponent<ToolSelectionButton>().activated = true;
                        c.GetComponent<ToolSelectionButton>().ChangeColor();
                    }
                }
                c.GetComponent<ToolSelectionButton>().OnStateChange += OnSelectionChange;
            }
        }

        public void OnSelectionChange(string id, bool state)
        {
            manager.SwitchTool(id,state);
        }

        public void DeactivateIncompatibleButtons(string[] ids)
        {
            foreach (GameObject button in buttonsList)
            {
                foreach (string s in ids)
                {
                    if(button.name == s)
                    {
                        button.GetComponent<ToolSelectionButton>().ChangeState(false);
                        break;
                    }
                }
            }
        }

    }
}