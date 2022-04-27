using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotController 
    : AdvancedBehaviour
{
    public PlayerController player;

    public void SetPlayer(PlayerController player)
    {
        this.player = player;
    }

    public void Decide()
    {
        // Get the "best" card to play
        CardTemplate bestCard = GetBestCard();
        // If there is no playable card
        if(bestCard == null)
            Master.EndTurn();
        else
        {
            // wild card patch
            if(bestCard.card is WildCard)
            {
                WildCard wildCard = bestCard.card as WildCard;
                wildCard.chosenColor = bestCard.hand.GetDominantColor();
            }
            // Animate card playing
            player.hand.PlayCard(bestCard);
        }
    }

    public CardTemplate GetBestCard()
    {
        if(this.player == null)
            throw new System.Exception("BotController.GetBestCard#Exception: [PlayerController] object reference is missing");

        // First of all, the bot should know what's playable in its hand
        List<CardTemplate> playableCards = this.player.hand.FetchPlayableCards();
        // If there are no playable cards this time, end turn
        if(playableCards.Count == 0)
            return null;
        // Retrieve number cards
        List<CardTemplate> numberCards = this.player.hand.GetCardsByType<NumberCard> (playableCards);
        #region Next player is almost out of cards in hand
        // Check if the next player to play has less than 3 cards
        if(Master.GetNextPlayer().hand.cards.Count < 3)
        {
            // An order of priority on how to deal with the situation
            List<System.Type> priorityOrder = new List<System.Type>() {
                typeof(Draw2Card),
                typeof(Draw4Card),
                typeof(SkipCard),
                typeof(ReverseCard)
            };
            // Loop through the priority order
            foreach(System.Type type in priorityOrder)
            {
                // Retrieve crads of that type
                List<CardTemplate> typeCards = this.player.hand.GetCardsByType(playableCards, type);
                // Return a card if there are any
                if(typeCards.Count != 0)
                    return typeCards.First();
            }
        }
        #endregion

        #region Disposing of highest number cards
        // Retrieve highest number cards
        if(numberCards.Count != 0)
        {
            List<CardTemplate> highestNumberCards = this.player.hand.GetHighestNumberCards(numberCards);
            // Play a highest number card 
            if(highestNumberCards.Count != 0)
                return highestNumberCards.First();
        }
        #endregion

        // TODO: optimize
        // Play a random card
        return playableCards[Random.Range(0, playableCards.Count)];
    }
}
