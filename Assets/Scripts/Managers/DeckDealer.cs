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

    [Header("Debugging Helpers")]
    public List<string> cardNames;

    public void UpdateDeck(List<byte[]> cardBytesList)
    {
        if (deck == null)
            deck = new List<Card>();

        if (deckQueue == null)
            deckQueue = new Queue<Card>();

        deck.Clear();
        deckQueue.Clear();

        foreach (byte[] bytes in cardBytesList)
            deck.Add(Card.Deserialize(bytes));

        deckQueue = new Queue<Card> (deck);
    }

    public List<byte[]> GetDeckBytesList()
    {
        List<byte[]> list = new List<byte[]>();

        foreach (Card card in deck)
            list.Add(card.Serialize());

        return list;
    }

    public void Prepare()
    {
        // total should be 72 cards
        this.deck = new List<Card> ();
        // retrieve colors
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
        deck = deck.OrderBy(i => random.Next()).ToList();
        // Move them to a queue, in order for the players to be able to draw them in LAST-IN-LAST-OUT manner
        deckQueue = new Queue<Card> (deck);
    }

    public void DealCard(PlayerController player, System.Action onFinish = null)
    {
        player.ReceiveCard(Dequeue(), onFinish);
    }

    public void DealCards(List<PlayerController> players, System.Action onFinishDealing = null)
    {
        foreach(PlayerController player in players)
            DealCards(player, startingHand, onFinishDealing);
    }

    public void DealCards(PlayerController player, int number, System.Action onFinish = null)
    {
        player.ReceiveCard(Dequeue(), ActionRecursiveCardDeal(player, player.hand.CardsCount + number, onFinish));
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

            player.ReceiveCard(Dequeue(), ActionRecursiveCardDeal(player, targetCardTotal, onFinishDealing));
        };
    }

    public void SetBoardCard(Card card = null)
    {
        // Retrieve a number card from deck
        boardCard = card ?? deckQueue.First(card => card is NumberCard);
    }

    public Card Dequeue()
    {
        Card dequeued = deckQueue.Dequeue();

        if (deck.Contains(dequeued))
            deck.Remove(dequeued);

        return dequeued;
    }

    public void Enqueue(Card card)
    {
        deck.Add(card);
        deckQueue.Enqueue(card);
    }

    public void PrintCardNames()
    {
        cardNames = deck.Select(c => c.GetMaterialName()).ToList();
    }
}
