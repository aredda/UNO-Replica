using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController 
    : MonoBehaviour
{
    void OnMouseDown()
    {
        // Retrieve the master
        GameMaster master = ManagerDirector.director.gameMaster;
        // Check for turn object reference
        if(master.turn == null)
            throw new System.Exception("DeckController.OnMouseDown#Exception: Can't deal a card because the turn is not set");
        // Check if there's auto draw
        if(master.rules.enableAutoDraw)
            return;
        // Check if it's the player
        if(!master.turn.isLocalPlayer)
            return;
        // Check if he can play
        if(!master.turn.CanPlay())
            return;
        // Check if draw is not imposed on players
        if(master.isDrawImposed)
            return;
        // Deal card to the player
        master.director.deckDealer.DealCard(master.turn, delegate() {
            // Update the playable cards
            int playableCardsCount = master.turn.hand.FetchPlayableCards().Count;
            // End turn if quick play is disabled
            if(!master.rules.enableDrawQuickPlay)
                master.EndTurn();
        });
    }
}
