using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIElement 
    : MonoBehaviour
{
    public RectTransform RectTransform
    {
        get { return GetComponent<RectTransform> (); }
    }
}
