using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuCardActionPicker 
    : MonoBehaviour
{
    [Header("Template Reference")]
    public CardTemplate template;

    [Header("UI Elements")]
    public Button buttonPlay;
    public Button buttonEndTurn;

    [Header("Position Settings")]
    public int yOffset = 1;

    public void Show(CardTemplate template, System.Action onPlay, System.Action onEndTurn)
    {
        this.template = template;

        if(this.template == null)
            throw new System.Exception("UIMenuCardActionPicker.Show#Exception: Card template is missing");

        // Show menu
        this.gameObject.SetActive(true);
        // Re-position
        this.transform.position = Camera.main.WorldToScreenPoint(template.transform.position) + Vector3.up * yOffset;
        // Clear previous click listeners
        buttonPlay.onClick.RemoveAllListeners();
        buttonEndTurn.onClick.RemoveAllListeners();
        // Set up listeners
        buttonPlay.onClick.AddListener(delegate() 
        { 
            onPlay.Invoke();
            // Hide menu
            this.gameObject.SetActive(false);
        });
        buttonEndTurn.onClick.AddListener(delegate() 
        {
            onEndTurn.Invoke();
            // Hide menu
            this.gameObject.SetActive(false);
        });
    }
}
