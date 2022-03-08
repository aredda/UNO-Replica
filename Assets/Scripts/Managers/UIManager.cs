using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager 
    : Manager
{
    [Header("Essential UI Elements")]
    public UIMenuColorPicker menuColorPicker;
    public UIMenuCardActionPicker menuCardActionPicker;
    public UIMenuChallenger menuChallenger;
    public UILabelTotalDraw labelDrawTotal;
    public UILabelPlayerState labelPlayerState;
    public UIButtonDeclare buttonDeclare;

    [Header("Player Card Positions")]
    public UICardPlayerID cardPlayerPrefab;
    public Transform cardParent;
    public List<UIPlayerCardDisposition> cardDispositions;

    private List<UICardPlayerID> cardPlayerIDs;

    public void PreparePlayerCards(List<PlayerController> players)
    {
        // initialise list
        cardPlayerIDs = new List<UICardPlayerID> ();
        // retrieve correct disposition for this case
        UIPlayerCardDisposition disposition = cardDispositions.Single(cd => cd.cardPositions.Count == director.gameMaster.players.Count);
        // set player cards
        for (int i = 0; i < players.Count; i++)
        {
            UICardPlayerID card = Instantiate(cardPlayerPrefab, cardParent);
            card.SetPlayer(players[i]);
            card.UpdateUsername();
            card.UpdateHandCount();
            card.gameObject.SetActive(true);
            card.RectTransform.anchoredPosition = disposition.cardPositions[i];

            cardPlayerIDs.Add(card);
        }
    }

    public UICardPlayerID GetPlayerCardID(PlayerController player)
    {
        return cardPlayerIDs.Single(p => p.playerController.Equals(player));
    }

    public void UpdatePlayerCards()
    {
        foreach(var card in cardPlayerIDs)
            card.UpdateHandCount();
    }

    public void HighlightPlayerCard(PlayerController player)
    {
        foreach(var card in cardPlayerIDs)
            if(player != card.playerController)
                director.cardAnimator.ChangePlayerCardScale(card.transform, Vector3.one * 0.75f);
        
        director.cardAnimator.ChangePlayerCardScale(cardPlayerIDs.Single(c => c.playerController == player).transform, Vector3.one);
    }
}
