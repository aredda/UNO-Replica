using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UILabelTotalDraw 
    : UIElement
{
    public Text label;
    public int fontSize = 40;

    public void Show(int total)
    {
        if(label == null)
            throw new System.Exception("UILabelTotalDraw.Show#Exception: [Text] object reference is missing");

        // Update text
        this.label.text = $"+{total}";
        // Show
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        // reset position
        this.RectTransform.anchoredPosition = Vector2.zero;
        // hide
        this.gameObject.SetActive(false);
    }
}
