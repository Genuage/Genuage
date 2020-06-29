using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopInterface
{


    public class DeleteClippingPlaneButton : IButtonScript
    {
        // Start is called before the first frame update
        void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
        }

        public override void Execute()
        {
            
        }

    }
}