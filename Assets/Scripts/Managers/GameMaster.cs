using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMaster 
    : Manager
{
    [Header("Game Settings")]
    public GameRule rules = new GameRule();

    [Header("Hand Positions")]
    public Transform handPositions;

    [Header("Turn Settings")]
    public PlayerController turn;
    public int turnDirection = 1; // 1: Clockwise | -1: Counterclockwise

    [Header("Player Settings")]
    public int totalPlayers = 4;
    public List<PlayerController> players;
    public bool enableBots = true;

    [Header("Board Settings")]
    public CardTemplate boardCardTemplate;
    public Transform deck;
    public BoardController board;

    public void PreparePlayers()
    {
        for(int i=0; i < totalPlayers; i++)
        {
            PlayerController player_controller = Instantiate(director.prefabManager.playerController);
            player_controller.SetHandPosition(handPositions.GetChild(i));
            player_controller.InitializeHand();

            players.Add(player_controller);
        }
        players[0].isLocalPlayer = true;
        // If bots are allowed
        if(enableBots)
            for(int i=1; i < totalPlayers; i++)
                players[i].BecomeBot();
    }

    public void DisplayBoardCard()
    {
        boardCardTemplate.SetCard(director.deckDealer.boardCard);
    }

    public void PassTurn(PlayerController player)
    {
        turn = player;

        foreach(var p in players)
            if(p != player)
                p.SetCanPlay(false);

        player.SetCanPlay();
    }

    public void EndTurn()
    {
        StartCoroutine(RoutineEndTurn());
    }

    IEnumerator RoutineEndTurn()
    {
        yield return new WaitForSeconds(1f);
        // Pass turn to the next player
        PassTurn(GetNextPlayer());
        // If the next player is a bot
        if(this.turn.isBot)
            this.turn.bot.Decide();
    }

    public PlayerController GetNextPlayer()
    {
        int turnIndex = players.IndexOf(turn);

        if(turnDirection == 1)
            return turnIndex == players.Count - 1 ? players.First() : players[turnIndex + 1];
        
        return turnIndex == 0 ? players.Last() : players[turnIndex - 1];
    }

    public void ChangeBoardCard(CardTemplate template)
    {
        this.boardCardTemplate.SetCard(template.card);

        // Retrieve color
        ECardColor color = template.card is ColorCard ? ((ColorCard) template.card).color : ((WildCard) template.card).chosenColor;
        // Change board color
        this.board.Change(color);
    }
}
