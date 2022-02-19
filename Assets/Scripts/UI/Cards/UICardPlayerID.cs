using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UICardPlayerID 
    : MonoBehaviour
{
    [Header("Targeted Player")]
    public PlayerController playerController;

    [Header("UI Elements")]
    public Image imageAvatar;
    public Text labelUsername;
    public Text labelHandCount;

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
}
