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
        // Check if it's the player
        if(!master.turn.isLocalPlayer)
            return;
        // Check if he can play
        if(!master.turn.CanPlay())
            return;
        // Deal card to the player
        master.director.deckDealer.DealCard(master.turn, delegate() {
            // Update the playable cards
            master.turn.hand.FetchPlayableCards();
        });
    }
}
