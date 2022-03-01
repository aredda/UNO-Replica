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
    public Vector2 drawCounterOffset;
    public Vector2 stateTextOffset;

    public void SetPlayer(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public void UpdateUsername()
    {
        // TODO: must be refactored
        if(Master.isOnline)
            labelUsername.text = $"Player {playerController.networkAgent.netId}";
    }

    public void UpdateHandCount()
    {
        labelHandCount.text = $"{(Master.isOnline ? playerController.networkAgent.handCardCount : playerController.hand.CardsCount)} cards";
    }

    public bool Concerns(PlayerController player)
    {
        return player.Equals(playerController);
    }

    public Vector2 GetDrawCounterPosition()
    {
        return RectTransform.anchoredPosition + drawCounterOffset;
    }

    public Vector2 GetStateTextPosition()
    {
        return RectTransform.anchoredPosition + stateTextOffset;
    }
}
