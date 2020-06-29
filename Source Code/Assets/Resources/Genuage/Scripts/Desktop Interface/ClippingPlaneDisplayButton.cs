using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DesktopInterface
{


    public class ClippingPlaneDisplayButton : IButtonScript
    {
        private static bool MultiClipPlaneactivated = true;
        public void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();
            if(MultiClipPlaneactivated == true)
            {
                GetComponent<Image>().color = Color.green;
            }
        }

        public override void Execute()
        {
            if (MultiClipPlaneactivated == false)
            {
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_1");
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_2");
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_3");
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_4");
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_5");
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_6");
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_7");
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_8");
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_9");
                Shader.EnableKeyword("FIXED_CLIPPING_PLANE_10");
                MultiClipPlaneactivated = true;
                GetComponent<Image>().color = Color.green;

            }
            else
            {
                Shader.DisableKeyword("FIXED_CLIPPING_PLANE_1");
                Shader.DisableKeyword("FIXED_CLIPPING_PLANE_2");
                Shader.DisableKeyword("FIXED_CLIPPING_PLANE_3");
                Shader.DisableKeyword("FIXED_CLIPPING_PLANE_4");
                Shader.DisableKeyword("FIXED_CLIPPING_PLANE_5");
                Shader.DisableKeyword("FIXED_CLIPPING_PLANE_6");
                Shader.DisableKeyword("FIXED_CLIPPING_PLANE_7");
                Shader.DisableKeyword("FIXED_CLIPPING_PLANE_8");
                Shader.DisableKeyword("FIXED_CLIPPING_PLANE_9");
                Shader.DisableKeyword("FIXED_CLIPPING_PLANE_10");
                MultiClipPlaneactivated = false;
                GetComponent<Image>().color = Color.white;

            }
        }
    }
}