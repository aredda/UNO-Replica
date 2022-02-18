using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UILabelTotalDraw 
    : MonoBehaviour
{
    public Text label;
    public int fontSize = 40;

    public void Show(int total)
    {
        if(label == null)
            throw new System.Exception("UILabelTotalDraw.Show#Exception: [Text] object reference is missing");

        // Update text
        this.label.text = $"+{total}";
        // Update font size
        this.label.fontSize = fontSize + total * 2;
        // Show
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
