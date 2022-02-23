using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckDealer 
    : Manager
{
    public List<Card> deck;
    public Queue<Card> deckQueue;
    public int startingHand = 7;
    public Card boardCard;

    public void Prepare()
    {
        this.deck = new List<Card> ();
        ECardColor[] card_colors = (ECardColor[]) System.Enum.GetValues(typeof(ECardColor));
        // Add numbered cards (10 * 4 in total: 40)
        int card_number = 0;
        do
        {
            foreach(ECardColor card_color in card_colors)
                this.deck.Add(new NumberCard() {
                    number = card_number,
                    color = card_color
                });

            card_number++;
        }
        while(card_number < 10);
        // Add special cards (3 * 4 * 2 in total: 24)
        for(int i=0; i<2; i++)
        foreach(ECardColor card_color in card_colors)
        foreach(System.Type card_type in new List<System.Type> { typeof(SkipCard), typeof(ReverseCard), typeof(Draw2Card) })
        {
            ColorCard special_card = (ColorCard) System.Activator.CreateInstance(card_type);
            special_card.color = card_color;

            this.deck.Add(special_card);
        }
        // Add wild cards (4 * 2 in total: 8)
        foreach(System.Type card_type in new List<System.Type> { typeof(Draw4Card), typeof(SwitchCard) })
        for(int i=0; i<4; i++)
            this.deck.Add((WildCard) System.Activator.CreateInstance(card_type));
    }

    public void Shuffle()
    {
        if(this.deck == null)
            throw new System.Exception("DeckDealer.Shuffle#Exception: Cannot shuffle the deck because it is not prepared yet");

        // Sort deck randomly
        System.Random random = new System.Random();
        this.deck = this.deck.OrderBy(i => random.Next()).ToList();
        // Move them to a queue, in order for the players to be able to draw them in LAST-IN-LAST-OUT manner
        this.deckQueue = new Queue<Card> ();
        foreach(Card card in this.deck)
            this.deckQueue.Enqueue(card);
        this.deck.Clear();
    }

    public void DealCard(PlayerController player, System.Action onFinish = null)
    {
        player.ReceiveCard(this.deckQueue.Dequeue(), onFinish);
    }

    public void DealCards(List<PlayerController> players, System.Action onFinishDealing = null)
    {
        foreach(PlayerController player in players)
            DealCards(player, startingHand, onFinishDealing);
    }

    public void DealCards(PlayerController player, int number, System.Action onFinish = null)
    {
        player.ReceiveCard(deckQueue.Dequeue(), ActionRecursiveCardDeal(player, player.hand.CardsCount + number, onFinish));
    }

    public System.Action ActionRecursiveCardDeal(PlayerController player, int targetCardTotal, System.Action onFinishDealing)
    {
        return delegate() 
        {
            if(player.hand.CardsCount == targetCardTotal)
            {
                if(onFinishDealing != null)
                    onFinishDealing.Invoke();

                return;
            }

            player.ReceiveCard(this.deckQueue.Dequeue(), ActionRecursiveCardDeal(player, targetCardTotal, onFinishDealing));
        };
    }

    public void SetBoardCard()
    {
        // Retrieve a number card from deck
        this.boardCard = this.deckQueue.First(card => card is NumberCard);
    }
}
