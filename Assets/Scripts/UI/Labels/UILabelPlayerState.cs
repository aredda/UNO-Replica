using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UILabelPlayerState 
    : UIElement
{
    [Header("UI Reference")]
    public Text textState;

    [Header("Customization")]
    public Color colorSuccess;
    public Color colorFailure;

    public void Show(string text, bool success, Vector2 position)
    {
        textState.rectTransform.anchoredPosition = position;
        textState.text = text;
        textState.color = success ? colorSuccess : colorFailure;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
