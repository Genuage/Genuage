using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using IO;
using Data;

namespace DesktopInterface
{


    public class LoadVideoScriptButton : IButtonScript
    {
        GameObject obj;
        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
        }

        public override void Execute()
        {
            obj = new GameObject();
            obj.AddComponent<VideoScriptFileReader>();
            if (!CloudSelector.instance.noSelection)
            {
                CloudData data = CloudUpdater.instance.LoadCurrentStatus();
                data.transform.parent.gameObject.GetComponent<CloudObjectRefference>().box.transform.position = Vector3.zero;
                data.transform.parent.gameObject.GetComponent<CloudObjectRefference>().box.transform.eulerAngles = Vector3.zero;
                data.transform.position = Vector3.zero;
                data.transform.eulerAngles = Vector3.zero;

            }
            UIManager.instance.ChangeStatusText("Loading Video Script...");

            // Open file with filter
            var extensions = new[]
            {
                new ExtensionFilter("text format", "txt"),
            };
            StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensions, false, (string[] paths) => { LaunchInterpreter(paths); });

        }

        public void LaunchInterpreter(string[] paths)
        {

            foreach (string path in paths)
            {
                obj.GetComponent<VideoScriptFileReader>().ReadFile(path);
            }
        }

    }
}