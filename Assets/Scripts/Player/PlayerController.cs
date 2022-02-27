using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController 
    : AdvancedBehaviour
{
    public PlayerHand hand;
    
    [Header("Player State")]
    public bool canPlay = false;
    public PlayerState state;

    [Header("Bot Settings")]
    public bool isBot = false;
    public BotController bot;
    
    [Header("Network Settings")]
    public bool isLocalPlayer = false;

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
        // change player state to "drawing"
        state = PlayerState.Drawing;
        // prepare template
        var template = hand.CreateCardTemplate(card);
        // add card data
        hand.cards.Add(card);
        // add card template
        hand.cardTemplates.Add(template);
        // play draw animation
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
        {
            // reset state
            state = PlayerState.Ready;
            // fetch playable cards
            hand.FetchPlayableCards();
        }
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
