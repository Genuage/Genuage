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


    public class PlotColumnDistribution : IButtonScript
    {
        public HistogramPointSelector selector;
        public Button SaveHistogramButton;
        public Button ClearHistogramButton;
        public Dropdown ColumnDropdown;
        private int SelectedColumn;
        private int HistogramBars = 50;
        private List<float> xValues;
        private List<int> yValues;
        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
            SaveHistogramButton.onClick.AddListener(ExportHistogram);
            ClearHistogramButton.onClick.AddListener(ClearHistogram);
            CloudSelector.instance.OnSelectionChange+=(ChangeDropdown);

        }

        private void OnEnable()
        {
            CloudSelector.instance.OnSelectionChange += (ChangeDropdown);
            if (!CloudSelector.instance.noSelection)
            {
                ChangeDropdown(CloudSelector.instance._selectedID);
            }

        }

        private void OnDisable()
        {
            CloudSelector.instance.OnSelectionChange -= (ChangeDropdown);
        }


        private void ChangeDropdown(int value)
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();
            ColumnDropdown.ClearOptions();
            List<string> stringlist = new List<string>();
            for (int i = 0; i < data.columnData.Count; i++)
            {
                int v = i + 1;
                stringlist.Add(v.ToString());
            }
            ColumnDropdown.AddOptions(stringlist);
            ColumnDropdown.value = 0;

        }

        public override void Execute()
        {
            SelectedColumn = ColumnDropdown.value;
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();

            xValues = new List<float>();
            yValues = new List<int>();

            float steps = data.globalMetaData.columnMetaDataList[SelectedColumn].Range / HistogramBars;
            Debug.Log("Steps : " + steps);

            for (int i = 0; i < HistogramBars+1; i++)
            {
                xValues.Add(data.globalMetaData.columnMetaDataList[SelectedColumn].MinValue + i*steps);
                yValues.Add(0);
            }


            foreach (var pointID in data.globalMetaData.SelectedPointsList)
            {
                for (int i = 0;i < xValues.Count-1; i++)
                {
                    if (data.columnData[SelectedColumn][pointID] >= xValues[i] &&
                        data.columnData[SelectedColumn][pointID] < xValues[i + 1])
                    {
                        yValues[i] += 1;
                    }
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

        private void ClearHistogram()
        {
            if (selector.canvas != null)
            {
                Destroy(selector.canvascontainer);
            }
        }
    }
}