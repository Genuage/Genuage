using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that hold the normalized color code for UI
/// </summary>
public class UIColors : MonoBehaviour {

    public static readonly Color _hovered = Color.white;
    public static readonly Color _clicked = Color.cyan;

    public static readonly Color _hovered_image = Color.gray;
    public static readonly Color _clicked_image = Color.black;
    public static readonly Color _normal_image = Color.white;

    public static readonly Color _movable = Color.white;
    public static readonly Color _actionnable = Color.yellow;

    public static readonly Color _destroyable = Color.red;
    public static readonly Color _normal_controller_button = Color.black;
    public static readonly Color _triggerable = Color.yellow;

    public static readonly Color _normal_laser = Color.black;
    public static readonly Color _pressed_laser = Color.green;
}
