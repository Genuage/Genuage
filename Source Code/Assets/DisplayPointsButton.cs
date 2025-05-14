using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

namespace DesktopInterface{

    public class DisplayPointsButton : IButtonScript
    {
        public void Awake()
        {
            button=GetComponent<Button>();
            initializeClickEvent();
        }

        public override void Execute()
        {
            CloudUpdater.instance.ShowHidePointSpritesSwitch();
        }
    }
}