using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIElement 
    : AdvancedBehaviour
{
    public RectTransform RectTransform
    {
        get { return GetComponent<RectTransform> (); }
    }
}
