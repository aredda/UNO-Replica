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
        this.buttonSurrender.GetComponentInChildren<Text>().text = $"Draw +{Master.drawTotal}";
        this.panelColor.color = Master.board.colors[Master.board.selectedColor];
        // show
        this.gameObject.SetActive(true);
        // configure events
        buttonSurrender.onClick.RemoveAllListeners();
        buttonNo.onClick.RemoveAllListeners();
        buttonYes.onClick.RemoveAllListeners();
        // prepare callbacks
        System.Action challengeWon = delegate()
        {
            // change this player's state
            Master.turn.SetCanPlay(false);
            // move the total draw text back to the previous player
            var previousPlayerCardID = Master.director.uiManager.GetPlayerCardID(Master.previousTurn);
            var currentPlayerCardID = Master.director.uiManager.GetPlayerCardID(Master.turn);

            Master.director.cardAnimator.MoveDrawTotalText(
                Master.director.uiManager.labelDrawTotal.RectTransform,
                previousPlayerCardID.GetDrawCounterPosition()
            );
            // show the player state
            Master.director.uiManager.labelPlayerState.Show("Challenge Won", true, currentPlayerCardID.GetStateTextPosition());
            // make the challenged player draw
            Master.director.deckDealer.DealCards(challenged, Master.drawTotal, delegate()
            {
                // hide state text
                Master.director.uiManager.labelPlayerState.Hide();
                // reset draw mode
                Master.ResetDrawing();
                // when cards are dealt, return the ability to play
                Master.turn.SetCanPlay();
                // if there is nothing to play end turn
                if(Master.turn.hand.FetchPlayableCards().Count == 0)
                    Master.EndTurn();
            });
            // hide
            Hide();
        };
        System.Action challengeLost = delegate()
        {
            // change player state
            Master.turn.SetCanPlay(false);
            // due to losing the challenge, a +2 penalty is added
            Master.drawTotal += 2;
            // update text of draw total
            Master.director.uiManager.labelDrawTotal.Show(Master.drawTotal);
            // update draw total text
            Master.director.cardAnimator.MoveDrawTotalText(
                Master.director.uiManager.labelDrawTotal.RectTransform,
                Master.director.uiManager.GetPlayerCardID(Master.turn).GetDrawCounterPosition()
            );
            // show the player state
            Master.director.uiManager.labelPlayerState.Show("Challenge Lost (+2 penalty)", false, Master.director.uiManager.GetPlayerCardID(Master.turn).GetStateTextPosition());
            // imposed drawing
            Master.DrawImposedCards(delegate()
            {
                // hide state text
                Master.director.uiManager.labelPlayerState.Hide();
            });
            // hide
            Hide();
        };
        // surrender logic
        buttonSurrender.onClick.AddListener(delegate()
        {
            // if the player wishes not to engage in a challenge, make him draw the intended cards
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
        this.gameObject.SetActive(false);
    }
}
