using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{
    public class ZMQSyncComValueDropdown : IDropdownScript
    {
        private void Awake()
        {
            dropdown = GetComponent<Dropdown>();
            InitialiseClickEvent();
        }

        public override void Execute(int value)
        {
            CloudUpdater.instance.SetZMQSyncComDefaultValue(value);
        }
    }
}