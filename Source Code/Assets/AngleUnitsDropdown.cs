using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
namespace DesktopInterface
{


    public class AngleUnitsDropdown : IDropdownScript
    {

        private void Awake()
        {
            dropdown = GetComponent<Dropdown>();
            InitialiseClickEvent();
            CloudSelector.instance.OnSelectionChange += UpdateDropdown;
        }

        private void UpdateDropdown(int cloud_id)
        {
            CloudData data = CloudUpdater.instance.LoadCurrentStatus();
            if(data.globalMetaData.angleUnit == AngleUnit.DEGREES)
            {
                dropdown.value = 0;
            }
            else if (data.globalMetaData.angleUnit == AngleUnit.RADIANS)
            {
                dropdown.value = 1;
            }
        }

        public override void Execute(int value)
        {
            if(value == 0)
            {
                CloudUpdater.instance.ChangeAngleUnit(AngleUnit.DEGREES);
            }
            else if (value == 1)
            {
                CloudUpdater.instance.ChangeAngleUnit(AngleUnit.RADIANS);
            }
        }

    }
}