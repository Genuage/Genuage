using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopInterface
{


    public class FreeSelectionActivationButton : IButtonScript
    {
        void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
        }

        public override void Execute()
        {
            CloudUpdater.instance.SwitchFreeSelectionActivation();
        }

    }
}