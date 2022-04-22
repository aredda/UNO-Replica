using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController 
    : AdvancedBehaviour
{
    void OnMouseDown()
    {
        // Check for turn object reference
        if(Master.turn == null)
            throw new System.Exception("DeckController.OnMouseDown#Exception: Can't deal a card because the turn is not set");
        // Check if there's auto draw
        if(Master.rules.enableAutoDraw)
            return;
        // Check if it's the local player
        if(!Master.turn.IsLocalPlayer())
            return;
        // Check if he can play
        if(!Master.turn.CanPlay())
            return;
        // Check if draw is not imposed on players
        if(Master.isDrawImposed)
            return;
        // Deal card to the player
        Director.deckDealer.DealCard(Master.turn, delegate() {
            // Update the playable cards
            Master.turn.hand.FetchPlayableCards();
            // End turn if quick play is disabled
            if(!Master.rules.enableDrawQuickPlay)
                Master.EndTurn();
        });
    }
}
