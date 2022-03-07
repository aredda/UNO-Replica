using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIMenuChallenger 
    : UIElement
{
    [Header("UI Elements")]
    public Image panelColor;
    public Button buttonSurrender;
    public Button buttonNo;
    public Button buttonYes;

    public void Show()
    {
        // the challenged
        PlayerController challenged = Master.previousTurn;
        bool hasColor = challenged.hand.GetCardsByColor(challenged.hand.cardTemplates, Master.board.selectedColor).Count != 0;
        // ui changes
        buttonSurrender.GetComponentInChildren<Text>().text = $"Draw +{Master.drawTotal}";
        panelColor.color = Master.board.colors[Master.board.selectedColor];
        // show
        gameObject.SetActive(true);
        // configure events
        buttonSurrender.onClick.RemoveAllListeners();
        buttonNo.onClick.RemoveAllListeners();
        buttonYes.onClick.RemoveAllListeners();
        // prepare callbacks
        System.Action challengeWon = delegate()
        {
            if (Master.isOnline)
                Master.turn.agent.CmdWinChallenge();
            else
                Master.WinChallenge(challenged);
            // hide
            Hide();
        };
        System.Action challengeLost = delegate()
        {
            if (Master.isOnline)
                Master.turn.agent.CmdLoseChallenge();
            else
                Master.LoseChallenge();
            // hide
            Hide();
        };
        // surrender logic
        buttonSurrender.onClick.AddListener(delegate()
        {
            // if the player wishes not to engage in a challenge, make him draw the intended cards
            if (Master.isOnline)
                Master.turn.agent.CmdSurrenderChallenge();
            else
                Master.DrawImposedCards();
            // hide
            Hide();
        });
        // "no" answer logic
        buttonNo.onClick.AddListener(delegate()
        {
            // if the player selected "no", and happens that the challenged player doesn't have that color, the challenger wins
            if(!hasColor)
                challengeWon.Invoke();
            else
                challengeLost.Invoke();
        });
        // "yes" answer logic
        buttonYes.onClick.AddListener(delegate()
        {
            // if the player selected "yes", and happens that the challenged player have that color, the challenger wins
            if(hasColor)
                challengeWon.Invoke();
            else
                challengeLost.Invoke();
        });
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
