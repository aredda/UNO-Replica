using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
- Responsible for:
- Spawning card templates
- Adjusting the templates' transform
- 
*/
public class PlayerHand 
    : AdvancedBehaviour
{
    [Header("Player Reference")]
    public PlayerController player;
    public Transform parent;

    [Header("Card Management")]
    [SerializeReference]
    public List<Card> cards;
    public List<CardTemplate> cardTemplates;
    public float zIndex = 0.005f;
    public Dictionary<ECardColor, int> colorCounters = new Dictionary<ECardColor, int> ();
    public int CardsCount
    {
        get { return cards.Count; }
    }

    public void SetPlayer(PlayerController player)
    {
        this.player = player;

        this.InitializeColorCounters();
    }

    public void ReceiveCard(Card card)
    {
        if (cards == null)
            cards = new List<Card>();

        cards.Add(card);
    }

    public void ReturnCard(Card card)
    {
        if (cards == null)
            return;

        if (!cards.Exists(c => c.Equals(card)))
            throw new System.Exception($"PlayerHand.ReturnCard: [{card}] doesn't exist in list");

        cards.Remove(card);
    }

    public CardTemplate CreateCardTemplate(Card card)
    {
        CardTemplate template = Director.poolingManager.GetTemplate(parent);
        template.SetCard(card);
        template.SetHand(this);
        
        return template;
    }

    public void CreateCardTemplates()
    {
        if(cards == null)
            throw new System.Exception("PlayerHand.CreateCardTemplates#Exception: Cannot show an empty hand");
        
        foreach(var card in cards)
            cardTemplates.Add(CreateCardTemplate(card));
    }

    public void OrganizeCardTemplates()
    {
        if(cards == null)
            throw new System.Exception("PlayerHand.OrganizeCardTemplates#Exception: Cannot organize an empty hand");

        int cards_count = cards.Count;
        float card_width = ComputeCardWidth(cards_count);
        float full_hand_width = card_width * cards_count;
        float x0 = (full_hand_width / 2) - (card_width / 2);
        int index = 0;

        foreach(var template in cardTemplates)
        {
            // Move card animation
            Director.cardAnimator.MoveCard(template.transform, new Vector3(-x0 + index * card_width, 0, -index * zIndex));

            index++;
        }
    }

    public Vector3 GetDrawnCardFuturePosition()
    {
        int cards_count = cards.Count;
        float card_width = ComputeCardWidth(cards_count);
        float full_hand_width = card_width * cards_count;
        float x0 = (full_hand_width / 2) - (card_width / 2);
        int index = 0;

        Vector3 lastTemplatePosition = parent.position;
        foreach(var template in cardTemplates)
        {
            lastTemplatePosition = new Vector3(-x0 + index * card_width, 0, -index * zIndex);
            if(index == cards_count - 1)
                break;

            // Move card animation
            Director.cardAnimator.MoveCard(template.transform, new Vector3(-x0 + index * card_width, 0, -index * zIndex));

            index++;
        }

        return parent.TransformPoint(lastTemplatePosition);
    }

    public List<CardTemplate> FetchPlayableCards()
    {
        List<CardTemplate> playables = new List<CardTemplate>();

        foreach(var template in cardTemplates)
        {
            // Reset color counters
            ResetColorCounters();
            // Check if playable
            bool playable = template.card.IsPlayable(Master.boardCardTemplate.card, Master.isDrawImposed);
            // Mark this card template as playable
            template.MarkAs(playable);
            // Count color
            if(template.card is ColorCard)
                colorCounters[((ColorCard) template.card).color]++;
            // Add to the list
            if(playable)
                playables.Add(template);
        }

        return playables;
    }

    public void PlayCard(CardTemplate template)
    {
        // Card Set Animation
        Director.cardAnimator.PlayCard(template, delegate() {
            //// Do Something When Set Card Animation Finishes ////
            // has the player declared?
            // if this is a bot, randomly declare
            if(Master.rules.enableOneCardCall)
                if(Master.playerToDeclare != player && CardsCount == 2)
                    Master.SetPlayerToDeclare(player, player.isBot ? (Random.Range(0, 10) > 5 ? true : false) : false);
            // activate card effect
            template.card.Activate(template);
            // Change Board Card
            Master.ChangeBoardCard(template);
            // Hide Template
            template.Disable();
            // Return the card to bottom of the deck
            Director.deckDealer.Enqueue(template.card);
            cards.Remove(template.card);
            // Remove template from this hand, return it to the pool
            cardTemplates.Remove(template);
            Director.poolingManager.SaveTemplate(template);
            // Update Hand
            OrganizeCardTemplates();
            // End Turn
            Master.EndTurn();
        });
    }

    public float ComputeCardWidth(int handCount)
    {
        return handCount < 9 ? 0.75f : 0.75f - (0.75f / (handCount / 2));
    }

    public List<byte[]> GetHandCardBytes()
    {
        List<byte[]> byteList = new List<byte[]>();

        foreach (var card in cards)
            byteList.Add(card.Serialize());

        return byteList;
    }

    #region Data Queries

    public CardTemplate GetCardTemplate(Card card)
    {
        if (!cards.Exists(c => c.Equals(card)))
            throw new System.Exception("PlayerHand.GetCardTemplate: card doesn't exist in card list");

        if (!cardTemplates.Exists(tc => tc.card.Equals(card)))
            throw new System.Exception("PlayerHand.GetCardTemplate: card doesn't have a corresponding template");

        return cardTemplates.Single(ct => ct.card.Equals(card));
    }

    public void InitializeColorCounters()
    {
        foreach(ECardColor color in System.Enum.GetValues(typeof(ECardColor)))
            this.colorCounters.Add(color, 0);
    }

    public void ResetColorCounters()
    {
        foreach(ECardColor color in System.Enum.GetValues(typeof(ECardColor)))
            this.colorCounters[color] = 0;
    }

    public List<CardTemplate> GetCardsByType <T> (List<CardTemplate> cardList)
    {
        return cardList.Where(template => template.card is T).ToList();
    }

    public List<CardTemplate> GetCardsByType(List<CardTemplate> cardList, System.Type type)
    {
        return cardList.Where(template => template.card.GetType() == type).ToList();
    }

    public List<CardTemplate> GetCardsByColor(List<CardTemplate> cardList, ECardColor color)
    {
        return cardList.Where(template => template.card is ColorCard).Where(template => ((ColorCard) template.card).color == color).ToList();
    }

    public List<CardTemplate> GetHighestNumberCards(List<CardTemplate> cardList)
    {
        int highestNumber = cardList.Max(template => ((NumberCard) template.card).number);

        return cardList.Where(template => ((NumberCard) template.card).number >= highestNumber).ToList();
    }

    public ECardColor GetDominantColor()
    {
        ECardColor dominantColor = ECardColor.Blue;
        int maxCounter = colorCounters[dominantColor];

        foreach(ECardColor color in System.Enum.GetValues(typeof(ECardColor)))
            if(maxCounter < colorCounters[color])
            {
                dominantColor = color;
                maxCounter = colorCounters[color];
            }

        return dominantColor;
    }

    #endregion
}
