using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
namespace DesktopInterface
{


    public class SlidingWindowUImanager : MonoBehaviour
    {
        public Dropdown ColumnSelectionDropdown;
        public Slider WindowSizeSlider;
        public Slider WindowSlidingSlider;
        public Button ResetButton;
        public Toggle noWrapparoundToggle;
        public Toggle d180WrapparoundToggle;
        public Toggle d360WrapparoundToggle;
        private float WindowSize;

        private void Awake()
        {
            WindowSize = 0;
            CloudSelector.instance.OnSelectionChange += OnSelectionChange;
            WindowSizeSlider.onValueChanged.AddListener(ChangeWindowSize);
            WindowSlidingSlider.onValueChanged.AddListener(ChangeWindowPosition);
            ColumnSelectionDropdown.onValueChanged.AddListener(ChangeColumn);
            ResetButton.onClick.AddListener(ResetDisplay);
        }

        private void OnEnable()
        {
            WindowSize = 0;
            CloudSelector.instance.OnSelectionChange += OnSelectionChange;
            WindowSizeSlider.onValueChanged.AddListener(ChangeWindowSize);
            WindowSlidingSlider.onValueChanged.AddListener(ChangeWindowPosition);
            ColumnSelectionDropdown.onValueChanged.AddListener(ChangeColumn);
            ResetButton.onClick.AddListener(ResetDisplay);
            if (!CloudSelector.instance.noSelection)
            {
                OnSelectionChange(CloudSelector.instance._selectedID);
            }


        }

        private void OnDisable()
        {
            CloudSelector.instance.OnSelectionChange -= OnSelectionChange;
            WindowSizeSlider.onValueChanged.RemoveListener(ChangeWindowSize);
            WindowSlidingSlider.onValueChanged.RemoveListener(ChangeWindowPosition);
            ColumnSelectionDropdown.onValueChanged.RemoveListener(ChangeColumn);
            ResetButton.onClick.RemoveListener(ResetDisplay);

        }

        public void ChangeWindowSize(float value)
        {
            WindowSize = value;
        }

        public void ChangeColumn(int value)
        {
            WindowSizeSlider.onValueChanged.RemoveListener(ChangeWindowSize);
            WindowSlidingSlider.onValueChanged.RemoveListener(ChangeWindowPosition);



            CloudData data = CloudUpdater.instance.LoadCurrentStatus();

            WindowSizeSlider.minValue = 0;
            WindowSizeSlider.maxValue = data.globalMetaData.columnMetaDataList[value].Range;
            WindowSizeSlider.value = 0;

            WindowSlidingSlider.minValue = data.globalMetaData.columnMetaDataList[value].MinValue;
            WindowSlidingSlider.maxValue = data.globalMetaData.columnMetaDataList[value].MaxValue;
            WindowSlidingSlider.value = data.globalMetaData.columnMetaDataList[value].MinValue;


            WindowSizeSlider.onValueChanged.AddListener(ChangeWindowSize);
            WindowSlidingSlider.onValueChanged.AddListener(ChangeWindowPosition);

        }

        public void ChangeWindowPosition(float value)
        {
            CloudUpdater.instance.ChangeSlidingWindow(ColumnSelectionDropdown.value, WindowSize, value);
        }

        public void OnSelectionChange(int id)
        {
            WindowSizeSlider.onValueChanged.RemoveListener(ChangeWindowSize);
            WindowSlidingSlider.onValueChanged.RemoveListener(ChangeWindowPosition);

            CloudData data = CloudUpdater.instance.LoadCurrentStatus();
            ColumnSelectionDropdown.ClearOptions();
            List<string> stringlist = new List<string>();
            for(int i = 0; i < data.columnData.Count; i++)
            {
                int v = i + 1;
                stringlist.Add("column : " + v.ToString());
            }
            ColumnSelectionDropdown.AddOptions(stringlist);
            ColumnSelectionDropdown.value = 0;
            WindowSizeSlider.minValue = 0;
            WindowSizeSlider.maxValue = data.globalMetaData.columnMetaDataList[0].Range;
            WindowSizeSlider.value = 0;

            WindowSlidingSlider.minValue = data.globalMetaData.columnMetaDataList[0].MinValue;
            WindowSlidingSlider.maxValue = data.globalMetaData.columnMetaDataList[0].MaxValue;
            WindowSlidingSlider.value = data.globalMetaData.columnMetaDataList[0].MinValue;
            
            WindowSizeSlider.onValueChanged.AddListener(ChangeWindowSize);
            WindowSlidingSlider.onValueChanged.AddListener(ChangeWindowPosition);

        }

        private void ResetDisplay()
        {
            CloudUpdater.instance.ResetSlidingWindow();
        }


        /**
         * TODO : 
         * UPDATE SLIDERS MIN AND MAX AND DROPDOWN ON CLOUD LOAD
         * UPDATE SLIDER MIN AND MAX WHEN DROPDOWN CHANGES
         * WHEN WINDOW SLIDER IS MOVED, SEND TO CLOUDUPDATER AND DISPLAY APPROPRIATE POINTS
        
         
         
        **/
    }
}