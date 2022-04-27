using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonDeclare 
    : UIElement
{
    [Header("UI Element Reference")]
    public Button buttonDeclare;

    [Header("Target")]
    public PlayerController playerToDeclare;

    public void Show(PlayerController player)
    {
        playerToDeclare = player;

        buttonDeclare.onClick.RemoveAllListeners();
        buttonDeclare.onClick.AddListener(delegate ()
        {
            Master.SetPlayerToDeclare(playerToDeclare, true);
            Hide();
        });

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
