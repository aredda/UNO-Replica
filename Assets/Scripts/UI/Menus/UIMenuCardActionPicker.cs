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
    public Button buttonDrawCard;
    public Button buttonChallenge;

    [Header("Position Settings")]
    public int yOffset = 1;

    public void Show(CardTemplate template, System.Action onPlay, System.Action onDrawCard, System.Action onChallenge)
    {
        this.template = template;

        if(this.template == null)
            throw new System.Exception("UIMenuCardActionPicker.Show#Exception: Card template is missing");

        // Get master
        GameMaster master = ManagerDirector.director.gameMaster;
        // Decide if challenge button is enabled
        buttonChallenge.interactable = master.rules.enableWildDrawChallenge && master.boardCardTemplate.card is Draw4Card && master.isDrawImposed;
        // Show menu
        this.gameObject.SetActive(true);
        // Re-position
        this.transform.position = Camera.main.WorldToScreenPoint(template.transform.position) + Vector3.up * yOffset;
        // Clear previous click listeners
        buttonPlay.onClick.RemoveAllListeners();
        buttonDrawCard.onClick.RemoveAllListeners();
        buttonChallenge.onClick.RemoveAllListeners();
        // Set up listeners
        buttonPlay.onClick.AddListener(delegate() 
        { 
            onPlay.Invoke();
            // Hide menu
            this.gameObject.SetActive(false);
        });
        buttonDrawCard.onClick.AddListener(delegate() 
        {
            onDrawCard.Invoke();
            // Hide menu
            this.gameObject.SetActive(false);
        });
        buttonChallenge.onClick.AddListener(delegate()
        {
            onChallenge.Invoke();
            // Hide menu
            this.gameObject.SetActive(false);
        });
    }

    public void SetDrawButtonText(string text)
    {
        this.buttonDrawCard.GetComponentInChildren<Text>().text = text;
    }
}
