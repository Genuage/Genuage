using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
using VR_Interaction;
using IO;
using SFB;
using System.IO;
using System.Linq;


namespace DesktopInterface
{


    public class PlotOrientationDistributionButton : IButtonScript
    {
        public HistogramPointSelector selector;
        public Button SaveHistogramButton;
        private List<float> xValues;
        private List<int> yValues;
        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
            SaveHistogramButton.onClick.AddListener(ExportHistogram);
        }
        public override void Execute()
        {
            xValues = new List<float>();
            yValues = new List<int>();
            for (int i = 0; i < 181; i++)
            {
                xValues.Add(i);
                yValues.Add(0);
            }


            CloudData data = CloudUpdater.instance.LoadCurrentStatus();
            foreach(var pointID in data.globalMetaData.SelectedPointsList)
            {
                int anglevalue = Mathf.RoundToInt(data.pointDataTable[pointID].theta_angle);
                if (anglevalue <= 180 && anglevalue >= 0)
                {
                    yValues[anglevalue] = yValues[anglevalue] + 1;
                }

            }
            selector.CreateCanvas(xValues, yValues);


        }

        private void ExportHistogram()
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();

            HistogramSaveable hsave = new HistogramSaveable();
            hsave.HistogramXValues = xValues;
            hsave.HistogramYValues = yValues;
            hsave.SelectedPointsIDs = data.globalMetaData.SelectedPointsList.ToList<int>();

            var extensions = new[] {
                new ExtensionFilter("JSON", ".JSON")};
            StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "", extensions, (string path) => { SaveJSON(path, hsave); });
        }

        public void SaveJSON(string path, HistogramSaveable histogramData)
        {
            string JSON = JsonUtility.ToJson(histogramData);
            string directory = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(directory + Path.DirectorySeparatorChar + filename + ".JSON"))
            {
                writer.WriteLine(JSON);
            }
        }



    }
}