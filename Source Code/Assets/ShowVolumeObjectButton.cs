using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface
{


    public class ShowVolumeObjectButton : IButtonScript
    {
        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
        }
        public override void Execute()
        {
            CloudUpdater.instance.ShowVolumeObject();
        }
    }
}