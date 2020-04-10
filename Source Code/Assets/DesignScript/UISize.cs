using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script that holds the number of size of UI component in function of the computer screen resolution
/// </summary>
public class UISize : MonoBehaviour {
    
    public static readonly float _vr_slider_height = 20;
    public static readonly float _vr_slider_width = 150;
    public static readonly float _desktop_height = Screen.width / 80;
    public static readonly float _desktop_width = Screen.width / 50;
    public static readonly Vector3 _vr_slider_size = new Vector3(0.0045f, 0.0045f, 0.0045f);
}
