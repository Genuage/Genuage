using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{
    public class WobbleColorVariableDropdown : IDropdownScript
    {
        // Start is called before the first frame update
        private void Awake()
        {
            dropdown = GetComponent<Dropdown>();
            InitialiseClickEvent();
        }
        public override void Execute(int value)
        {
            CloudUpdater.instance.ChangeWobbleColorVariable(value);
        }
    }
}