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

using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using SFB;
using IO;

namespace DesktopInterface
{


    /// <summary>
    /// Script to put on button that load new CloudStatus
    /// </summary>
    public class MenuLoaderButton : IButtonScript
    {

        //public CloudLoader _cloud_loader;    
        /// <summary>
        /// From SFB API, launch native file explorer and launch new CloudStatus loading if path is valid
        /// </summary>

        public override void Execute()
        {
            // Open file with filter
            var extensions = new[] {
            new ExtensionFilter("3D format", "3d"),
            new ExtensionFilter("text format", "txt"),
            new ExtensionFilter("All Files", "*" ),
        };
            StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensions, true, (string[] paths) => { LaunchCloudLoader(paths); });
        }

        /// <summary>
        /// Init button listener
        /// </summary>
        public void Start()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
            //_cloud_loader = new GameObject("CloudLoader").AddComponent<CloudLoader>();
            //_cloud_loader.CreateCloudLoader();
        }

        /// <summary>
        /// Method to launch the CloudLoader to load the current file paths
        /// </summary>
        /// <param name="paths"></param>
        public void LaunchCloudLoader(string[] paths)
        {
            foreach (string path in paths)
            {
                CloudLoader.instance.LoadFromFile(path);
            }
        }




    }
}


