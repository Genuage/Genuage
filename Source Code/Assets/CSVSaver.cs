using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IO;
using SFB;
namespace DesktopInterface
{


    public class CSVSaver : IButtonScript
    {
        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
        }

        public override void Execute()
        {
            var extensions = new[] {
                new ExtensionFilter("CSV Format", "csv")};
            StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "", extensions, (string path) => {CloudSaver.instance.SaveSelectionCSV(path);});

        }
    }
}