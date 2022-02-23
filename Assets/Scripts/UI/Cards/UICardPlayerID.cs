using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UICardPlayerID 
    : UIElement
{
    [Header("Targeted Player")]
    public PlayerController playerController;

    [Header("UI Elements")]
    public Image imageAvatar;
    public Text labelUsername;
    public Text labelHandCount;

    [Header("Meta Settings")]
    public Vector2 metaOffset;

    public void SetPlayer(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public void UpdateUsername()
    {
        
    }

    public void UpdateHandCount()
    {
        this.labelHandCount.text = $"{this.playerController.hand.cards.Count} cards";
    }

    public bool Concerns(PlayerController player)
    {
        return player.Equals(playerController);
    }

    public Vector2 GetMetaPosition()
    {
        return RectTransform.anchoredPosition + metaOffset;
    }
}
