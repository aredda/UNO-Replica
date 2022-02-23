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
    public RectTransform cardListPlayerIDs;
    public List<UICardPlayerID> cardPlayerIDs;

    public void PreparePlayerCards(List<PlayerController> players)
    {
        this.cardPlayerIDs = new List<UICardPlayerID> ();

        for (int i = 0; i < players.Count; i++)
        {
            Transform cardTransform = cardListPlayerIDs.GetChild(i);
            UICardPlayerID card = cardTransform.GetComponent<UICardPlayerID>();
            card.SetPlayer(players[i]);
            card.UpdateUsername();
            card.UpdateHandCount();
            card.gameObject.SetActive(true);

            this.cardPlayerIDs.Add(card);
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
