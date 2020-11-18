using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{


    public class ZMQSyncComToggle : MonoBehaviour
    {
        private Toggle toggle;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(Execute);
        }

        private void Execute(bool value)
        {
            CloudUpdater.instance.SwitchZMQSyncComDefaultMode(value);
        }
    }
}