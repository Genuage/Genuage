using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{

    public class OrientationColorVariableDropdown : IDropdownScript
    {
        private void Awake()
        {
            dropdown = GetComponent<Dropdown>();
            InitialiseClickEvent();
        }
        public override void Execute(int value)
        {
            CloudUpdater.instance.ChangeOrientationColorVariable(value);
        }

    }
}