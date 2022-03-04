using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTemplate 
    : AdvancedBehaviour
{
    public bool displayOnly = false;
    public PlayerHand hand;

    [Header("Graphics")]
    public MeshRenderer faceMeshRenderer;
    public Material faceMaterial;
    public Color unplayableStateColor;

    [Header("Card Details")]
    public Card card;
    public string cardDescription = "None";
    public bool isPlayable = false;

    public void SetHand(PlayerHand hand)
    {
        this.hand = hand;
    }

    public void SetCard(Card card)
    {
        this.card = card;
        // Set description
        this.cardDescription = this.card.ToString();
        // Assign Material
        this.faceMaterial = Director.materialManager.GetMaterial(card.GetMaterialName());
        // Initial Setup
        this.faceMeshRenderer.materials = new Material[1] { faceMaterial };
    }

    public void StandOut()
    {
        if(hand == null)
            throw new System.Exception("CardTemplate.StandOut#Exception: [PlayerHand] reference is not set");

        hand.OrganizeCardTemplates();
        // Lift the card a little bit
        transform.localPosition += new Vector3(0, 0.4f, 0);
    }

    public void Disable()
    {
        this.gameObject.SetActive(false);
    }

    public void Enable()
    {
        this.gameObject.SetActive(true);
    }

    public void MarkAs(bool playable = true)
    {
        this.isPlayable = playable;

        // Visualise it
        Director.cardAnimator.ChangeCardColor(this, playable ? Color.white : this.unplayableStateColor);
    }

    void OnMouseDown()
    {
        // It means this template is just for show (example: board card template is not interactive)
        if(displayOnly)
            return;

        if(hand == null)
            throw new System.Exception("CardTemplate.OnMouseDown#Exception: [PlayerHand] reference is not set");

        if(hand.player == null)
            throw new System.Exception("CardTemplate.OnMouseDown#Exception: [PlayerController] reference is not set");

        // Stop If the player can't play
        if(!hand.player.CanPlay())
            return;

        // Can't play an un-playable card
        if(!isPlayable)
            return;

        // retrieve game master
        GameMaster master = ManagerDirector.director.gameMaster;

        // Show card actions
        System.Action actionPlay = delegate() 
        {
            System.Action actionPlayStep = delegate () {
                // if online call the command instead
                if (Master.isOnline)
                    hand.player.agent.CmdPlayCard(card.Serialize());
                else
                    // Play card functionality
                    hand.PlayCard(this);
            };

            // we need to check if this card is a wild one
            if (card is WildCard)
            {
                if ((master.isOnline && hand.player.agent.isLocalPlayer) || (!master.isOnline && hand.player.isLocalPlayer))
                {
                    // Change the player's state
                    hand.player.state = PlayerState.DecidingColor;
                    // Show menu
                    ManagerDirector.director.uiManager.menuColorPicker.Show(this, actionPlayStep);
                }
            }
            else
                actionPlayStep.Invoke();
        };
        System.Action actionDrawCard = delegate() 
        {
            // if it's online game
            if (Master.isOnline)
                hand.player.agent.CmdDrawCard();
            else
                // first, check if draw mode is imposed
                if(Master.isDrawImposed)
                {
                    Master.DrawImposedCards();
                }
                else
                {
                    // Last resort card draw
                    Master.LastResortDraw(hand.player);
                    // TODO: the player can only call for one last resort draw
                    // now, there's a hole in the logic of this button, because the player can
                    // draw infinitely
                }
        };
        System.Action actionChallenge = delegate()
        {
            // show challenge menu
            Director.uiManager.menuChallenger.Show();
        };
        // Show action menu
        Director.uiManager.menuCardActionPicker.Show(this, actionPlay, actionDrawCard, actionChallenge);
    }
}
