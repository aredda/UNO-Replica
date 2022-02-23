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
        GameMaster master = ManagerDirector.director.gameMaster;
        // the challenged
        PlayerController challenged = master.previousTurn;
        bool hasColor = challenged.hand.GetCardsByColor(challenged.hand.cardTemplates, master.board.selectedColor).Count != 0;
        // ui changes
        this.buttonSurrender.GetComponentInChildren<Text>().text = $"Draw +{master.drawTotal}";
        this.panelColor.color = master.board.colors[master.board.selectedColor];
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
            master.turn.SetCanPlay(false);
            // move the total draw text back to the previous player
            var previousPlayerCardID = master.director.uiManager.GetPlayerCardID(master.previousTurn);
            var currentPlayerCardID = master.director.uiManager.GetPlayerCardID(master.turn);

            master.director.cardAnimator.MoveDrawTotalText(
                master.director.uiManager.labelDrawTotal.RectTransform,
                previousPlayerCardID.GetDrawCounterPosition()
            );
            // show the player state
            master.director.uiManager.labelPlayerState.Show("Challenge Won", true, currentPlayerCardID.GetStateTextPosition());
            // make the challenged player draw
            master.director.deckDealer.DealCards(challenged, master.drawTotal, delegate()
            {
                // hide state text
                master.director.uiManager.labelPlayerState.Hide();
                // reset draw mode
                master.ResetDrawing();
                // when cards are dealt, return the ability to play
                master.turn.SetCanPlay();
                // if there is nothing to play end turn
                if(master.turn.hand.FetchPlayableCards().Count == 0)
                    master.EndTurn();
            });
            // hide
            Hide();
        };
        System.Action challengeLost = delegate()
        {
            // change player state
            master.turn.SetCanPlay(false);
            // due to losing the challenge, a +2 penalty is added
            master.drawTotal += 2;
            // update text of draw total
            master.director.uiManager.labelDrawTotal.Show(master.drawTotal);
            // update draw total text
            master.director.cardAnimator.MoveDrawTotalText(
                master.director.uiManager.labelDrawTotal.RectTransform,
                master.director.uiManager.GetPlayerCardID(master.turn).GetDrawCounterPosition()
            );
            // show the player state
            master.director.uiManager.labelPlayerState.Show("Challenge Lost (+2 penalty)", false, master.director.uiManager.GetPlayerCardID(master.turn).GetStateTextPosition());
            // imposed drawing
            master.DrawImposedCards(delegate()
            {
                // hide state text
                master.director.uiManager.labelPlayerState.Hide();
            });
            // hide
            Hide();
        };
        // surrender logic
        buttonSurrender.onClick.AddListener(delegate()
        {
            // if the player wishes not to engage in a challenge, make him draw the intended cards
            master.DrawImposedCards();
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
