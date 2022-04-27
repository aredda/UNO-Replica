using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuGameOver 
    : MonoBehaviour
{
    [Header("Menu Elements")]
    public Text labelPlayerState;

    [Header("Player State Settings")]
    public Color colorWinner;
    public Color colorLoser;

    public void Show(bool hasWon)
    {
        SetPlayerStateText(hasWon);

        // show menu
        gameObject.SetActive(true);
    }

    public void SetPlayerStateText(bool hasWon)
    {
        labelPlayerState.text = hasWon ? "Congratulations! You are the winner!" : "Oops! Hard luck next time.";
        labelPlayerState.color = hasWon ? colorWinner : colorLoser;
    }
}
