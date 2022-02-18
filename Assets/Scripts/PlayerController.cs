using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController 
    : MonoBehaviour
{
    public bool isLocalPlayer = false;
    public bool isBot = false;
    public bool canPlay = false;
    public PlayerHand hand;
    public PlayerState state;
    public BotController bot;

    public void SetHandPosition(Transform position)
    {
        hand.parent = position;
    }

    public void InitializeHand()
    {
        hand.cards = new List<Card> ();
        hand.player = this;
    }

    public void ReceiveCard(Card card, System.Action onFinishAnimation = null)
    {
        var template = hand.CreateCardTemplate(card);

        hand.cards.Add(card);
        hand.cardTemplates.Add(template);

        ManagerDirector.director.cardAnimator.DrawCard(template, hand.GetDrawnCardFuturePosition(), hand.parent.rotation.eulerAngles, onFinishAnimation);
    }

    public void SetCanPlay(bool canPlay = true)
    {
        this.canPlay = canPlay;
        
        // If it's not the player's turn, darken all his cards
        if(!canPlay)
            foreach(var template in hand.cardTemplates)
                template.MarkAs(false);
        else
            hand.FetchPlayableCards();
    }

    public bool CanPlay()
    {
        // Can't trigger a template's effect if it's not the player turn
        if(!canPlay)
            return false;

        // If the player has other state than "ready", forget about it
        if(state != PlayerState.Ready)
            return false;

        return true;
    }

    public void BecomeBot()
    {
        this.isBot = true;
        this.bot = this.gameObject.AddComponent<BotController> ();
        this.bot.SetPlayer(this);
    }

    public void DebugShowHand()
    {
        Debug.Log(string.Join(" | ", hand.cards));
    }
}
